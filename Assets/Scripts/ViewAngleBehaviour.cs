using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewAngleBehaviour : MonoBehaviour
{
    public float viewAngle = 90f;
    public float viewRange = 10f;
    public float fanAngleStep = 5f;
    public float fanShapeRange = 0.5f;
    public float fanShapeHeight = 0.01f;
    public Material material;

    private GameObject fanShape;
    private Mesh fanMesh;
    private Light spotLight;

    public class FanShape
    {
        public static GameObject CreateObject(Mesh mesh, Material material)
        {
            var fan = new GameObject("FanShape");

            var meshFilter = fan.AddComponent<MeshFilter>();
            meshFilter.mesh = mesh;

            var meshRenderer = fan.AddComponent<MeshRenderer>();
            meshRenderer.material = material;

            return fan;
        }

        public static Mesh CreateMesh(float angle, float range, float angleStep)
        {
            var mesh = new Mesh();

            UpdateMesh(mesh, angle, range, angleStep);

            return mesh;
        }

        public static void UpdateMesh(Mesh mesh, float angle, float range, float angleStep)
        {
            var vertices = CreateVertices(angle, range, angleStep);
            var triangles = CreateTriangles(vertices.Length - 2);

            mesh.Clear();
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
        }

        private static Vector3[] CreateVertices(float angle, float range, float angleStep)
        {
            if (angle <= 0f || angle > 360f)
            {
                return null;
            }
            if (angleStep < 1f || angleStep > (360f / 3))
            {
                return null;
            }

            int count = (int)((angle + angleStep) / angleStep) - 1;
            var vertices = new List<Vector3>(1 + count + 1);

            float radian = angle * Mathf.Deg2Rad;
            float startRad = -(radian / 2);
            float radianStep = radian / count;

            vertices.Add(Vector3.zero);
            for (int i = 0; i < count; i++)
            {
                float currRad = startRad + radianStep * i;
                Vector3 vertex = new Vector3(Mathf.Sin(currRad) * range, 0f, Mathf.Cos(currRad) * range);
                vertices.Add(vertex);
            }
            {   // Last vertex
                float lastRad = startRad + radian;
                Vector3 vertex = new Vector3(Mathf.Sin(lastRad) * range, 0f, Mathf.Cos(lastRad) * range);
                vertices.Add(vertex);
            }
            return vertices.ToArray();
        }

        private static int[] CreateTriangles(int count)
        {
            var triangles = new List<int>(count * 3);

            for (int i = 0; i < count; i++)
            {
                triangles.Add(0);
                triangles.Add(i + 1);
                triangles.Add(i + 2);
            }
            return triangles.ToArray();
        }
    }

    public class View
    {
        private float srcBody = 0f;
        private float dstBody = 0f;
        private float srcNeck = 0f;
        private float dstNeck = 0f;
        private float totalTime = 0f;
        private float pastTime = 0f;

        public void UpdateRotation(Transform tf, float delta)
        {
            float remainingTime = totalTime - pastTime;

            if (remainingTime > 0f)
            {
                delta = (delta > remainingTime) ? remainingTime : delta;
                pastTime += delta;

                float distanceBody = (dstBody - srcBody);
                distanceBody = (distanceBody > 180) ? (distanceBody - 360) : (distanceBody < -180) ? (distanceBody + 360) : distanceBody;

                float angleBody = srcBody + distanceBody * pastTime / totalTime;
                if (pastTime == totalTime)
                {
                    angleBody = dstBody;
                }

                float distanceNeck = (dstNeck - srcNeck);
                distanceNeck = (distanceNeck > 180) ? (distanceNeck - 360) : (distanceNeck < -180) ? (distanceNeck + 360) : distanceNeck;

                float angleNeck = srcNeck + distanceNeck * pastTime / totalTime;
                if (pastTime == totalTime)
                {
                    angleNeck = dstNeck;
                }

                float angle = angleBody + angleNeck;
                tf.rotation = Quaternion.AngleAxis(angle, Vector3.up);
            }
        }

        public void TurnTo(Transform tf, float body, float neck, float seconds)
        {
            srcBody = dstBody;
            dstBody = body;
            srcNeck = dstNeck;
            dstNeck = neck;
            totalTime = seconds;
            pastTime = 0f;

            if (seconds == 0)
            {
                // Apply immediately.
                float angle = body + neck;
                tf.rotation = Quaternion.AngleAxis(angle, Vector3.up);
            }
        }
    }
    private View view = new View();

    public class AngleRange
    {
        private readonly float fanAngleStep;
        private readonly float fanShapeRange;

        private float srcAngle = 0f;
        private float dstAngle = 0f;
        private float srcRange = 0f;
        private float dstRange = 0f;
        private float totalTime = 0f;
        private float pastTime = 0f;

        public AngleRange(float angle, float range, float angleStep, float fanRange)
        {
            dstAngle = angle;
            dstRange = range;
            fanAngleStep = angleStep;
            fanShapeRange = fanRange;
        }

        public void UpdateAngle(Mesh mesh, Light spot, float delta)
        {
            float remainingTime = totalTime - pastTime;

            if (remainingTime > 0f)
            {
                delta = (delta > remainingTime) ? remainingTime : delta;
                pastTime += delta;

                float distanceAngle = (dstAngle - srcAngle);
                float angle = srcAngle + distanceAngle * pastTime / totalTime;
                if (pastTime == totalTime)
                {
                    angle = dstAngle;
                }

                float distanceRange = (dstRange - srcRange);
                float range = srcRange + distanceRange * pastTime / totalTime;
                if (pastTime == totalTime)
                {
                    range = dstRange;
                }

                // FanShape
                FanShape.UpdateMesh(mesh, angle, fanShapeRange, fanAngleStep);
                // Spotlight
                spot.spotAngle = angle;
                spot.range = range;
            }
        }

        public void ChangeTo(Mesh mesh, Light spot, float angle, float range, float seconds)
        {
            srcAngle = dstAngle;
            dstAngle = angle;
            srcRange = dstRange;
            dstRange = range;
            totalTime = seconds;
            pastTime = 0f;

            if (seconds == 0)
            {
                // Apply immediately.
                // FanShape
                FanShape.UpdateMesh(mesh, angle, fanShapeRange, fanAngleStep);
                // Spotlight
                spot.spotAngle = angle;
                spot.range = range;
            }
        }
    }
    private AngleRange angleRange = null;

    // Start is called before the first frame update
    void Start()
    {
        fanMesh = FanShape.CreateMesh(viewAngle, fanShapeRange, fanAngleStep);
        fanShape = FanShape.CreateObject(fanMesh, material);
        fanShape.transform.parent = this.transform;
        fanShape.transform.position = this.transform.position + Vector3.up * fanShapeHeight;

        spotLight = GetComponentInChildren<Light>();
        spotLight.spotAngle = viewAngle;
        spotLight.range = viewRange;

        angleRange = new AngleRange(viewAngle, viewRange, fanAngleStep, fanShapeRange);
    }

    private void OnDestroy()
    {
        Destroy(fanShape);
    }

    private void OnDrawGizmosSelected()
    {
        var mesh = FanShape.CreateMesh(viewAngle, fanShapeRange, fanAngleStep);
        Vector3 pos = this.transform.position + Vector3.up * fanShapeHeight;

        Gizmos.color = material.color;
        Gizmos.DrawMesh(mesh, pos);
    }

    private void FixedUpdate()
    {
        view.UpdateRotation(this.transform, Time.fixedDeltaTime);
        angleRange.UpdateAngle(fanMesh, spotLight, Time.fixedDeltaTime);
    }

    public void TurnTo(float body, float neck, float seconds)
    {
        view.TurnTo(this.transform, body, neck, seconds);
    }

    public void ChangeAngle(float angle, float range, float seconds)
    {
        if (angle <= 0f || angle > 360f)
        {
            return;
        }
        if (range <= 0f)
        {
            return;
        }

        viewAngle = angle;
        viewRange = range;
        angleRange.ChangeTo(fanMesh, spotLight, angle, range, seconds);
    }
}
