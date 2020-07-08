using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UserInfoBase : MonoBehaviour
{
    public UserInfoUnityUI userinfo;

    public GameObject parent;
    public VerticalLayoutGroup contentVLG;

    public UITweenFormTransition openAnimation;
    public UITweenFormTransition closeAnimation;
    public UITweenSizeDelta closeAnimationSize;
    public RectTransform rectTransform;

    protected string playerUniqueID;

    public void SetValue(UserInfoData userInfoData)
    {
        playerUniqueID = userInfoData.UserUniqueID;
        userinfo.SetValue(userInfoData);
    }

    public void ActivateAndPlayAnimation(float delay)
    {
        IEnumerator coroutine()
        {
            yield return new WaitForSeconds(delay);

            parent.gameObject.SetActive(true);
            openAnimation.Execute();
        }

        StartCoroutine(coroutine());
    }

    protected void DestroyWithAnimation()
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

            Destroy(gameObject);
        };

        closeAnimation.TransitionCompleted += closeAnimationSize.Execute;
        closeAnimation.Execute();
    }

    private void OnDestroy()
    {
        Destroy(gameObject);
    }
}
