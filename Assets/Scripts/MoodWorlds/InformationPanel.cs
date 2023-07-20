using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiltBrush;
using UnityEngine;

namespace MoodWorlds
{
    public class InformationPanel : BasePanel
    {
        public MoodWorldsStage[] Stages;
        public GameObject[] StageVisuals;
        public GameObject Visual;
        public GameObject VirtualRoot;

        private bool moving;
        private MoodWorldsStage shownStage;
        private Dictionary<MoodWorldsStage, GameObject> stageMap;

        public bool Dismissed
        {
            get => !Visual.activeSelf;
            private set
            {
                Visual.SetActive(!value);
                MoodWorldsManager.StageInformationDismissed = value;
            }
        }

        private int stages => Math.Min(Stages.Length, StageVisuals.Length);

        public void Dismiss()
        {
            Dismissed = true;
        }

        protected override void Awake()
        {
            base.Awake();

            stageMap = Enumerable.Range(0, Math.Min(Stages.Length, StageVisuals.Length)).ToDictionary(i => Stages[i], i => StageVisuals[i]);

            foreach (var stage in StageVisuals)
                stage.SetActive(false);

            var offsetDirection = ViewpointScript.Gaze.direction;
            offsetDirection.y = 0;

            var lookDirection = Visual.transform.position - ViewpointScript.Head.position;
            lookDirection.y = 0;

            var newPosition = ViewpointScript.Head.position + 10 * offsetDirection.normalized;
            newPosition.y = .75f;

            Visual.transform.position = newPosition;
            Visual.transform.LookAt(Visual.transform.position + lookDirection.normalized);
        }

        private void Update()
        {
            BaseUpdate();

            VirtualRoot.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);

            if (MoodWorldsManager.Stage == shownStage)
                return;

            Dismissed = false;
            shownStage = MoodWorldsManager.Stage;

            for (var i = 0; i < stages; ++i)
                StageVisuals[i].SetActive(Stages[i] == MoodWorldsManager.Stage);
        }

        private void LateUpdate()
        {
            var offsetDirection = ViewpointScript.Gaze.direction;
            offsetDirection.y = 0;

            var lookDirection = Visual.transform.position - ViewpointScript.Head.position;
            lookDirection.y = 0;

            var newPosition = ViewpointScript.Head.position + 10 * offsetDirection.normalized;
            newPosition.y -= .75f;

            if (moving || (newPosition - Visual.transform.position).magnitude > 4)
            {
                Visual.transform.position = Vector3.Lerp(Visual.transform.position, newPosition, Mathf.SmoothStep(0, 1, 10 * Time.deltaTime));
                Visual.transform.transform.LookAt(Visual.transform.position + lookDirection.normalized);

                moving = (newPosition - Visual.transform.position).magnitude > 1;
            }
        }
    }
}