using System;
using System.Collections;

using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 左から現れ、右へ消え。消えた時に Y サイズが０まで縮める。
/// </summary>
[Serializable]
public class UITweenHorizontalTransition
{
    public MonoBehaviour myself;
    public GameObject animationGO;
    public VerticalLayoutGroup contentVLG;

    public UITweenFormTransition openAnimation;
    public UITweenFormTransition closeAnimation;
    public UITweenSizeDelta closeAnimationSize;

    /// <summary>
    /// アニメーションを実行する。
    /// </summary>
    /// <param name="delay"></param>
    public void ActivateAndPlayAnimation(float delay)
    {
        IEnumerator coroutine()
        {
            yield return new WaitForSeconds(delay);

            openAnimation.Execute();
        }

        animationGO.SetActive(false);
        openAnimation.TransitionStarted += ActivateGameObject;
        myself.StartCoroutine(coroutine());
    }

    private void ActivateGameObject()
    {
        openAnimation.TransitionStarted -= ActivateGameObject;
        animationGO.SetActive(true);
    }


    /// <summary>
    /// アニメーションごと破棄する。
    /// </summary>
    /// <param name="delay"></param>
    public void DestroyWithAnimation()
    {
        closeAnimationSize.Update += () =>
        {
            Canvas.ForceUpdateCanvases();
            contentVLG.enabled = false;
            contentVLG.enabled = true;
        };

        closeAnimationSize.TransitionCompleted += () =>
        {
            Canvas.ForceUpdateCanvases();
            contentVLG.enabled = false;
            contentVLG.enabled = true;

            UnityEngine.Object.Destroy(myself.gameObject);
        };

        closeAnimation.TransitionCompleted += closeAnimationSize.Execute;
        closeAnimation.Execute();
    }
}