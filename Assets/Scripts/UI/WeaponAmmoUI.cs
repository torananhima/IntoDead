using System.Collections;
using TMPro;
using UnityEngine;

public class WeaponAmmoUI : MonoBehaviour
{
    [SerializeField] float moveSpeed = 4f;
    [SerializeField] Vector2 offset;
    [Header("References")]
    [SerializeField] private TMP_Text currentAmmoText;
    [SerializeField] private TMP_Text maxAmmoText;
    Vector2 defaultPos;
    Vector2 targetPos;
    Coroutine lerpCoroutine;

    private void Start()
    {
        defaultPos = transform.position;
    }
    public void UpdateAmmoUI(int currentAmmo, int maxAmmo)
    {
        currentAmmoText.text = currentAmmo.ToString();
        maxAmmoText.text = maxAmmo.ToString();
    }

    private void Update()
    {
        SetTargetPosition();
    }
    public void SetTargetPosition()
    {
        targetPos = defaultPos + offset;

        transform.position = Vector2.Lerp(transform.position, targetPos
    , moveSpeed * Time.deltaTime);
        //if (lerpCoroutine != null)
        //{
        //    StopCoroutine(lerpCoroutine);
        //    lerpCoroutine = null;
        //}

        //lerpCoroutine = StartCoroutine(LerpToTargetCoroutine());
    }

    //IEnumerator LerpToTargetCoroutine()
    //{
    //    while (Vector2.Distance(transform.position, targetPos) > 0.1f)
    //    {
    //        transform.position = Vector2.Lerp(transform.position, targetPos
    //, moveSpeed * Time.deltaTime);
    //        yield return null;
    //    }

    //    lerpCoroutine = null;
    //}
}
