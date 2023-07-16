using ODS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TiltBrush;
using UnityAsyncAwaitUtil;
using UnityEngine;

namespace MoodWorlds
{
    public static class MoodWorldsManager
    {
        private static bool curEnvironmentPositive = true;
        private static CustomLights prevLights;
        private static CustomEnvironment prevBackdrop;
        private static TiltBrush.Environment prevEnvironment;

        private static BaseTool.ToolType? previousActiveTool;
        private static MoodWorldsStage stage = MoodWorldsStage.CreatingPositiveWorld;

        public static bool IsReturningToPositiveWorld => (Stage & MoodWorldsStage.ReturningToPositiveWorld) > 0;

        public static float RadialSegmentAngle => 360f / RadialSegments;

        public static int RadialSegments => App.UserConfig.MoodWorlds.RadialSegments;

        public static float RadialSegmentTolerance => App.UserConfig.MoodWorlds.RadialTolerance;

        public static double StageChangeTime { get; private set; }

        public static MoodWorldsStage Stage
        {
            get => stage;
            private set
            {
                stage = value;
                StageChangeTime = Time.realtimeSinceStartupAsDouble;
                Debug.Log("MoodWorlds stage set to " + stage);
            }
        }

        public static int GetRadialSegment(Vector3 direction)
        {
            return Mathf.RoundToInt(GetRadialSegmentPosition(direction)) % RadialSegments;
        }

        public static float GetRadialSegmentPosition(Vector3 direction)
        {
            var groundDirection = new Vector2(direction.x, direction.z).normalized;

            // No need to divide by magnitudes as both are normalized
            var angle = Mathf.Acos(Vector2.Dot(groundDirection, Vector2.up)) * Mathf.Rad2Deg;
            if (direction.x < 0)
                angle = 360 - angle;

            return angle / RadialSegmentAngle;
        }

        public static void SetCreatingNegativeWorld()
        {
            Stage = MoodWorldsStage.CreatingNegativeWorld;
            SwitchEnvironment(false);

            App.Scene.ActiveCanvas.gameObject.SetActive(false);
            foreach (var canvas in App.Scene.AllCanvases.Skip(1))
                canvas.gameObject.SetActiveRecursively(true);

            SetPreviousTool();
        }

        public static void SetCreatingPositiveWorld()
        {
            Stage = MoodWorldsStage.CreatingPositiveWorld;
            SwitchEnvironment(true);

            App.Scene.ActiveCanvas.gameObject.SetActive(true);
            foreach (var canvas in App.Scene.AllCanvases.Skip(1))
                canvas.gameObject.SetActive(false);

            SetPreviousTool();
        }

        public static void SetLettingItGo()
        {
            SetNewTool(BaseTool.ToolType.LettingItGoTool);

            Stage = MoodWorldsStage.LettingItGo;
        }

        public static void SetReturnedToPositiveWorld()
        {
            Stage = MoodWorldsStage.ReturnedToPositiveWorld;
            SwitchEnvironment(true);

            App.Scene.MainCanvas.gameObject.SetActive(true);
            foreach (var canvas in App.Scene.AllCanvases.Skip(1))
                canvas.gameObject.SetActive(false);

            SetPreviousTool();
        }

        public static void SetTyingItIn()
        {
            SetNewTool(BaseTool.ToolType.TyingItInTool);

            Stage = MoodWorldsStage.TyingItIn;
        }

        public static void SetWrappingItUp()
        {
            SetNewTool(BaseTool.ToolType.WrappingItUpTool);

            Stage = MoodWorldsStage.WrappingItUp;
        }

        public static void TriggerHide(Vector3 direction)
        {
            var canvasSlice = App.Scene.GetOrCreateLayer(GetRadialSegment(direction) + 1);

            if (canvasSlice.gameObject.activeSelf)
                if (canvasSlice.transform.Cast<Transform>().FirstOrDefault(tf => tf.gameObject.activeSelf) is Transform activeBatch)
                {
                    activeBatch.gameObject.SetActive(false);

                    if (canvasSlice.transform.GetChild(canvasSlice.transform.childCount - 1) == activeBatch)
                        canvasSlice.gameObject.SetActive(false);
                }
                else
                    canvasSlice.gameObject.SetActive(false);

            if (App.Scene.LayerCanvases.Skip(1).All(canvas => !canvas.gameObject.activeSelf || canvas.gameObject.transform.childCount == 0))
                SetReturnedToPositiveWorld();
        }

        private static void SetNewTool(BaseTool.ToolType type)
        {
            previousActiveTool = SketchSurfacePanel.m_Instance.ActiveTool.m_Type;

            SketchSurfacePanel.m_Instance.RequestHideActiveTool(true);
            SketchSurfacePanel.m_Instance.EnableSpecificTool(type);
        }

        private static void SetPreviousTool()
        {
            if (!previousActiveTool.HasValue)
                return;

            SketchSurfacePanel.m_Instance.RequestHideActiveTool(true);
            SketchSurfacePanel.m_Instance.EnableSpecificTool(previousActiveTool.Value);

            previousActiveTool = null;
        }

        private static void SwitchEnvironment(bool newEnvironmentPositive)
        {
            if (curEnvironmentPositive == newEnvironmentPositive)
                return;

            var curLights = LightsControlScript.m_Instance.CustomLights;
            var curBackdrop = SceneSettings.m_Instance.CustomEnvironment;
            var curEnvironment = SceneSettings.m_Instance.CurrentEnvironment;

            SceneSettings.m_Instance.RecordSkyColorsForFading();

            // prevBackdrop can only be != null, when the environment was switched before
            if (prevBackdrop != null)
                SceneSettings.m_Instance.SetCustomEnvironment(prevBackdrop, prevEnvironment);

            SceneSettings.m_Instance.SetDesiredPreset(
                prevEnvironment != null ? prevEnvironment : EnvironmentCatalog.m_Instance.DefaultEnvironment,
                keepSceneTransform: true,
                forceTransition: prevEnvironment == curEnvironment && prevBackdrop == null && prevLights == null,
                hasCustomLights: prevLights != null);

            if (prevLights != null)
                LightsControlScript.m_Instance.CustomLights = prevLights;

            prevLights = curLights;
            prevBackdrop = curBackdrop;
            prevEnvironment = curEnvironment;
            curEnvironmentPositive = newEnvironmentPositive;
        }
    }
}