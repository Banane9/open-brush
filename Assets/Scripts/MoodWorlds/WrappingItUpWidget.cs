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

        [SerializeField]
        private Material renderMaterial;

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

                visual.transform.position = position + 10f * gaze;

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

            updateShaderSlices();
        }

        private void updateShaderSlices()
        {
            var layers = App.Scene.LayerCanvases.Skip(1).Select(layer => layer.transform).ToArray();

            var input = new Texture2D(MoodWorldsManager.RadialSegments, 1, TextureFormat.RFloat, false)
            {
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp
            };

            //var totalSteps = Mathf.Max(1f, App.Scene.LayerCanvases.Skip(1).Sum(c => c.transform.childCount));
            //var remainingSteps = (float)App.Scene.LayerCanvases.Skip(1).SelectMany(c => c.transform.Cast<Transform>()).Count(t => t.gameObject.activeSelf);

            var progress = layers.Select(layer => layer.childCount == 0 ? 1f : Mathf.Min(.2f, (float)layer.Cast<Transform>().Count(t => !t.gameObject.activeSelf) / (float)layer.childCount))
                .Concat(Enumerable.Repeat(1f, MoodWorldsManager.RadialSegments - layers.Length))
                .ToArray();

            input.SetPixelData(progress, 0);
            input.Apply();

            renderMaterial.SetTexture("slices", input);
        }
    }
}
