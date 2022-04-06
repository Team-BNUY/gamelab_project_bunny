using Cinemachine;
using UnityEngine;

namespace Player
{
    /// <summary>
    /// An add-on module for Cinemachine Virtual Camera that locks the camera's Z co-ordinate
    /// </summary>
    //[ExecuteInEditMode] [SaveDuringPlay] [AddComponentMenu("")] // Hide in menu
    public class CameraStabilizer : CinemachineExtension
    {
        public NetworkStudentController CameraOwner { get; set; }
        private float _lockLeftX = -7f;
        
        protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
        {
            
            if (vcam == null) return;
            if (stage != CinemachineCore.Stage.Body || !CameraOwner.IsCameraDeadZone) return;
            
            var pos = state.RawPosition;
            pos.z = _lockLeftX;
            state.RawPosition = pos;
        }
    }
}
