using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// チャット時のメッセージ内容
/// </summary>
public class Chat_Messaging_Detail : MonoBehaviour
{
    public TextMeshProUGUI message;
    public TextMeshProUGUI date;

    public RectTransform rectTransform;
    public LayoutElement messageLayoutElement;
    public LayoutElement dateLayoutElement;

    public string MessageUniqueID { get; private set; }

    /// <summary>
    /// 値を代入する。
    /// </summary>
    /// <param name="chatObject"></param>
    public void SetValue(ChatObject chatObject)
    {
        message.text = chatObject.Message;
        date.text = chatObject.ElapsedTimeSinceMessageSentString;
        MessageUniqueID = chatObject.MessageUniqueID;

        gameObject.SetActive(true);

        IEnumerator setBoxSize()
        {
            yield return null;
            var size = rectTransform.sizeDelta;
            size.y = dateLayoutElement.preferredHeight + Mathf.Max(messageLayoutElement.minHeight, message.preferredHeight);
            rectTransform.sizeDelta = size;
        }
        StartCoroutine(setBoxSize());
    }

    /// <summary>
    /// 値を更新する。
    /// </summary>
    /// <param name="chatObject"></param>
    public void UpdateValue(ChatObject chatObject)
    {
        date.text = chatObject.ElapsedTimeSinceMessageSentString;
        MessageUniqueID = chatObject.MessageUniqueID;
    }

    /// <summary>
    /// 送ったときの経過時間を更新する。
    /// </summary>
    /// <param name="text"></param>
    public void SetDateTextValue(string text)
    {
        date.text = text;
    }

    private void OnDestroy()
    {
        Destroy(gameObject);
    }
}
