using UnityEngine.UI;
using UnityEngine.Events;

[System.Serializable]
public class UMPSingleton : Singleton<UMPSingleton>
{
    public UMP_ConfirmationDialogUI ConfirmationDialog;
    public UMP_InputDialogUI InputFieldDialog;
    public UMP_TwoInputDialogUI TwoInputFieldDialog;
    public UMP_ThreeInputDialogUI ThreeInputFieldDialog;
    public UMP_TwoInputDialogUI InputFieldAndTextArea;

    public void ShowConfirmationDialog(
        string title,
        string message,
        UnityAction confirmAction,
        UnityAction cancelAction,
        string confirmLabel = "Confirm",
        string cancelLabel = "Cancel"
    )
    {
        ConfirmationDialog.SetTitle(title);
        ConfirmationDialog.SetMessage(message);

        ConfirmationDialog.SetConfirmAction(confirmAction);

        if (cancelAction == null)
        {
            ConfirmationDialog.SetCancelButtonActive(false);
        }
        else
        {
            ConfirmationDialog.SetCancelAction(cancelAction);
        }

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

        InputFieldDialog.SetConfirmLabel(confirmLabel);
        InputFieldDialog.SetCancelLabel(cancelLabel);

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
        string confirmLabel = "Confirm",
        string cancelLabel = "Cancel",
        InputField.ContentType contentTypeOne = InputField.ContentType.Standard,
        InputField.ContentType contentTypeTwo = InputField.ContentType.Standard
    )
    {
        TwoInputFieldDialog.SetTitle(title);
        TwoInputFieldDialog.SetMessage(message);

        TwoInputFieldDialog.SetConfirmAction(confirmAction);
        TwoInputFieldDialog.SetCancelAction(cancelAction);

        TwoInputFieldDialog.SetConfirmLabel(confirmLabel);
        TwoInputFieldDialog.SetCancelLabel(cancelLabel);

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

        ThreeInputFieldDialog.SetConfirmLabel(confirmLabel);
        ThreeInputFieldDialog.SetCancelLabel(cancelLabel);

        ThreeInputFieldDialog.SetInputContextOne(contextOne);
        ThreeInputFieldDialog.SetInputContextTwo(contextTwo);
        ThreeInputFieldDialog.SetInputContextThree(contextThree);

        ThreeInputFieldDialog.SetInputContentTypeOne(contentTypeOne);
        ThreeInputFieldDialog.SetInputContentTypeOne(contentTypeTwo);
        ThreeInputFieldDialog.SetInputContentTypeThree(contentTypeThree);

        ThreeInputFieldDialog.gameObject.SetActive(true);
    }

    public void ShowInputFieldAndAreaDialog(
        string title,
        string message,
        string contextOne,  //example: "value" => "Enter value..." in placeholder
        string contextTwo,
        UnityAction<UMP_TwoInputDialogUI, string, string> confirmAction,
        UnityAction cancelAction,
        string confirmLabel = "Confirm",
        string cancelLabel = "Cancel",
        InputField.ContentType contentTypeOne = InputField.ContentType.Standard,
        InputField.ContentType contentTypeTwo = InputField.ContentType.Standard
    )
    {
        InputFieldAndTextArea.SetTitle(title);
        InputFieldAndTextArea.SetMessage(message);

        InputFieldAndTextArea.SetConfirmAction(confirmAction);
        InputFieldAndTextArea.SetCancelAction(cancelAction);

        InputFieldAndTextArea.SetConfirmLabel(confirmLabel);
        InputFieldAndTextArea.SetCancelLabel(cancelLabel);

        InputFieldAndTextArea.SetInputContextOne(contextOne);
        InputFieldAndTextArea.SetInputContextTwo(contextTwo);

        InputFieldAndTextArea.SetInputContentTypeOne(contentTypeOne);
        InputFieldAndTextArea.SetInputContentTypeTwo(contentTypeTwo);

        InputFieldAndTextArea.gameObject.SetActive(true);
    }
}
