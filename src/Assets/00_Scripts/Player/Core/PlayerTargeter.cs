using HBDinosaur_ER_Project.Player;
using UnityEngine;

public class PlayerTargeter : MonoBehaviour
{
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private LayerMask clickableLayer;

    private Camera mainCamera;
    private IClickable lastHovered;

    private void Awake()
    {
        mainCamera = Camera.main;
    }
    public Transform GetHoveredTarget(Vector2 mousePos)
    {
        if (TryRaycast(mousePos, out RaycastHit hit))
        {
            if (hit.transform.TryGetComponent(out IClickable clickable))
            {
                if (lastHovered != clickable)
                {
                    lastHovered?.UnHoverd();
                    clickable.Hovered();
                    lastHovered = clickable;
                }
                return hit.transform;
            }
            else
            {
                lastHovered?.UnHoverd();
                lastHovered = null;
                return null;
            }
        }
        return null;
    }

    public bool TryRaycast(Vector2 mousePos, out RaycastHit hit)
    {
        Ray ray = mainCamera.ScreenPointToRay(mousePos);
        return Physics.Raycast(ray, out hit, 100f, clickableLayer);
    }
}
