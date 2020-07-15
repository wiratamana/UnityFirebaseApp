using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Raycast をブロックする。
/// </summary>
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

    /// <summary>
    /// タッチ判定を OFF にする。
    /// </summary>
    public static void Block()
    {
        Instance.image.raycastTarget = true;
    }

    /// <summary>
    /// タッチ判定を ON にする。
    /// </summary>
    public static void Unblock()
    {
        Instance.image.raycastTarget = false;
    }
}
