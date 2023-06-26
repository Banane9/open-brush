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
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class ReturnProgressIndicator : MonoBehaviour
    {
        private Material[] materials;
        public static ReturnProgressIndicator Instance { get; private set; }

        private void Awake()
        {
            Instance = this;

            var mesh = new Mesh
            {
                name = "Procedural Indicator",
                vertices = GenerateVertices().ToArray(),
                subMeshCount = MoodWorldsManager.RadialSegments
            };

            var i = 0;
            foreach (var subTriangles in GenerateTriangles())
                mesh.SetTriangles(subTriangles, i++);

            GetComponent<MeshFilter>().mesh = mesh;

            materials = Enumerable.Repeat<Material>(null, MoodWorldsManager.RadialSegments).Select(_ => new Material(Shader.Find("UI/Unlit/Transparent"))).ToArray();
            GetComponent<MeshRenderer>().materials = materials;
        }

        private IEnumerable<int[]> GenerateTriangles()
        {
            for (var i = 0; i < MoodWorldsManager.RadialSegments - 1; ++i)
            {
                yield return new[] { 0, i + 1, i + 2 };
            }

            yield return new[] { 0, MoodWorldsManager.RadialSegments, 1 };
        }

        private IEnumerable<Vector3> GenerateVertices()
        {
            yield return Vector3.zero;

            for (var i = 0; i < MoodWorldsManager.RadialSegments; ++i)
            {
                var angle = (i - .5f) * MoodWorldsManager.RadialSegmentAngle * Mathf.Deg2Rad;
                yield return new(Mathf.Sin(angle), 0, Mathf.Cos(angle));
            }
        }

        private void LateUpdate()
        {
            var position = ViewpointScript.Head.position;
            position.y = .05f;
            transform.position = position;

            var layers = App.Scene.LayerCanvases.Skip(1).ToArray();
            var wrongStage = (MoodWorldsManager.Stage & MoodWorldsStage.ReturningToPositiveWorld) == 0;

            for (var i = 0; i < materials.Length; ++i)
            {
                if (wrongStage)
                {
                    materials[i].color = new Color(1, 1, 1, 0);
                    continue;
                }

                if (i >= layers.Length || !layers[i].gameObject.activeSelf || layers[i].transform.childCount == 0)
                {
                    materials[i].color = new Color(1, 1, 1, .1f);
                    continue;
                }

                var activeChildren = layers[i].transform.Cast<Transform>().Count(tr => tr.gameObject.activeSelf);
                materials[i].color = new Color(1, 1, 1, Mathf.Lerp(.25f, .45f, activeChildren / layers[i].transform.childCount));
            }
        }
    }
}