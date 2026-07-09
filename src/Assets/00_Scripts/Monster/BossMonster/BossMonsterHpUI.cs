using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace HBDinosaur_ER_Project.Monster.BossMonster
{
    [System.Serializable]
    public class HpPhaseBar
    {
        public Slider slider;
        public float minHp;
        public float maxHp;
    }

    public class BossMonsterHpUI : MonoBehaviour
    {
        [SerializeField] private List<HpPhaseBar> hpFills;

        public void UpdateHpBar(float currentHp)
        {
            foreach (var bar in hpFills)
            {
                if (currentHp <= bar.minHp)
                {
                    bar.slider.gameObject.SetActive(false);
                    continue;
                }
                else
                {
                    bar.slider.gameObject.SetActive(true);
                }

                float range = Mathf.Max(bar.maxHp - bar.minHp, 0.01f);

                float hpInThisBar = Mathf.Clamp(currentHp - bar.minHp, 0, range);

                float value = hpInThisBar / range;

                bar.slider.value = (value < 0.001f) ? 0 : value;
            }
        }
    }
}