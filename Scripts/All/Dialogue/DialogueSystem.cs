using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueSystem : MonoBehaviour {
    public static DialogueSystem Instance { get; set; }
    public GameObject dialoguePanel;
    public GameObject implyPanel;
    [HideInInspector] public List<string> dialogueLines = new List<string>();
    [HideInInspector] public List<string> npcName = new List<string>();
    [HideInInspector] public string implyLine;
    Button continueButton;
    public AudioSource BGM2;

    Text dialogueText, nameText,implyText;
    int dialogueIndex;

    private void Awake()
    {
        //对话
        continueButton = dialoguePanel.transform.Find("Continue").GetComponent<Button>();
        dialogueText = dialoguePanel.transform.Find("Text").GetComponent<Text>();
        nameText = dialoguePanel.transform.Find("Name").GetComponent<Text>();
        continueButton.onClick.AddListener(delegate { ContinueDialogue(); });
        dialoguePanel.SetActive(false);

        //提示
        implyText = implyPanel.transform.Find("ImplyText").GetComponent<Text>();
        implyPanel.SetActive(false);

        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void AddNewDialogue(string[] lines, string[]  NpcName)
    {
        dialogueIndex = 0;
        dialogueLines = new List<string>(lines.Length);
        dialogueLines.AddRange(lines);
        npcName = new List<string>(NpcName.Length);
        npcName.AddRange(NpcName);
        CreateDialogue();
    }

    public void CreateDialogue()
    {
        dialogueText.text = dialogueLines[dialogueIndex];
        nameText.text = npcName[dialogueIndex];
        dialoguePanel.SetActive(true);
    }

    public void CreateImply(string Line)
    {
        implyLine = Line;
        implyPanel.SetActive(true);
        implyText.text = implyLine;
    }

    public void CloseImply()
    {
        implyPanel.SetActive(false);
    }

    public void ContinueDialogue()
    {
        if(dialogueIndex < dialogueLines.Count - 1)
        {
            dialogueIndex++;
            dialogueText.text = dialogueLines[dialogueIndex];
            nameText.text = npcName[dialogueIndex];
        }
        else
        {
            dialoguePanel.SetActive(false);
            myNpcInteraction.isIteracting = false;
        }
    }
}
