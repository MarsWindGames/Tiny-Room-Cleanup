using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    public int tutorialIndex = -1;
    public Canvas tutorialCanvas;
    public TMPro.TMP_Text scoreText, countdownText;
    public TMPro.TMP_Text tutorialText;
    public RawImage tutImage1, tutImage2;

    public static Tutorial instance;

    private void Awake()
    {
        instance = this;
        tutorialNext();
    }

    private void Start()
    {
        scoreText.enabled = false;
        countdownText.enabled = false;
    }
    public void tutorialNext()
    {
        if (tutorialIndex == 0)
        {
            tutorialText.text = "Press the button to clear the floor. The button will turn green when floor is cleaned";
            tutImage1.gameObject.SetActive(true);
        }
        if (tutorialIndex == 1)
        {
            tutImage1.gameObject.SetActive(false);
            tutorialText.text = "You need to clean your cleaning cloth sometimes. Move it to the bucket when it turns black.";
        }

        if (tutorialIndex == 2)
        {
            tutorialText.text = "Press the button to clean up the garbage.";
            tutImage2.gameObject.SetActive(true);
        }

        if (tutorialIndex == 3)
        {
            tutorialCanvas.enabled = false;
            PlayerPrefs.SetInt("PlayerTutorial", 1);
        }

    }

}
