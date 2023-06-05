using System;
using System.Collections;
using System.Collections.Generic;
using TiltBrush;
using UnityEngine;

namespace MoodWorlds
{
    public static class MoodWorldsManager
    {
        private static MoodWorldsStage stage = MoodWorldsStage.CreatingPositiveWorld;

        public static bool IsReturningToPositiveWorld => (Stage & MoodWorldsStage.ReturningToPositiveWorld) > 0;

        public static float RadialSegmentAngle => 360f / RadialSegments;

        public static int RadialSegments => App.UserConfig.MoodWorlds.RadialSegments;

        public static float RadialSegmentTolerance => App.UserConfig.MoodWorlds.RadialTolerance;

        private static BaseTool.ToolType? previousActiveTool;

        public static MoodWorldsStage Stage
        {
            get => stage;
            private set
            {
                stage = value;
                Debug.Log("MoodWorlds stage set to " + stage);
            }
        }

        public static void SetCreatingNegativeWorld()
        {
            Stage = MoodWorldsStage.CreatingNegativeWorld;

            SetPreviousTool();
        }

        public static void SetCreatingPositiveWorld()
        {
            Stage = MoodWorldsStage.CreatingPositiveWorld;

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
    }
}