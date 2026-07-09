using HBDinosaur_ER_Project.Player;
using System.Collections;
using UnityEngine;

namespace HBDinosaur_ER_Project.Monster.BossMonster
{
    public class BossProjectileAttack : MonoBehaviour
    {
        [SerializeField] private float _speed = 10f;
        private BossContext _context;
        private Coroutine _lifeCoroutine;
        private BossProjectileType type = BossProjectileType.PosionNiddle;

        private void OnEnable()
        {
            _lifeCoroutine = StartCoroutine(ReturnToPoolAfterDelay(3f));
        }

        private void OnDisable()
        {
            if (_lifeCoroutine != null)
            {
                StopCoroutine(_lifeCoroutine);
                _lifeCoroutine = null;
            }
        }

        private void Update()
        {
            transform.position += transform.forward * _speed * Time.deltaTime;
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                DamageContext con = new DamageContext
                {
                    Damage = (int)_context.runtimeMonsterData.damage,
                    Attacker = _context.owner.transform
                };

                if (other.TryGetComponent(out IDamageable damageable))
                {
                    damageable.TakeDamage(con);
                }

                Debug.Log("플레이어가 공격에 맞았습니다!");
            }
            this.transform.SetParent(_context.projectilePool.transform);
            _context.projectilePool.ReturnProjectile(type, this.gameObject);
        }

        private IEnumerator ReturnToPoolAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            _context.projectilePool.ReturnProjectile(type, this.gameObject);
            this.transform.SetParent(_context.projectilePool.transform);
        }

        public void Fire(BossContext con, Vector3 position, Quaternion rotation)
        {
            _context = con;

            transform.position = position;
            transform.rotation = rotation;

            gameObject.SetActive(true);
        }
    }
}