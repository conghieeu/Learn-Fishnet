using FishNet.Connection;
using FishNet.Object;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

namespace Basic
{
    public class PlayerCamera : NetworkBehaviour
    {
        [SerializeField] private CinemachineCamera _cinemachineCamera;

        public override void OnOwnershipClient(NetworkConnection prevOwner)
        {
            _cinemachineCamera.enabled = IsOwner;
        }
    }
}