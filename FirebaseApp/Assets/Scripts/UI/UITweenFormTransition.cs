using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITweenFormTransition : MonoBehaviour
{
    [SerializeField] private UITweenTransition[] forms;
    [SerializeField] private float transitionSpeed;

    public event System.Action TransitionStarted;
    public event System.Action TransitionCompleted;
    private static int sample = 5;
    private static float threshold = 0.01f;

    public void Execute()
    {
        RaycastBlocker.Block();
        StartCoroutine(DoTransition());
    }

    private IEnumerator DoTransition()
    {
        yield return new WaitUntilFrameRateStable();

        float val = 0.0f;
        UpdateForms(val);

        TransitionStarted?.Invoke();

        while (Mathf.Approximately(val, 1.0f) == false)
        {
            val = Mathf.MoveTowards(val, 1.0f, transitionSpeed * Time.deltaTime);
            UpdateForms(val);
            yield return null;
        }

        TransitionCompleted?.Invoke();
        RaycastBlocker.Unblock();
    }

    private void UpdateForms(float val)
    {
        for (int i = 0; i < forms.Length; i++)
        {
            forms[i].Execute(val);
        }
    }
}
