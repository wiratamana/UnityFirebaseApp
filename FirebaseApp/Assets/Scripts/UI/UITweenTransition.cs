using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UITweenTransition
{
    [SerializeField] private AnimationCurve curve;
    [SerializeField] private bool isAxisY;
    [SerializeField] private float endPosition;
    [SerializeField] private RectTransform rectTransform;

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
