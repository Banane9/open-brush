using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiltBrush;
using UnityEngine;

namespace MoodWorlds
{
    public class InformationPanel : MonoBehaviour
    {
        public MoodWorldsStage[] Stages;
        public GameObject[] StageVisuals;
        public GameObject Visual;

        private bool moving;
        private MoodWorldsStage shownStage;
        private Dictionary<MoodWorldsStage, GameObject> stageMap;
        public static InformationPanel Instance { get; private set; }

        public bool Dismissed
        {
            get => !Visual.activeSelf;
            private set => Visual.SetActive(!value);
        }

        private int stages => Math.Min(Stages.Length, StageVisuals.Length);

        public void Dismiss()
        {
            Dismissed = true;
        }

        private void Awake()
        {
            Instance = this;
            stageMap = Enumerable.Range(0, Math.Min(Stages.Length, StageVisuals.Length)).ToDictionary(i => Stages[i], i => StageVisuals[i]);

            foreach (var stage in StageVisuals)
                stage.SetActive(false);
        }

        private void Update()
        {
            var direction = ViewpointScript.Gaze.direction;
            direction.y = 0;

            var newPosition = ViewpointScript.Head.position + 20 * direction;

            if (moving || (newPosition - transform.position).magnitude > 4)
            {
                transform.position = Vector3.Lerp(transform.position, newPosition, Mathf.SmoothStep(0, 1, Time.deltaTime));
                transform.LookAt(transform.position + direction);

                moving = (newPosition - transform.position).magnitude > 1;
            }

            if (MoodWorldsManager.Stage == shownStage)
                return;

            Dismissed = false;
            shownStage = MoodWorldsManager.Stage;

            for (var i = 0; i < stages; ++i)
                StageVisuals[i].SetActive(Stages[i] == MoodWorldsManager.Stage);
        }
    }
}