using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public string _id;
    public string _secondPortalId;
    public Vector3 _secondPortalPosition;
    public Quaternion _secondPortalRotation; //
    public string _sceneName;
    public string _secondSceneName;
    public List<ItemP> _objects;
    public List<Fireball> _enemyBullets;
    public Camera _secondCamera;
    private Material _material;
    private RenderTexture _secondCameraRenTex;
    public float _w = 4f;
    public float _h = 8f;
    public float _f = 0.15f;
    public MeshFilter _meshFilter;
    public MeshRenderer _meshRenderer;
    RenderTexture[] _rts;
    public Vector3[] _quad;
    public Vector3[] _quadCentered;
    public ushort _resolutionIndex;
    Vector2 _sensorSize;
    Mesh _meshForward;
    Mesh _meshBackward;
    Mesh _meshCentered;
    bool _initialised;
    bool _fixerStarted;
    private int _layerMaskForPlayer;
    bool _playerOnFirstSide;

    //All cameras has set gateFit to none
    //And Physical.

    void Start()
    {
        StartCoroutine(LateStart());

        IEnumerator LateStart()
        {
            while (string.IsNullOrEmpty(_secondPortalId))
                yield return new WaitForSeconds(0.32f);

            while (!S.PortalsBase.Portals.ContainsKey(_secondSceneName))
                yield return new WaitForSeconds(0.32f);

            while (!S.PortalsBase.Portals[_secondSceneName].ContainsKey(_secondPortalId))
                yield return new WaitForSeconds(0.32f);

            while (S.PortalsBase.Portals[_secondSceneName][_secondPortalId] == null)
                yield return new WaitForSeconds(0.32f);

            InitLayerMask();
            InitSecondCamera();
            MeshInit();
            CreateQuadMesh();
            CreateMaterial();
            CreateGates();
            _initialised = true;
        }

        void InitLayerMask()
        {
            _layerMaskForPlayer = 1 << LayerMask.NameToLayer("Player") |
                1 << LayerMask.NameToLayer("Static") |
                1 << LayerMask.NameToLayer("Items") |
                1 << LayerMask.NameToLayer("Default");
        }
    }

    private void CreateGates()
    {
        GameObject gates = Instantiate(S.PortalObj1, this.transform);
        gates.transform.localPosition = Vector3.zero;
        gates.transform.localRotation = Quaternion.identity;
        gates.transform.localScale = Vector3.one;
    }

    private bool UpdateSecondPortalParams()
    {
        if (!S.PortalsBase.Portals.ContainsKey(_secondSceneName))
            return false;
        if (!S.PortalsBase.Portals[_secondSceneName].ContainsKey(_secondPortalId))
            return false;
        if (S.PortalsBase.Portals[_secondSceneName][_secondPortalId] == null)
            return false;

        Transform secPortTrans = S.PortalsBase.Portals[_secondSceneName][_secondPortalId].transform;
        _secondPortalPosition = secPortTrans.position;
        _secondPortalRotation = secPortTrans.rotation;

        return true;
    }

    private void MeshInit()
    {
        _meshFilter = gameObject.GetComponent<MeshFilter>();
        if (_meshFilter == null)
            _meshFilter = gameObject.AddComponent<MeshFilter>();

        _meshRenderer = GetComponent<MeshRenderer>();
        if (_meshRenderer == null)
            _meshRenderer = gameObject.AddComponent<MeshRenderer>();
    }

    void InitSecondCamera()
    {
        GameObject camObj = new GameObject("SecondPortalCamera");
        camObj.transform.SetParent(transform, true);
        _secondCamera = camObj.AddComponent<Camera>();
        _secondCamera.usePhysicalProperties = true;
        _secondCamera.fieldOfView = S.Camera.fieldOfView;
        _secondCamera.gateFit = Camera.GateFitMode.None;
        _sensorSize = S.Camera.sensorSize;
        _secondCamera.enabled = false;
        _secondCamera.farClipPlane = S.Camera.farClipPlane;
        Skybox sourceSkybox = S.Camera.GetComponent<Skybox>();
        if (sourceSkybox != null)
        {
            Skybox targetSkybox = _secondCamera.gameObject.AddComponent<Skybox>();
            targetSkybox.material = sourceSkybox.material;
        }
    }

    private void CreateQuadMesh()
    {
        _meshForward = new Mesh();
        _meshForward.name = "quadF";

        _meshBackward = new Mesh();
        _meshBackward.name = "quadB";

        _meshCentered = new Mesh();
        _meshCentered.name = "quadC";

        Vector3[] vertices0 = new Vector3[4];

        vertices0[0] = new Vector3(-_w / 2, 0, _f);
        vertices0[1] = new Vector3(_w / 2, 0, _f);
        vertices0[2] = new Vector3(_w / 2, _h, _f);
        vertices0[3] = new Vector3(-_w / 2, _h, _f);

        Vector3[] vertices1 = new Vector3[4];

        vertices1[0] = new Vector3(-_w / 2, 0, -_f);
        vertices1[1] = new Vector3(_w / 2, 0, -_f);
        vertices1[2] = new Vector3(_w / 2, _h, -_f);
        vertices1[3] = new Vector3(-_w / 2, _h, -_f);

        Vector3[] vertices2 = new Vector3[4];

        vertices2[0] = new Vector3(-_w / 2, 0, 0);
        vertices2[1] = new Vector3(_w / 2, 0, 0);
        vertices2[2] = new Vector3(_w / 2, _h, 0);
        vertices2[3] = new Vector3(-_w / 2, _h, 0);

        int[] triangles0 = new int[] { 0, 1, 2, 2, 0, 3 };
        int[] triangles1 = new int[] { 0, 1, 2, 2, 0, 3 };
        int[] triangles2 = new int[] { 0, 1, 2, 2, 0, 3 };

        Vector2[] uv0 = new Vector2[4]
        {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(1, 1),
            new Vector2(0, 1)
        };

        Vector2[] uv1 = new Vector2[4]
        {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(1, 1),
            new Vector2(0, 1)
        };


        Vector2[] uv2 = new Vector2[4]
        {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(1, 1),
            new Vector2(0, 1)
        };

        _meshForward.vertices = vertices0;
        _meshForward.triangles = triangles0;
        _meshForward.uv = uv0;
        _meshForward.RecalculateNormals();

        _meshBackward.vertices = vertices1;
        _meshBackward.triangles = triangles1;
        _meshBackward.uv = uv1;
        _meshBackward.RecalculateNormals();

        _meshCentered.vertices = vertices2;
        _meshCentered.triangles = triangles2;
        _meshCentered.uv = uv2;
        _meshCentered.RecalculateNormals();

        _meshFilter.mesh = _meshBackward;

        _quad = new Vector3[4];
        _quadCentered = new Vector3[4];

        UpdateQuad();
    }

    private void CreateMaterial()
    {
        _rts = new RenderTexture[6];
   
        _rts[0] = new RenderTexture(1280, 720, 24); // 100%
        _rts[0].Create();

        _rts[1] = new RenderTexture(540, 540, 24); // 75%
        _rts[1].Create();

        _rts[2] = new RenderTexture(360, 360, 24); // 50%
        _rts[2].Create();

        _rts[3] = new RenderTexture(270, 270, 24); // 37.5%
        _rts[3].Create();

        _rts[4] = new RenderTexture(180, 180, 24); // 25%
        _rts[4].Create();

        _rts[5] = new RenderTexture(135, 135, 24); // 18.75%
        _rts[5].Create();

        _resolutionIndex = 5;

        _secondCamera.targetTexture = _rts[_resolutionIndex]; //

        _material = new Material(Resources.Load<Shader>("Shaders/PortalShader"));
        _material.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);

        _material.mainTexture = _rts[_resolutionIndex];

        GetComponent<MeshRenderer>().material = _material;
    }

    void Update()
    {
        if (!_initialised || string.IsNullOrEmpty(_secondSceneName))
            return;

        if (!S.PortalsBase.Portals.ContainsKey(_secondSceneName))
            return;

        if (!S.PortalsBase.Portals[_secondSceneName].ContainsKey(_secondPortalId))
            return;

        if (S.PortalsBase.Portals[_secondSceneName][_secondPortalId] == null)
            return;

        if (S.PS._currentSceneName == _sceneName && UpdateSecondPortalParams())
        {
            PutFakePlayer();
            UpdateQuad();
            EnableCamera();
            UpdateResolution();
            PlaceSecondCamera();
            CheckTeleports();
        }
        else
        {
            DisableCamera();
        }

        void EnableCamera()
        {
            _secondCamera.enabled = true;
            _meshRenderer.enabled = true;
        }

        void DisableCamera()
        {
            _secondCamera.enabled = false;
            _meshRenderer.enabled = false;
        }
    }

    private void UpdateQuad()
    {
        Vector3 toPoint = S.Camera.transform.position - transform.position;
        float dot = Vector3.Dot(transform.forward, toPoint);

        if (dot > 0 && _playerOnFirstSide)
        {
            _playerOnFirstSide = false;
            _meshFilter.mesh = _meshBackward;
        }
        else if (dot < 0 && !_playerOnFirstSide)
        {
            _playerOnFirstSide = true;
            _meshFilter.mesh = _meshForward;
        }

        for (int i = 0; i < 4; i++)
        {
            _quad[i] = transform.TransformPoint(Mesh.vertices[i]);
            _quadCentered[i] = transform.TransformPoint(_meshCentered.vertices[i]);
        }
    }

    private Vector3[] Quad
    {
        get
        {
            return _quad;
        }
    }

    private Mesh Mesh
    {
        get
        {
            if (_playerOnFirstSide)
                return _meshForward;
            else
                return _meshBackward;
        }
    }

    private void PutFakePlayer()
    {
        //Fake player is to make everybody see him through portal
        //Player already in scene when we here
        S.PortalToPlayerDistance = Math.Clamp(S.PortalToPlayerDistance * 1.2f, 0.05f, 10000f);

        Vector3 center = new Vector3(transform.position.x, transform.position.y + _h / 2, transform.position.z);
        Vector3 toPlayer = S.Camera.transform.position - center;
        Ray ray = new Ray(transform.position, toPlayer);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, _layerMaskForPlayer))
        {
            bool see = hit.collider.gameObject.tag == "Player";

            float distance = toPlayer.magnitude;

            if (see || distance < _h * 3f)
            {
                if (distance < S.PortalToPlayerDistance)
                {
                    //Debug.Log($"Dist: {distance}, SPS: {_secondSceneName}");

                    S.FakePlayerLastUpdated = Time.time;
                    S.PortalToPlayerDistance = distance;
                    S.FakePlayerScene = _secondSceneName;
                    S.FakePlayer.position = SecondPortal.transform.position + new Vector3(0, _h / 2, 0);

                    S.FakePlayerCamera.rotation = _secondCamera.transform.rotation;
                }
            }
        }
    }

    private void CheckTeleports()
    {
        if (S.PS._currentSceneName == _sceneName)
        {
            Vector3 a = S.PS._prevCamPos;
            Vector3 b = S.Camera.transform.position;
            Vector3 dir = b - a;
            Vector3 c;

            if (S.PS._isTeleporting)
                return;

            if (SegmentIntersectingRectangle(a, b, _quadCentered[0], _quadCentered[1], _quadCentered[2], _quadCentered[3]))
            {
                S.PS._isTeleporting = true;

                S.PS._currentSceneName = _secondSceneName;
                S.SaveManager.CurrentSave.SaveString("sceneName", _secondSceneName);

                //S.Ph.transform.position += dir;
                Vector3 localPos = Quaternion.Inverse(transform.rotation) * (S.Ph.transform.position - transform.position);
                Vector3 newWorldPos = SecondPortal.transform.position + SecondPortal.transform.rotation * localPos;
                S.Ph.transform.position = newWorldPos;

                Vector3 oldForward = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
                Vector3 newForward = Vector3.ProjectOnPlane(SecondPortal.transform.forward, Vector3.up).normalized;

                float angle = Vector3.SignedAngle(oldForward, newForward, Vector3.up);
                S.PlayerCamScript.Rotate(angle);

                Quaternion rotation = Quaternion.FromToRotation(oldForward, newForward);
                S.PM.rb.velocity = rotation * S.PM.rb.velocity;

                c = S.Camera.transform.position;
                S.PS._prevCamPos = c;
                S.PS._camPos = c;

                S.Loader.GoTo(_secondSceneName, -1, Vector3.zero);

                S.SDC.RequestCleanup();

                StartCoroutine(UnlockLater());
                StartCoroutine(Fixer());

                IEnumerator UnlockLater()
                {
                    yield return new WaitForSeconds(0.2f);
                    S.PS._isTeleporting = false;
                }
            }
        }

        IEnumerator Fixer()
        {
            if (_fixerStarted)
                yield break;

            _fixerStarted = true;

            while (S.PS._isTeleporting)
                yield return new WaitForSeconds(0.333f);

            yield return new WaitForSeconds(0.5f);

            float a = (S.Camera.transform.position - transform.position).magnitude;
            float b = (S.Camera.transform.position - S.PortalsBase.Portals[_secondSceneName][_secondPortalId].transform.position).magnitude;

            if (a < b)
            {
                S.PS._currentSceneName = _sceneName;
                S.SaveManager.CurrentSave.SaveString("sceneName", _sceneName);
            }
            else
            {
                S.PS._currentSceneName = _secondSceneName;
                S.SaveManager.CurrentSave.SaveString("sceneName", _secondSceneName);
            }

            _fixerStarted = false;
        }
    }

    Portal SecondPortal
    {
        get
        {
            return S.PortalsBase.Portals[_secondSceneName][_secondPortalId];
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

        if (resId != _resolutionIndex)
        {
            _meshRenderer.material.mainTexture = _rts[resId];
            _secondCamera.targetTexture = _rts[resId];
            _resolutionIndex = resId;
        }
    }

    private void PlaceSecondCamera()
    {
        Camera mainCam = S.Camera;

        Vector3 relativePosition = transform.InverseTransformPoint(mainCam.transform.position);
        Quaternion relativeRotation = Quaternion.Inverse(transform.rotation) * mainCam.transform.rotation;

        _secondCamera.transform.position = _secondPortalPosition + _secondPortalRotation * relativePosition;
        _secondCamera.transform.rotation = _secondPortalRotation * relativeRotation;

        WTH(_quad);
        SetObliqueNearPlane();
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

            Vector3 planePoint = cam.transform.position + cam.transform.forward * _secondCamera.nearClipPlane;

            Vector3[] vps = new Vector3[4];

            for (int i = 0; i < quad.Length; i++)
                vps[i] = cam.WorldToViewportPoint(quad[i]);

            for (int i = 0; i < quad.Length; i++)
                if (vps[i].z > 0)
                    points.Add(vps[i]);
                else
                {
                    int prev = i - 1;
                    if (prev < 0)
                        prev = 3;
                    int next = i + 1;
                    if (next > 3)
                        next = 0;

                    if (vps[prev].z > 0)
                    {
                        Vector3 p1 = IntersectSegmentPlane(quad[i], quad[prev], planePoint, cam.transform.forward);
                        points.Add(cam.WorldToViewportPoint(p1));
                    }
                    if (vps[next].z > 0)
                    {
                        Vector3 p2 = IntersectSegmentPlane(quad[i], quad[next], planePoint, cam.transform.forward);
                        points.Add(cam.WorldToViewportPoint(p2));
                    }
                }

            float minX = float.MaxValue, minY = float.MaxValue, maxX = float.MinValue, maxY = float.MinValue;
            foreach (var pp in points)
            {
                Vector3 worldPos = S.Camera.ScreenToWorldPoint((Vector3)pp + Vector3.forward);

                minX = Mathf.Min(minX, pp.x);
                minY = Mathf.Min(minY, pp.y);
                maxX = Mathf.Max(maxX, pp.x);
                maxY = Mathf.Max(maxY, pp.y);
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

        foreach (Vector3 point in _quad)
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

        if (minDistance < 0.05f)
            S.Camera.nearClipPlane = minDistance;
        else
            S.Camera.nearClipPlane = 0.05f;

        _secondCamera.nearClipPlane = minDistance;// * S.Camera.fieldOfView / _secondCamera.fieldOfView;
        return minDistance;
    }

    public Vector3 IntersectSegmentPlane(Vector3 a, Vector3 b, Vector3 planePoint, Vector3 planeNormal)
    {
        Vector3 direction = b - a;
        float denominator = Vector3.Dot(planeNormal, direction);

        float t = Vector3.Dot(planeNormal, planePoint - a) / denominator;
        return a + t * direction;
    }

    bool SegmentIntersectingRectangle(Vector3 segA, Vector3 segB,
                                       Vector3 rect0, Vector3 rect1, Vector3 rect2, Vector3 rect3)
    {
        Plane plane = new Plane(rect0, rect1, rect2);

        Vector3 intersectionPoint;
        float epsilon = 1e-6f;

        float distA = plane.GetDistanceToPoint(segA);
        float distB = plane.GetDistanceToPoint(segB);

        if (Mathf.Abs(distA) < epsilon && Mathf.Abs(distB) < epsilon)
            intersectionPoint = (segA + segB) * 0.5f;

        if (distA * distB > epsilon)
            return false;

        float t = distA / (distA - distB);

        if (t < -epsilon || t > 1 + epsilon)
            return false;

        intersectionPoint = segA + t * (segB - segA);

        return IsPointInRectangle(intersectionPoint);
    }

    public Vector2 WorldToPlaneCoordinates(Vector3 point, Vector3 center, Vector3 normal, Vector3 up, Vector3 right)
    {
        Vector3 offset = point - center;

        float normalComponent = Vector3.Dot(offset, normal);
        Vector3 onPlane = offset - normalComponent * normal;

        float x = Vector3.Dot(onPlane, right);
        float y = Vector3.Dot(onPlane, up);

        return new Vector2(x, y);
    }

    bool IsPointInRectangle(Vector3 point)
    {
        Vector2 cd = WorldToPlaneCoordinates(point, transform.position, transform.forward, transform.up, transform.right);
            return cd.y > 0 && cd.y < _h && cd.x < _w / 2 && cd.x > -_w / 2;
    }

    void OnDestroy()
    {
        if (_rts != null)
        {
            foreach (var rt in _rts)
            {
                if (rt != null)
                    rt.Release();
            }
        }
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
        Vector3 forward = center + new Vector3(0, 0, _w);

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