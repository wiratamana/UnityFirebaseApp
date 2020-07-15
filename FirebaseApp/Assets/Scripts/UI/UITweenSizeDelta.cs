using System.Collections;
using UnityEngine;

/// <summary>
/// Tween 用のクラスです。<see cref="RectTransform.sizeDelta"/> を変動する。
/// </summary>
public class UITweenSizeDelta : MonoBehaviour
{
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private AnimationCurve curve;
    [SerializeField] private float destinationX;
    [SerializeField] private float destinationY;
    [SerializeField] private bool isSizeX;
    [SerializeField] private bool isSizeY;
    [SerializeField] private float transitionSpeed;

    public event System.Action Update;
    public event System.Action TransitionCompleted;

    /// <summary>
    /// アニメーションを実行する。
    /// </summary>
    public void Execute()
    {
        RaycastBlocker.Block();
        StartCoroutine(DoTransition());
    }

    private IEnumerator DoTransition()
    {
        float startPosX = rectTransform.sizeDelta.x;
        float startPosY = rectTransform.sizeDelta.y;
        float distanceX = destinationX - startPosX;
        float distanceY = destinationY - startPosY;

        float val = 0.0f;

        while (true)
        {
            var sizeDelta = rectTransform.sizeDelta;
            var x = sizeDelta.x;
            var y = sizeDelta.y;
            val = Mathf.MoveTowards(val, 1.0f, transitionSpeed * Time.deltaTime);
            var curvePos = curve.Evaluate(val);

            if (isSizeX)
            {
                x = startPosX + (curvePos * distanceX);
            }

            if (isSizeY)
            {
                y = startPosY + (curvePos * distanceY);
            }

            rectTransform.sizeDelta = new Vector2(x, y);
            Update?.Invoke();

            var isTransitionXCompleted = isSizeX ? isSizeX && Mathf.Approximately(destinationX, x) : true;
            var isTransitionYCompleted = isSizeY ? isSizeY && Mathf.Approximately(destinationY, y) : true;

            if (isTransitionXCompleted && isTransitionYCompleted)
            {
                break;
            }

            yield return null;
        }

        TransitionCompleted?.Invoke();
        RaycastBlocker.Unblock();
    }
}