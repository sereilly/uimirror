using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PanZoom : MonoBehaviour, IDragHandler
{
    public float Zoom = 1.0f;
    public float ZoomSpeed = 0.01f;
    public float MinZoom = 1.0f;
    public float MaxZoom = 4.0f;

    [SerializeField]
    protected RectTransform canvasTransform;
    [SerializeField]
    protected Camera clientCamera;

    private Rect CameraBounds
    {
        get
        {
            Vector3 tr = clientCamera.ViewportToWorldPoint(new Vector3(1, 1, clientCamera.nearClipPlane));
            Vector3 br = clientCamera.ViewportToWorldPoint(new Vector3(1, 0, clientCamera.nearClipPlane));
            Vector3 bl = clientCamera.ViewportToWorldPoint(new Vector3(0, 0, clientCamera.nearClipPlane));
            Vector3 tl = clientCamera.ViewportToWorldPoint(new Vector3(0, 1, clientCamera.nearClipPlane));
            Rect cameraBounds = new Rect(tl.x, bl.y, tr.x - tl.x, tr.y - br.y);
            return cameraBounds;
        }
    }

    protected void Update()
    {
        UpdatePan();
        UpdateZoom();
    }

    private void UpdatePan()
    {
        Rect cameraBounds = CameraBounds;

        if (cameraBounds.width < canvasTransform.rect.width)
        {
            if (cameraBounds.xMin < canvasTransform.rect.xMin)
            {
                clientCamera.transform.localPosition -= Vector3.right * (cameraBounds.xMin - canvasTransform.rect.xMin);
            }
            else if (cameraBounds.xMax > canvasTransform.rect.xMax)
            {
                clientCamera.transform.localPosition -= Vector3.right * (cameraBounds.xMax - canvasTransform.rect.xMax);
            }
        }
        else
        {
            Vector3 cameraPosition = clientCamera.transform.localPosition;
            cameraPosition.x = 0;
            clientCamera.transform.localPosition = cameraPosition;
        }

        if (cameraBounds.height < canvasTransform.rect.height)
        {
            if (cameraBounds.yMin < canvasTransform.rect.yMin)
            {
                clientCamera.transform.localPosition -= Vector3.up * (cameraBounds.yMin - canvasTransform.rect.yMin);
            }
            else if (cameraBounds.yMax > canvasTransform.rect.yMax)
            {
                clientCamera.transform.localPosition -= Vector3.up * (cameraBounds.yMax - canvasTransform.rect.yMax);
            }
        }
        else
        {
            Vector3 cameraPosition = clientCamera.transform.localPosition;
            cameraPosition.y = 0;
            clientCamera.transform.localPosition = cameraPosition;
        }
    }

    private void UpdateZoom()
    {
        // If there are two touches on the device...
        if (Input.touchCount == 2)
        {
            // Store both touches.
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            // Find the position in the previous frame of each touch.
            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            // Find the magnitude of the vector (the distance) between the touches in each frame.
            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            // Find the difference in the distances between each frame.
            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

            // ... change the canvas size based on the change in distance between the touches.
            Zoom -= deltaMagnitudeDiff * ZoomSpeed;

            Zoom = Mathf.Clamp(Zoom, MinZoom, MaxZoom);
        }
        
        clientCamera.orthographicSize = canvasTransform.sizeDelta.y / 2 / Zoom;
    }

    private bool TouchingSelectable(Vector2 position)
    {
        List<RaycastResult> results = RaycastCanvas(position);

        foreach (RaycastResult result in results)
        {
            Selectable selectable = result.gameObject.GetComponentInParent<Selectable>();
            if (selectable)
            {
                return true;
            }
        }
        return false;
    }

    private List<RaycastResult> RaycastCanvas(Vector2 position)
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = position;
        List<RaycastResult> results = new List<RaycastResult>();
        GraphicRaycaster raycaster = GetComponent<GraphicRaycaster>();
        raycaster.Raycast(eventData, results);
        return results;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (Input.touchCount == 1)
        {
            Vector3 worldDelta = clientCamera.ScreenToWorldPoint(eventData.delta) - clientCamera.ScreenToWorldPoint(Vector3.zero);
            clientCamera.transform.position -= worldDelta;
        }
    }
}
