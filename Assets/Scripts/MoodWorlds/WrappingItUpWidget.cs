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

        private bool wrappingUp;

        private void Update()
        {
            if (MoodWorldsManager.Stage == MoodWorldsStage.WrappingItUp && !wrappingUp)
            {
                wrappingUp = true;
                visual.SetActive(true);

                var position = ViewpointScript.Head.position;
                position.y = 8f;

                var gaze = ViewpointScript.Gaze.direction.normalized;
                gaze.y = 0;

                visual.transform.position = position + 4f * gaze;

                renderCamera.enabled = true;
                renderCamera.RenderToCubemap(renderTarget);
                renderCamera.enabled = false;
            }
            else if (MoodWorldsManager.Stage != MoodWorldsStage.WrappingItUp && MoodWorldsManager.Stage != MoodWorldsStage.ReturnedToPositiveWorld)
            {
                wrappingUp = false;
                visual.SetActive(false);
            }

            if (!wrappingUp)
                transform.position = ViewpointScript.Head.position;


            // Add code to update the slices shown of the render
        }
    }
}
