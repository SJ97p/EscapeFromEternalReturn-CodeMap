using UnityEngine;
using HBDinosaur_ER_Project.Common;
using HBDinosaur_ER_Project.UI;

namespace HBDinosaur_ER_Project.SceneSystem
{
    public class BootstrapSceneController : SceneController
    {
        [SerializeField] private BootstrapUIView bootstrapUI;
        public override GameScene SceneType { get => GameScene.BootStrap; }

        public override void Initialize(SceneEnterContext context) { }
        public override void Enter() 
        {
            if (bootstrapUI != null)
            {
                // 연출이 끝나면 실행할 함수를 등록(구독)합니다.
                bootstrapUI.OnSequenceComplete += GoToNextScene;

                // 연출 시작 명령!
                bootstrapUI.PlayTitleSequence();
            }
            else
            {
                // UI가 없으면 그냥 바로 다음 씬으로
                GoToNextScene();
            }
        }
        private void GoToNextScene()
        {
            // 다음 씬(예: 로비나 로그인)으로 이동하는 로직
            Debug.Log("타이틀 연출 종료, 다음 씬으로 이동합니다.");
            GameSceneManager.Instance.ChangeScene(GameScene.Title);
        }

        public override void Exit()
        {
            // 이벤트 구독 해제 (메모리 누수 방지)
            if (bootstrapUI != null)
            {
                bootstrapUI.OnSequenceComplete -= GoToNextScene;
            }
        }
    }

}
