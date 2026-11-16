using System.Collections;
using TMPro;
using UnityEngine;

public class InteractIndicator : MonoBehaviour
{
    [SerializeField] GameObject root;

    [SerializeField] Vector2 interactIndicatorOffset;
    [SerializeField] float moveSpeedInteractIndicator = 4f;
    [SerializeField] TMP_Text interactAction_text;
    Coroutine InteractIndicatorLerp;
    Vector2 targetPos;
    public void UpdateIndicator(Vector2 screenPos, string actionName)
    {
        int xFlip = UIManager.instance.IsRightHalfScreen(screenPos) ? 1 : -1;
        int yFlip = UIManager.instance.IsAboveHalfScreen(screenPos) ? 1 : -1;
        screenPos.x += interactIndicatorOffset.x * xFlip;
        screenPos.y += interactIndicatorOffset.y * yFlip;
        targetPos = screenPos;

        interactAction_text.text = actionName;

        if (InteractIndicatorLerp != null)
        {
            StopCoroutine(InteractIndicatorLerp);
            InteractIndicatorLerp = null;
        }
        InteractIndicatorLerp = StartCoroutine(InteractIndicatorLerpCoroutine());
    }

    public void Show()
    {
        if (!root.activeSelf) root.SetActive(true);
    }
    public void Hide()
    {
        root.SetActive(false);

        if (InteractIndicatorLerp != null)
        {
            StopCoroutine(InteractIndicatorLerp);
            InteractIndicatorLerp = null;
        }
    }
    IEnumerator InteractIndicatorLerpCoroutine()
    {
        while (Vector2.Distance(transform.position, targetPos) > 0.1f)
        {
            transform.position = Vector2.Lerp(transform.position, targetPos
                , moveSpeedInteractIndicator * Time.deltaTime);
            yield return null;
        }

        InteractIndicatorLerp = null;
    }
}
