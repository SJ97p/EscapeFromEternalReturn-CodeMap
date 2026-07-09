using System;
using System.Collections.Generic;
using System.Linq;
using HBDinosaur_ER_Project.ItemData;
using SingletonPattern_Scripts;
using UnityEngine;

namespace HBDinosaur_ER_Project.Database
{
    public class GameDataStore : Singleton<GameDataStore>
    {
        [SerializeField] private MonsterDatabase _monsterDatabase;
        [SerializeField] private VFXDatabase _vfxDatabase;
        [SerializeField] private SFXDatabase _sfxDatabase;
        [SerializeField] private BGMDatabase _bgmDatabase;
        [SerializeField] private FootStepDatabase _footstepDatabase;
        [SerializeField] private VoiceDatabase _voiceDatabase;
        [SerializeField] private GameObject _itemContainerPrefab;

        public GameObject ItemContainerPrefab => _itemContainerPrefab;
        public MonsterDatabase MonsterDatabase => _monsterDatabase;
        public VFXDatabase VFXDatabase => _vfxDatabase;
        public SFXDatabase SFXDatabase => _sfxDatabase;
        public FootStepDatabase FootStepDatabase => _footstepDatabase;
        public VoiceDatabase VoiceDatabase => _voiceDatabase;
        public BGMDatabase BGMDatabase => _bgmDatabase;
    }
}
