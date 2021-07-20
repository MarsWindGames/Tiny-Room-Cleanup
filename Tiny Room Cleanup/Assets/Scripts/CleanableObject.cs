using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class CleanableObject : MonoBehaviour
{

    [Header("Room Properties")]
    public float moveSpeed = 2f;
    public Texture2D[] dirtTexture;
    public GameObject[] trashPrefabs;
    public Transform[] SpawnPoints;

    [Header("Parent Trash Transform")]
    public Transform trashes;

    [Header("Object's Renderer")]
    public Renderer rend;

    //Unity
    Trash trash;
    int trashCount = 0;
    public int trashedCount = 0;
    List<GameObject> trashPool;
    Vector3 pointToGo;
    bool move = false;
    Vector3 startPos;
    bool isSetActive;
    float currentLerpTime;

    //Instances
    GameManager gameManager;
    CleaningAction cleaningAction;

    private void Awake()
    {
        trashPool = new List<GameObject>();
        rend.material.mainTexture = dirtTexture[Random.Range(0, dirtTexture.Length)];
        cleaningAction = GetComponentInChildren<CleaningAction>();
        trash = GetComponentInChildren<Trash>();
    }

    private void OnEnable()
    {
        rend.material.mainTexture = dirtTexture[Random.Range(0, dirtTexture.Length)];
        trashCount = GameManager.instantiate.TrashCount;

        cleaningAction.dirtMeter = 0;
        DestroyOldTrashes();
        SpawnTrash();
    }

    private void Start()
    {
        trashPool = new List<GameObject>();
        rend.material.mainTexture = dirtTexture[Random.Range(0, dirtTexture.Length)];
        cleaningAction = GetComponentInChildren<CleaningAction>();
        trash = GetComponentInChildren<Trash>();

        gameManager = GameManager.instantiate;

    }

    private void DestroyOldTrashes()
    {
        for (int i = 0; i < trashes.childCount; i++)
        {
            Destroy(trashes.GetChild(i).gameObject);
        }
    }

    private void SpawnTrash()
    {
        for (int i = 0; i < trashCount; i++)
        {
            int rndPrefabIndex = Random.Range(0, trashPrefabs.Length);
            int rndSpawnIndex = Random.Range(0, SpawnPoints.Length);
            GameObject trash = Instantiate(trashPrefabs[rndPrefabIndex], SpawnPoints[rndSpawnIndex].position, Quaternion.identity, trashes);
        }
    }

    public void BringThis(Vector3 point, bool _isSetActive)
    {
        isSetActive = _isSetActive;
        currentLerpTime = 0f;
        startPos = transform.position;
        pointToGo = point;
        move = true;
    }

    void Update()
    {
        if (transform.position != pointToGo)
        {
            currentLerpTime += Time.deltaTime;
            if (currentLerpTime > 3f)
            {
                currentLerpTime = 3f;
            }
            float perc = currentLerpTime / 3f;

            transform.position = Vector3.Lerp(startPos, pointToGo, perc);
            gameManager.IsPlaced = false;
        }
        else if (!isSetActive)
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameManager.IsPlaced = true;
        }
    }
}
