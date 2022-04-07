using Cinemachine;
using UnityEngine;

namespace Player
{
    /// <summary>
    /// An add-on module for Cinemachine Virtual Camera that locks the camera's Z co-ordinate
    /// </summary>
    public class LockCinemachineFollow : CinemachineExtension
    {
        public NetworkStudentController PlayerOwner { get; set; }
        private float _XPosition, _YPosition, _ZPosition;
        private bool _lockX, _lockY, _lockZ;
        private Vector3 _rawPos;

        protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
        {
            if (vcam && enabled && stage == CinemachineCore.Stage.Body && PlayerOwner)
            {
                if (_lockX)
                {
                    var pos = state.RawPosition;
                    pos.x =_XPosition;
                    state.RawPosition = pos;
                }

                if (_lockY)
                {
                    var pos = state.RawPosition;
                    pos.y = _YPosition;
                    state.RawPosition = pos;
                }
                
                if (_lockZ)
                {
                    var pos = state.RawPosition;
                    pos.z = _ZPosition;
                    state.RawPosition = pos;
                }
                
                if (!PlayerOwner.IsInFollowZone && !PlayerOwner.IsBeingControlled)
                {
                    _rawPos = state.RawPosition;
                    _XPosition = PlayerOwner.CameraLockX;
                    var pos = state.RawPosition;
                    pos.x = _XPosition;
                    state.RawPosition = pos;
                    /*var transformNew = vcam.Follow;
                    var transformNewPosition = transformNew.position;
                    transformNewPosition.x = _XPosition;
                    transformNew.position = transformNewPosition;
                    vcam.Follow = transformNew;*/
                }
                /*else if (PlayerOwner.IsInFollowZone)
                {
                    var pos = state.RawPosition;
                    pos.x = PlayerOwner.CameraLockX;
                    state.RawPosition = pos;
                }*/
            }
        }
    }
}