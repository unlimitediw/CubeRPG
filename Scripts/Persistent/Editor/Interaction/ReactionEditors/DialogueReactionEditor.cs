using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(DialogueReaction))]
public class DialogueReactionEditor : ReactionEditor {

    private SerializedProperty dialogueProperty;
    private SerializedProperty NameProperty;
    private SerializedProperty implyLineProperty;
    


    private const float messageGUILines = 3f;
    private const float areaWidthOffset = 19f;
    private const float indexGUILines = 1f;

    private const string DialogueReactionPropDialogueName = "dialogue";
    private const string DialogueReactionPropNameName = "Name";
    private const string DialogueReactionPropImplyLineName = "implyLine";


    protected override void Init()
    {
        dialogueProperty = serializedObject.FindProperty(DialogueReactionPropDialogueName);
        NameProperty = serializedObject.FindProperty(DialogueReactionPropNameName);
        implyLineProperty = serializedObject.FindProperty(DialogueReactionPropImplyLineName);
    }


    protected override void DrawReaction()
    {
        ArrayGUI(dialogueProperty);

        ArrayGUI(NameProperty);

        EditorGUILayout.PropertyField(implyLineProperty);

    }

    void ArrayGUI(SerializedProperty dialogueP)
    {
        SerializedProperty arraySizeP = dialogueP.FindPropertyRelative("Array.size");
        EditorGUILayout.PropertyField(arraySizeP);

        EditorGUI.indentLevel++;

        for(int i = 0; i<arraySizeP.intValue; i++) 
        {
            EditorGUILayout.PropertyField(dialogueP.GetArrayElementAtIndex(i));
        }

        EditorGUI.indentLevel--;
    }


    protected override string GetFoldoutLabel()
    {
        return "Dialogue Reaction";
    }
}
