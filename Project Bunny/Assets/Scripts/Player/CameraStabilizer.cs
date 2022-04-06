using Cinemachine;
using UnityEngine;

namespace Player
{
    /// <summary>
    /// An add-on module for Cinemachine Virtual Camera that locks the camera's Z co-ordinate
    /// </summary>
    [ExecuteInEditMode] [SaveDuringPlay] [AddComponentMenu("")] // Hide in menu
    public class CameraStabilizer : CinemachineExtension
    {
        public NetworkStudentController CameraOwner { get; set; }
        
        protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
        {
            if (stage != CinemachineCore.Stage.Body || !CameraOwner.IsCameraDeadZone) return;
            
            var pos = state.RawPosition;
            var followPos = vcam.Follow.position;
            state.RawPosition = new Vector3(followPos.x, followPos.y, pos.z);
        }
    }
}
