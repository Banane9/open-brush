using TiltBrush;
using UnityEngine;

namespace MoodWorlds
{
    public abstract class MoodWorldsTool : BaseTool
    {
        private bool alreadyActive;
        private bool armed;
        protected GameObject brushVisual;
        protected GameObject wandVisual;

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
            brushVisual?.SetActive(!bHide);
            wandVisual?.SetActive(!bHide);
        }

        public override void Init()
        {
            base.Init();

            brushVisual = transform.Find("BrushVisual")?.gameObject;
            wandVisual = transform.Find("WandVisual")?.gameObject;
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
            if (brushVisual != null)
            {
                var brushAttachPoint = InputManager.m_Instance.GetBrushControllerAttachPoint();
                // Lock tool to camera controller.
                //if (m_LockToController)
                //{
                brushVisual.transform.SetPositionAndRotation(brushAttachPoint.position, brushAttachPoint.rotation);
                //}
                //else
                //{
                //    transform.position = SketchSurfacePanel.m_Instance.transform.position;
                //    transform.rotation = SketchSurfacePanel.m_Instance.transform.rotation;
                //}
            }

            if (wandVisual != null)
            {

                var wandAttachPoint = InputManager.m_Instance.GetWandControllerAttachPoint();
                wandVisual.transform.SetPositionAndRotation(wandAttachPoint.position, wandAttachPoint.rotation);
            }
        }
    }
}