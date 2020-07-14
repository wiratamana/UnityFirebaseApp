using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UITweenImageColor : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public const float COLOR_CHANGE_SPEED = 5.0f;

    [SerializeField] private Gradient highlightColor;

    private Image image;

    private float targetColor;
    private Coroutine changeColor;

    private void OnValidate()
    {
        image = GetComponent<Image>();
        if (image == null)
        {
            Destroy(this);
        }

        image.raycastTarget = true;
        image.color = highlightColor.Evaluate(0);
    }

    private void Start()
    {
        image = GetComponent<Image>();
        if (image == null)
        {
            Destroy(this);
        }
    }

    private void OnEnable()
    {
        targetColor = 0;
    }

    private void OnDisable()
    {
        targetColor = 0;

        if (changeColor != null)
        {
            StopCoroutine(ChangeColorCoroutine());
            changeColor = null;
        }        
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        targetColor = 1.0f;

        if (changeColor != null)
        {
            return;
        }

        changeColor = StartCoroutine(ChangeColorCoroutine());
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        targetColor = 0.0f;

        if (changeColor != null)
        {
            return;
        }

        changeColor = StartCoroutine(ChangeColorCoroutine());
    }

    private IEnumerator ChangeColorCoroutine()
    {
        float current = Mathf.Abs(targetColor - 1.0f);

        while (Mathf.Approximately(current, targetColor) == false)
        {
            current = Mathf.MoveTowards(current, targetColor, COLOR_CHANGE_SPEED * Time.deltaTime);
            image.color = highlightColor.Evaluate(current);
            yield return null;
        }

        changeColor = null;
    }
}
