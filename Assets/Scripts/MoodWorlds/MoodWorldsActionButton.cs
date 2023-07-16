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
        private static readonly MoodWorldsStage ActualStages = MoodWorldsStage.CreatingPositiveWorld | MoodWorldsStage.CreatingNegativeWorld
                                                    | MoodWorldsStage.ReturningToPositiveWorld | MoodWorldsStage.ReturnedToPositiveWorld;

        [field: SerializeField]
        public MoodWorldsStage TargetStage { get; private set; }

        public override void UpdateVisuals()
        {
            var currentStage = MoodWorldsManager.Stage;
            var stageMatch = currentStage == TargetStage;

            SetButtonAvailable(stageMatch);
            SetButtonActivated(stageMatch);

            if (!stageMatch)
                switch (TargetStage & ActualStages)
                {
                    case MoodWorldsStage.CreatingPositiveWorld:
                        SetButtonAvailable(currentStage == MoodWorldsStage.CreatingNegativeWorld);
                        break;

                    case MoodWorldsStage.CreatingNegativeWorld:
                        SetButtonAvailable(currentStage == MoodWorldsStage.CreatingPositiveWorld || (currentStage & MoodWorldsStage.ReturningToPositiveWorld) > 0 || currentStage == MoodWorldsStage.ReturnedToPositiveWorld);
                        break;

                    case MoodWorldsStage.ReturningToPositiveWorld:
                        SetButtonAvailable(currentStage == MoodWorldsStage.CreatingNegativeWorld);
                        break;

                    case MoodWorldsStage.ReturnedToPositiveWorld:
                        SetButtonAvailable(currentStage == MoodWorldsStage.ReturningToPositiveWorld && (MoodWorldsManager.StageChangeTime + (5 * 60)) < Time.realtimeSinceStartupAsDouble);
                        break;

                    default:
                        break;
                }

            base.UpdateVisuals();
        }
    }
}
