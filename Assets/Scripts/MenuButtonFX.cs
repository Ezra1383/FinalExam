using UnityEngine;
using UnityEngine.EventSystems;

// Small grow-on-hover effect for menu buttons.
// Uses unscaled time so it still animates while the game is paused (timeScale = 0).
public class MenuButtonFX : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public float hoverScale = 1.06f;
    public float speed = 14f;

    private Vector3 targetScale = Vector3.one;

    void OnDisable()
    {
        targetScale = Vector3.one;
        transform.localScale = Vector3.one;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        targetScale = Vector3.one * hoverScale;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        targetScale = Vector3.one;
    }

    void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.unscaledDeltaTime * speed);
    }
}
