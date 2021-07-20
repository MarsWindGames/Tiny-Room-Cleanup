using UnityEngine;

public class CleaningCloth : MonoBehaviour
{
    [Header("Cloth Properties")]
    public AudioSource sweepSound; //sweep sound
    public ParticleSystem particlePrefab;

    //Instances
    CleaningAction cleaningAction;
    ParticleSystem bubbleParticle;
    Renderer rend;
    public ParticleSystem water_bucketParticle;

    private void Awake()
    {
        cleaningAction = GetComponentInParent<CleaningAction>();
        rend = GetComponent<Renderer>();
    }
    private void OnEnable()
    {
        if (cleaningAction.dirtMeter == 0)
        {
            ClothPaint_White();
        }
    }

    void Update()
    {
        if (cleaningAction.dirtMeter > cleaningAction.ClothMaxDirtiness)
        {
            rend.material.color = Color.black;
            ClothPaint_Black();

        }
        else if (bubbleParticle != null)
        {
            ClothPaint_White();
        }

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            transform.position = hit.point;

            if (bubbleParticle == null)
            {
                bubbleParticle = Instantiate(particlePrefab, new Vector3(0, 1, 0), Quaternion.identity);
            }

        }

    }

    private void ClothPaint_White()
    {
        if (bubbleParticle != null)
        {
            var main = bubbleParticle.main;
            main.startColor = Color.white;
            main.maxParticles = 10;
            rend.material.color = new Color32(255, 230, 0, 255);
        }
    }

    private void ClothPaint_Black()
    {
        if (bubbleParticle != null)
        {
            var main = bubbleParticle.main;
            main.startColor = Color.black;
            main.maxParticles = 10;
            rend.material.color = Color.black;
        }
    }

    private void OnDisable()
    {
        if (bubbleParticle != null)
        {
            var main = bubbleParticle.main;
            main.maxParticles = 0;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("WaterBucket"))
        {
            cleaningAction.dirtMeter = 0;
            rend.material.color = new Color32(255, 230, 0, 255);

            if (water_bucketParticle.IsAlive() == false)
            {
                water_bucketParticle = Instantiate(water_bucketParticle, transform.position, Quaternion.identity);
            }
            water_bucketParticle.Play();

        }
    }
}
