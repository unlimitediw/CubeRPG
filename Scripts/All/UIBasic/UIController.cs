using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

    #region Variables
    //按钮区
    Button EasyToStateButton;
    public Button BagButton;
    public Button MessageButton;
    public Button SysButton;
    public Button PetButton;
    public Button QuestButton;
    public Button CloseQuestButton;

    //面板区
    public GameObject StatePanel;
    public GameObject EasyPanel;
    public GameObject FourG;
    public GameObject BagPanel;
    public GameObject QuestPanel;

    public GameObject FourGTextPanel;

    //幕布区
    public CanvasGroup QuestPanelCanvasGroup;

    //文字区
    Text EasyToStateText;
    //Four文字区
    Text upText;
    Text downText;
    Text rightText;
    Text leftText;

    //状态机
    bool Easy = true;
    bool StateAmp = false;
    bool StateMin = false;

    bool EasyRight = false;
    bool EasyLeft = false;

    bool QuestPanelOn = false;
    bool QuestPanelOff = false;

    private bool BagClose = true;
    private bool BagAmp = false;
    private bool BagMin = false;

    private bool changeState = false;
    private int num = 0;
    //平滑
    public float smoothing = 5;
    private float b = 0;
    private float s = 0;
    #endregion

    #region Initialization
    private void Start()
    {
        //EasyToState
        EasyToStateButton = transform.Find("Buttons").Find("EasyToState").GetComponent<Button>();
        EasyToStateButton.onClick.AddListener(delegate { EasyToState(); });
        EasyToStateText = transform.Find("Buttons").Find("EasyToState").Find("Text").GetComponent<Text>();
        EasyToStateText.text = "α";
        upText = FourGTextPanel.transform.Find("upText").GetComponent<Text>();
        downText = FourGTextPanel.transform.Find("downText").GetComponent<Text>();
        rightText = FourGTextPanel.transform.Find("rightText").GetComponent<Text>();
        leftText = FourGTextPanel.transform.Find("leftText").GetComponent<Text>();

        //Bag
        MessageButton.onClick.AddListener(delegate { num = 1;changeState = true; CloseBag(); });
        PetButton.onClick.AddListener(delegate { num = 2; changeState = true; CloseBag(); });
        BagButton.onClick.AddListener(delegate { num = 3; changeState = true; CloseBag(); });
        SysButton.onClick.AddListener(delegate { num = 4; changeState = true; CloseBag(); });
        QuestButton.onClick.AddListener(delegate { QuestPanelOn = true;QuestPanelOff = false; });
        CloseQuestButton.onClick.AddListener(delegate { QuestPanelOff = true; QuestPanelOn = false; });

        //放大缩小预设
        BagPanel.transform.localScale = new Vector3(0, 0, 0);
    }

    #endregion

    #region Update

    private void Update()
    {
        //Bag
        b = AmpMin(b, BagAmp, BagMin, BagPanel);
        if (b >= 0.99 && BagAmp)
        {
            BagAmp = false;
        }
        if (b <= 0.01 && BagMin)
        {
            BagMin = false;
            BagClose = true;
            BagPanel.SetActive(false);
        }

        //keyBoardControl
        if (Input.GetKeyDown(KeyCode.M))
        {
            num = 1; changeState = true; CloseBag();
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            num = 2; changeState = true; CloseBag();
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            num = 3; changeState = true; CloseBag();
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            num = 4; changeState = true; CloseBag();
        }
        //State
        s = AmpMin(s, StateAmp, StateMin, StatePanel);
        if (s >= 0.99 && StateAmp)
        {
            StateAmp = false;
        }
        if (b <= 0.01 && BagMin)
        {
            StateMin = false;
            Easy = true;
        }

        //Easy

        //FourGRotation
        TurnState();

        //QuestPanelOn&Off
        if(QuestPanelOn)
        {
            StartCoroutine(merge(QuestPanelCanvasGroup, 10f, QuestPanel));
        }
        if(QuestPanelOff)
        {
            StartCoroutine(Dispear(QuestPanelCanvasGroup, 10f, QuestPanel));
        }
    }
    #endregion

    #region ButtonChange
    void EasyToState()
    {
        if (Easy)
        {
            StatePanel.SetActive(true);
            StateAmp = true;
            StateMin = false;
            EasyLeft = true;
            EasyRight = false;
            EasyPanel.SetActive(false);
            EasyToStateText.text = "β";
            Easy = false;
        }
        else
        {
            StateAmp = false;
            StateMin = true;
            EasyPanel.SetActive(true);
            EasyToStateText.text = "α";
            Easy = true;
        }
    }

    void TurnState()
    {
        if(num == 1)
        {
            Quaternion rotation = Quaternion.Euler(0.0f, 0.0f, 45f);
            if (BagClose)
            {
                FourG.transform.rotation = Quaternion.RotateTowards(FourG.transform.rotation, rotation, 300 * Time.deltaTime);
            }
            if (FourG.transform.rotation == rotation)
            {
                leftText.text = "M";
                downText.text = "P";
                rightText.text = "B";
                upText.text = "S";
                FourGTextPanel.SetActive(true);
                if(changeState)
                {
                    OpenCloseBag();
                    changeState = false;
                }
            }
            else
            {
                FourGTextPanel.SetActive(false);
            }
        }
        else if(num == 2)
        {
            Quaternion rotation = Quaternion.Euler(0.0f, 0.0f, -45f);
            if (BagClose)
            {
                FourG.transform.rotation = Quaternion.RotateTowards(FourG.transform.rotation, rotation, 300 * Time.deltaTime);
            }
            if (FourG.transform.rotation == rotation)
            {
                leftText.text = "P";
                downText.text = "B";
                rightText.text = "S";
                upText.text = "M";
                FourGTextPanel.SetActive(true);
                if (changeState)
                {
                    OpenCloseBag();
                    changeState = false;
                }
            }
            else
            {
                FourGTextPanel.SetActive(false);
            }
        }
        else if (num == 3)
        {
            Quaternion rotation = Quaternion.Euler(0.0f, 0.0f, -135f);
            if (BagClose)
            {
                FourG.transform.rotation = Quaternion.RotateTowards(FourG.transform.rotation, rotation, 300 * Time.deltaTime);
            }
            if (FourG.transform.rotation == rotation)
            {
                leftText.text = "B";
                downText.text = "S";
                rightText.text = "M";
                upText.text = "P";
                FourGTextPanel.SetActive(true);
                if (changeState)
                {
                    OpenCloseBag();
                    changeState = false;
                }
            }
            else
            {
                FourGTextPanel.SetActive(false);
            }
        }
        else if (num == 4)
        {
            Quaternion rotation = Quaternion.Euler(0.0f, 0.0f, -225f);
            if (BagClose)
            {
                FourG.transform.rotation = Quaternion.RotateTowards(FourG.transform.rotation, rotation, 300 * Time.deltaTime);
            }
            if (FourG.transform.rotation == rotation)
            {
                leftText.text = "S";
                downText.text = "M";
                rightText.text = "P";
                upText.text = "B";
                FourGTextPanel.SetActive(true);
                if (changeState)
                {
                    OpenCloseBag();
                    changeState = false;
                }
            }
            else
            {
                FourGTextPanel.SetActive(false);
            }
        }
    }

    void CloseBag()
    {
        if (!BagClose)
        {
            BagMin = true;
            BagAmp = false;
        }
        else
        {
            return;
        }

    }

    void OpenCloseBag()
    {
        if(BagClose)
        {
            BagPanel.SetActive(true);
            BagAmp = true;
            BagMin = false;
            BagClose = false;
        }
        else
        {
            BagMin = true;
            BagAmp = false;
            
        }
    }

    float AmpMin(float a,bool Amp,bool Min,GameObject Panel)
    {
        if (Amp)
        {
            a = Mathf.Lerp(a, 1, Time.deltaTime * smoothing);
        }
        if (Min)
        {
            a = Mathf.Lerp(a, 0, Time.deltaTime * smoothing);
            if (a <= 0.01)
            {
                Panel.SetActive(false);
            }
        }
        Panel.transform.localScale = new Vector3(a, a, a);
        return a;
    }
    #endregion

        #region 淡入淡出
    private IEnumerator Dispear(CanvasGroup canvasGroup, float DispearDuration, GameObject DispearObject)
    {
        canvasGroup.blocksRaycasts = false;

        float fadeSpeed = Mathf.Abs(canvasGroup.alpha - 0f) / DispearDuration;

        if (canvasGroup.alpha == 0)
        {
            DispearObject.SetActive(false);
        }

        while (!Mathf.Approximately(canvasGroup.alpha, 0f))
        {
            canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, 0f,
                fadeSpeed * Time.deltaTime);

            yield return null;

        }

    }

    private IEnumerator merge(CanvasGroup canvasGroup, float MergeDuration, GameObject MergeObject)
    {
        canvasGroup.blocksRaycasts = false;
        MergeObject.SetActive(true);
        float mergeSpeed = Mathf.Abs(1f - canvasGroup.alpha) / MergeDuration;


        while (!Mathf.Approximately(canvasGroup.alpha, 1f))
        {
            canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, 1f,
                mergeSpeed * Time.deltaTime);

            yield return null;

        }
        canvasGroup.blocksRaycasts = true;
    }
    #endregion
}
