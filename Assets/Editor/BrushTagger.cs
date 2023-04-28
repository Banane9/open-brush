// Copyright 2021 The Tilt Brush Authors
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.Collections.Generic;
using System.Linq;
using UnityEditor;


namespace TiltBrush
{
    public class BrushTagger
    {
        [MenuItem("Open Brush/Rewrite Brush Tags")]
        private static void TagBrushes()
        {
            var brushManifest = AssetDatabase.LoadAssetAtPath<TiltBrushManifest>("Assets/Manifest.asset");
            var brushManifestX = AssetDatabase.LoadAssetAtPath<TiltBrushManifest>("Assets/Manifest_Experimental.asset");

            foreach (var brush in AssetDatabase.FindAssets("t:BrushDescriptor")
                .Select(guid => AssetDatabase.LoadAssetAtPath<BrushDescriptor>(AssetDatabase.GUIDToAssetPath(guid))))
            {
                brush.m_Tags = new List<string>();

                foreach (var brushTag in brushManifest.BrushTags.Concat(brushManifestX.BrushTags))
                    if (brushTag.Brushes.Contains(brush))
                        brush.m_Tags.Add(brushTag.DurableName);

                if (brushManifest.Brushes.Contains(brush)) brush.m_Tags.Add("default");
                if (brushManifestX.Brushes.Contains(brush)) brush.m_Tags.Add("experimental");
                if (brush.m_AudioReactive) brush.m_Tags.Add("audioreactive");

                if (brush.m_BrushPrefab == null) continue;
                if (brush.m_BrushPrefab.GetComponent<HullBrush>() != null) brush.m_Tags.Add("hull");
                if (brush.m_BrushPrefab.GetComponent<GeniusParticlesBrush>() != null) brush.m_Tags.Add("particle");
                if (brush.m_BrushPrefab.GetComponent<ParentBrush>() != null) brush.m_Tags.Add("broken");

                EditorUtility.SetDirty(brush);
            }
        }
    }
}
