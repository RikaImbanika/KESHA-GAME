using System;
using System.Collections;
using System.Collections.Generic;
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

    void Start()
    {
        StartCoroutine(LateStart());

        IEnumerator LateStart()
        {
            while (_secondPortal == null)
                yield return new WaitForSeconds(0.32f);

            GameObject camObj = new GameObject("SecondPortalCamera");
            _secondCamera = camObj.AddComponent<Camera>();
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

        Vector3 bottomLeft = _secondPortal.transform.TransformPoint(vertices[0]);
        Vector3 bottomRight = _secondPortal.transform.TransformPoint(vertices[1]);
        Vector3 topLeft = _secondPortal.transform.TransformPoint(vertices[2]);
        Vector3 topRight = _secondPortal.transform.TransformPoint(vertices[3]);

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
        _secondCamera.focalLength = mainCam.focalLength;// * _focalLengthMultiplier;

        float clip = SetObliqueNearPlane();
        WTH(_worldSecPort);
    }

    private void WTH(Vector3[] quad0)
    {
        Vector3[] quad = (Vector3[])quad0.Clone();

        Vector3 look = _secondCamera.transform.forward;
        Vector3 c = _secondCamera.transform.position;

        _secondCamera.rect = new Rect(0, 0, 1, 1);

        //for (int i = 0; i < 4; i++)
        //    Adjust(i);

        Vector3[] uv = new Vector3[4];

        float left = 1f;
        float right = 0f;
        float top = 0f;
        float bottom = 1f;

        for (int i = 0; i < 4; i++)
            FindUV(i);

        if (right > left && top > bottom)
        {
            left = MathF.Max(0, left);
            right = MathF.Min(1, right);
            bottom = MathF.Max(0, bottom);
            top = MathF.Min(1, top);
        }

        float w = right - left;
        float h = top - bottom;

        Rect cropRect = new Rect(left, bottom, w, h);
        _secondCamera.rect = cropRect;

        _meshRenderer.material.SetFloat("_RectX", left);
        _meshRenderer.material.SetFloat("_RectY", bottom);
        _meshRenderer.material.SetFloat("_RectWidth", w);
        _meshRenderer.material.SetFloat("_RectHeight", h);

        void FindUV(int i)
        {
            uv[i] = _secondCamera.WorldToViewportPoint(quad[i]);

            if (uv[i].z < 0)
            {
                // if (uv[i].x < 0.5)
                //     left = 0;
                // else
                //     right = 1;

                // if (uv[i].y < 0.5)
                //     bottom = 0;
                // else
                //     top = 1;
            }
            else
            {
                if (uv[i].x < left)
                    left = uv[i].x;
                if (uv[i].x > right)
                    right = uv[i].x;
                if (uv[i].y > top)
                    top = uv[i].y;
                if (uv[i].y < bottom)
                    bottom = uv[i].y;
            }
        }
    }

    private float SetObliqueNearPlane()
    {
        float minDistance = float.MaxValue;
        Transform cameraTransform = _secondCamera.transform;

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

        _secondCamera.nearClipPlane = minDistance;
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