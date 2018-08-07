using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Events;
using System.Collections.Generic;

public class UMP_InputDialogUI : UMP_ConfirmationDialogUI
{
    [SerializeField]
    private InputField inputField;

    public void SetConfirmAction<T>(UnityAction<T> action)
    {
        this.confirmButton.onClick.RemoveAllListeners();
        //this.confirmButton.onClick.AddListener(() => action.Invoke(context));
        this.confirmButton.onClick.AddListener(Close);
    }

    public void SetCancelAction<T>(UnityAction<T> action)
    {
        this.cancelButton.onClick.RemoveAllListeners();
        //this.cancelButton.onClick.AddListener(() => action.Invoke(context));
        this.cancelButton.onClick.AddListener(Close);
    }

    public void SetInputPlaceholder(ref InputField inputField, string placeholder)
    {
        Text placeholderText = (Text)inputField.placeholder;
        placeholderText.text = placeholder;
    }

    public string GetInputValue()
    {
        return inputField.text;
    }
}
