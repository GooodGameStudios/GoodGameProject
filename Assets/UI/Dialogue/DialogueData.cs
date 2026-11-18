using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogues/Dialogue")]
public class DialogueData : ScriptableObject
{
    [Serializable]
    public class DialogueLine
    {
        [TextArea(2, 5)]
        public string message;
        public float duration = 1f;
    }

    public DialogueLine[] lines;
}