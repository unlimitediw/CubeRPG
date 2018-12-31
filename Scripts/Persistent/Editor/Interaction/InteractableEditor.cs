using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Interactable))]
public class InteractableEditor : EditorWithSubEditors<ConditionCollectionEditor, ConditionCollection>
{
    private Interactable interactable;
    private SerializedProperty collectionsProperty;
    private SerializedProperty defaultReactionCollectionProperty;


    private const float collectionButtonWidth = 125f;
    private const string interactablePropConditionCollectionsName = "conditionCollections";
    private const string interactablePropDefaultReactionCollectionName = "defaultReactionCollection";


    private void OnEnable ()
    {
        interactable = (Interactable)target;

        collectionsProperty = serializedObject.FindProperty(interactablePropConditionCollectionsName);
        defaultReactionCollectionProperty = serializedObject.FindProperty(interactablePropDefaultReactionCollectionName);
        
        CheckAndCreateSubEditors(interactable.conditionCollections);
    }


    private void OnDisable ()
    {
        CleanupEditors ();
    }


    protected override void SubEditorSetup(ConditionCollectionEditor editor)
    {
        editor.collectionsProperty = collectionsProperty;
    }


    public override void OnInspectorGUI ()
    {
        serializedObject.Update ();
        
        CheckAndCreateSubEditors(interactable.conditionCollections);
        

        for (int i = 0; i < subEditors.Length; i++)
        {
            subEditors[i].OnInspectorGUI ();
            EditorGUILayout.Space ();
        }

        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace ();
        if (GUILayout.Button("Add Collection", GUILayout.Width(collectionButtonWidth)))
        {
            ConditionCollection newCollection = ConditionCollectionEditor.CreateConditionCollection ();
            collectionsProperty.AddToObjectArray (newCollection);
        }
        EditorGUILayout.EndHorizontal ();

        EditorGUILayout.Space ();

        EditorGUILayout.PropertyField (defaultReactionCollectionProperty);

        serializedObject.ApplyModifiedProperties ();
    }
}
