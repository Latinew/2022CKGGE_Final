using UnityEngine;
using UnityEngine.InputSystem;

namespace StarterAssets
{
    public class StarterAssetsInputs : MonoBehaviour
    {
        [Header("Character Input Values")]
        public Vector2 Move;
        public Vector2 Look;
        public bool Jump;
        public bool Sprint;

        [Header("Movement Settings")]
        public bool AnalogMovement;

        [Header("Mouse Cursor Settings")] public bool CursorLocked = true;
        public bool CursorInputForLook = true;

        public void OnMoveInput(InputAction.CallbackContext value)
        {
            Move = value.ReadValue<Vector2>();
        }

        public void OnLookInput(InputAction.CallbackContext value)
        {
            if (CursorInputForLook) 
                Look = value.ReadValue<Vector2>();
        }

        public void OnJumpInput(InputAction.CallbackContext value)
        {
            Jump = value.ReadValueAsButton();
        }

        public void OnSprintInput(InputAction.CallbackContext value)
        {
            Sprint = value.ReadValueAsButton();
        }

        
        private void OnApplicationFocus(bool hasFocus)
        {
            SetCursorState(CursorLocked);
        }

        private void SetCursorState(bool newState)
        {
            Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
        }
    }
}