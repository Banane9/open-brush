// Copyright 2020 The Tilt Brush Authors
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

using MoodWorlds;
using UnityEngine;

namespace TiltBrush
{
    public class BaseTool : MonoBehaviour
    {
        public bool m_ShowTransformGizmo = false;

        public ToolType m_Type;

        protected bool m_AllowDrawing;

        protected bool m_EatInput;

        [SerializeField] protected bool m_ExitOnAbortCommand = true;

        protected Transform m_Parent;

        protected Vector3 m_ParentBaseScale;

        protected bool m_RequestExit;

        [SerializeField] protected bool m_ScalingSupported = false;

        protected SketchSurfacePanel m_SketchSurface;

        protected bool m_ToolHidden;

        private Vector3 m_ParentScale;

        public virtual float ButtonHoldDuration
        { get { return 1.0f; } }

        public bool IsEatingInput
        { get { return m_EatInput; } }

        // If this is true, the user can use the default tool toggle.
        public virtual bool AllowDefaultToolToggle()
        {
            return !PointerManager.m_Instance.IsMainPointerCreatingStroke();
        }

        public void AllowDrawing(bool bAllow)
        { m_AllowDrawing = bAllow; }

        public virtual bool AllowsWidgetManipulation()
        {
            return true;
        }

        public virtual bool AllowWorldTransformation()
        {
            return true;
        }

        // Called to notify the tool that it should assign controller materials.
        public virtual void AssignControllerMaterials(InputManager.ControllerName controller)
        {
        }

        // True if this tool can be used while a sketch is loading.
        public virtual bool AvailableDuringLoading()
        {
            return false;
        }

        public virtual void BacksideActive(bool bActive)
        {
        }

        // If this is true, the tool will disallow the pin cushion from spawning.
        public virtual bool BlockPinCushion()
        {
            return false;
        }

        // Overridden by classes to set when tool sizing interaction should be disabled.
        public virtual bool CanAdjustSize()
        {
            return true;
        }

        public virtual bool CanShowPromosWhileInUse()
        {
            return true;
        }

        public void EatInput()
        { m_EatInput = true; }

        public virtual void EnableRenderer(bool enable)
        {
            gameObject.SetActive(enable);
        }

        public virtual void EnableTool(bool bEnable)
        {
            m_RequestExit = false;
            gameObject.SetActive(bEnable);

            if (bEnable)
            {
                if (m_Parent != null)
                {
                    m_ParentScale = m_Parent.localScale;
                    m_Parent.localScale = m_ParentBaseScale;
                }
                PointerManager.m_Instance.RequestPointerRendering(ShouldShowPointer());
            }
            else
            {
                if (m_Parent != null)
                {
                    m_Parent.localScale = m_ParentScale;
                }
                m_EatInput = false;
                m_AllowDrawing = false;
            }
        }

        public bool ExitRequested()
        { return m_RequestExit; }

        public virtual float GetSize()
        {
            return 0.0f;
        }

        // Returns a number in [0,1]
        public virtual float GetSize01()
        {
            return 0.0f;
        }

        public virtual float GetSizeRatio(
            InputManager.ControllerName controller, VrInput input)
        {
            if (controller == InputManager.ControllerName.Brush)
            {
                return GetSize01();
            }
            return 0.0f;
        }

        // If this is true, the tool will tell the panels to hide.
        public virtual bool HidePanels()
        {
            return false;
        }

        public virtual void HideTool(bool bHide)
        {
            m_ToolHidden = bHide;
        }

        public virtual void Init()
        {
            m_Parent = transform.parent;
            if (m_Parent != null)
            {
                m_ParentBaseScale = m_Parent.localScale;
                m_SketchSurface = m_Parent.GetComponent<SketchSurfacePanel>();
            }
        }

        // Overridden by classes to set when gaze and widget interaction should be disabled.
        public virtual bool InputBlocked()
        {
            return false;
        }

        // Called only on frames that UpdateTool() has been called.
        // Guaranteed to be called after new poses have been received from OpenVR.
        public virtual void LateUpdateTool()
        { }

        public virtual bool LockPointerToSketchSurface()
        {
            return true;
        }

        public virtual void Monitor()
        { }

        public bool ScalingSupported()
        {
            return m_ScalingSupported;
        }

        public virtual void SetColor(Color rColor)
        {
        }

        public virtual void SetExtraText(string sExtra)
        {
        }

