using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueReaction : Reaction {


    public string[] dialogue;
    public string[] Name;
    public string implyLine;

    private DialogueSystem dialogueSystem;

    protected override void SpecificInit()
    {
        dialogueSystem = FindObjectOfType<DialogueSystem>();
    }

    protected override void ImmediateReaction()
    {
        if (myNpcInteraction.isIteracting)
        {
            dialogueSystem.CloseImply();
            dialogueSystem.AddNewDialogue(dialogue, Name);
        }
        else if(!myNpcInteraction.interactPermit)
        {
            dialogueSystem.CloseImply();
        }
        else
        {
            dialogueSystem.CreateImply(implyLine);
        }
    }
}

