using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

[Serializable]
public class UITweenHorizontalTransition
{
    public MonoBehaviour myself;
    public GameObject animationGO;
    public VerticalLayoutGroup contentVLG;

    public UITweenFormTransition openAnimation;
    public UITweenFormTransition closeAnimation;
    public UITweenSizeDelta closeAnimationSize;

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