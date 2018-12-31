using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpenController : MonoBehaviour {

    #region Variables
    //面板区
    public GameObject MainPanelObject;
    public GameObject StoryPanelObject;

    public GameObject OpenPanelObject;
    public GameObject MemoPanelObject;
    public GameObject SysPanelObject;

    public GameObject WaitObject;
    //按钮区

    //开始按钮区
    public Button StartButton;
    public Button MemoButton;
    public Button SetButton;

    //回忆按钮区
    public Button MemoBackButton;

    //系统按钮区
    public Button SetBackButton;

    //文本区
    public Text StoryText;
    float StoryTextAlpha = 0;

    //幕布区
    public CanvasGroup MainPanel;
    public CanvasGroup StoryPanel;
    public CanvasGroup OpenPanel;
    public CanvasGroup MemoPanel;
    public CanvasGroup SysPanel;
    public CanvasGroup WaitPanel;

    //参数
    public float fadeDuration;
    public float storyMergeDuration;
    public float mergeDuration;
    private float finalAlpha = 0f;
    private float destiAlpha = 1f;

    //状态机区
    bool MainToStory = false;
    bool OpenState = true;
    bool MemoState = false;
    bool SysState = false;
    public bool changed = false;
    bool dreamOpen = false;
    bool waitExist = false;
    public bool dreamStart = false;
    bool haveDreamStart = false;



    //音乐区
    public AudioSource clickButton;
    public AudioSource BGM1;
    public AudioSource BGM2;
    bool BGM1OUT = false;


    //故事开启区
    public string[] story;
    public string[] dream;



    #endregion

    #region Initialization

    private void Start()
    {
        //AddListener
        StartButton.onClick.AddListener(delegate { MainToStory = true; changed = true; clickButton.Play(); });
        MemoButton.onClick.AddListener(delegate { OpenState = false;MemoState = true; clickButton.Play(); });
        SetButton.onClick.AddListener(delegate { OpenState = false;SysState = true; clickButton.Play(); });

        MemoBackButton.onClick.AddListener(delegate { MemoState = false;OpenState = true; clickButton.Play(); });
        SetBackButton.onClick.AddListener(delegate { SysState = false;OpenState = true; clickButton.Play(); });


    }
    #endregion

    #region Update

    private void Update()
    {
        if(StorySystem.Instance.O2B)
        {
            StartCoroutine(Dispear(StoryPanel, fadeDuration, StoryPanelObject));
            MainToStory = false;
        }

        if (MainToStory)
        {
            StartCoroutine(Dispear(MainPanel,fadeDuration,MainPanelObject));
            StartCoroutine(merge(StoryPanel, storyMergeDuration,StoryPanelObject));
        }
        if(changed)
        {
            StorySystem.Instance.AddNewStory(story);
            changed = false;
        }
        if(dreamOpen)
        {
            StartCoroutine(Dispear(WaitPanel, mergeDuration, WaitObject));
        }
        if(dreamStart && !haveDreamStart)
        {
            MainToStory = true;
            StorySystem.Instance.AddNewStory(dream);
            haveDreamStart = true;
        }
        if(StorySystem.Instance.turn && !dreamStart)
        {
            MainToStory = false;
            StartCoroutine( Dispear(StoryPanel, fadeDuration, StoryPanelObject));
            StartCoroutine(merge(WaitPanel, mergeDuration, WaitObject));
            StartCoroutine(Wait());
            BGM1OUT = true;
            BGM2.Play();
        }
        if(!OpenState && MemoState)
        {
            StartCoroutine(Dispear(OpenPanel,fadeDuration,OpenPanelObject));
            StartCoroutine(merge(MemoPanel, mergeDuration,MemoPanelObject));
        }
        if(!MemoState && OpenState)
        {
            StartCoroutine(Dispear(MemoPanel,fadeDuration,MemoPanelObject));
            StartCoroutine(merge(OpenPanel, mergeDuration,OpenPanelObject));
        }
        if (!OpenState && SysState)
        {
            StartCoroutine(Dispear(OpenPanel, fadeDuration, OpenPanelObject));
            StartCoroutine(merge(SysPanel, mergeDuration, SysPanelObject));
        }
        if (!SysState && OpenState)
        {
            StartCoroutine(Dispear(MemoPanel, fadeDuration, SysPanelObject));
            StartCoroutine(merge(OpenPanel, mergeDuration, OpenPanelObject));
        }
        if(BGM1OUT)
        {
            StartCoroutine(BGM1minus());
        }
    }



    #endregion

    #region Functions



    #endregion

    #region Ienumerator

    private IEnumerator BGM1minus()
    {
        while (BGM1.volume >0f)
        {
            BGM1.volume = BGM1.volume -0.004f;
            yield return null;
        }
       
    }

    private IEnumerator Dispear(CanvasGroup canvasGroup,float DispearDuration,GameObject DispearObject)
    {
        canvasGroup.blocksRaycasts = false;

        float fadeSpeed = Mathf.Abs(canvasGroup.alpha - finalAlpha) / DispearDuration;

        if(canvasGroup.alpha == 0)
        {
            DispearObject.SetActive(false);
        }

        while (!Mathf.Approximately(canvasGroup.alpha, finalAlpha))
        {
            canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, finalAlpha,
                fadeSpeed * Time.deltaTime);

            yield return null;

        }

    }

    private IEnumerator merge(CanvasGroup canvasGroup,float MergeDuration,GameObject MergeObject)
    {
        canvasGroup.blocksRaycasts = false;
        MergeObject.SetActive(true);
        float mergeSpeed = Mathf.Abs(destiAlpha - canvasGroup.alpha) / MergeDuration;


        while (!Mathf.Approximately(canvasGroup.alpha, destiAlpha))
        {
            canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, destiAlpha,
                mergeSpeed * Time.deltaTime);

            yield return null;

        }
        canvasGroup.blocksRaycasts = true;
    }

    private IEnumerator TextMerge(float alpha)
    {
        float mergeSpeed = Mathf.Abs(destiAlpha - alpha) / fadeDuration;

        while (!Mathf.Approximately(alpha, destiAlpha))
        {
            alpha = Mathf.MoveTowards(alpha, destiAlpha,
                mergeSpeed * Time.deltaTime);
            StoryText.material.color = new Color(1, 1, 1, StoryTextAlpha);
            yield return null;

        }
    }

    private IEnumerator Wait()
    {
        yield return new WaitForSeconds(1.5f);
        BGM1.Stop();
        BGM2.Play();
        yield return new WaitForSeconds(3.5f);
        StorySystem.Instance.turn = false;
        dreamOpen = true;
        waitExist = false;
        dreamStart = true;
    }


    

    #endregion
}
