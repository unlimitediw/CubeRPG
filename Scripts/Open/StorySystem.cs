using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StorySystem : MonoBehaviour {

	public static StorySystem Instance { get; set; }
    SceneController sceneController;



    public GameObject storyPanel;

    public List<string> storyLines = new List<string>();

    public Button continueButton;
    public AudioSource BGM2;

    public bool turn = false;
    public bool O2B = false;
    bool BGM2OUT = false;

    public Text storyText;
    public int storyIndex;

    int a = 4;

    public AudioSource NextButton;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        continueButton.onClick.AddListener(delegate { ContinueStory(); NextButton.Play(); });
        sceneController = FindObjectOfType<SceneController>();
    }

    private void Update()
    {
        if (BGM2OUT)
        {
            StartCoroutine(BGM2minus());
        }
    }

    private IEnumerator BGM2minus()
    {
        while (BGM2 != null && BGM2.volume > 0f)
        {
            BGM2.volume = BGM2.volume - 0.004f;
            yield return null;
        }
        
    }

    public void AddNewStory(string[] lines)
    {
        a = 4;
        storyIndex = 0;
        storyLines = new List<string>(lines.Length);
        storyLines.AddRange(lines);
        Instance = this;
        CreateStory();
    }

    public void CreateStory()
    {
        storyText.text = storyLines[storyIndex];
    }

    public void ContinueStory()
    {
        if (storyIndex < storyLines.Count - 1)
        {
            storyIndex++;
            if (storyIndex == a)
            {
                storyText.text = storyLines[storyIndex];
                a = a + 5;
            }
            else
            {
                storyText.text = storyText.text + "\n" + "\n" + storyLines[storyIndex];
            }
            if(storyLines[storyIndex] == "...")
            {
                BGM2OUT = true;
                sceneController.BaseFadeAndLoadScene("Base");
                O2B = true;
            }
        }
        else
        {
            turn = true;
        }
        Debug.Log(storyIndex);
    }


}
