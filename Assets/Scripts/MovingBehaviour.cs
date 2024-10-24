using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingBehaviour : MonoBehaviour
{
    public class Move
    {
        private Vector3 srcPos = Vector3.zero;
        private Vector3 dstPos = Vector3.zero;
        private float totalTime = 0f;
        private float pastTime = 0f;

        public void UpdatePosition(Transform tf, float delta)
        {
            float remainingTime = totalTime - pastTime;

            if (remainingTime > 0f)
            {
                delta = (delta > remainingTime) ? remainingTime : delta;
                pastTime += delta;

                Vector3 distance = (dstPos - srcPos) * delta / totalTime;
                if (pastTime == totalTime)
                {
                    distance = dstPos - tf.position;
                }
                tf.Translate(distance, Space.World);
            }
        }

        public void MoveTo(Transform tf, Vector3 destination, float seconds)
        {
            srcPos = tf.position;
            dstPos = destination;
            totalTime = seconds;
            pastTime = 0f;

            if (seconds == 0)
            {
                // Apply immediately.
                tf.position = dstPos;
            }
        }
    }
    private Move move = new Move();

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

    protected void UpdatePosition(float delta)
    {
        move.UpdatePosition(this.transform, delta);
    }

    public void MoveTo(Vector3 destination, float seconds)
    {
        move.MoveTo(this.transform, destination, seconds);
    }
}
