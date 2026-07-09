using System;
using HBDinosaur_ER_Project.Database;
using HBDinosaur_ER_Project.SceneSystem;
using HBDinosaur_ER_Project.ZoneSystem;
using UnityEngine;

namespace HBDinosaur_ER_Project.Player
{
    public class PlayerRegionTracker : MonoBehaviour
    {
        [SerializeField] private Region _region;
        public Region CurrentRegion { get; private set; }

        public event Action<Region> OnRegionChanged;
        public event Action<float> OnRestrictZone;
        public event Action OnEnterRestrictZone;
        public event Action OnExitRestrictZone;
        public bool isRestrictZone = false;

        [SerializeField] private Transform feetPoint;
        [SerializeField] private float groundCheckDistance = 2.0f;
        [SerializeField] private LayerMask groundLayer;

        private PlayerStat stat;

        private void Awake()
        {
            stat = GetComponent<PlayerStat>();
        }

        private void FixedUpdate()
        {
            bool hasGround = TryGetGroundRegion(out Region newRegion);

            if (hasGround && newRegion != Region.None)
            {
                if (CurrentRegion != newRegion)
                {
                    SetRegion(newRegion);
                }
            }
            else
            {
                SetRegion(Region.None);
            }

            InRestrictedArea();
        }

        private int previousSeconds = -1;

        private void InRestrictedArea()
        {
            if (CurrentRegion == Region.None) return;
            if (InGameSceneController.Instance.IsRestricted(CurrentRegion) && stat.ZoneTimer > 0)
            {
                stat.ZoneTimer -= Time.fixedDeltaTime;
                if (stat.ZoneTimer < 0)
                {
                    stat.ZoneTimer = 0;
                    StateEventBus.RaiseZoneTimerZero();
                }

                int currentSeconds = Mathf.CeilToInt(stat.ZoneTimer);
                if (currentSeconds != previousSeconds)
                {
                    previousSeconds = currentSeconds;
                    OnEnterRestrictZone?.Invoke();
                    OnRestrictZone?.Invoke(stat.ZoneTimer);
                }
            }
            else
            {
                OnExitRestrictZone?.Invoke();
            }
        }

        public void SetRegion(Region newRegion)
        {
            if (CurrentRegion == newRegion)
                return;

            CurrentRegion = newRegion;
            _region = newRegion;

            Debug.Log($"플레이어 지역 : {CurrentRegion} / 현재 지역 : {newRegion}");
            OnRegionChanged?.Invoke(newRegion);
        }

        private bool TryGetGroundRegion(out Region region)
        {
            region = Region.None;

            if (Physics.Raycast(feetPoint.position, Vector3.down, out RaycastHit hit, groundCheckDistance, groundLayer))
            {
                RegionSurface surface = hit.collider.GetComponentInParent<RegionSurface>();
                if (surface != null)
                {
                    region = surface.Region;
                    return true;
                }
            }

            return false;
        }
    }
}

