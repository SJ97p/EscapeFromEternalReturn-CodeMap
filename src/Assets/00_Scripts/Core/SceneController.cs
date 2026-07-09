using HBDinosaur_ER_Project.EnumType;
using UnityEngine;

namespace HBDinosaur_ER_Project.SceneSystem
{
    public abstract class SceneController : MonoBehaviour
    {
        public abstract GameScene SceneType { get; }

        public virtual BGMType BGM => BGMType.Title;

        public virtual void Initialize(SceneEnterContext context) { }
        public virtual void Enter() { }
        public virtual void Exit() { }
    }
}