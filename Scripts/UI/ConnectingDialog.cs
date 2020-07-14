using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ConnectingDialog : MonoBehaviour
{
    [SerializeField] private Image blockerButton;
    private List<GameObject> gameObjects;
    private TextMeshProUGUI text;
    private Button button;

    private static ConnectingDialog Instance;
    private static bool autoHideAfterSucceeded;
    public static event System.Action<string> OnHide;

    public const string MESSAGE_CONNECTING = "接続中...";
    public const string MESSAGE_FAILED = "失敗";
    public const string MESSAGE_SUCCEEDED = "成功";

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = FindObjectOfType<ConnectingDialog>();

        gameObjects = new List<GameObject>();
        for (int i = 0; i < transform.childCount; i++)
        {
            gameObjects.Add(transform.GetChild(i).gameObject);
        }

        text = GetComponentInChildren<TextMeshProUGUI>(true);
        button = GetComponentInChildren<Button>(true);

        button.interactable = false;
        button.onClick.AddListener(HideMessage);
    }

    public static void Connecting(bool autoHideAfterSucceeded = false)
    {
        SetActive(true);
        Instance.text.text = MESSAGE_CONNECTING;
        Instance.button.interactable = false;
        Instance.blockerButton.raycastTarget = false;

        ConnectingDialog.autoHideAfterSucceeded = autoHideAfterSucceeded;
    }

    public static void Failed()
    {
        SetActive(true);
        Instance.text.text = MESSAGE_FAILED;
        Instance.button.interactable = true;
        Instance.blockerButton.raycastTarget = true;
    }

    public static void Success()
    {
        SetActive(true);
        Instance.text.text = MESSAGE_SUCCEEDED;
        Instance.button.interactable = true;
        Instance.blockerButton.raycastTarget = true;

        if (autoHideAfterSucceeded)
        {
            HideMessage();
        }
    }

    private static void HideMessage()
    {
        OnHide?.Invoke(Instance.text.text);
        SetActive(false);
    }

    private static void SetActive(bool value)
    {
        for (int i = 0; i < Instance.gameObjects.Count; i++)
        {
            Instance.gameObjects[i].SetActive(value);
        }
    }
}
