using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_Interact : MonoBehaviour
{

    public Camera rendererCam;

    [Header("Canvases")]
    public Canvas startCanvas;
    public Canvas tutorialCanvas;
    public Canvas cleanedMessageCanvas;
    public Canvas notcleanedMessageCanvas;
    public Canvas EndCanvas;

    [Header("Animator of the buttons")]
    public Animator buttonTrashAnim;
    public Animator buttonCleaningAnim;

    [Header("End Game Sounds")]
    public AudioSource soundPlayer;
    public AudioClip successfullSound;
    public AudioClip unsuccessfullSound;

    [Header("End Game Texts")]
    public TMPro.TMP_Text scoreText, totalHomeText, summaryText;

    [Header("Other Texts")]
    public TMPro.TMP_Text trashText;
    public TMPro.TMP_Text housesText;
    public TMPro.TMP_Text timeText;
    public TMPro.TMP_Text gameScoreText;
    public Animator gameScoreAnim;

    //Unity
    public static UI_Interact instance;
    GameManager gm;
    int scoreHolder = 0;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        //this class holds button click events, and canvas events
        gm = GameManager.instantiate;

        housesText.text = "Houses: " + gm.roomCount;
        timeText.text = "Time: " + gm.CountDown + "s";
    }

    public void EnableCleaning()
    {
        if (tutorialCanvas.enabled)
        {
            if (gm.isPlaced && Tutorial.instance.tutorialIndex == 0)
            {
                gm.EnableCleaning();
                Tutorial.instance.tutImage1.enabled = false;
                Tutorial.instance.tutorialNext();
                ButtonCleaningAnimPlay();
                Tutorial.instance.tutorialIndex++;
                Tutorial.instance.tutorialNext();
            }
        }
        else
        {
            if (gm.isPlaced && !gm.IsSurfaceCleaned)
            {
                gm.EnableCleaning();
                ButtonCleaningAnimPlay();
            }
        }

    }

    public void EnableTrash()
    {
        if (tutorialCanvas.enabled)
        {
            if (Tutorial.instance.tutorialIndex == 2)
            {
                gm.EnableTrash();
                Tutorial.instance.tutImage2.enabled = false;
                ButtonTrashAnimPlay();
            }

        }
        else
        {
            if (gm.isPlaced)
            {
                gm.EnableTrash();

                ButtonTrashAnimPlay();
            }
        }

    }

    public void StartGame()
    {
        int tutorialDone = PlayerPrefs.GetInt("PlayerTutorial", 0);

        if (tutorialDone == 0)
        {
            tutorialCanvas.gameObject.SetActive(true);
            startCanvas.enabled = false;
            gm.StartTutorial();

        }
        else
        {
            if (gm != null)
            {
                startCanvas.enabled = false;
                gm.StartGame();
            }
        }


    }

    public void playButtonCleaningAnim()
    {

        if (buttonCleaningAnim.GetBool("Done"))
        {
            soundPlayer.Play();
        }
        buttonCleaningAnim.SetBool("Done", true);

        if (tutorialCanvas.enabled)
        {
            Tutorial.instance.tutorialIndex++;
            Tutorial.instance.tutorialNext();
        }


    }

    public void stopButtonTrashAnim()
    {
        if (gm != null)
        {
            trashText.text = (gm.TrashCountHolder + "/" + gm.TrashCount);

            if (gm.trashCount == 0)
                buttonTrashAnim.SetBool("Clicked", false);
        }
    }

    public void stopAllButtonAnims()
    {
        if (gm != null && gm.isPlaced)
        {
            buttonCleaningAnim.SetBool("Clicked", false);
            buttonTrashAnim.SetBool("Clicked", false);
            buttonCleaningAnim.SetBool("Done", false);


            rendererCam.clearFlags = CameraClearFlags.Depth;

        }
        else
        {

            rendererCam.clearFlags = CameraClearFlags.SolidColor;
            Color clr = new Color(0, 0, 0, 0);
            rendererCam.backgroundColor = clr;
        }

    }

    public void showCleanedMessage()
    {
        cleanedMessageCanvas.gameObject.SetActive(false);
        cleanedMessageCanvas.gameObject.SetActive(true);
    }

    public void showDirtyMessage()
    {
        notcleanedMessageCanvas.gameObject.SetActive(false);
        notcleanedMessageCanvas.gameObject.SetActive(true);
    }

    private void ButtonTrashAnimPlay()
    {
        if (!buttonTrashAnim.GetBool("Done") && !buttonTrashAnim.GetBool("Clicked"))
        {
            buttonTrashAnim.SetBool("Clicked", true);
            buttonCleaningAnim.SetBool("Clicked", false);
        }
    }

    private void ButtonCleaningAnimPlay()
    {
        if (!buttonCleaningAnim.GetBool("Done") && !buttonCleaningAnim.GetBool("Clicked"))
        {
            buttonCleaningAnim.SetBool("Clicked", true);
            buttonTrashAnim.SetBool("Clicked", false);
        }
    }

    public void ShowEndCanvas()
    {
        //disable other canvases
        #region 
        startCanvas.gameObject.SetActive(false);
        cleanedMessageCanvas.gameObject.SetActive(false);
        notcleanedMessageCanvas.gameObject.SetActive(false);
        ScoreManager scoreManager = ScoreManager.instance;
        #endregion

        EndCanvas.gameObject.SetActive(true);

        int totalScore = scoreManager._TotalScore;
        int totalHomeCleaned = scoreManager.totalHomeCleaned;

        int oldScore = PlayerPrefs.GetInt("PlayerScore", 0);
        int oldHomeCleaned = PlayerPrefs.GetInt("TotalHomeCleaned", 0);
        if (totalScore > 0)
        {
            summaryText.text = "CLEANED";
            soundPlayer.PlayOneShot(successfullSound);
        }
        else
        {
            summaryText.text = "DIRTY";
            soundPlayer.PlayOneShot(unsuccessfullSound);
            summaryText.color = Color.gray;
        }

        scoreText.text = totalScore.ToString();
        totalHomeText.text = totalHomeCleaned.ToString();

        PlayerPrefs.SetInt("PlayerScore", oldScore + totalScore);
        PlayerPrefs.SetInt("TotalHomeCleaned", oldHomeCleaned + totalHomeCleaned);
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void RestartGame()
    {
        StartCoroutine(LoadPlayScene());
    }

    private IEnumerator LoadPlayScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(1);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    public void UpdateScoreText()
    {
        int score = ScoreManager.instance._TotalScore;
        if (score > scoreHolder)
        {
            gameScoreText.text = ScoreManager.instance._TotalScore.ToString();
            gameScoreAnim.SetTrigger("increase");
            scoreHolder = score;
        }
        else
        {
            gameScoreText.text = ScoreManager.instance._TotalScore.ToString();
            gameScoreAnim.SetTrigger("decrease");
            scoreHolder = score;
        }


    }
}
