using UnityEngine;
using UnityEngine.InputSystem;

namespace MellowAbelson.Core.Input
{
    public class InputManager : MonoBehaviour
    {
        [SerializeField] private InputSystem_Actions _inputActions;

        public InputSystem_Actions Actions => _inputActions;

        private void Awake()
        {
            if (_inputActions == null)
                _inputActions = new InputSystem_Actions();
        }

        private void OnEnable() => _inputActions.Enable();
        private void OnDisable() => _inputActions.Disable();

        public Vector2 GetMove() => _inputActions.Player.Move.ReadValue<Vector2>();
        public Vector2 GetLook() => _inputActions.Player.Look.ReadValue<Vector2>();

        public bool WasAttackPressed() => _inputActions.Player.Attack.WasPressedThisFrame();
        public bool WasInteractPressed() => _inputActions.Player.Interact.WasPressedThisFrame();
        public bool WasJumpPressed() => _inputActions.Player.Jump.WasPressedThisFrame();
        public bool WasSprintPressed() => _inputActions.Player.Sprint.WasPressedThisFrame();
        public bool WasCrouchPressed() => _inputActions.Player.Crouch.WasPressedThisFrame();
    }
}
