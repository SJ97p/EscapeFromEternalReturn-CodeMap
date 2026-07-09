using UnityEngine;
using UnityEngine.SceneManagement;
using SingletonPattern_Scripts;
using System.Collections;

namespace HBDinosaur_ER_Project.SceneSystem
{
    public class GameSceneManager : Singleton<GameSceneManager>
    {
        public GameScene CurrentScene { get; private set; }
        public bool IsChangingScene { get; private set; }
        private SceneController currentSceneController;

        private void Start()
        {
            // 1. 현재 켜진 유니티 씬의 이름 문자열을 가져옵니다.
            string activeSceneName = SceneManager.GetActiveScene().name;

            // 2. 씬 이름을 우리가 만든 GameScene 열거형(Enum)으로 변환합니다.
            CurrentScene = GetGameSceneFromName(activeSceneName);

            // 3. 현재 씬에 배치된 SceneController를 찾아서 작동시킵니다.
            currentSceneController = FindObjectOfType<SceneController>();

            if (currentSceneController != null)
            {
                // 최초 씬이므로 이전 씬은 자기 자신으로 처리하거나 기본값 처리
                var context = new SceneEnterContext
                {
                    PreviousScene = CurrentScene,
                    Payload = null
                };

                currentSceneController.Initialize(context);
                currentSceneController.Enter();

                Debug.Log($"[GameSceneManager] 최초 씬 [{activeSceneName}]의 Controller를 성공적으로 시작했습니다.");
            }
            else
            {
                Debug.LogWarning($"[GameSceneManager] 최초 씬 [{activeSceneName}]에 SceneController가 없습니다.");
            }
        }

        public void ChangeScene(GameScene nextScene, object payload = null)
        {
            if (IsChangingScene)
                return;

            StartCoroutine(ChangeSceneRoutine(nextScene, payload));
        }
        private IEnumerator ChangeSceneRoutine(GameScene nextScene, object payload)
        {
            IsChangingScene = true;

            // try 블록 안에서 싱크 및 씬 로드를 진행합니다.
            try
            {
                if (currentSceneController != null)
                {
                    currentSceneController.Exit();
                }

                var context = new SceneEnterContext
                {
                    PreviousScene = CurrentScene,
                    Payload = payload
                };

                string sceneName = GetSceneName(nextScene);
                AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

                while (!operation.isDone)
                {
                    yield return null;
                }

                CurrentScene = nextScene;
                currentSceneController = FindObjectOfType<SceneController>();

                if (currentSceneController != null)
                {
                    currentSceneController.Initialize(context);
                    currentSceneController.Enter();
                }
                else
                {
                    // [디버그 팁] 만약 로비나 인게임에 SceneController가 배치 안 되어 있다면 로그를 찍어줍니다.
                    Debug.LogWarning($"[{sceneName}] 씬에 SceneController가 존재하지 않습니다! 확인이 필요합니다.");
                }
            }
            finally
            {
                // try 블록 내부에서 에러가 나든, if문을 건너뛰든 
                // 코루틴이 종료되는 시점에 '무조건' 실행이 보장되는 안전 구역입니다.
                IsChangingScene = false;
                Debug.Log($"[GameSceneManager] 씬 전환 플래그가 안전하게 해제되었습니다. (IsChangingScene = false)");
            }
        }


        private string GetSceneName(GameScene scene)
        {
            return scene switch
            {
                GameScene.Title => "Title",
                GameScene.Lobby => "Lobby",
                GameScene.InGame => "InGame",
                GameScene.Result => "Result",
                GameScene.BootStrap=> "Bootstrap",
                _ => throw new System.ArgumentOutOfRangeException(nameof(scene), scene, null)
            };
        }
        private GameScene GetGameSceneFromName(string sceneName)
        {
            return sceneName switch
            {
                "Bootstrap" => GameScene.BootStrap, //  기획하신 BootStrap 이름에 맞추어 추가
                "Title" => GameScene.Title,
                "Lobby" => GameScene.Lobby,
                "InGame" => GameScene.InGame,
                "Result" => GameScene.Result,
                _ => GameScene.BootStrap // 기본 예외 처리용 (혹은 다른 기본값)
            };
        }

        public void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        }
    }
}