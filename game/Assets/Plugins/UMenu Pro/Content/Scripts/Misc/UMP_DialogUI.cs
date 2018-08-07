using UnityEngine;
using UnityEngine.UI;

public class UMP_DialogUI : MonoBehaviour
{
    [Header("Settings")]
    public string AnimationParamenter = "show";

    [Header("References")]
    [SerializeField]
    private Text message;

    [SerializeField]
    private Animator m_Animator;

    void OnEnable()
    {
        if (m_Animator)
        {
            m_Animator.SetBool(AnimationParamenter, true);
        }
    }

    public void Close()
    {
        if (m_Animator)
        {
            m_Animator.SetBool(AnimationParamenter, false);
            Invoke("Deactive", m_Animator.GetCurrentAnimatorStateInfo(0).length);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    public void SetMessage(string infoText)
    {
        message.text = infoText;
    }

    public void SetMessageColor(Color color)
    {
        message.color = color;
    }

    void Deactive()
    {
        gameObject.SetActive(false);
    }
}