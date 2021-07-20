using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Collections;

public class CleaningAction : MonoBehaviour
{
    [Header("Brush Information")]
    public GameObject brush;
    public float brushSize = 0.1f;
    public RenderTexture texture;

    [Header("Dirt Information")]
    [Range(0.3f, 0.8f)]
    [Tooltip("Which alpha is the dirt considered as clean?")]
    public float CleanAlpha = 0.5f;

    [Header("Cleaning Cloth")]
    public int ClothMaxDirtiness = 20;
    public GameObject cleaningClothPrefab;
    bool cleaningClothIsDirty = false;

    Camera mainCamera; //camera for raycasting
    Renderer rend;
    GameObject cleanerObject;

    //About pixels
    int cleanedCount = 0;
    int cleanMeter = 0;
    public float dirtMeter = 0;

    //Unity
    Texture2D renderedTexture;
    GameObject cleaningCloth;
    AudioSource audioSource;
    public Animator animator;
    bool mouseOnObject = false;

    //mouse position holder for calculating sweep speed
    Vector3 lastPos;
    Vector3 delta;

    void Start()
    {
        mainCamera = Camera.main;

        cleaningCloth = Instantiate(cleaningClothPrefab, transform.position, Quaternion.identity, transform);

        cleanerObject = Instantiate(brush, transform.position + Vector3.up * 0.1f, Quaternion.identity, transform);
        cleanerObject.transform.localScale = Vector3.one * brushSize;

        rend = GetComponent<Renderer>();

        audioSource = mainCamera.GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        if (animator != null)
            animator.SetBool("Move", true);

        if (cleaningCloth != null)
            cleaningCloth.SetActive(true);

        newRendered = false;
    }

    private void OnDisable()
    {
        animator.SetBool("Move", false);
        cleaningCloth.SetActive(false);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            lastPos = Input.mousePosition;
        }
        if (Input.GetMouseButton(0) && GameManager.instantiate.IsPlaced)
        {
            CleaningAndCalculations();
            CheckIfCleaned();
        }
    }

    private void CleaningAndCalculations()
    {
        RaycastHit hit;
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out hit);

        MoveBrush(ray, hit);

        cleaningCloth.SetActive(true);

        if (dirtMeter < ClothMaxDirtiness && hit.transform == transform) // if the cloth is still clean and ray is hitting us.
        {
            delta = Input.mousePosition - lastPos; //we need to calculate player's swiping speed. The faster the player swipes, the faster the cloth gets dirty..
            delta.x = delta.x / Screen.width;
            delta.y = delta.y / Screen.height;

            dirtMeter += delta.magnitude;
            if (!audioSource.isPlaying && delta.magnitude > 0.14f)    // if player is sweeping fast enough play audio
            {
                audioSource.Play();
                //print(dirtMeter);
            }

            cleaningClothIsDirty = false;
        }
        if (dirtMeter >= ClothMaxDirtiness)
        {
            cleaningClothIsDirty = true;

            cleaningCloth.GetComponent<Renderer>().material.color = Color.black;
        }
    }

    private void MoveBrush(Ray ray, RaycastHit hit)
    {
        // hit.tag ile degistirilebilir, direk kaldirilabilir
        // instantiate the brush so our camera will render it to the texture.
        // if we want to clean multiple objects individually in scene, we have to check if hit.transform == transform
        if (!cleaningClothIsDirty && Physics.Raycast(ray, out hit) && hit.transform == transform)
        {

            cleanerObject.transform.position = hit.point + Vector3.up * 0.1f;
            cleaningCloth.transform.position = hit.point + Vector3.up * 0.1f;
            if (!cleanerObject.activeSelf)
            {
                cleanerObject.SetActive(true);
            }
            mouseOnObject = true;
            RenderToTexture();
        }
    }

    bool newRendered = false;
    private void RenderToTexture()
    {

        RenderTexture.active = texture;

        renderedTexture = new Texture2D(texture.width, texture.height, TextureFormat.RGBAHalf, false); // Created a new texture with rgbA format so we can deal with transparency

        renderedTexture.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0); // It's good to keep the texture size low. 

        renderedTexture.Apply();

        if (rend != null)
            rend.material.mainTexture = renderedTexture;

        newRendered = true;
    }

    void CheckIfCleaned()
    {
        if (renderedTexture == null) return;
        Color[] pixelArray = renderedTexture.GetPixels();
        cleanMeter = 0;
        //int pixelCount = pixelArray.Length;
        List<Color> colors = pixelArray.ToList();

        for (int i = 0; i < pixelArray.Length; i++)
        {
            if (pixelArray[i].a < CleanAlpha)
            {
                cleanMeter++;

            }
        }

        // if 32*32 pixels cleaned, newRendered is checking for RenderTexture is new or old
        if (cleanMeter >= renderedTexture.height * renderedTexture.width && newRendered)
        {
            GameManager.instantiate.IsSurfaceCleaned = true;
            cleanMeter = 0;
            this.enabled = false;
        }
    }
}
