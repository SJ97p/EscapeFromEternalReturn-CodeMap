using HBDinosaur_ER_Project.Player;
using UnityEngine;
using UnityEngine.UI;

namespace HBDinosaur_ER_Project.UI
{
    public class HPManager : MonoBehaviour
    {
        [SerializeField] private Slider hpFill;

        private float maxHp;

        private void OnEnable()
        {
            StateEventBus.OnHpChanged += UpdateHpBar;
            StateEventBus.OnMaxHpChanged += UpdateMaxHp;
        }

        private void UpdateMaxHp(float maxHp)
        {
            this.maxHp = maxHp;
        }

        private void UpdateHpBar(float hp)
        {
            hpFill.value = hp / maxHp;
        }

        private void OnDestroy()
        {
            StateEventBus.OnHpChanged -= UpdateHpBar;
            StateEventBus.OnMaxHpChanged -= UpdateMaxHp;
        }
    }
}