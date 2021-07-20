using System.Collections;
using UnityEngine;

public class Trash : MonoBehaviour
{
    Animator animator;
    public AudioSource audioSource;
    Camera mainCam;
    public Transform sweep;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Trash"))
        {
            Destroy(other.gameObject); // Simply destroy the trash objects enter the collider.
            GameManager.instantiate.TrashCount--;
            
            if (!audioSource.isPlaying)
                audioSource.Play();
        }

        if (GameManager.instantiate.TrashCount == 0)
        {
            BringBucket(false);
        }

    }
    private void Awake()
    {
        animator = GetComponent<Animator>();
        mainCam = Camera.main;

    }

    public void BringBucket(bool isTrue)
    {
        sweep.gameObject.SetActive(isTrue);
        animator.SetBool("Move", isTrue);
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            RaycastHit hit;
            Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                Vector3 newPos = hit.point;
                newPos.y = sweep.position.y;
                //newPos.z -= 0.5f;
                sweep.position = newPos;
            }
        }
    }
}
