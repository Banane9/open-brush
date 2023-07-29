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
        private static WrappingItUpWidget instance;

        [SerializeField]
        private GameObject visual;

        [SerializeField]
        private ParticleSystem sparkle;

        [SerializeField]
        private Camera renderCamera;

        [SerializeField]
        private RenderTexture renderTarget;

        [SerializeField]
        private Material renderMaterial;

        private bool wrappingUp;

        internal static void TriggerBurst()
        {
            instance.sparkle.Emit(25);
        }

        protected override void Start()
        {
            base.Start();

            instance = this;
        }

        private void Update()
        {
            if (MoodWorldsManager.Stage == MoodWorldsStage.WrappingItUp && !wrappingUp)
            {
                wrappingUp = true;
                visual.SetActive(true);
                sparkle.gameObject.SetActive(true);

                var emission = sparkle.emission;
                emission.rateOverTimeMultiplier = 7;

                var position = ViewpointScript.Head.position;
                position.y -= 5f;

                var gaze = ViewpointScript.Gaze.direction.normalized;
                gaze.y = 0;

                transform.position = position + 10f * gaze;
                transform.localRotation = Quaternion.identity;

                renderCamera.enabled = true;
                renderCamera.RenderToCubemap(renderTarget);
                renderCamera.enabled = false;
            }
            else if (MoodWorldsManager.Stage != MoodWorldsStage.WrappingItUp && MoodWorldsManager.Stage != MoodWorldsStage.ReturnedToPositiveWorld)
            {
                wrappingUp = false;
                visual.SetActive(false);
                sparkle.gameObject.SetActive(false);
            }

            if (!wrappingUp)
            {
                transform.position = ViewpointScript.Head.position;
                
                return;
            }

            if (MoodWorldsManager.Stage == MoodWorldsStage.ReturnedToPositiveWorld)
            {
                var emission = sparkle.emission;
                emission.rateOverTimeMultiplier = 0;
            }

            updateShaderSlices();
        }

        private void updateShaderSlices()
        {
            var input = new Texture2D(MoodWorldsManager.RadialSegments, 1, TextureFormat.RFloat, false)
            {
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Repeat
            };

            float[] progress;

            if (MoodWorldsManager.Stage == MoodWorldsStage.WrappingItUp)
            {

                var layers = App.Scene.LayerCanvases.Skip(1).Select(layer => layer.transform).ToArray();

                progress = layers.Select(layer => layer.childCount switch
                    {
                        0 => .85f,
                        1 => layer.GetChild(0).gameObject.activeSelf ? 0 : .85f,
                        _ => getLogisticAlpha(layer)
                    })
                    .Concat(Enumerable.Repeat(.85f, MoodWorldsManager.RadialSegments - layers.Length))
                    .ToArray();
            }
            else
                progress = Enumerable.Repeat(1f, MoodWorldsManager.RadialSegments).ToArray();

            input.SetPixelData(progress, 0);
            input.Apply();

            renderMaterial.SetTexture("slices", input);
            renderMaterial.SetInt("count", MoodWorldsManager.RadialSegments);
        }

        private static float getLogisticAlpha(Transform layer)
        {
            var progress = layer.Cast<Transform>().Count(t => !t.gameObject.activeSelf);

            if (progress == 0)
                return 0;
            else if (progress == 1)
                return .45f;
            else
            {
                var center = 1 / (float)layer.childCount;
                var ratio = progress * center;

                return .45f + .4f / (1 + Mathf.Pow((float)Math.E, -4 * (ratio - center)));
            }
        }
    }
}
