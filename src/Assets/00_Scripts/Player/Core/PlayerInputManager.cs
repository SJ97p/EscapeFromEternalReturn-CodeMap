using UnityEngine;
using UnityEngine.InputSystem;
using HBDinosaur_ER_Project.UI;
using HBDinosaur_ER_Project.Core;

namespace HBDinosaur_ER_Project.Player
{
    public class PlayerInputManager : MonoBehaviour
    {
        private PlayerInput input;

        private InputAction skillQ;
        private InputAction skillW;
        private InputAction skillE;
        private InputAction skillR;
        private InputAction stop;

        private void Awake()
        {
            input = GetComponent<PlayerInput>();

            skillQ = input.actions["Skill_Q"];
            skillW = input.actions["Skill_W"];
            skillE = input.actions["Skill_E"];
            skillR = input.actions["Skill_R"];
            stop = input.actions["Stop"];
        }

        private void OnEnable()
        {
            skillQ.Enable();
            skillW.Enable();
            skillE.Enable();
            skillR.Enable();
            stop.Enable();
        }

        private void OnDisable()
        {
            skillQ?.Disable();
            skillW?.Disable();
            skillE?.Disable();
            skillR?.Disable();
            stop?.Disable();
        }

        private void Update()
        {
            HandleMouse();
            HandleKeyInput();
        }

        private void HandleMouse()
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            InputEventBus.RaiseMouseMove(mousePos);

            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                InputEventBus.RaiseMousePressed(mousePos, InputType.LEFT_CLICK);
            }

            if (Mouse.current.rightButton.wasPressedThisFrame)
            {
                InputEventBus.RaiseMousePressed(mousePos, InputType.RIGHT_CLICK);
            }

            if (Mouse.current.rightButton.wasReleasedThisFrame)
            {
                InputEventBus.RaiseMouseReleased(mousePos, InputType.RIGHT_CLICK);
            }

            if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                InputEventBus.RaiseMouseReleased(mousePos, InputType.LEFT_CLICK);
            }
        }

        private void HandleKeyInput()
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            if (skillQ.triggered) InputEventBus.RaiseSkillPressed(0, mousePos);
            if (skillW.triggered) InputEventBus.RaiseSkillPressed(1, mousePos);
            if (skillE.triggered) InputEventBus.RaiseSkillPressed(2, mousePos);
            if (skillR.triggered) InputEventBus.RaiseSkillPressed(3, mousePos);

            if (stop.triggered) InputEventBus.RaiseStop();

            if (Keyboard.current != null && Keyboard.current.mKey.wasPressedThisFrame)
            {
                NewUIManager.Instance?.Toggle(UIPanelId.Minimap);
            }
        }
    }
}