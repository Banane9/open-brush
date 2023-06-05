using Assets.Scripts.MoodWorlds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiltBrush;
using UnityEngine;
using UnityEngine.UI;

namespace MoodWorlds
{
    public class MoodWorldsPanel : BasePanel
    {
        private BaseButton[][] stageButtons;

        [SerializeField]
        private GameObject[] stages;

        public static MoodWorldsPanel Instance { get; private set; }

        protected override void Awake()
        {
            base.Awake();

            Instance = this;
        }

        protected virtual void OnDestroy()
        {
            Instance = null;
        }

        public void SetButtonsAvailable(int stageIndex)
        {
            var max = stageIndex + 1;
            var min = stageIndex - 1;

            for (var i = 0; i < stageButtons.Length; ++i)
            {
                var available = i >= min && i <= max;

                foreach (var button in stageButtons[i])
                    button.SetButtonAvailable(available);
            }
        }

        public void SetCreatingNegativeWorld()
        {
            MoodWorldsManager.SetCreatingNegativeWorld();

            SetButtonsAvailable(1);
        }

        public void SetCreatingPositiveWorld()
        {
            MoodWorldsManager.SetCreatingPositiveWorld();

            SetButtonsAvailable(0);
        }

        public void SetLettingItGo()
        {
            MoodWorldsManager.SetLettingItGo();

            SetButtonsAvailable(2);
        }

        public void SetTyingItIn()
        {
            MoodWorldsManager.SetTyingItIn();

            SetButtonsAvailable(2);
        }

        public void SetWrappingItUp()
        {
            MoodWorldsManager.SetWrappingItUp();

            SetButtonsAvailable(2);
        }

        private void Start()
        {
            stageButtons = stages.Select(stage => stage.GetComponentsInChildren<BaseButton>()).ToArray();

            SetButtonsAvailable(0);
        }
    }
}