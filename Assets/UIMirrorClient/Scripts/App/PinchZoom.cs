using UnityEngine;

public class PinchZoom : MonoBehaviour
{
    public Canvas Canvas;
    public float ZoomSpeed = 0.1f;
    public float MinScale = 0.5f;
    public float MaxScale = 4.0f;    
    public float ScaleFactor = 1.0f;
    void Update()
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
            ScaleFactor -= deltaMagnitudeDiff * ZoomSpeed;

            ScaleFactor = Mathf.Clamp(ScaleFactor, MinScale, MaxScale);
        }
        Canvas.scaleFactor = ScaleFactor;
    }
}