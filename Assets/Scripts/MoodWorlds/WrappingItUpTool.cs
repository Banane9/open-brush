using MoodWorlds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiltBrush;
using UnityEngine;

namespace Assets.Scripts.MoodWorlds
{
    public class WrappingItUpTool : MoodWorldsTool
    {
        [SerializeField]
        private Camera previewCamera;

        [SerializeField]
        private Material previewScreen;

        [SerializeField]
        private GvrAudioSource clickSource;

        [SerializeField]
        private double flashDuration;

        [SerializeField]
        private double freezeCameraDuration;

        private double freezeCameraTime;
        private Vector3 freezeDirection;
        private bool waitingForHide;

        protected override bool CommandActive()
        {
            return base.CommandActive() && !AnimationActive && InputManager.m_Instance.GetCommand(InputManager.SketchCommands.WrappingUp);
        }

        public override void LateUpdateTool()
        {
            base.LateUpdateTool();

            if (AnimationActive)
            {
                previewCamera.enabled = false;

                if ((freezeCameraTime + flashDuration) > Time.realtimeSinceStartupAsDouble)
                    previewScreen.SetTexture("_MainTex", null);
                else
                    previewScreen.SetTexture("_MainTex", previewCamera.targetTexture);
            }
            else
            {
                if (waitingForHide)
                {
                    waitingForHide = false;
                    MoodWorldsManager.TriggerHide(freezeDirection);
                }

                previewCamera.enabled = true;
                previewScreen.SetTexture("_MainTex", previewCamera.targetTexture);
            }
        }

        protected override void OnCommandActivated()
        {
            base.OnCommandActivated();

            waitingForHide = true;
            freezeCameraTime = Time.realtimeSinceStartupAsDouble;
            freezeDirection = previewCamera.transform.forward;

            clickSource.Play();
        }

        public bool AnimationActive => (freezeCameraTime + freezeCameraDuration) > Time.realtimeSinceStartupAsDouble;
    }
}
