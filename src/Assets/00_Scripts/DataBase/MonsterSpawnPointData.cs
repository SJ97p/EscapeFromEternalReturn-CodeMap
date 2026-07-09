using HBDinosaur_ER_Project.Monster;
using UnityEngine;
using UnityEngine.UIElements;

namespace HBDinosaur_ER_Project.Database
{
    public class MonsterSpawnPointData
    {
        public int Id { get; }
        public MonsterType MonsterType { get; }
        public Region Region { get; }
        public Vector3 Position { get; }
        public Quaternion Rotation { get; }
        public Vector3 Scale { get; }

        public MonsterSpawnPointData(MonsterType monsterType, Region region, Vector3 position, Quaternion rotation, Vector3 scale)
        {
            MonsterType = monsterType;
            Region = region;
            Position = position;
            Rotation = rotation;
            Scale = scale;
        }

        public MonsterSpawnPointData(
            int id, 
            int monsterType, 
            int region, 
            float posX, float posY, float posZ, 
            float rotX, float rotY, float rotZ,
            float scaleX, float scaleY, float scaleZ)
        {
            Id = id;
            MonsterType = (MonsterType)monsterType;
            Region = (Region)region;
            Position = new Vector3(posX, posY, posZ);
            Rotation = Quaternion.Euler(rotX, rotY, rotZ);
            Scale = new Vector3(scaleX, scaleY, scaleZ);
        }
    }
}

