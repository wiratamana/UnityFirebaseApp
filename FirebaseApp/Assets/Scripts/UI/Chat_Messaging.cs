using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Chat_Messaging : MonoBehaviour
{
    private class SubmitHandler : MonoBehaviour
    {
        public event System.Action<string> Submit;

        private void Update()
        {
#if UNITY_EDITOR
            if (Instance.messageInput.isFocused && string.IsNullOrEmpty(Instance.messageInput.text) == false && (Input.GetKey(KeyCode.RightShift) || Input.GetKey(KeyCode.LeftShift)) && Input.GetKeyDown(KeyCode.Return))
            {
                Submit?.Invoke(Instance.messageInput.text);
            }
#elif UNITY_ANDROID
            if (TouchScreenKeyboard.visible)
            {
                var height = Utils.GetTouchScreenKeyboardHeight(true);    
                Instance.touchscreenKeyboardLayoutElement.preferredHeight = height;
                Instance.touchscreenKeyboardLayoutElement.enabled = true;                
                Instance.messageInputLayoutElement.enabled = false;
            }
            else
            {
                Canvas.ForceUpdateCanvases();
                Instance.touchscreenKeyboardLayoutElement.enabled = false;
                Instance.messageInputLayoutElement.enabled = true;
                instance.VLG.enabled = false;
                instance.VLG.enabled = true;
            }
#endif

            Instance.messageInputLayoutElement.preferredHeight = Instance.messageInput.preferredHeight + 20.0f;
        }
    }

    private static Chat_Messaging instance;
    public static Chat_Messaging Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<Chat_Messaging>();
            }

            return instance;
        }
    }

    public Chat_Messaging_Detail prefab_me;
    public Chat_Messaging_Detail prefab_friend;
    public TMP_InputField messageInput;
    public VerticalLayoutGroup VLG;
    public LayoutElement messageInputLayoutElement;
    public LayoutElement touchscreenKeyboardLayoutElement;

    public Transform content;

    public UITweenFormTransition toChatFriendList;

    private readonly List<Chat_Messaging_Detail> messageDetails = new List<Chat_Messaging_Detail>();
    private ChatRoom metadata;
    private SubmitHandler submitHandler;

    private void Start()
    {
        prefab_me.gameObject.SetActive(false);
        prefab_friend.gameObject.SetActive(false);

        submitHandler = GetComponent<SubmitHandler>();
        if (submitHandler == null)
        {
            submitHandler = gameObject.AddComponent<SubmitHandler>();
        }

        messageInput.onSubmit.AddListener(SendMessageAsync);
        submitHandler.enabled = false;
    }

    public void SetValue(ChatRoom metadata)
    {
        this.metadata = metadata;
        this.metadata.NewMessageReceived += OnNewMessageReceived;

        submitHandler.enabled = true;
        submitHandler.Submit += SendMessageAsync;

        InstantiateMessageObject(metadata.ChatObjects);
    }

    private void OnNewMessageReceived(ReadOnlyCollection<ChatObject> chatObjcets)
    {
        InstantiateMessageObject(chatObjcets);
    }

    [Utils.InvokeByUnity]
    public void OnClick_Back()
    {
        submitHandler.Submit -= SendMessageAsync;
        submitHandler.enabled = false;

        metadata.NewMessageReceived -= OnNewMessageReceived;
        ToChatFriendList();
    }

    [Utils.InvokeByUnity]
    public void OnClick_SendMessage()
    {
        SendMessageAsync(messageInput.text);
    }

    private void ToChatFriendList()
    {
        toChatFriendList.TransitionCompleted += DestroyMessageObjects;
        toChatFriendList.Execute();
    }

    private void InstantiateMessageObject(IEnumerable<ChatObject> chatObjects)
    {
        var userUniqueID = UserData.UserUniqueID;

        foreach (var item in chatObjects)
        {
            var detail = Instantiate(item.UserUniqueID == userUniqueID ? prefab_me : prefab_friend, content);
            messageDetails.Add(detail);
            detail.SetValue(item);
        }
    }

    private void DestroyMessageObjects()
    {
        toChatFriendList.TransitionCompleted -= DestroyMessageObjects;
        foreach (var item in messageDetails)
        {
            Destroy(item);
        }

        messageDetails.Clear();
    }

    private async void SendMessageAsync(string message)
    {
        var chatObject = new ChatObject(message, UserData.UserUniqueID);

        var detail = Instantiate(prefab_me, content);
        messageDetails.Add(detail);
        detail.SetValue(chatObject);

        messageInput.text = null;
        var chatObjectFromServer = await FBSDK.ChatManager.SendMessageAsyncTask(chatObject, metadata.ChatRoomID);
        if (chatObjectFromServer == null)
        {
            detail.SetDateTextValue("送信失敗");
        }
        else
        {
            detail.UpdateValue(new ChatObject(chatObjectFromServer));
        }
    }
}
