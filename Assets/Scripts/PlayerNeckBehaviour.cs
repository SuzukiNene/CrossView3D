using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNeckBehaviour : MonoBehaviour
{
    public class Neck
    {
        private float srcDir = 0f;
        private float dstDir = 0f;
        private float curDir = 0f;
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

                curDir = srcDir + distance * pastTime / totalTime;
                if (pastTime == totalTime)
                {
                    curDir = dstDir;
                }
            }
        }

        public void UpdateAnimator(Transform tf, Animator animator)
        {
            if (srcDir == 0 && dstDir == 0)
            {
                return;
            }
            if (animator != null)
            {
                // Always based on direction of Body.
                Vector3 lookAt = tf.position + (Quaternion.Euler(0f, curDir, 0f) * (tf.forward * 2f));
                lookAt.y = animator.GetBoneTransform(HumanBodyBones.Head).position.y;

                animator.SetLookAtWeight(1f, 0f, 1f, 0f, 0.5f);
                animator.SetLookAtPosition(lookAt);
            }
        }

        public void TurnTo(Transform tf, float direction, float seconds)
        {
            srcDir = dstDir;
            curDir = srcDir;
            dstDir = direction;
            totalTime = seconds;
            pastTime = 0f;

            if (seconds == 0)
            {
                // Apply immediately.
                curDir = dstDir;
            }
        }
    }
    private Neck neck = new Neck();

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

    protected void UpdateNeckRotation(float delta)
    {
        neck.UpdateRotation(this.transform, delta);
    }

    protected void UpdateNeckAnimator(Animator animator)
    {
        neck.UpdateAnimator(this.transform, animator);
    }

    public void TurnNeckTo(float direction, float seconds)
    {
        neck.TurnTo(this.transform, direction, seconds);
    }
}
