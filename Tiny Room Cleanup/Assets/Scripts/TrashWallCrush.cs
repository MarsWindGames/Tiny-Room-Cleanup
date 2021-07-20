using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashWallCrush : MonoBehaviour
{
    public int scoreToDecrease = 200;
    public AudioSource breakSound;
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Trash"))
        {
            Destroy(other.gameObject);
            GameManager.instantiate.TrashCount--;
            ScoreManager.instance.decreaseScore(200);

            if (!breakSound.isPlaying)
            {
                float randomPitch = Random.Range(0.8f, 1.2f);
                breakSound.pitch = randomPitch;
                breakSound.Play();
            }

            return;
        }
        else
            return;
    }
}
