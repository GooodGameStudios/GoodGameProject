using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] private DialogueData dialogue;
    [SerializeField] private DialogueUI dialogueUI;

    [SerializeField] private bool triggerOnlyOnce = true;

    private bool hasTriggered;

    //Trigger =xd
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("fsdfsdfdsdfsfsd");
        if (!other.CompareTag("Player")) return;
        if (hasTriggered) return;

        dialogueUI.ShowDialogue(dialogue);

        hasTriggered = true;
    }
}