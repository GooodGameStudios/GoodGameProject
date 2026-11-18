using System.Collections;
using UnityEngine;
using TMPro;

public class DialogueUI : MonoBehaviour
{
    [SerializeField] private TMP_Text dialogueText;

    private Coroutine currentDialogue;

    private void Awake()
    {
        dialogueText.gameObject.SetActive(false);
    }


    public void ShowDialogue(DialogueData dialogue)
    {
        if (dialogue == null) 
            return;

        if (currentDialogue != null)
            StopCoroutine(currentDialogue);
        currentDialogue = StartCoroutine(DisplayDialogue(dialogue));
    }


    private IEnumerator DisplayDialogue(DialogueData dialogue)
    {
        dialogueText.gameObject.SetActive(true);

        foreach (DialogueData.DialogueLine line in dialogue.lines)
        {
            if (line == null) continue;

            dialogueText.text = line.message;

            yield return new WaitForSeconds(line.duration);
        }

        dialogueText.gameObject.SetActive(false);
        currentDialogue = null;
    }
}