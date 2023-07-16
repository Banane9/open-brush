using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiltBrush;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

namespace MoodWorlds
{
    public class TyingItInTool : MoodWorldsTool
    {
        private static readonly GradientAlphaKey[] gradientAlpha = new[] { new GradientAlphaKey(.9f, 0), new GradientAlphaKey(.4f, 1) };
        private static readonly Gradient inactiveGradient = new();

        public ParticleSystem WandParticles;
        public ParticleSystem BrushParticles;
        public ParticleSystem BodyParticles;

        private Vector2 wandStartPosition;
        private Vector2 brushStartPosition;
        private bool triggeredHide;
        private double hideTime;

        static TyingItInTool()
        {
            inactiveGradient.SetKeys(
                new[] { new GradientColorKey(Color.white, 0), new GradientColorKey(Color.white, 1) },
                gradientAlpha);
        }

        public override void UpdateTool()
        {
            base.UpdateTool();

            var targetPosition = ViewpointScript.Head.position;
            targetPosition.y /= 2;

            BodyParticles.transform.position = targetPosition;

            var shape = BodyParticles.shape;
            var scale = shape.scale;
            scale.y = targetPosition.y / shape.radius;
            shape.scale = scale;

            if (triggeredHide && !InputBlocked())
            {
                var emission = WandParticles.emission;
                emission.rateOverTimeMultiplier = 2;
                emission.rateOverDistanceMultiplier = 1;

                emission = BrushParticles.emission;
                emission.rateOverTimeMultiplier = 2;
                emission.rateOverDistanceMultiplier = 1;

                var colorOverLifetime = WandParticles.colorOverLifetime;
                colorOverLifetime.color = inactiveGradient;

                colorOverLifetime = BrushParticles.colorOverLifetime;
                colorOverLifetime.color = inactiveGradient;
            }
        }

        protected override bool CommandActive()
        {
            return InputManager.m_Instance.GetCommand(InputManager.SketchCommands.TyingIn);
        }

        public override bool InputBlocked()
        {
            return base.InputBlocked() || hideTime + 2 > Time.realtimeSinceStartupAsDouble;
        }

        public override void EnableTool(bool bEnable)
        {
            base.EnableTool(bEnable);

            var targetPosition = ViewpointScript.Head.position;
            targetPosition.y /= 2;

            BodyParticles.transform.position = targetPosition;

            var shape = BodyParticles.shape;
            var scale = shape.scale;
            scale.y = targetPosition.y / shape.radius;
            shape.scale = scale;

            var emission = WandParticles.emission;
            emission.rateOverTimeMultiplier = 2;
            emission.rateOverDistanceMultiplier = 1;

            emission = BrushParticles.emission;
            emission.rateOverTimeMultiplier = 2;
            emission.rateOverDistanceMultiplier = 1;

            var colorOverLifetime = WandParticles.colorOverLifetime;
            colorOverLifetime.color = inactiveGradient;

            colorOverLifetime = BrushParticles.colorOverLifetime;
            colorOverLifetime.color = inactiveGradient;

            var totalSteps = Mathf.Max(1f, App.Scene.LayerCanvases.Skip(1).Sum(c => c.transform.childCount));
            var remainingSteps = (float)App.Scene.LayerCanvases.Skip(1).SelectMany(c => c.transform.Cast<Transform>()).Count(t => t.gameObject.activeSelf);

            var completion = remainingSteps / totalSteps;
            var color = Color.HSVToRGB(Mathf.Max(0, Mathf.Lerp(.333f, .083f, completion)), 1, 1);

            var gradient = new Gradient();
            gradient.SetKeys(
            new[] { new GradientColorKey(color, 0), new GradientColorKey(color, 1) },
            gradientAlpha);

            colorOverLifetime = BodyParticles.colorOverLifetime;
            colorOverLifetime.color = gradient;

            emission = WandParticles.emission;
            emission.rateOverTimeMultiplier = Mathf.Lerp(25, 2, completion);
            emission.rateOverDistanceMultiplier = Mathf.Lerp(4, .5f, completion);
        }

