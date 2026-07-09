using UnityEngine;


namespace HBDinosaur_ER_Project.Player
{
    public class PlayerOcclusionChecker : MonoBehaviour
    {
        [SerializeField] Transform player;
        [SerializeField] LayerMask obstacleLayer;

        public bool IsOccluded { get; private set; }

        void LateUpdate()
        {
            Vector3 dir = player.position - transform.position;

            if (Physics.Raycast(transform.position, dir, out RaycastHit hit, dir.magnitude, obstacleLayer))
            {
                if (hit.transform != player)
                    IsOccluded = true;
            }
            else
            {
                IsOccluded = false;
            }
        }
    }
}