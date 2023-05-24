using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiltBrush;
using UnityEngine;

namespace MoodWorlds
{
    public class MoodWorldsPanel : BasePanel
    {
        [SerializeField]
        private GameObject[] stages;

        public void SetCreatingNegativeWorld()
        {
            MoodWorldsManager.SetCreatingNegativeWorld();
        }

        public void SetLettingItGo()
        {
            MoodWorldsManager.SetLettingItGo();
        }

        public void SetWrappingItUp()
        {
            MoodWorldsManager.SetWrappingItUp();
        }

        public void SetTyingItIn()
        {
            MoodWorldsManager.SetTyingItIn();
        }

        public void SetCreatingPositiveWorld()
        {
            MoodWorldsManager.SetCreatingPositiveWorld();
        }
    }
}
