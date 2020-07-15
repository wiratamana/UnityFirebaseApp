using UnityEngine;

/// <summary>
/// Tween 用のクラス。Tween はコードによってのアニメーションです。
/// </summary>
[System.Serializable]
public class UITweenTransition
{
    [SerializeField] private AnimationCurve curve;
    [SerializeField] private bool isAxisY;
    [SerializeField] private float endPosition;
    [SerializeField] private RectTransform rectTransform;

    // アニメーションを実行する。
    public void Execute(float val)
    {
        var updatedPosition = curve.Evaluate(val) * endPosition;

        if (isAxisY)
        {
            rectTransform.localPosition = new Vector3(0.0f, updatedPosition);
        }
        else
        {
            rectTransform.localPosition = new Vector3(updatedPosition, 0.0f);
        }        
    }
}
