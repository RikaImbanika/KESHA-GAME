using UnityEngine;
using System.Collections.Generic;

public class WallColorCapturer : MonoBehaviour
{
    private Camera _captureCamera;
    private readonly int _texSize = 32; // Small enough?
    private int _layerMask;

    public void Start()
    {
        if (_captureCamera != null) return;

        _layerMask = 1 << LayerMask.NameToLayer("Static") |
             1 << LayerMask.NameToLayer("Default");

        var go = new GameObject("__WallColorCaptureCam");
        Object.DontDestroyOnLoad(go);
        go.hideFlags = HideFlags.HideAndDontSave;
        _captureCamera = go.AddComponent<Camera>();
        _captureCamera.enabled = false;
        _captureCamera.clearFlags = CameraClearFlags.SolidColor;
        _captureCamera.backgroundColor = Color.black; // doesn't matter
        _captureCamera.orthographic = true;
        _captureCamera.allowHDR = false;
        _captureCamera.allowMSAA = false;
        _captureCamera.renderingPath = RenderingPath.Forward; //fast

        S.WallColorCapturer = this;
    }

    public Color[] CaptureAtPoints(Vector3[] points, Vector3 normal, LayerMask layerMask)
    {
        if (points == null || points.Length == 0)
            return new Color[0];

        // Center
        Vector3 center = points[0];

        float offset = 0.5f;
        _captureCamera.transform.position = center + normal * offset;

        _captureCamera.transform.rotation = Quaternion.LookRotation(-normal, Vector3.up);
        _captureCamera.cullingMask = layerMask;

        // Bounding box
        Matrix4x4 worldToLocal = _captureCamera.transform.worldToLocalMatrix;
        Vector2 min = new Vector2(float.MaxValue, float.MaxValue);
        Vector2 max = new Vector2(float.MinValue, float.MinValue);

        foreach (var p in points)
        {
            Vector3 local = worldToLocal.MultiplyPoint3x4(p);
            min.x = Mathf.Min(min.x, local.x);
            min.y = Mathf.Min(min.y, local.y);
            max.x = Mathf.Max(max.x, local.x);
            max.y = Mathf.Max(max.y, local.y);
        }

        float width = max.x - min.x;
        float height = max.y - min.y;

        _captureCamera.orthographicSize = Mathf.Max(width, height) * 0.5f;
        _captureCamera.aspect = 1f;

        // Temp RenderTexture and render
        RenderTexture rt = RenderTexture.GetTemporary(_texSize, _texSize, 24, RenderTextureFormat.ARGB32);
        _captureCamera.targetTexture = rt;
        _captureCamera.Render();

        // Read to Texture2D
        RenderTexture.active = rt;
        Texture2D captured = new Texture2D(_texSize, _texSize, TextureFormat.RGBA32, false);
        captured.ReadPixels(new Rect(0, 0, _texSize, _texSize), 0, 0);
        captured.Apply();
        RenderTexture.active = null;

        Color[] colors = new Color[points.Length];
        for (int i = 0; i < points.Length; i++)
        {
            Vector3 screenPos = _captureCamera.WorldToScreenPoint(points[i]);
            int px = Mathf.RoundToInt(screenPos.x);
            int py = Mathf.RoundToInt(screenPos.y);
            px = Mathf.Clamp(px, 0, _texSize - 1);
            py = Mathf.Clamp(py, 0, _texSize - 1);
            colors[i] = captured.GetPixel(px, py);
        }

        _captureCamera.targetTexture = null;
        RenderTexture.ReleaseTemporary(rt);
        Object.Destroy(captured);

        return colors;
    }
}