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
        private Transform cameraPov;

        private Vector3 freezePosition;
        private Quaternion freezeRotation;
        private double freezeCameraUntilTime;
        private bool waitingForHide;

        private Vector3 originalLocalPosition;
        private Quaternion originalLocalRotation;

        public override void Init()
        {
            base.Init();

            originalLocalPosition = cameraPov.localPosition;
            originalLocalRotation = cameraPov.localRotation;
        }

        protected override bool CommandActive()
        {
            return InputManager.m_Instance.GetCommand(InputManager.SketchCommands.WrappingUp);
        }

        public override void LateUpdateTool()
        {
            base.LateUpdateTool();

            if (AnimationActive)
                cameraPov.SetPositionAndRotation(freezePosition, freezeRotation);
            else
            {
                if (waitingForHide)
                    MoodWorldsManager.TriggerHide(cameraPov.forward);

                cameraPov.SetLocalPositionAndRotation(originalLocalPosition, originalLocalRotation);
            }
        }

        protected override void OnCommandActivated()
        {
            base.OnCommandActivated();

            waitingForHide = true;
            freezePosition = cameraPov.position;
            freezeRotation = cameraPov.rotation;
            freezeCameraUntilTime = Time.realtimeSinceStartupAsDouble + 1;
        }

        public bool AnimationActive => (freezeCameraUntilTime - Time.realtimeSinceStartupAsDouble) > 0;

        public override bool InputBlocked()
        {
            return AnimationActive || base.InputBlocked();
        }
    }
}
