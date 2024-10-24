using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallBehaviour : MovingBehaviour
{
    [SerializeField] private Vector3 rollPos;

    // Start is called before the first frame update
    void Start()
    {
        rollPos = this.transform.position;
    }

    /*
    // Update is called once per frame
    void Update()
    {
    }
    */

    private void FixedUpdate()
    {
        UpdatePosition(Time.fixedDeltaTime);
        UpdateRotation(5f);
    }

    protected void UpdateRotation(float angle)
    {
        if (this.transform.position.Equals(rollPos) == false)
        {
            float margin = 0.01f;   // 1cm

            if (Mathf.Abs(this.transform.position.x - rollPos.x) > margin || Mathf.Abs(this.transform.position.z - rollPos.z) > margin)
            {
                this.transform.Rotate(Vector3.right, angle);
            }
            else
            {
                //Debug.Log("No Rotation - " + this.transform.position + " -> " + rollPos);
            }
        }
    }

    public void RollTo(Vector3 destination)
    {
        rollPos = destination;

        this.transform.LookAt(rollPos);
    }
}
