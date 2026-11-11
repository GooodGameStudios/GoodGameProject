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
        if (dialogue == null) return;

        if (currentDialogue != null)
            StopCoroutine(currentDialogue);

        currentDialogue = StartCoroutine(DisplayDialogue(dialogue));
    }

    private IEnumerator DisplayDialogue(DialogueData dialogue)
    {
        dialogueText.text = dialogue.message;
        dialogueText.gameObject.SetActive(true);

        yield return new WaitForSeconds(dialogue.duration);

        dialogueText.gameObject.SetActive(false);
        currentDialogue = null;
    }
}