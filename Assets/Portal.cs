using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class Portal : MonoBehaviour
{
    public Quaternion _rotation;
    public Portal _secondPortal;
    public List<ItemP> _objects;
    public List<EnemyBullet> _enemyBullets;
    public string _sceneName;
    public string _otherSceneName;
    public Camera _secondCamera;
    private Material _material;
    private RenderTexture _secondCameraRenTex;
    public float _w = 2.5f;
    public float _h = 5f;
    public MeshFilter _meshFilter;
    public MeshRenderer _meshRenderer;
    RenderTexture[] _rts;
    public Vector3[] _worldSecPort;
    public ushort _resolutionIndex;
    Vector2 _sensorSize;
    public Rect _THE_RECT;

    void Start()
    {
        StartCoroutine(LateStart());

        IEnumerator LateStart()
        {
            while (_secondPortal == null)
                yield return new WaitForSeconds(0.32f);

            GameObject camObj = new GameObject("SecondPortalCamera");
            _secondCamera = camObj.AddComponent<Camera>();
            _secondCamera.usePhysicalProperties = true;
            _secondCamera.fieldOfView = S.Camera.fieldOfView;
            _secondCamera.gateFit = Camera.GateFitMode.None;
            _sensorSize = S.Camera.sensorSize;
            _secondCamera.enabled = false;

            MeshInit();
            CreateQuadMesh();
            CreateMaterial();
        }
    }

    private void MeshInit()
    {
        _meshFilter = gameObject.AddComponent<MeshFilter>();
        if (_meshFilter == null)
            _meshFilter = gameObject.AddComponent<MeshFilter>();

        _meshRenderer = GetComponent<MeshRenderer>();
        if (_meshRenderer == null)
            _meshRenderer = gameObject.AddComponent<MeshRenderer>();
    }

    private void CreateQuadMesh()
    {
        Mesh mesh = new Mesh();
        mesh.name = "quad";

        Vector3[] vertices = new Vector3[4];
        vertices[0] = new Vector3(-_w / 2, 0, 0);
        vertices[1] = new Vector3(_w / 2, 0, 0);
        vertices[2] = new Vector3(-_w / 2, _h, 0);
        vertices[3] = new Vector3(_w / 2, _h, 0);

        int[] triangles = new int[] { 0, 1, 2, 2, 1, 3 };
        Vector2[] uv = new Vector2[4]
        {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(0, 1),
            new Vector2(1, 1)
        };

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uv;
        mesh.RecalculateNormals();

        _meshFilter.mesh = mesh;

        Vector3 bottomLeft = transform.TransformPoint(vertices[0]);
        Vector3 bottomRight = transform.TransformPoint(vertices[1]);
        Vector3 topLeft = transform.TransformPoint(vertices[2]);
        Vector3 topRight = transform.TransformPoint(vertices[3]);

        _worldSecPort = new Vector3[]
        {
            bottomLeft,
            bottomRight,
            topLeft,
            topRight
        };
    }

    private void CreateMaterial()
    {
        _rts = new RenderTexture[6];
        //_rts[0] = new RenderTexture(1280, 720, 24); // 100% (исходное)
        _rts[0] = new RenderTexture(1280, 720, 24); // 100% (исходное)
        _rts[0].Create();

        _rts[1] = new RenderTexture(960, 540, 24); // 75% (между 0 и 2)
        _rts[1].Create();

        _rts[2] = new RenderTexture(640, 360, 24); // 50% (исходное)
        _rts[2].Create();

        _rts[3] = new RenderTexture(480, 270, 24); // 37.5% (между 2 и 4)
        _rts[3].Create();

        _rts[4] = new RenderTexture(320, 180, 24); // 25% (исходное)
        _rts[4].Create();

        _rts[5] = new RenderTexture(240, 135, 24); // 18.75% (меньше 4)
        _rts[5].Create();

        _resolutionIndex = 5;

        _secondCamera.targetTexture = _rts[_resolutionIndex]; //

        _material = new Material(Shader.Find("Custom/S5"));
        _material.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);

        _material.mainTexture = _rts[_resolutionIndex];

        GetComponent<MeshRenderer>().material = _material;
    }

    void Update()
    {
        if (_secondPortal == null) return;
        if (_secondCamera == null) return;

        if (S.PS._currentSceneName == _sceneName)
        {
            _secondCamera.enabled = true;
            _meshRenderer.enabled = true;
            UpdateResolution();
            PlaceSecondCamera();
        }
        else
        {
            _secondCamera.enabled = false;
            _meshRenderer.enabled = false;
        }
    }

    private void UpdateResolution()
    {
        ushort resId;
        float baseDist = 10f;
        float dist = (S.Ph.transform.position - transform.position).magnitude;

        if (dist < baseDist * 1 / 0.75f)
            resId = 0;
        else if (dist < baseDist * 1 / 0.5f)
            resId = 1;
        else if (dist < baseDist * 1 / 0.375f)
            resId = 2;
        else if (dist < baseDist * 1 / 0.25f)
            resId = 3;
        else if (dist < baseDist * 1 / 0.1875f)
            resId = 4;
        else
            resId = 5;

        resId = 0; ///////////////

        if (resId != _resolutionIndex)
        {
            _meshRenderer.material.mainTexture = _rts[resId];
            _secondCamera.targetTexture = _rts[resId]; //
            _resolutionIndex = resId;
        }
    }

    private void PlaceSecondCamera()
    {
        Camera mainCam = S.Camera;

        Vector3 relativePosition = transform.InverseTransformPoint(mainCam.transform.position);
        Quaternion relativeRotation = Quaternion.Inverse(transform.rotation) * mainCam.transform.rotation;

        _secondCamera.transform.position = _secondPortal.transform.TransformPoint(relativePosition);
        _secondCamera.transform.rotation = _secondPortal.transform.rotation * relativeRotation;
        _secondCamera.fieldOfView = mainCam.fieldOfView;

        SetObliqueNearPlane();
        WTH(_worldSecPort);
    }

    private void WTH(Vector3[] quad0)
    {
        Vector3[] quad = (Vector3[])quad0.Clone();

        Rect r = GetVisibleRect(quad, S.Camera);

        SetCameraRect(r);      

        _meshRenderer.material.SetFloat("_RectX", r.xMin);
        _meshRenderer.material.SetFloat("_RectY", r.yMin);
        _meshRenderer.material.SetFloat("_RectWidth", r.width);
        _meshRenderer.material.SetFloat("_RectHeight", r.height);

        Rect GetVisibleRect(Vector3[] quad, Camera cam)
        {
            List<Vector2> points = new List<Vector2>();

            for (int i = 0; i < quad.Length; i++)
            {
                Vector3 vp = cam.WorldToViewportPoint(quad[i]);
                if (vp.z >= 0)
                {
                    points.Add(vp);
                }
                else
                {
                    Vector3 fromPointOnPlaneToPoint = quad[i] - cam.transform.position;

                    float halfVert = cam.fieldOfView * Mathf.Deg2Rad * 0.5f;
                    float halfHoriz = Mathf.Atan(Mathf.Tan(halfVert) * cam.aspect);

                    Vector3 bottomNormal = Quaternion.AngleAxis(halfVert * Mathf.Rad2Deg, cam.transform.right) * -cam.transform.up;                 
                    Vector3 topNormal = Quaternion.AngleAxis(-halfVert * Mathf.Rad2Deg, cam.transform.right) * cam.transform.up;               
                    Vector3 leftNormal = Quaternion.AngleAxis(-halfHoriz * Mathf.Rad2Deg, cam.transform.up) * -cam.transform.right;              
                    Vector3 rightNormal = Quaternion.AngleAxis(halfHoriz * Mathf.Rad2Deg, cam.transform.up) * cam.transform.right;

                    float dr = Vector3.Dot(fromPointOnPlaneToPoint, rightNormal);
                    float dl = Vector3.Dot(fromPointOnPlaneToPoint, leftNormal);
                    float dt = Vector3.Dot(fromPointOnPlaneToPoint, topNormal);
                    float db = Vector3.Dot(fromPointOnPlaneToPoint, bottomNormal);

                    float l = vp.x;
                    float r = vp.x;
                    float t = vp.y;
                    float b = vp.y;

                    if (dr < 0f)
                        r = 1000f;
                    else if (dl < 0f)
                        l = -1000f;
                    else
                    {
                        r = 1000f;
                        l = -1000f;
                    }

                    if (dt < 0f)
                        t = 1000f;
                    else if (db < 0f)
                        b = -1000f;
                    else
                    {
                        t = 1000f;
                        b = -1000f;
                    }

                    vp = new Vector3(r, t);
                    points.Add(vp);
                    vp = new Vector3(l, b);
                    points.Add(vp);
                }
            }

            float minX = float.MaxValue, minY = float.MaxValue, maxX = float.MinValue, maxY = float.MinValue;
            foreach (var p in points)
            {
                minX = Mathf.Min(minX, p.x);
                minY = Mathf.Min(minY, p.y);
                maxX = Mathf.Max(maxX, p.x);
                maxY = Mathf.Max(maxY, p.y);
            }

            if (maxX < 0f || minX > 1f || maxY < 0f || minY > 1f)
                return Rect.zero;

            minX = Mathf.Clamp01(minX);
            minY = Mathf.Clamp01(minY);
            maxX = Mathf.Clamp01(maxX);
            maxY = Mathf.Clamp01(maxY);

            return Rect.MinMaxRect(minX, minY, maxX, maxY);
        }

        void SetCameraRect(Rect rect)
        {
            Camera cam = S.Camera;
            float origSW = cam.sensorSize.x;
            float origSH = cam.sensorSize.y;

            float newSW = rect.width * origSW;
            float newSH = rect.height * origSH;

            float newLX = (0.5f - rect.x) / rect.width - 0.5f;
            float newLY = (0.5f - rect.y) / rect.height - 0.5f;

            _secondCamera.sensorSize = new Vector2(newSW, newSH);
            _secondCamera.lensShift = new Vector2(-newLX, -newLY);
            _secondCamera.focalLength = cam.focalLength;
        }
    }

    private float SetObliqueNearPlane()
    {
        float minDistance = float.MaxValue;
        Transform cameraTransform = S.Camera.transform;

        foreach (Vector3 point in _worldSecPort)
        {
            Vector3 directionToPoint = point - cameraTransform.position;

            float distanceToPlane = Vector3.Dot(directionToPoint, cameraTransform.forward);

            if (distanceToPlane < minDistance)
                minDistance = distanceToPlane;
        }

        if (minDistance < 0.001f)
            minDistance = 0.001f;

        if (minDistance == float.MaxValue)
            minDistance = _secondCamera.nearClipPlane;

        _secondCamera.nearClipPlane = minDistance * S.Camera.fieldOfView / _secondCamera.fieldOfView;
        return minDistance;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Vector3 bottomLeft = new Vector3(-_w / 2, 0, 0);
        Vector3 bottomRight = new Vector3(_w / 2, 0, 0);
        Vector3 topLeft = new Vector3(-_w / 2, _h, 0);
        Vector3 topRight = new Vector3(_w / 2, _h, 0);
        Vector3 center = new Vector3(0, _h / 2, 0);
        Vector3 forward = center + transform.forward;

        bottomLeft = transform.TransformPoint(bottomLeft);
        bottomRight = transform.TransformPoint(bottomRight);
        topLeft = transform.TransformPoint(topLeft);
        topRight = transform.TransformPoint(topRight);
        center = transform.TransformPoint(center);
        forward = transform.TransformPoint(forward);

        Gizmos.DrawLine(bottomLeft, bottomRight);
        Gizmos.DrawLine(bottomRight, topRight);
        Gizmos.DrawLine(topRight, topLeft);
        Gizmos.DrawLine(topLeft, bottomLeft);
        Gizmos.DrawLine(center, forward);
    }
#endif
}