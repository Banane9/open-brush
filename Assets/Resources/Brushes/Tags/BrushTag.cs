using System;
using System.Collections;
using System.Collections.Generic;
using TiltBrush;
using UnityEngine;
using UnityEngine.Localization;

namespace TiltBrush
{
    [CreateAssetMenu(fileName = "Tag", menuName = "Brush Tag")]
    public class BrushTag : ScriptableObject
    {
        [field: SerializeField]
        [Tooltip("The display name of the tag.")]
        public string DisplayName { get; private set; }

        [field: SerializeField]
        [Tooltip("The internally used version of the tag. Should be safe for cfg files.")]
        public string DurableName { get; private set; }

        [SerializeField]
        [DisplayName("Brushes")]
        [Tooltip("The brushes this tag gets assigned to.")]
        private BrushDescriptor[] brushes;

        public HashSet<BrushDescriptor> Brushes { get; private set; }

        private void OnEnable() => Brushes = new(brushes);
    }
}