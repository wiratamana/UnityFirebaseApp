using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Chat_Messaging_Detail : MonoBehaviour
{
    public TextMeshProUGUI message;
    public TextMeshProUGUI date;

    public RectTransform rectTransform;
    public LayoutElement messageLayoutElement;
    public LayoutElement dateLayoutElement;

    public string MessageUniqueID { get; private set; }

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

    public void UpdateValue(ChatObject chatObject)
    {
        date.text = chatObject.ElapsedTimeSinceMessageSentString;
        MessageUniqueID = chatObject.MessageUniqueID;
    }

    public void SetDateTextValue(string text)
    {
        date.text = text;
    }

    private void OnDestroy()
    {
        Destroy(gameObject);
    }
}
