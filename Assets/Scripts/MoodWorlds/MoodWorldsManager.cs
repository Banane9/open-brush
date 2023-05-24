using System;
using System.Collections;
using System.Collections.Generic;
using TiltBrush;
using UnityEngine;

namespace MoodWorlds
{
    public static class MoodWorldsManager
    {
        private static MoodWorldsStage stage;

        public static MoodWorldsStage Stage
        {
            get => stage;
            private set
            {
                stage = value;
                Debug.Log("MoodWorlds stage set to " + stage);
            }
        }

        public static bool IsReturningToPositiveWorld => (Stage & MoodWorldsStage.ReturningToPositiveWorld) > 0;

        public static int RadialSegments => App.UserConfig.MoodWorlds.RadialSegments;
        public static float RadialSegmentAngle => 360f / RadialSegments;
        public static float RadialSegmentTolerance => App.UserConfig.MoodWorlds.RadialTolerance;

        public static void SetCreatingPositiveWorld()
        {
            Stage = MoodWorldsStage.CreatingPositiveWorld;
        }

        public static void SetCreatingNegativeWorld()
        {
            Stage = MoodWorldsStage.CreatingNegativeWorld;
        }

        public static void SetLettingItGo()
        {
            Stage = MoodWorldsStage.LettingItGo;
        }

        public static void SetTyingItIn()
        {
            Stage = MoodWorldsStage.TyingItIn;
        }

        public static void SetWrappingItUp()
        {
            Stage = MoodWorldsStage.WrappingItUp;
        }

        public static void SetReturnedToPositiveWorld()
        {
            Stage = MoodWorldsStage.ReturnedToPositiveWorld;
        }
    }
}