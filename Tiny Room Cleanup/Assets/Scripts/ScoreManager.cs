using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    //Unity
    public int _TotalScore = 0;
    public int totalHomeCleaned = 0;
    public static ScoreManager instance;

    //Events
    #region 
    public delegate void OnScoreChangedDelegate();
    public event OnScoreChangedDelegate OnScoreChanged;

    public int totalScore
    {
        get { return _TotalScore; }
        set
        {
            if (_TotalScore == value) return;
            _TotalScore = value;
            if (OnScoreChanged != null)
                OnScoreChanged();
        }
    }
    #endregion

    private void Awake()
    {
        instance = this;
        OnScoreChanged += UI_Interact.instance.UpdateScoreText;
    }

    public void increaseScore(int amount)
    {
        totalScore += amount;
        increaseTotalCleanedHome();
    }

    public void decreaseScore(int amount)
    {
        totalScore -= amount;
    }

    public void increaseTotalCleanedHome()
    {
        totalHomeCleaned++;
    }

    public void SaveScore()
    {

    }
}
