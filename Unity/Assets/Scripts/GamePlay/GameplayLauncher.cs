using Cysharp.Threading.Tasks;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SunAndMoon
{
    public class GameplayLauncher
    {
        public static async UniTask StartLevel(int level, Action<AsyncOperation> loadLevelFinishCallback = null, LoadSceneMode mode = LoadSceneMode.Additive)
        {
            var levelLoader = SceneManager.LoadSceneAsync("Level", mode);
            loadLevelFinishCallback?.Invoke(levelLoader);
            await UniTask.WaitUntil(() => levelLoader.isDone);
            if (mode == LoadSceneMode.Additive)
            {
                SceneManager.SetActiveScene(SceneManager.GetSceneByName("Level"));
            }
            else
            {
                GameplayUI.Instance.ShowMainUI();
            }
            
            var controller = await GameplayController.Init(level);
            
            var result = await controller.StartGame();


            GameplayUI.Instance.HideMainUI();
            var settlementLoader = SceneManager.LoadSceneAsync("Settlement", LoadSceneMode.Additive);
            await UniTask.WaitUntil(() => settlementLoader.isDone);
            if (result.Success)
            {
                await LevelManager.Instance.LevelComplete();
            }
            await SettlementPanel.Instance.ShowResult(level, result);
        }
    }
}