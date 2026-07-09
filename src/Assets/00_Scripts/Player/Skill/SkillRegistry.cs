using HBDinosaur_ER_Project.Player.Skill;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System;


namespace HBDinosaur_ER_Project.Player.Skill
{
    public class SkillRegistry
    {
        private static readonly Dictionary<SkillId, SkillRawData> skillMap = new();
        private static readonly Dictionary<SkillId, Type> skillDB = new();

        public static void Initialize()
        {
            var skillTypes = Assembly.GetExecutingAssembly()
               .GetTypes()
               .Where(t => typeof(SkillContainer).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

            foreach (var type in skillTypes)
            {
                var attr = type.GetCustomAttribute<SkillScriptAttribute>();
                if (attr == null) continue;

                skillDB[attr.skillId] = type;
            }
        }

        public static void RegisterData(SkillRawData data)
        {
            skillMap[data.skillId] = data;
        }

        public static SkillRawData GetRawData(SkillId id)
        {
            skillMap.TryGetValue(id, out var data);
            return data;
        }

        public static SkillContainer CreateSkill(SkillId id)
        {
            if (!skillDB.TryGetValue(id, out var data))
            {
                UnityEngine.Debug.LogError($"Skill not found: {id}");
                return null;
            }

            var skill = (SkillContainer)Activator.CreateInstance(data);
            skill.SkillId = id;

            if (skillMap.TryGetValue(id, out var rawData))
            {
                // Note: SkillBase.Initialize will be called by SkillCaster or similar
                // But we can store the Data here if we want to initialize it early
                // Actually, SkillCaster.Cast calls Initialize. 
                // Let's see SkillCaster.cs.
            }

            return skill;
        }
    }

}