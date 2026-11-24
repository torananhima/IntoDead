using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemUsedIndicator : MonoBehaviour
{
    [SerializeField] GameObject root;
    [SerializeField] Image progressImage;
    [SerializeField] TMP_Text timeText;
    float timer;
    float delay;
    Coroutine countdown;

    public void Initialize(float delay)
    {
        Debug.Log("Initialize");
        timer = 0;
        this.delay = delay;
        Show();
        if (progressImage != null) progressImage.fillAmount = 1f;
        if (timeText != null) timeText.text = delay.ToString();
        if (countdown != null)
        {
            StopCoroutine(countdown);
            countdown = null;
        }
        countdown = StartCoroutine(Countdown());
    }

    private IEnumerator Countdown()
    {
        while (timer < delay)
        {
            timer += Time.deltaTime;
            float progress = timer / delay;
            if (progressImage != null) progressImage.fillAmount = Mathf.InverseLerp(0, 1, progress);
            if (timeText != null) timeText.text = $"{progress.ToString("F1")}";
            yield return null;
        }
        Hide();
    }

    public void Show()
    {
        root.SetActive(true);
    }

    public void Hide()
    {
        if (countdown != null)
        {
            StopCoroutine(countdown);
            countdown = null;
        }
        root.SetActive(false);
    }
}
