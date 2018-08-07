using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Events;
using System.Collections.Generic;

public class UMP_InputDialogUI : UMP_ConfirmationDialogUI
{
    [SerializeField]
    private InputField inputField;

    public void SetConfirmAction(UnityAction<UMP_InputDialogUI, string> action)
    {
        this.confirmButton.onClick.RemoveAllListeners();
        this.confirmButton.onClick.AddListener(() => action.Invoke(this, GetInputValue()));
    }

    public void SetInputPlaceholder(string placeholder)
    {
        Text placeholderText = (Text)inputField.placeholder;
        placeholderText.text = placeholder;
    }

    public void SetInputContentType(InputField.ContentType contentType)
    {
        inputField.contentType = contentType;
    }

    public string GetInputValue()
    {
        return inputField.text;
    }
}

public class UMP_TwoInputDialogUI : UMP_ConfirmationDialogUI
{
    [SerializeField]
    private InputField inputFieldOne;

    [SerializeField]
    private InputField inputFieldTwo;

    public void SetConfirmAction(UnityAction<UMP_TwoInputDialogUI, string, string> action)
    {
        this.confirmButton.onClick.RemoveAllListeners();
        this.confirmButton.onClick.AddListener(() => action.Invoke(this, GetInputOneValue(), GetInputTwoValue()));
    }

    //Begin setting placeholders
    public void SetInputOnePlaceholder(string placeholder)
    {
        Text placeholderText = (Text)inputFieldOne.placeholder;
        placeholderText.text = placeholder;
    }

    public void SetInputTwoPlaceholder(string placeholder)
    {
        Text placeholderText = (Text)inputFieldTwo.placeholder;
        placeholderText.text = placeholder;
    }

    //Begin contentType
    public void SetInputOneContentType(InputField.ContentType contentType)
    {
        inputFieldOne.contentType = contentType;
    }

    public void SetInputTwoContentType(InputField.ContentType contentType)
    {
        inputFieldTwo.contentType = contentType;
    }

    //Get inputField values
    public string GetInputOneValue()
    {
        return inputFieldOne.text;
    }

    public string GetInputTwoValue()
    {
        return inputFieldTwo.text;
    }
}

public class UMP_ThreeInputDialogUI : UMP_ConfirmationDialogUI
{
    [SerializeField]
    private InputField inputFieldOne;

    [SerializeField]
    private InputField inputFieldTwo;

    [SerializeField]
    private InputField inputFieldThree;

    public void SetConfirmAction(UnityAction<UMP_ThreeInputDialogUI, string, string, string> action)
    {
        this.confirmButton.onClick.RemoveAllListeners();
        this.confirmButton.onClick.AddListener(() => action.Invoke(this, GetInputOneValue(), GetInputTwoValue(), GetInputThreeValue()));
    }

    //Begin setting placeholders
    public void SetInputOnePlaceholder(string placeholder)
    {
        Text placeholderText = (Text)inputFieldOne.placeholder;
        placeholderText.text = placeholder;
    }

    public void SetInputTwoPlaceholder(string placeholder)
    {
        Text placeholderText = (Text)inputFieldTwo.placeholder;
        placeholderText.text = placeholder;
    }

    public void SetInputThreePlaceholder(string placeholder)
    {
        Text placeholderText = (Text)inputFieldThree.placeholder;
        placeholderText.text = placeholder;
    }

    //Begin contentType
    public void SetInputOneContentType(InputField.ContentType contentType)
    {
        inputFieldOne.contentType = contentType;
    }

    public void SetInputTwoContentType(InputField.ContentType contentType)
    {
        inputFieldTwo.contentType = contentType;
    }

    public void SetInputThreeContentType(InputField.ContentType contentType)
    {
        inputFieldTwo.contentType = contentType;
    }

    //Get inputField values
    public string GetInputOneValue()
    {
        return inputFieldOne.text;
    }

    public string GetInputTwoValue()
    {
        return inputFieldTwo.text;
    }

    public string GetInputThreeValue()
    {
        return inputFieldThree.text;
    }
}
