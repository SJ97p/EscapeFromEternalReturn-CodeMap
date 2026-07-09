using System;
using SingletonPattern_Scripts;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HBDinosaur_ER_Project.Core
{
    public enum GameScene
    {
        TITLE,
        LOBBY,
        INGAME,
        BOOTSTRAP
    }

    public class TimeManager : Singleton<TimeManager>
    {
        [Header("Light")]
        [SerializeField] private Light _light;

        [Header("LightColor")]
        [SerializeField] private Color _dayColor;
        [SerializeField] private Color _nightColor;
        [SerializeField] private Color _dawnColor;

        [Header("Time Rules")]
        [SerializeField] private float _firstDayDuration = 120f;
        [SerializeField] private float _firstNightDuration = 40f;
        [SerializeField] private float _firstDawnDuration = 20f;
        [SerializeField] private float _normalDayDuration = 100f;
        [SerializeField] private float _normalNightDuration = 50f;
        [SerializeField] private float _normalDawnDuration = 30f;

        [Header("Runtime")]
        [SerializeField] private int _currentDay = 1;
        [SerializeField] private InGameState _currentState = InGameState.DAY;
        [SerializeField] private float _currentTime;

        private GameScene _gameScene;

        public Action<int, InGameState> OnStateChanged;
        public Action<float> OnTimeUpdated;

        public int CurrentDay => _currentDay;
        public InGameState CurrentState => _currentState;
        public float CurrentTime => _currentTime;


        private void OnEnable()
        {
            SceneManager.sceneLoaded += CheckScene;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= CheckScene;
        }

        private void Start()
        {
            _currentTime = GetDuration(_currentDay, _currentState);
        }

        private void Update()
        {
            if (_gameScene == GameScene.INGAME)
            {
                _currentTime -= Time.deltaTime;
                OnTimeUpdated?.Invoke(_currentTime);

                if (_currentTime <= 0f)
                {
                    ChangePhase();
                }
            }
        }

        private void CheckScene(Scene scene, LoadSceneMode loadScene)
        {
            scene = SceneManager.GetActiveScene();

            if (_light == null)
            {
                _light = FindObjectOfType<Light>();
            }
            
            _dayColor = new Color32(255, 255, 255, 255);
            _nightColor = new Color32(0, 0, 0, 255);
            _dawnColor = new Color32(111, 0, 255, 255);

            if (_light == null)
                _light = FindFirstObjectByType<Light>();

            if (scene.name == "Title")
            {
                _gameScene = GameScene.TITLE;
                _currentTime = _firstDayDuration;
                _currentState = InGameState.DAY;
                _currentDay = 1;
            }
            if (scene.name == "Lobby")
            {
                _gameScene = GameScene.LOBBY;
                _currentTime = _firstDayDuration;
                _currentState = InGameState.DAY;
                _currentDay = 1;
            }
            if (scene.name == "InGame")
            {
                OnTimeUpdated?.Invoke(_currentTime);
                OnStateChanged?.Invoke(_currentDay, _currentState);
                _gameScene = GameScene.INGAME;
            }
        }

        private void ChangePhase()
        {
            switch (_currentState)
            {
                case InGameState.DAY:
                    _currentState = InGameState.NIGHT;
                    _light.color = _nightColor;
                    OnStateChanged?.Invoke(_currentDay, _currentState);
                    break;

                case InGameState.NIGHT:
                    _currentState = InGameState.DAWN;
                    _light.color = _dawnColor;
                    OnStateChanged?.Invoke(_currentDay, _currentState);
                    break;

                case InGameState.DAWN:
                    _currentState = InGameState.DAY;
                    _light.color = _dayColor;
                    OnStateChanged?.Invoke(_currentDay, _currentState);
                    _currentDay++;
                    break;
            }

            _currentTime = GetDuration(_currentDay, _currentState);
            OnStateChanged?.Invoke(_currentDay, _currentState);

            Debug.Log($"[TimeManager] Day {_currentDay} / State : {_currentState} / Duration : {_currentTime}");
        }

        private float GetDuration(int day, InGameState state)
        {
            if (day == 1)
            {
                if (state == InGameState.DAY)
                    return _firstDayDuration;
                if (state == InGameState.NIGHT)
                    return _firstNightDuration;
                if (state == InGameState.DAWN)
                    return _firstDawnDuration;
            }

            if (day < 8)
            {
                if (state == InGameState.DAY)
                    return _normalDayDuration;
                if (state == InGameState.NIGHT)
                    return _normalNightDuration;
                if (state == InGameState.DAWN)
                    return _normalDawnDuration;
            }

            return _normalDayDuration;
        }
    }
}

