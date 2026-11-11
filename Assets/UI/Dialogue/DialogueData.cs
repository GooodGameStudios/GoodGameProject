using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogues/Dialogue")]
public class DialogueData : ScriptableObject
{
    [TextArea(2, 5)]
    public string message;

    public float duration = 1f;
}