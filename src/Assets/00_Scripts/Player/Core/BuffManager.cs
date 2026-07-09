using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HBDinosaur_ER_Project.Common;
using HBDinosaur_ER_Project.Effects;

namespace HBDinosaur_ER_Project.Player
{
    public class BuffManager : MonoBehaviour
    {
        private PlayerStat stat;
        private PlayerMove playerMove;

        private Dictionary<BuffType, Coroutine> activeBuffs = new();
        private Dictionary<BuffType, GameObject> activeVFXs = new();

        private void Awake()
        {
            stat = GetComponent<PlayerStat>();
            playerMove = GetComponent<PlayerMove>();
        }

        public void ApplyBuff(BuffData data)
        {
            if (activeBuffs.TryGetValue(data.Type, out var routine))
            {
                StopCoroutine(routine);
                RemoveBuff(data);
            }

            var newRoutine = StartCoroutine(BuffRoutine(data));
            activeBuffs[data.Type] = newRoutine;
        }

        private IEnumerator BuffRoutine(BuffData data)
        {
            Debug.Log($"{data.Type} 적용");
            ApplyEffect(data);

            yield return new WaitForSeconds(data.Duration);

            Debug.Log($"{data.Type} 해제");
            RemoveBuff(data);

            activeBuffs.Remove(data.Type);
        }

        private void ApplyEffect(BuffData data)
        {
            switch (data.Type)
            {
                case BuffType.DefenseUp:
                    stat.AddDefense(data.DefenseAmount);
                    break;
                case BuffType.Slow:
                    playerMove.SetSpeedMultiplier(1f - data.SlowPercentage);
                    break;
            }

            if (data.EffectType != EffectType.None)
            {
                Vector3 pos = transform.position;
                Quaternion rot = transform.rotation * Quaternion.Euler(-90, 0, 0);
                GameObject vfx = VFXManager.Instance.GetVFX(data.EffectType, pos, rot, transform);
                activeVFXs[data.Type] = vfx;
            }
        }

        private void RemoveBuff(BuffData data)
        {
            switch (data.Type)
            {
                case BuffType.DefenseUp:
                    stat.RemoveDefense(data.DefenseAmount);
                    break;
                case BuffType.Slow:
                    playerMove.ResetSpeed();
                    break;
            }

            if (activeVFXs.TryGetValue(data.Type, out var vfx))
            {
                VFXManager.Instance.ReleaseVFX(vfx);
                activeVFXs.Remove(data.Type);
            }
        }
    }
}
