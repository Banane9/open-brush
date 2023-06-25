using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiltBrush;
using UnityEngine;

namespace MoodWorlds
{
    public class WrappingItUpWidget : GrabWidget
    {
        [SerializeField]
        private GameObject visual;

        [SerializeField]
        private Camera renderCamera;

        [SerializeField]
        private RenderTexture renderTarget;

        private void Update()
        {
            transform.position = ViewpointScript.Head.position;

            if (MoodWorldsManager.Stage != MoodWorldsStage.WrappingItUp && MoodWorldsManager.Stage != MoodWorldsStage.ReturnedToPositiveWorld)
            {
                visual.SetActive(false);
                return;
            }

            if (!visual.activeSelf)
            {
                visual.SetActive(true);

                var position = ViewpointScript.Head.position;
                position.y -= 1;

                var gaze = ViewpointScript.Gaze.direction.normalized;
                gaze.y = 0;

                visual.transform.position = position + 1.5f * gaze;

                renderCamera.RenderToCubemap(renderTarget);
            }

            // Add code to update the slices shown of the render
        }
    }
}
