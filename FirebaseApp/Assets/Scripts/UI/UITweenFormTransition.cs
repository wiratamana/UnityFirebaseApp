using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITweenFormTransition : MonoBehaviour
{
    [SerializeField] private UITweenTransition[] forms;
    [SerializeField] private float transitionSpeed;

    public event System.Action TransitionCompleted;

    public void Execute()
    {
        RaycastBlocker.Block();
        StartCoroutine(DoTransition());
    }

    private IEnumerator DoTransition()
    {
        float val = 0.0f;

        while (Mathf.Approximately(val, 1.0f) == false)
        {
            val = Mathf.MoveTowards(val, 1.0f, transitionSpeed * Time.deltaTime);
            for (int i = 0; i < forms.Length; i++)
            {
                forms[i].Execute(val);
            }

            yield return null;
        }

        TransitionCompleted?.Invoke();
        RaycastBlocker.Unblock();
    }
}
