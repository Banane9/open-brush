using System.Collections;
using System.Collections.Generic;
using TiltBrush;
using UnityEngine;

namespace MoodWorlds
{
    public class MoodWorldsManager : MonoBehaviour
    {
        public static MoodWorldsManager Instance { get; private set; }

        public static MoodWorldsStage Stage { get; private set; }

        public static int RadialSegments => App.UserConfig.MoodWorlds.RadialSegments;
        public static float RadialSegmentAngle => 360f / RadialSegments;
        public static float RadialSegmentTolerance => App.UserConfig.MoodWorlds.RadialTolerance;

        // Start is called before the first frame update
        void Start()
        {
            Instance = this;
        }

    }
}