        protected override void OnCommandActivated()
        {
            base.OnCommandActivated();

            triggeredHide = false;

            var wandPosition = InputManager.m_Instance.GetWandControllerAttachPoint().position;
            wandStartPosition = new(wandPosition.x, wandPosition.z);

            var brushPosition = InputManager.m_Instance.GetBrushControllerAttachPoint().position;
            brushStartPosition = new(brushPosition.x, brushPosition.z);

            var emission = WandParticles.emission;
            emission.rateOverTimeMultiplier = 5;
            emission.rateOverDistanceMultiplier = 2.5f;

            emission = BrushParticles.emission;
            emission.rateOverTimeMultiplier = 5;
            emission.rateOverDistanceMultiplier = 2.5f;
        }

        protected override void WhileCommandActive()
        {
            base.WhileCommandActive();

            if (triggeredHide)
                return;

            var target = new Vector2(ViewpointScript.Head.position.x, ViewpointScript.Head.position.z);

            var position = new Vector2(InputManager.m_Instance.GetWandControllerAttachPoint().position.x, InputManager.m_Instance.GetWandControllerAttachPoint().position.z);
            var startDistance = (wandStartPosition - target).magnitude;
            var currentWandDistance = (position - target).magnitude;
            var wandProgress = currentWandDistance / startDistance;

            if (wandProgress > 1)
                wandProgress += 5 * (wandProgress - 1);

            var color = Color.HSVToRGB(Mathf.Max(0, Mathf.LerpUnclamped(.333f, .167f, wandProgress)), 1, 1);

            var gradient = new Gradient();
            gradient.SetKeys(
                new[] { new GradientColorKey(color, 0), new GradientColorKey(color, 1) },
                gradientAlpha);

            var colorOverLifetime = WandParticles.colorOverLifetime;
            colorOverLifetime.color = gradient;

            position = new Vector2(InputManager.m_Instance.GetBrushControllerAttachPoint().position.x, InputManager.m_Instance.GetBrushControllerAttachPoint().position.z);
            startDistance = (brushStartPosition - target).magnitude;
            var currentBrushDistance = (position - target).magnitude;
            var brushProgress = currentBrushDistance / startDistance;

            if (brushProgress > 1)
                brushProgress += 5 * (brushProgress - 1);

            color = Color.HSVToRGB(Mathf.Max(0, Mathf.LerpUnclamped(.333f, .167f, brushProgress)), 1, 1);

            gradient = new Gradient();
            gradient.SetKeys(
                new[] { new GradientColorKey(color, 0), new GradientColorKey(color, 1) },
                gradientAlpha);

            colorOverLifetime = BrushParticles.colorOverLifetime;
            colorOverLifetime.color = gradient;

            if (currentBrushDistance < 2 && currentWandDistance < 2)
            {
                triggeredHide = true;
                hideTime = Time.realtimeSinceStartupAsDouble;

                WandParticles.Emit(40);
                BrushParticles.Emit(40);
                BodyParticles.Emit(100);

                Invoke(nameof(triggerHide), 1f);

                var emission = WandParticles.emission;
                emission.rateOverTimeMultiplier = 0;
                emission.rateOverDistanceMultiplier = 0;

                emission = BrushParticles.emission;
                emission.rateOverTimeMultiplier = 0;
                emission.rateOverDistanceMultiplier = 0;

                var totalSteps = Mathf.Max(1f, App.Scene.LayerCanvases.Skip(1).Sum(c => c.transform.childCount));
                var remainingSteps = (float)App.Scene.LayerCanvases.Skip(1).SelectMany(c => c.transform.Cast<Transform>()).Count(t => t.gameObject.activeSelf) - 1;

                var completion = remainingSteps / totalSteps;
                color = Color.HSVToRGB(Mathf.Max(0, Mathf.Lerp(.333f, .083f, completion)), 1, 1);

                gradient = new Gradient();
                gradient.SetKeys(
                    new[] { new GradientColorKey(color, 0), new GradientColorKey(color, 1) },
                    gradientAlpha);

                colorOverLifetime = BodyParticles.colorOverLifetime;
                colorOverLifetime.color = gradient;

                emission = WandParticles.emission;
                emission.rateOverTimeMultiplier = Mathf.Lerp(25, 2, completion);
                emission.rateOverDistanceMultiplier = Mathf.Lerp(4, .5f, completion);
            }
        }

        private void triggerHide()
        {
            var target = new Vector2(ViewpointScript.Head.position.x, ViewpointScript.Head.position.z);
            var startPosition = (wandStartPosition + brushStartPosition) / 2f;
            var direction = startPosition - target;

            MoodWorldsManager.TriggerHide(new Vector3(direction.x, 0, direction.y));
        }
    }
}
