using TiltBrush;
using UnityEngine;

namespace MoodWorlds
{
    public abstract class MoodWorldsTool : BaseTool
    {
        private bool alreadyActive;
        private bool armed;
        protected GameObject visual;

        public override void EnableTool(bool bEnable)
        {
            base.EnableTool(bEnable);

            EatInput();

            armed = !bEnable;

            // Make sure our UI reticle isn't active.
            SketchControlsScript.m_Instance.ForceShowUIReticle(false);
        }

        public override void HideTool(bool bHide)
        {
            base.HideTool(bHide);
            visual.SetActive(!bHide);
        }

        public override void Init()
        {
            base.Init();

            visual = transform.Find("Visual").gameObject;
        }

        public override bool InputBlocked() => !armed;

        public override void LateUpdateTool()
        {
            base.LateUpdateTool();
            UpdateTransformsFromControllers();
        }

        public override void UpdateTool()
        {
            base.UpdateTool();

            if (!armed)
            {
                if (!CommandActive())
                    armed = true;

                return;
            }

            if (InputBlocked() || !CommandActive())
            {
                if (alreadyActive)
                {
                    alreadyActive = false;
                    OnCommandDeactivated();
                }
                
                return;
            }

            if (!alreadyActive)
            {
                alreadyActive = true;
                OnCommandActivated();
            }

            WhileCommandActive();
        }

        protected abstract bool CommandActive();

        protected virtual void OnCommandActivated()
        { }

        protected virtual void WhileCommandActive()
        { }

        protected virtual void OnCommandDeactivated()
        { }

        private void UpdateTransformsFromControllers()
        {
            Transform rAttachPoint = InputManager.m_Instance.GetBrushControllerAttachPoint();
            // Lock tool to camera controller.
            //if (m_LockToController)
            //{
            visual.transform.SetPositionAndRotation(rAttachPoint.position, rAttachPoint.rotation);
            //}
            //else
            //{
            //    transform.position = SketchSurfacePanel.m_Instance.transform.position;
            //    transform.rotation = SketchSurfacePanel.m_Instance.transform.rotation;
            //}
        }
    }
}