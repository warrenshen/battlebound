using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Events;
using System.Collections.Generic;

[System.Serializable]
public class UMPSingleton : Singleton<UMPSingleton>
{
    public UMP_ConfirmationDialogUI ConfirmationDialog;
    public UMP_InputDialogUI InputFieldDialog;
    public UMP_TwoInputDialogUI TwoInputFieldDialog;
    public UMP_ThreeInputDialogUI ThreeInputFieldDialog;

    public void ShowConfirmationDialog(
        string title,
        string message,
        UnityAction confirmAction,
        UnityAction cancelAction,
        string cancelLabel = "Cancel",
        string confirmLabel = "Confirm")
    {
        ConfirmationDialog.SetTitle(title);
        ConfirmationDialog.SetMessage(message);

        ConfirmationDialog.SetConfirmAction(confirmAction);
        ConfirmationDialog.SetCancelAction(cancelAction);

        ConfirmationDialog.SetConfirmLabel(confirmLabel);
        ConfirmationDialog.SetCancelLabel(cancelLabel);

        ConfirmationDialog.gameObject.SetActive(true);
    }

    public void ShowInputFieldDialog(
        string title,
        string message,
        UnityAction<UMP_InputDialogUI, string> confirmAction,
        UnityAction cancelAction,
        string cancelLabel = "Cancel",
        string confirmLabel = "Confirm",
        string placeholderMessage = "Enter value...",
        InputField.ContentType contentType = InputField.ContentType.Standard
    )
    {
        InputFieldDialog.SetTitle(title);
        InputFieldDialog.SetMessage(message);

        InputFieldDialog.SetConfirmAction(confirmAction);
        InputFieldDialog.SetCancelAction(cancelAction);

        ConfirmationDialog.SetConfirmLabel(confirmLabel);
        ConfirmationDialog.SetCancelLabel(cancelLabel);

        InputFieldDialog.SetInputPlaceholder(placeholderMessage);
        InputFieldDialog.SetInputContentType(contentType);

        InputFieldDialog.gameObject.SetActive(true);
    }

    public void ShowTwoInputFieldDialog(
        string title,
        string message,
        string contextOne,  //example: "value" => "Enter value..." in placeholder
        string contextTwo,
        UnityAction<UMP_TwoInputDialogUI, string, string> confirmAction,
        UnityAction cancelAction,
        string cancelLabel = "Cancel",
        string confirmLabel = "Confirm",
        InputField.ContentType contentTypeOne = InputField.ContentType.Standard,
        InputField.ContentType contentTypeTwo = InputField.ContentType.Standard
    )
    {
        TwoInputFieldDialog.SetTitle(title);
        TwoInputFieldDialog.SetMessage(message);

        TwoInputFieldDialog.SetConfirmAction(confirmAction);
        TwoInputFieldDialog.SetCancelAction(cancelAction);

        ConfirmationDialog.SetConfirmLabel(confirmLabel);
        ConfirmationDialog.SetCancelLabel(cancelLabel);

        TwoInputFieldDialog.SetInputContextOne(contextOne);
        TwoInputFieldDialog.SetInputContextTwo(contextTwo);

        TwoInputFieldDialog.SetInputContentTypeOne(contentTypeOne);
        TwoInputFieldDialog.SetInputContentTypeTwo(contentTypeTwo);

        TwoInputFieldDialog.gameObject.SetActive(true);
    }

    public void ShowThreeInputFieldDialog(
        string title,
        string message,
        string contextOne,  //example: "value" => "Enter value..." in placeholder
        string contextTwo,
        string contextThree,
        UnityAction<UMP_ThreeInputDialogUI, string, string, string> confirmAction,
        UnityAction cancelAction,
        string cancelLabel = "Cancel",
        string confirmLabel = "Confirm",
        InputField.ContentType contentTypeOne = InputField.ContentType.Standard,
        InputField.ContentType contentTypeTwo = InputField.ContentType.Standard,
        InputField.ContentType contentTypeThree = InputField.ContentType.Standard
    )
    {
        ThreeInputFieldDialog.SetTitle(title);
        ThreeInputFieldDialog.SetMessage(message);

        ThreeInputFieldDialog.SetConfirmAction(confirmAction);
        ThreeInputFieldDialog.SetCancelAction(cancelAction);

        ConfirmationDialog.SetConfirmLabel(confirmLabel);
        ConfirmationDialog.SetCancelLabel(cancelLabel);

        ThreeInputFieldDialog.SetInputContextOne(contextOne);
        ThreeInputFieldDialog.SetInputContextTwo(contextTwo);
        ThreeInputFieldDialog.SetInputContextThree(contextThree);

        ThreeInputFieldDialog.SetInputContentTypeOne(contentTypeOne);
        ThreeInputFieldDialog.SetInputContentTypeOne(contentTypeTwo);
        ThreeInputFieldDialog.SetInputContentTypeThree(contentTypeThree);

        ThreeInputFieldDialog.gameObject.SetActive(true);
    }
}