        public virtual void SetToolProgress(float fProgress)
        {
        }

        public virtual bool ShouldShowPointer()
        { return false; }

        public virtual bool ShouldShowTouch()
        {
            return true;
        }

        public bool ToolHidden()
        { return m_ToolHidden; }

        // Modifies the size by some amount determined by the implementation.
        // _usually_ this will have the effect that GetSize() changes by fAdjustAmount,
        // but not necessarily (see FreePaintTool).
        public virtual void UpdateSize(float fAdjustAmount)
        { }

        public virtual void UpdateTool()
        {
            if (m_EatInput)
            {
                if (!InputManager.m_Instance.GetCommand(InputManager.SketchCommands.Activate))
                {
                    m_EatInput = false;
                }
            }
            if (m_ExitOnAbortCommand && InputManager.m_Instance.GetCommandDown(InputManager.SketchCommands.Abort))
            {
                m_RequestExit = true;
            }
        }

        protected virtual void Awake()
        {
            // Some tools attach things to controllers (like the camera to the brush in SaveIconTool)
            // and these need to be swapped when the controllers are swapped.
            InputManager.OnSwapControllers += OnSwap;
        }

        protected virtual void OnDestroy()
        {
            InputManager.OnSwapControllers -= OnSwap;
        }

        protected virtual void OnSwap()
        { }

        protected bool PointInTriangle(ref Vector3 rPoint, ref Vector3 rA, ref Vector3 rB, ref Vector3 rC)
        {
            if (SameSide(ref rPoint, ref rA, ref rB, ref rC) &&
                SameSide(ref rPoint, ref rB, ref rA, ref rC) &&
                SameSide(ref rPoint, ref rC, ref rA, ref rB))
            {
                return true;
            }
            return false;
        }

        protected bool SameSide(ref Vector3 rPoint1, ref Vector3 rPoint2, ref Vector3 rA, ref Vector3 rB)
        {
            Vector3 vCross1 = Vector3.Cross(rB - rA, rPoint1 - rA);
            Vector3 vCross2 = Vector3.Cross(rB - rA, rPoint2 - rA);
            return (Vector3.Dot(vCross1, vCross2) >= 0);
        }

        protected bool SegmentSphereIntersection(Vector3 vSegA, Vector3 vSegB, Vector3 vSphereCenter, float fSphereRadSq)
        {
            //check segment start to sphere
            Vector3 vStartToSphere = vSphereCenter - vSegA;
            if (vStartToSphere.sqrMagnitude < fSphereRadSq)
            {
                return true;
            }

            //check to see if our ray is pointing in the right direction
            Vector3 vSegment = vSegB - vSegA;
            Ray segmentRay = new Ray(vSegA, vSegment.normalized);
            float fDistToCenterProj = Vector3.Dot(vStartToSphere, segmentRay.direction);
            if (fDistToCenterProj < 0.0f)
            {
                return false;
            }

            //if the distance to our projection is within the segment bounds, we're on the right track
            if (fDistToCenterProj * fDistToCenterProj > vSegment.sqrMagnitude)
            {
                return false;
            }

            //see if this projected point is within the sphere
            Vector3 vProjectedPoint = segmentRay.GetPoint(fDistToCenterProj);
            Vector3 vToProjectedPoint = vProjectedPoint - vSphereCenter;
            return vToProjectedPoint.sqrMagnitude <= fSphereRadSq;
        }

        public enum ToolType
        {
            SketchSurface,
            Selection,
            ColorPicker,
            BrushPicker,
            BrushAndColorPicker,
            SketchOrigin,
            AutoGif,
            CanvasTool,
            TransformTool,
            StampTool,
            FreePaintTool,
            EraserTool,
            ScreenshotTool,
            DropperTool,
            SaveIconTool,
            ThreeDofViewingTool,
            MultiCamTool,
            TeleportTool,
            RepaintTool,
            RecolorTool,
            RebrushTool,
            SelectionTool,
            PinTool,
            EmptyTool,
            CameraPathTool,
            FlyTool,
            SnipTool = 11000,
            JoinTool = 11001,
            MoodWorldsTool = MoodWorldsStage.ReturningToPositiveWorld,
            LettingItGoTool = MoodWorldsStage.LettingItGo,
            WrappingItUpTool = MoodWorldsStage.WrappingItUp,
            TyingItInTool = MoodWorldsStage.TyingItIn
        }
    }
} // namespace TiltBrush