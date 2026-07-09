using UnityEngine;

namespace HBDinosaur_ER_Project.Player
{
    public class PlayerCamera : MonoBehaviour
    {
        [SerializeField] private float smootSpeed = 10f;
        [SerializeField] private Vector3 offset = new Vector3(0, 10f, -8f);
        [SerializeField] private GameObject player;

        private void Awake()
        {
            if (player == null)
            {
                player = GameObject.FindWithTag("Player");
            }
        }

        private void LateUpdate()
        {
            if (player == null) return;
            Vector3 desiredPos = player.transform.position + offset;

            transform.position = Vector3.Lerp(transform.position, desiredPos, smootSpeed * Time.deltaTime);
        }
    }
}