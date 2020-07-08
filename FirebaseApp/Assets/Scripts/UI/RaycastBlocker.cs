using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RaycastBlocker : MonoBehaviour
{
    private Image image;
    private static RaycastBlocker Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = FindObjectOfType<RaycastBlocker>();
        image = GetComponent<Image>();
    }

    public static void Block()
    {
        Instance.image.raycastTarget = true;
    }

    public static void Unblock()
    {
        Instance.image.raycastTarget = false;
    }
}
