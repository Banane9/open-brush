using System;
using System.Collections;
using System.Collections.Generic;
using TiltBrush;
using UnityEngine;
using UnityEngine.Localization;

namespace TiltBrush
{
    /// <summary>
    /// Create as Data Assets in Assets/Resources/Brushes/Tags to assign tags to <see cref="BrushDescriptor"/>s.<br/>
    /// The BrushTagger (Open Brush > Rewrite Brush Tags) assigns them to the descriptors.
    /// </summary>
    [CreateAssetMenu(fileName = "Tag", menuName = "Brush Tag")]
    public class BrushTag : ScriptableObject
    {
        [SerializeField]
        [Tooltip("The brushes this tag gets assigned to.")]
        private BrushDescriptor[] brushes;

        /// <summary>
        /// Gets the set of <see cref="BrushDescriptor"/>s that this tag should be assigned to.
        /// </summary>
        public HashSet<BrushDescriptor> Brushes { get; private set; }

        /// <summary>
        /// Gets this tag's display name.
        /// </summary>
        [field: SerializeField]
        [field: Tooltip("The display name of the tag.")]
        public string DisplayName { get; private set; }

        /// <summary>
        /// Gets this tag's internal and cfg-safe name.
        /// </summary>
        [field: SerializeField]
        [field: Tooltip("The internally used version of the tag. Should be safe for cfg files.")]
        public string DurableName { get; private set; }

        private void OnEnable() => Brushes = new(brushes ?? Array.Empty<BrushDescriptor>());
    }
}