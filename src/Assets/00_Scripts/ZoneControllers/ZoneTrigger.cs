using UnityEngine;

namespace HBDinosaur_ER_Project.ZoneSystem
{
    public class ZoneTrigger : MonoBehaviour
    {

        [SerializeField] private GameObject[] _zoneToLoad;
        [SerializeField] private GameObject[] _zoneToUnload;

        private GameObject _player;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Awake()
        {
            _player = GameObject.FindGameObjectWithTag("Player");
        }

        private void LoadScenes()
        {
        
        }

        private void UnloadScenes()
        {
        
        }
    }

}
