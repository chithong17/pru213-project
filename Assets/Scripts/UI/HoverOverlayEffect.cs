using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HoverOverlayEffect : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerDownHandler,
    IPointerUpHandler
{
    [Header("References")]
    [SerializeField] private RectTransform hoverOverlay;
    [SerializeField] private Image hoverImage;

    [Header("Scale")]
    [SerializeField] private float hoverScale = 1.04f;
    [SerializeField] private float pressedScale = 0.97f;

    [Header("Animation")]
    [SerializeField] private float duration = 0.12f;
    [SerializeField] private float normalAlpha = 0f;
    [SerializeField] private float hoverAlpha = 0.22f;

    [Header("Sound")]
    [SerializeField] private float hoverVolume = 0.65f;
    [SerializeField] private float clickVolume = 1f;

    private Vector3 originalScale;
    private Coroutine animationCoroutine;
    private bool pointerInside;

    private void Awake()
    {
        if (hoverOverlay == null)
        {
            return;
        }

        originalScale = hoverOverlay.localScale;

        if (hoverImage != null)
        {
            SetImageAlpha(normalAlpha);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        pointerInside = true;
        GameAudio.PlaySFX(GameAudio.UiHover, hoverVolume);

        StartAnimation(
            originalScale * hoverScale,
            hoverAlpha
        );
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        pointerInside = false;

        StartAnimation(
            originalScale,
            normalAlpha
        );
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        GameAudio.PlaySFX(GameAudio.UiClick, clickVolume);

        StartAnimation(
            originalScale * pressedScale,
            hoverAlpha
        );
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (pointerInside)
        {
            StartAnimation(
                originalScale * hoverScale,
                hoverAlpha
            );
        }
        else
        {
            StartAnimation(
                originalScale,
                normalAlpha
            );
        }
    }

    private void StartAnimation(
        Vector3 targetScale,
        float targetAlpha
    )
    {
        if (hoverOverlay == null)
        {
            return;
        }

        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
        }

        animationCoroutine = StartCoroutine(
            AnimateRoutine(targetScale, targetAlpha)
        );
    }

    private IEnumerator AnimateRoutine(
        Vector3 targetScale,
        float targetAlpha
    )
    {
        Vector3 startScale = hoverOverlay.localScale;

        float startAlpha = hoverImage != null
            ? hoverImage.color.a
            : 0f;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(elapsed / duration);
            t = t * t * (3f - 2f * t);

            hoverOverlay.localScale = Vector3.Lerp(
                startScale,
                targetScale,
                t
            );

            if (hoverImage != null)
            {
                SetImageAlpha(
                    Mathf.Lerp(startAlpha, targetAlpha, t)
                );
            }

            yield return null;
        }

        hoverOverlay.localScale = targetScale;

        if (hoverImage != null)
        {
            SetImageAlpha(targetAlpha);
        }

        animationCoroutine = null;
    }

    private void SetImageAlpha(float alpha)
    {
        Color color = hoverImage.color;
        color.a = alpha;
        hoverImage.color = color;
    }
}

