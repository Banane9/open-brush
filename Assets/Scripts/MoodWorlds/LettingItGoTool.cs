using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiltBrush;
using UnityEngine;

namespace MoodWorlds
{
    public class LettingItGoTool : MoodWorldsTool
    {
        private Vector3 startPosition;
        private double commandStartTime;
        private double flyUntilTime;
        private Vector3 flySpeed;
        private Transform flyer;
        private Transform flyerPitch;
        private bool hideTriggered;

        public override void Init()
        {
            base.Init();

            flyer = transform.Find("Flyer");
            flyerPitch = flyer.Find("Pitch");
        }

        protected override bool CommandActive()
        {
            return InputManager.m_Instance.GetCommand(InputManager.SketchCommands.LettingGo);
        }

        protected override void OnCommandActivated()
        {
            base.OnCommandActivated();

            commandStartTime = Time.realtimeSinceStartupAsDouble;
            startPosition = InputManager.m_Instance.GetBrushControllerAttachPoint().position;
        }

        public override void UpdateTool()
        {
            base.UpdateTool();

            visual.SetActive(!AnimationActive);
            flyer.gameObject.SetActive(AnimationActive);

            if (AnimationActive)
            {
                flyer.position += Time.deltaTime * flySpeed;

                var groundSpeed = new Vector3(flySpeed.x, 0, flySpeed.z);
                flyer.LookAt(flyer.position + groundSpeed);

                var lift = new Vector2(flySpeed.magnitude, flySpeed.y).normalized;

                // No need to divide by magnitudes as both are normalized
                var angle = Mathf.Acos(Vector2.Dot(lift, Vector2.right)) * Mathf.Rad2Deg;
                if (flySpeed.y < 0)
                    angle *= -1;

                var rotation = new Quaternion { eulerAngles = new Vector3(-angle, 0, 0) };

                flyerPitch.SetLocalPositionAndRotation(Vector3.zero, rotation);

                flySpeed.y -= Time.deltaTime * 98.1f / groundSpeed.magnitude;
                flySpeed.x *= Mathf.Pow(.9f, Time.deltaTime);
                flySpeed.z *= Mathf.Pow(.9f, Time.deltaTime);

                if (!hideTriggered && (visual.transform.position - flyer.position).magnitude > 42)
                {
                    hideTriggered = true;

                    MoodWorldsManager.TriggerHide(groundSpeed);
                }
            }
            else
            {
                flyer.SetLocalPositionAndRotation(visual.transform.localPosition, visual.transform.localRotation);
                flyerPitch.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            }
        }

        protected override void OnCommandDeactivated()
        {
            var time = Time.realtimeSinceStartupAsDouble - commandStartTime;
            var distance = InputManager.m_Instance.GetBrushControllerAttachPoint().position - startPosition;

            Debug.Log("Controller moved: " + distance.magnitude + " over " + time + "s: " + distance.magnitude / time + "dm/s");

            hideTriggered = false;
            flySpeed = distance / (float)time;
            flyUntilTime = Time.realtimeSinceStartupAsDouble + 2 + Mathf.Sqrt(flySpeed.magnitude / 10);
        }

        public override bool InputBlocked() => AnimationActive || base.InputBlocked();

        public bool AnimationActive => (flyUntilTime - Time.realtimeSinceStartupAsDouble) > 0;
    }
}