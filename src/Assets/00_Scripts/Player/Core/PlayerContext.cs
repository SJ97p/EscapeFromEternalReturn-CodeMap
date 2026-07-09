using HBDinosaur_ER_Project.Player.Skill;
using UnityEngine;

namespace HBDinosaur_ER_Project.Player
{
    public class PlayerContext
    {
        public GameObject Owner;
        public PlayerStat Stat;
        public PlayerMove Move;
        public BuffManager BuffManager;
        public SkillCaster SkillCaster;
        public PlayerFSM FSM;
        public SkillManager SkillManager;
    }
}