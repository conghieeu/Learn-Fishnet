using FishNet.Object;
using MellowAbelson.Core.Input;
using UnityEngine;

namespace MellowAbelson.Player.Interaction
{
    [RequireComponent(typeof(InputManager))]
    public class PlayerInteractor : NetworkBehaviour
    {
        [SerializeField] private Camera _playerCamera;
        [SerializeField] private float _interactionRange = 3f;
        [SerializeField] private LayerMask _interactableLayers = -1;

        private InputManager _input;
        private IInteractable _currentTarget;

        private void Awake()
        {
            _input = GetComponent<InputManager>();
            if (_playerCamera == null)
                _playerCamera = Camera.main;
        }

        private void Update()
        {
            if (!IsOwner) return;

            FindInteractableTarget();

            if (_input.WasInteractPressed() && _currentTarget != null)
            {
                ServerInteractRequest(_currentTarget.InteractionPrompt.GetHashCode());
            }
        }

        private void FindInteractableTarget()
        {
            Ray ray = _playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

            if (Physics.Raycast(ray, out RaycastHit hit, _interactionRange, _interactableLayers))
            {
                IInteractable interactable = hit.collider.GetComponentInParent<IInteractable>();
                _currentTarget = interactable;
            }
            else
            {
                _currentTarget = null;
            }
        }

        [ServerRpc]
        private void ServerInteractRequest(int targetHash)
        {
            // Server validates and processes interaction
            Debug.Log($"Server: Interact requested with hash {targetHash}");
        }
    }
}
