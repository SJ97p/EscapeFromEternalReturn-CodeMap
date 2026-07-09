using System.Collections.Generic;
using UnityEngine;

namespace HBDinosaur_ER_Project.UI
{
    public class TickContainer : MonoBehaviour
    {
        [SerializeField] private RectTransform tickContainer;
        [SerializeField] private GameObject tickPrefab;
        [SerializeField] private int maxHP = 100;
        [SerializeField] private int tickUnit = 10;

        private List<GameObject> ticks = new();

        public void CreateTicks()
        {
            foreach (var t in ticks)
                Destroy(t);
            ticks.Clear();

            int tickCount = maxHP / tickUnit;

            float width = tickContainer.rect.width;

            for (int i = 1; i < tickCount; i++)
            {
                float ratio = (float)i / tickCount;

                GameObject tick = Instantiate(tickPrefab, tickContainer);
                RectTransform rt = tick.GetComponent<RectTransform>();

                rt.anchorMin = new Vector2(ratio, 0);
                rt.anchorMax = new Vector2(ratio, 1);
                rt.offsetMin = Vector2.zero;
                rt.offsetMax = Vector2.zero;

                ticks.Add(tick);
            }
        }
    }
}