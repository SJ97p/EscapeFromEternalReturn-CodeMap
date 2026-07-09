using HBDinosaur_ER_Project.Core;
using HBDinosaur_ER_Project.Database;
using HBDinosaur_ER_Project.Player;
using HBDinosaur_ER_Project.UI;
using HBDinosaur_ER_Project.ZoneSystem;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HBDinosaur_ER_Project.HyperloopSystem
{
    public class HyperloopRegionUI : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Region region;
        [SerializeField] private Image outline;

        private ZoneController zoneController;
        private void Awake()
        {
            zoneController = FindAnyObjectByType<ZoneController>();
            if (outline != null)
            {
                outline.enabled = false;
            }
        }

        public void Hovered()
        {
            if (outline != null)
            {
                outline.enabled = true;
            }
        }

        public void UnHovered()
        {
            if (outline != null)
            {
                outline.enabled = false;
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                Click();
            }
            else
            {
                NewUIManager.Instance?.Close(UIPanelId.HyperloopUI);
            }
        }

        public void Click()
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player == null)
            {
                Debug.LogError("[HyperloopRegionUI] Player 태그를 가진 오브젝트를 찾을 수 없습니다.");
                return;
            }

            PlayerMove playerMove = player.GetComponent<PlayerMove>();
            if (playerMove == null)
            {
                Debug.LogError("[HyperloopRegionUI] Player 오브젝트에서 PlayerMove 컴포넌트를 찾을 수 없습니다.");
                return;
            }

            Vector3 targetPosition = Vector3.zero;
            bool foundDestination = false;

            Hyperloop[] hyperloops = Resources.FindObjectsOfTypeAll<Hyperloop>();
            foreach (var hl in hyperloops)
            {
                if (hl.Region == region)
                {
                    targetPosition = hl.transform.position;
                    foundDestination = true;
                    break;
                }
            }

            if (!foundDestination)
            {
                if (zoneController != null)
                {
                    Zone zone = zoneController.GetZone(region);
                    if (zone != null)
                    {
                        targetPosition = zone.transform.position;
                        foundDestination = true;
                        Debug.LogWarning($"[HyperloopRegionUI] {region} 지역의 Hyperloop 기기를 찾지 못해 Zone 위치({targetPosition})로 폴백합니다.");
                    }
                }
            }

            if (foundDestination)
            {
                playerMove.StartHyperloopCasting(targetPosition, region);
                AnimationEventBus.RaiseTelportStart();
                Debug.Log($"[HyperloopRegionUI] {region} 지역({targetPosition})으로 텔레포트 캐스팅을 시작합니다.");
                NewUIManager.Instance?.Close(UIPanelId.LobbyMenu);
                NewUIManager.Instance?.Close(UIPanelId.HyperloopUI);
            }
            else
            {
                Debug.LogError($"[HyperloopRegionUI] {region} 지역으로 이동하기 위한 목적지를 찾을 수 없습니다.");
            }
        }
    }
}