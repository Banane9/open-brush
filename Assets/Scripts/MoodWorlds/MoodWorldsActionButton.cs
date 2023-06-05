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
    public class MoodWorldsActionButton : ActionButton
    {
        [field: SerializeField]
        public MoodWorldsStage TargetStage { get; private set; }

        public override void UpdateVisuals()
        {
            SetButtonActivated(MoodWorldsManager.Stage == TargetStage);

            base.UpdateVisuals();
        }
    }
}
