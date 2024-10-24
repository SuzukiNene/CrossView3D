using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MovingBehaviour
{
    public class Body
    {
        private float srcDir = 0f;
        private float dstDir = 0f;
        private float totalTime = 0f;
        private float pastTime = 0f;

        public void UpdateRotation(Transform tf, float delta)
        {
            float remainingTime = totalTime - pastTime;

            if (remainingTime > 0f)
            {
                delta = (delta > remainingTime) ? remainingTime : delta;
                pastTime += delta;

                float distance = (dstDir - srcDir);
                distance = (distance > 180) ? (distance - 360) : (distance < -180) ? (distance + 360) : distance;

                float angle = srcDir + distance * pastTime / totalTime;
                if (pastTime == totalTime)
                {
                    angle = dstDir;
                }
                tf.rotation = Quaternion.AngleAxis(angle, Vector3.up);
            }
        }

        public void TurnTo(Transform tf, float direction, float seconds)
        {
            srcDir = tf.rotation.eulerAngles.y;
            dstDir = direction;
            totalTime = seconds;
            pastTime = 0f;

            if (seconds == 0)
            {
                // Apply immediately.
                tf.rotation = Quaternion.AngleAxis(dstDir, Vector3.up);
            }
        }
    }
    private Body body = new Body();

    /*
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    */

    protected void UpdateRotation(float delta)
    {
        body.UpdateRotation(this.transform, delta);
    }

    public void TurnTo(float direction, float seconds)
    {
        body.TurnTo(this.transform, direction, seconds);
    }
}
