using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Score Variables")]
    public int ScoreToIncrease = 100;
    public int ScoreToDecrease = 100;

    [Header("Game Setup")]
    public GameObject cleanableObjectPrefab;
    public Vector3 SpawnPoint;
    public Vector3 CenterPoint;
    public float CountDown = 45f;
    public int TrashCountMax = 7;
    public int roomCount = 10;

    [Header("UI Setup")]
    public TMPro.TMP_Text trashText;
    public TMPro.TMP_Text countdownText;

    //Instances
    GameObject cleanableObject;
    CleaningAction cleaningAction;
    Trash trashInstance;
    CleaningAction cleaningActionInstance;
    CleanableObject cleanableInstance;


    //Unity
    int objectCount = 2;
    Queue<GameObject> objectPool;
    public bool isPlaced = false;
    public int TrashCountHolder = 0;
    public bool isSurfaceCleaned = false;
    public int trashCount = -1;
    float countDownMax;
    public Material curtain_material;
    public Color[] colorList;
    public bool isGameFinished = false;
    public int tutorialDone;

    //Singleton
    public static GameManager instantiate;

    //Events
    #region 

    public delegate void OnTrashCountChangedDelegate();
    public event OnTrashCountChangedDelegate OnTrashCountChanged;

    public int TrashCount
    {
        get { return trashCount; }
        set
        {
            if (trashCount == value) return;
            trashCount = value;
            if (OnTrashCountChanged != null)
                OnTrashCountChanged();
        }
    }

    public delegate void OnGameHasEndDelegate();
    public event OnGameHasEndDelegate OnGameHasEnd;

    public bool IsGameFinished
    {
        get { return isGameFinished; }
        set
        {
            if (isGameFinished == value) return;
            isGameFinished = value;
            if (isGameFinished)
                OnGameHasEnd();
        }
    }

    public delegate void OnSurfaceCleanedDelegate();
    public event OnSurfaceCleanedDelegate OnSurfaceCleaned;

    public bool IsSurfaceCleaned
    {
        get { return isSurfaceCleaned; }
        set
        {
            if (isSurfaceCleaned == value) return;
            isSurfaceCleaned = value;
            if (isSurfaceCleaned || !isSurfaceCleaned)
                OnSurfaceCleaned();
        }
    }

    public delegate void OnPlacedDelegate();
    public event OnPlacedDelegate OnPlaced;

    public bool IsPlaced
    {
        get { return isPlaced; }
        set
        {
            if (isPlaced == value) return;
            isPlaced = value;
            if (isPlaced || !isPlaced)
                OnPlaced();
        }
    }


    #endregion

    private void Awake()
    {
        //PlayerPrefs.DeleteAll();
        //events
        OnTrashCountChanged += UI_Interact.instance.stopButtonTrashAnim;
        OnGameHasEnd += UI_Interact.instance.ShowEndCanvas;
        OnSurfaceCleaned += UI_Interact.instance.playButtonCleaningAnim;
        OnPlaced += UI_Interact.instance.stopAllButtonAnims;

        //singleton
        if (instantiate == null)
            instantiate = this;

        objectPool = new Queue<GameObject>();

        SetHardness();
        tutorialDone = PlayerPrefs.GetInt("PlayerTutorial", 0);

    }

    private void SetHardness()
    {
        int playerLevel = PlayerPrefs.GetInt("PlayerLevel");

        if (playerLevel < 10)
        {
            CountDown = Mathf.RoundToInt(45 / PlayerPrefs.GetInt("PlayerLevel") + 15);

        }
        else
        {
            CountDown = 15;
        }
        roomCount = playerLevel * 2;
    }

    private void TrashCountRandomizer()
    {

        int rndCount = Random.Range(0, TrashCountMax);
        TrashCountHolder = rndCount; // we use this to get total trash count, this value doesn't decrease at any time.
        TrashCount = rndCount;
    }

    public void StartGame()
    {
        //At the start of the game, we bring a new room to the scene. (Walls, sweep, cleaning cloth and environment).
        SpawnCleanableObject();
        countDownMax = CountDown;
    }

    public void StartTutorial()
    {
        SpawnTutorialObject();
        Tutorial.instance.tutorialCanvas.enabled = true;

    }

    private void SpawnTutorialObject()
    {
        TrashCount = 5;
        GameObject instantiated = Instantiate(cleanableObjectPrefab, SpawnPoint, Quaternion.identity);
        instantiated.SetActive(true);

        cleanableInstance = instantiated.GetComponent<CleanableObject>();
        cleanableInstance.BringThis(CenterPoint, true);

        trashInstance = instantiated.GetComponentInChildren<Trash>();
        trashInstance.BringBucket(false);

        cleaningActionInstance = instantiated.GetComponentInChildren<CleaningAction>();
        cleaningActionInstance.enabled = false;

        objectPool.Enqueue(instantiated);
        cleanableObject = instantiated;
        var curt_material = curtain_material;
        StartCoroutine(GetRandomColor());
    }

    public void SpawnCleanableObject()
    {
        //spawning rooms with object pooling
        roomCount--;
        if (roomCount != -1)
        {
            TrashCountRandomizer();
            if (objectPool.Count < objectCount)
            {
                GameObject instantiated = Instantiate(cleanableObjectPrefab, SpawnPoint, Quaternion.identity);
                instantiated.SetActive(true);

                cleanableInstance = instantiated.GetComponent<CleanableObject>();
                cleanableInstance.BringThis(CenterPoint, true);

                trashInstance = instantiated.GetComponentInChildren<Trash>();
                trashInstance.BringBucket(false);

                cleaningActionInstance = instantiated.GetComponentInChildren<CleaningAction>();
                cleaningActionInstance.enabled = false;

                objectPool.Enqueue(instantiated);
                cleanableObject = instantiated;
                var curt_material = curtain_material;
                StartCoroutine(GetRandomColor());
            }
            else
            {
                GameObject obj = objectPool.Dequeue();
                obj.SetActive(true);
                obj.transform.position = SpawnPoint;

                //we put the cleanableobject into a variable to use later.
                cleanableInstance = obj.GetComponent<CleanableObject>();
                cleanableInstance.BringThis(CenterPoint, true);

                trashInstance = obj.GetComponentInChildren<Trash>();
                trashInstance.BringBucket(false);

                cleaningActionInstance = obj.GetComponentInChildren<CleaningAction>();
                cleaningActionInstance.enabled = false;

                cleanableObject = obj;
                objectPool.Enqueue(obj);
                var curt_material = curtain_material;
                StartCoroutine(GetRandomColor());
            }
        }
    }

    private void Update()
    {
        if (IsPlaced && roomCount != -1 && tutorialDone == 1) // if the room is not moving, do:
        {
            CountDown = CountDown - Time.deltaTime;
            countdownText.text = ((int)CountDown).ToString();

            if (CountDown <= 0) // when countdown ends bring a new cleanable room
            {
                BringNewObject();
            }
        }

        //if all clean, bring a new cleanable object. Don't wait for the countdown
        BringNewObject();
    }

    public void BringNewObject()
    {
        if (roomCount < 0)
        {
            IsGameFinished = true;
        }
        else
        {
            if (TrashCount == 0 && IsSurfaceCleaned) // if all clean
            {
                IsSurfaceCleaned = false;

                cleanableInstance.BringThis(CenterPoint + Vector3.right * 5, false); // send the room to off screen

                SpawnCleanableObject();

                ScoreManager.instance.increaseScore((int)(ScoreToIncrease * CountDown));

                UI_Interact.instance.showCleanedMessage();
                CountDown = countDownMax;
            }
            else if (CountDown <= 0)
            {

                if (IsSurfaceCleaned)
                {
                    ScoreManager.instance.decreaseScore(TrashCount * ScoreToIncrease);
                    UI_Interact.instance.showDirtyMessage();

                }
                else
                {
                    ScoreManager.instance.decreaseScore(TrashCount * ScoreToDecrease + ScoreToDecrease);
                    UI_Interact.instance.showDirtyMessage();


                }
             
                trashInstance.BringBucket(false);
                cleaningActionInstance.enabled = false;
                IsSurfaceCleaned = false;


                cleanableInstance.BringThis(CenterPoint + Vector3.right * 5, false);
                SpawnCleanableObject();
                CountDown = countDownMax;
            }
        }
    }

    public void EnableCleaning()
    {
        cleaningActionInstance.enabled = true;
        trashInstance.BringBucket(false);
    }

    public void EnableTrash()
    {
        trashInstance.BringBucket(true);
        cleaningActionInstance.enabled = false;
    }

    private IEnumerator GetRandomColor()
    {
        //we delay it because we don't want the player to see the color change of the material.
        yield return new WaitForSeconds(1f);
        var curt_material = curtain_material;
        int colorIndex = Random.Range(0, colorList.Length);

        curt_material.color = colorList[colorIndex];


    }
}

