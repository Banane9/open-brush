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

        protected override bool CommandActive()
        {
            return InputManager.m_Instance.GetCommand(InputManager.SketchCommands.LettingGo);
        }

        protected override void OnCommandActivated()
        {
            base.OnCommandActivated();

            startPosition = InputManager.m_Instance.GetBrushControllerAttachPoint().position;
        }

        protected override void OnCommandDeactivated()
        {
            var distance = InputManager.m_Instance.GetBrushControllerAttachPoint().position - startPosition;

            Debug.Log("Controller moved: " + distance);
        }
    }
}