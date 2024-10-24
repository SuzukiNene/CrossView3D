using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCameraBehaviour : MonoBehaviour
{
    public GameObject Ball;
    public float screenMargin = 0.45f;
    public float minHeight = 5f;

    private Vector3 iniPosition;
    private Vector3 minPosition;

    // Start is called before the first frame update
    void Start()
    {
        float ratio = minHeight / transform.position.y;

        iniPosition = new Vector3(
            transform.position.x,
            transform.position.y,
            transform.position.z);
        minPosition = new Vector3(
            transform.position.x * ratio,
            transform.position.y * ratio,
            transform.position.z * ratio);
        /*
        Debug.Log("Screen=(" + Screen.width + ", " + Screen.height + ")");
        Debug.Log("Range=(" + (Screen.width * screenMargin) + ", " + (Screen.height * screenMargin) + ", " + 
            (Screen.width * (1f - screenMargin)) + ", " + (Screen.height * (1f - screenMargin)) + ")");
        */
    }

    /*
    // Update is called once per frame
    void Update()
    {
    }
    */

    public void Zoom(float wheel)
    {
        Vector3 step = (wheel * -1) * (iniPosition - minPosition);
        float margin_of_error = 0.1f;

        if (step.y < 0)
        {
            if (Camera.main.transform.position.y + step.y < (minPosition.y - margin_of_error))
            {
                step = Vector3.zero;
            }
        }
        else if (step.y > 0)
        {
            if (Camera.main.transform.position.y + step.y > (iniPosition.y + margin_of_error))
            {
                step = Vector3.zero;
            }
        }

        if (step != Vector3.zero)
        {
            Camera.main.transform.position += step;

            if (Camera.main.transform.position.y == iniPosition.y)
            {
                // Restore scrolling.
                Camera.main.transform.position = iniPosition;
            }
        }
    }

    void LateUpdate()
    {
        // Auto-Scroll
        if (Ball)
        {
            if (Camera.main.transform.position.y != iniPosition.y)
            {
                // If the Ball moves out of range (center area of the Screen with 90-10% Width/Height),
                // Camera moves 0.1meter per frame with same height.
                Vector3 pos = Camera.main.WorldToScreenPoint(Ball.transform.position);
                Vector3 diff = Vector3.zero;
                float ratio = 1f - (Camera.main.transform.position.y - minPosition.y) / (iniPosition.y - minPosition.y);
                float step = 0.2f;

                float CameraWidth = Screen.width * Camera.main.rect.width;
                float CameraHeight = Screen.height * Camera.main.rect.height;
                float screenRangeLeft = CameraWidth * screenMargin * ratio;
                float screenRangeRight = CameraWidth * (1f - screenMargin * ratio);
                float screenRangeTop = CameraHeight * (1f - screenMargin * ratio);
                float screenRangeBottom = CameraHeight * screenMargin * ratio;

                /*
                if (pos.x < screenRangeLeft ||
                    pos.x > screenRangeRight ||
                    pos.y < screenRangeBottom ||
                    pos.y > screenRangeTop)
                {
                    Debug.Log("Ball=(" + pos + ")");
                }
                */
                if (pos.x < screenRangeLeft)
                {
                    diff += new Vector3(-step, 0f, 0f);
                }
                else if (pos.x > screenRangeRight)
                {
                    diff += new Vector3(step, 0f, 0f);
                }
                else
                {
                    // If the Ball stay at center of Field, Camera moves to center.
                    Vector3 center = new Vector3(0f, Ball.transform.position.y, 0f);
                    if (Ball.transform.position == center)
                    {
                        if (pos.x < (CameraWidth / 2 - 1))
                        {
                            diff += new Vector3(-step, 0f, 0f);
                        }
                        else if (pos.x > (CameraWidth / 2 + 1))
                        {
                            diff += new Vector3(step, 0f, 0f);
                        }
                    }
                }
                if (pos.y < screenRangeBottom)
                {
                    diff += new Vector3(0f, 0f, -step);
                }
                else if (pos.y > screenRangeTop)
                {
                    diff += new Vector3(0f, 0f, step);
                }

                if (diff != Vector3.zero)
                {
                    Camera.main.transform.position += diff;
                    //Debug.Log("Ratio=" + (1f - screenMargin * ratio * 2) * 100 + "%");
                }
            }
        }
    }
}
