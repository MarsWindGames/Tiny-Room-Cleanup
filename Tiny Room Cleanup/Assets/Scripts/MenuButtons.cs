using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour
{
    public Canvas loadingCanvas;
    public TMPro.TMP_Text playerScore, totalHome;

    private void Start()
    {
        LoadPlayerData();
    }
    public void Play()
    {
        StartCoroutine(LoadPlayScene());
        loadingCanvas.enabled = true;
    }

    void LoadPlayerData()
    {
        int score = PlayerPrefs.GetInt("PlayerScore", 0);
        playerScore.text = "Score: " + score;
        totalHome.text = "Cleaned Houses: " + PlayerPrefs.GetInt("TotalHomeCleaned", 0);

        if (score >= 10000)
        {
            PlayerPrefs.SetInt("PlayerLevel", Mathf.RoundToInt(score / 10000)); // 10.000 score means level 1, 20.000 level 2
        }
        else
        {
            PlayerPrefs.SetInt("PlayerLevel", 1); // 10.000 score means level 1, 20.000 level 2
        }



    }

    private IEnumerator LoadPlayScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(1);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}
