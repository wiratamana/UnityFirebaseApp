using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    public void Open(string message)
    {
        text.text = message;
        background.gameObject.SetActive(true);
        transition.Execute();
    }

    public void Close()
    {
        background.gameObject.SetActive(false);
        transitionClose.Execute();
    }
}
