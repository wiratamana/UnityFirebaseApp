using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chat : MonoBehaviour
{
    private static Chat instance;
    public static Chat Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<Chat>();
            }

            return instance;
        }
    }

    public UITweenFormTransition ToHome;

    [Utils.InvokeByUnityButton]
    public void OnClick_Back()
    {
        ToHome.Execute();
    }
}
