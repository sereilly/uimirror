using UnityEngine;
using UnityEngine.EventSystems;

public class ConsoleDrag : MonoBehaviour, IDragHandler
{
    [SerializeField]
    protected RectTransform consolePanel;

    public void OnDrag(PointerEventData data)
    {
        float scaleFactor = consolePanel.GetComponentInParent<Canvas>().scaleFactor;
        Vector2 sizeDelta = consolePanel.sizeDelta;
        sizeDelta.y = Mathf.Clamp(Screen.height - data.position.y, 0, Screen.height - 30 * scaleFactor) / scaleFactor;
        if (sizeDelta.y < 20)
        {
            sizeDelta.y = 0;
        }
        consolePanel.sizeDelta = sizeDelta;
    }
}