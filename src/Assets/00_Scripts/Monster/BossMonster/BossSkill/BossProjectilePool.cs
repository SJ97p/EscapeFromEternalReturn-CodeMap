using UnityEngine;
using System.Collections.Generic;

namespace HBDinosaur_ER_Project.Monster.BossMonster
{
    public class BossProjectilePool : MonoBehaviour
    {
        public List<BossProjectile> projectilePrefabs;

        private void Awake()
        {
            foreach (var projectile in projectilePrefabs)
            {
                for (int i = 0; i < projectile.count; i++)
                {
                    GameObject obj = Instantiate(projectile.projectilePrefab);
                    obj.SetActive(false);
                    projectile.pool.Enqueue(obj);
                    obj.transform.SetParent(transform);
                }
            }
        }

        public GameObject GetProjectile(BossProjectileType type)
        {
            foreach (var projectile in projectilePrefabs)
            {
                if (projectile.type == type)
                {
                    if (projectile.pool.Count > 0)
                    {
                        return projectile.pool.Dequeue();
                    }
                }
            }
            return null;
        }

        public void ReturnProjectile(BossProjectileType type, GameObject obj)
        {
            obj.SetActive(false);

            foreach (var projectile in projectilePrefabs)
            {
                if (projectile.type != type)
                    continue;

                projectile.pool.Enqueue(obj);
                return;
            }
        }
    }
}