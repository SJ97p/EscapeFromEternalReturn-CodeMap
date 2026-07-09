using UnityEngine;


namespace HBDinosaur_ER_Project.Player
{
    public class PlayerInputHandler : MonoBehaviour
    {
        private PlayerTargeter targeter;
        private PlayerCommand command;

        private Transform target;

        private void Awake()
        {
            command = GetComponent<PlayerCommand>();
            targeter = GetComponent<PlayerTargeter>();

            InputEventBus.OnMouseMovement += HandleMouseMovement;
            InputEventBus.OnMousePressed += HandleClick;
            InputEventBus.OnSkillPressed += HandleSkill;
            InputEventBus.OnStop += HandleStop;
        }

        private void HandleSkill(int skillNum, Vector2 mousePos)
        {
            targeter.TryRaycast(mousePos, out RaycastHit hit);
            command.SkillCommand(skillNum, hit.point);
        }

        private void HandleStop()
        {
            command.StopCommand();
        }

        private void HandleClick(Vector2 mousePos, InputType type)
        {
            if (InputBlocker.BlockGameplayInput)
            {
                return;
            }

            if (type == InputType.LEFT_CLICK)
            {
                // 현재 좌클릭은 기능 없음
                return;
            }

            if (target == null)
            {
                targeter.TryRaycast(mousePos, out RaycastHit hit);
                command.MoveCommand(hit.point);
            }
            else
            {
                command.InteractCommand(target);
            }
        }

        private void HandleMouseMovement(Vector2 mousePos)
        {
            target = targeter.GetHoveredTarget(mousePos);
        }

        private void OnDestroy()
        {
            InputEventBus.OnMouseMovement -= HandleMouseMovement;
            InputEventBus.OnMousePressed -= HandleClick;
            InputEventBus.OnSkillPressed -= HandleSkill;
            InputEventBus.OnStop -= HandleStop;
        }
    }
}