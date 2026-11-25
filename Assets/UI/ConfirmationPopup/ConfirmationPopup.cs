using System;
using UnityEngine;

public class ConfirmationPopup : MonoBehaviour
{
    [SerializeField] private GameObject popupUI;

    private Action onConfirm;
    private Action onCancel;

    public void Show(Action onConfirm, Action onCancel = null)
    {
        this.onConfirm = onConfirm;
        this.onCancel = onCancel;
        popupUI.SetActive(true);
    }

    public void Confirm()
    {
        popupUI.SetActive(false);
        Action callback = onConfirm;
        ClearCallbacks();
        callback?.Invoke();
    }

    public void Cancel()
    {
        popupUI.SetActive(false);
        Action callback = onCancel;
        ClearCallbacks();
        callback?.Invoke();
    }

    private void ClearCallbacks()
    {
        onConfirm = null;
        onCancel = null;
    }
}
