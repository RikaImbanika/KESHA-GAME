// SPDX-License-Identifier: Apache-2.0
// Copyright (c) 2026 RIKA IMBANIKA

using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class Hotkeys
{
    private static bool isDragging = false;
    private static Plane dragPlane = new Plane(Vector3.up, 0f);
    private static Vector2 lastMousePos;
    private static int dragControlId = 0;

    static Hotkeys()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private static void OnSceneGUI(SceneView sceneView)
    {
        if (EditorGUIUtility.keyboardControl != 0 || !sceneView.hasFocus)
        {
            if (isDragging) StopDragging();
            return;
        }

        Event e = Event.current;

        if (e.type == EventType.KeyDown && e.keyCode == KeyCode.G && !isDragging)
        {
            StartDragging();
            e.Use();
            sceneView.Repaint();
            return;
        }

        if (e.type == EventType.KeyUp && e.keyCode == KeyCode.G && isDragging)
        {
            StopDragging();
            e.Use();
            sceneView.Repaint();
            return;
        }

        if (isDragging && !Input.GetKey(KeyCode.G))
        {
            StopDragging();
            sceneView.Repaint();
            return;
        }

        if (isDragging)
        {
            int id = dragControlId;
            HandleUtility.AddDefaultControl(id);

            if (e.type == EventType.MouseDrag || e.type == EventType.MouseMove)
            {
                if (GUIUtility.hotControl != id)
                {
                    StopDragging();
                    sceneView.Repaint();
                    return;
                }

                Vector2 mouseDelta = e.mousePosition - lastMousePos;
                if (mouseDelta != Vector2.zero)
                {
                    lastMousePos = e.mousePosition;

                    Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
                    Ray prevRay = HandleUtility.GUIPointToWorldRay(e.mousePosition - mouseDelta);

                    float enter, prevEnter;
                    if (dragPlane.Raycast(ray, out enter) && dragPlane.Raycast(prevRay, out prevEnter))
                    {
                        Vector3 worldDelta = ray.GetPoint(enter) - prevRay.GetPoint(prevEnter);
                        worldDelta.y = 0f; // только по XZ

                        foreach (GameObject go in Selection.gameObjects)
                            go.transform.position += worldDelta;

                        sceneView.Repaint();
                    }
                }
            }

            e.Use();
        }
    }

    private static void StartDragging()
    {
        if (Selection.gameObjects.Length == 0) return;
        isDragging = true;
        lastMousePos = Event.current.mousePosition;

        dragControlId = GUIUtility.GetControlID(FocusType.Passive);
        GUIUtility.hotControl = dragControlId;

        Undo.RecordObjects(Selection.gameObjects, "Drag XZ");
    }

    private static void StopDragging()
    {
        isDragging = false;
        GUIUtility.hotControl = 0;
        dragControlId = 0;
    }

    [MenuItem("Tools/Rotate 90° Y + _r")]
    static void RotatePlus90()
    {
        RotateSelected(90);
    }

    // Shift + D: duplicate and apply a fixed shift in the Y=0 plane
    [MenuItem("Edit/Duplicate and Shift #d", false, 100)]
    static void DuplicateAndShift()
    {
        if (Selection.gameObjects.Length == 0) return;

        // Duplicate (Unity will select the new copies)
        EditorApplication.ExecuteMenuItem("Edit/Duplicate");

        // Fixed offset – here 1 unit along the X axis, pure horizontal (Y=0 plane)
        Vector3 fixedOffset = new Vector3(1f, 0f, 0f);

        // Move each duplicated object
        foreach (GameObject go in Selection.gameObjects)
        {
            Undo.RecordObject(go.transform, "Shift Duplicate");
            go.transform.position += fixedOffset;
        }
    }

    private static void RotateSelected(float angle)
    {
        foreach (GameObject go in Selection.gameObjects)
        {
            Undo.RecordObject(go.transform, "Rotate 90°");
            go.transform.Rotate(0, angle, 0, Space.World);
        }
    }
}