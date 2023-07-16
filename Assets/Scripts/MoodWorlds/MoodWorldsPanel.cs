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

        public void SetCreatingNegativeWorld()
        {
            MoodWorldsManager.SetCreatingNegativeWorld();
        }

        public void SetCreatingPositiveWorld()
        {
            MoodWorldsManager.SetCreatingPositiveWorld();
        }

        public void SetLettingItGo()
        {
            MoodWorldsManager.SetLettingItGo();
        }

        public void SetTyingItIn()
        {
            MoodWorldsManager.SetTyingItIn();
        }

        public void SetWrappingItUp()
        {
            MoodWorldsManager.SetWrappingItUp();
        }

        public void SetReturnedToPositiveWorld()
        {
            MoodWorldsManager.SetReturnedToPositiveWorld();
        }

        private void Start()
        {
            stageButtons = stages.Select(stage => stage.GetComponentsInChildren<MoodWorldsActionButton>()).ToArray();
        }
    }
}