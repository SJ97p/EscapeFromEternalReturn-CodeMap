using UnityEngine;

namespace HBDinosaur_ER_Project.Player
{
    public class PlayerOutlineController : MonoBehaviour
    {
        [SerializeField] PlayerOcclusionChecker checker;
        [SerializeField] GameObject outlineObject;

        void Update()
        {
            outlineObject.SetActive(checker.IsOccluded);
        }
    }
}