using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// エラーが起きたときにメッセージを出す用。
/// </summary>
public class ErrorDialog : MonoBehaviour
{
    public static ErrorDialog Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Image background;
    [SerializeField] private UITweenFormTransition transition;
    [SerializeField] private UITweenFormTransition transitionClose;
    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    /// <summary>
    /// エラーダイアログを表示する。
    /// </summary>
    /// <param name="message">メッセージ</param>
    public void Open(string message)
    {
        text.text = message;
        background.gameObject.SetActive(true);
        transition.Execute();
    }

    /// <summary>
    /// 表示したエラーダイアログを非表示する。
    /// </summary>
    public void Close()
    {
        background.gameObject.SetActive(false);
        transitionClose.Execute();
    }
}
