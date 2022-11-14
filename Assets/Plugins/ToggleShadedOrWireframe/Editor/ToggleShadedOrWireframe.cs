#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine;

namespace Plugins.ToggleShadedOrWireframe.Editor
{
    public class ToggleShadedOrWireframe : UnityEditor.Editor
    {
        [Shortcut("ToggleShadedOrWireframe", KeyCode.F3)]
        [Obsolete("Obsolete")]
        private static void ShadedRender()
        {
            SceneView sceneView = SceneView.lastActiveSceneView;

            if (sceneView.renderMode is DrawCameraMode.Textured or DrawCameraMode.TexturedWire)
                sceneView.renderMode = DrawCameraMode.Wireframe;
            else if (sceneView.renderMode == DrawCameraMode.Wireframe)
                sceneView.renderMode = DrawCameraMode.Textured;
        }

        [Shortcut("ToggleShadedOrShadedWireframe", KeyCode.F4)]
        [Obsolete("Obsolete")]
        private static void WireRender()
        {
            SceneView sceneView = SceneView.lastActiveSceneView;
            if (sceneView.renderMode == DrawCameraMode.Textured)
                sceneView.renderMode = DrawCameraMode.TexturedWire;
            else if (sceneView.renderMode == DrawCameraMode.TexturedWire)
                sceneView.renderMode = DrawCameraMode.Textured;
        }
    }
}
#endif