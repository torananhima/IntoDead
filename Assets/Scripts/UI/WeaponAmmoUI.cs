
using TMPro;
using UnityEngine;

public class WeaponAmmoUI : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 400f;
    [SerializeField] private Vector2 offset;

    [Header("References")]
    [SerializeField] private TMP_Text currentAmmoText;
    [SerializeField] private TMP_Text maxAmmoText;

    private RectTransform rectTransform;

    private Vector2 defaultPos;
    private Vector2 targetPos;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        defaultPos = rectTransform.anchoredPosition;
        targetPos = defaultPos;
    }

    private void Update()
    {
        if (Vector2.Distance(rectTransform.anchoredPosition, targetPos) > 0.01f)
        {
            rectTransform.anchoredPosition = Vector2.Lerp(
                rectTransform.anchoredPosition,
                targetPos,
                moveSpeed * Time.deltaTime
            );
        }
    }

    public void UpdateAmmoUI(int currentAmmo, int maxAmmo)
    {
        currentAmmoText.text = currentAmmo.ToString();
        maxAmmoText.text = maxAmmo.ToString();
    }

    public void SetTargetPosition(bool isFlip)
    {
        float flipMult = isFlip ? -1f : 1f;

        targetPos = defaultPos + offset * flipMult;
    }
}
