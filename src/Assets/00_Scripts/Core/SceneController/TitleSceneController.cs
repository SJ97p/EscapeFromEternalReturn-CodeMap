using HBDinosaur_ER_Project.Common;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HBDinosaur_ER_Project.SceneSystem
{
    public class TitleSceneController : SceneController
    {

        public override GameScene SceneType { get => GameScene.Title; }
        public void LoadLobby()
        {
            GameSceneManager.Instance.ChangeScene(GameScene.Lobby);
        }

        public void QuitGame()
        {
            GameSceneManager.Instance.QuitGame();
        }
    }
}