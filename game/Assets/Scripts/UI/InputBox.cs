using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputBox : ObjectUI
{
    private TextMesh textMesh;
    private string placeholder;


    // Use this for initialization
    void Start()
    {
        base.Initialize();
        textMesh = gameObject.GetComponent<TextMesh>();
        placeholder = textMesh.text;
    }

    private void Update()
    {
        if (selected)
            ListenForInput();
    }

    public override void MouseUp()
    {
        Select();
        textMesh.text = "";
    }

    private void ListenForInput()
    {
        //need to make mobil keyboard appear
        textMesh.text += Input.inputString;
        if (Input.GetKeyDown(KeyCode.Backspace) ||
           Input.GetKeyDown(KeyCode.Delete) &&
           textMesh.text.Length > 1)
        {
            textMesh.text = textMesh.text.Substring(0, textMesh.text.Length - 2);
        }
        //TODO: tweak for mobile
        if (Input.GetKey(KeyCode.KeypadEnter) || Input.GetKey(KeyCode.Return))
        {
            Deselect();
        }
    }
}
