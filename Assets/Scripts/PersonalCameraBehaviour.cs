using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonalCameraBehaviour : MonoBehaviour
{
    public Transform Target;
    public float distance_z = -7f;
    public float distance_y = 4f;

    private GameObject lookAtTarget;

    // Start is called before the first frame update
    void Start()
    {
        lookAtTarget = new GameObject("Personal Camera LookAtTarget");
        lookAtTarget.transform.parent = this.transform.parent;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (Target)
        {
            // Position
            Vector3 camera_pos = Target.position + (Quaternion.Euler(0f, 0f, 0f) * (Target.forward * distance_z));
            camera_pos += new Vector3(0f, distance_y, 0f);
            this.transform.position = camera_pos;

            // Rotation
            lookAtTarget.transform.position = Target.position;
            lookAtTarget.transform.position += new Vector3(0f, distance_y, 0f);
            this.transform.LookAt(lookAtTarget.transform);
        }
    }
}
