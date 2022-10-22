using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace SunAndMoon
{
    public class GameplayUI : MonoBehaviour
    {
        public List<int> NeedRulePopLevels;
        public GameObject[] RulePopPanels;
        public GameObject MainUI;

        public static GameplayUI Instance { get; private set; }

        private void Awake()
        {
            var currentLevel = LevelManager.Instance.GetCurrentLevel();
            var index = NeedRulePopLevels.FindIndex(level => level == currentLevel);
            if (index >= 0)
            {
                RulePopPanels[index].gameObject.SetActive(true);
            }
            Instance = this;
            _ = PlayBGM();
        }

        public void ShowMainUI()
        {
            MainUI.SetActive(true);
        }

        private Dictionary<MapType, string[]> _bgms = new Dictionary<MapType, string[]>()
        {
            { MapType.Green, new string[]
                {
                    "music/Bgm/green/With love from Vertex Studio (23)",
                    "music/Bgm/green/With love from Vertex Studio (26)",
                } 
            },
            { MapType.Pink, new string[]
                {
                    "music/Bgm/pink/With love from Vertex Studio (34)"
                }
            },
            { MapType.Yellow, new string[]
                {
                    "music/Bgm/yellow/With love from Vertex Studio (18)",
                    "music/Bgm/yellow/With love from Vertex Studio (35)"
                }
            },
        };
        private bool _finishFlag;
        public void OnGameFinish()
        {

        }
        private async UniTask PlayBGM()
        {
            await UniTask.WaitUntil(() => GameplayController.Instance != null);
            var mapType  = await GameplayController.Instance.GetLevelType();
            while (!_finishFlag)
            {
                await AudioResourceManager.Instance.PlayBGM(RandomHelper.RandomInArray<string>(_bgms[mapType]));
            }
        }

        public void OnClickRevert()
        {
            GameplayController.Instance.GetCommandRollbackManager().RevertTo(1);
            CommandProgress.Instance.UpdateProgress();
        }

        public void OnClickExit()
        {
            Application.Quit();
        }

        public void OnClickShowLevelHelp()
        {
            var level = LevelManager.Instance.GetCurrentLevel();
            for (var i = 0; i < RulePopPanels.Length; i++)
            {
                var rule = RulePopPanels[i];
                rule.SetActive(i + 1 == level);
            }
            MainUI.SetActive(false);
        }

        public void HideMainUI()
        {
            MainUI.SetActive(false);
        }

        public void OnCloseLevelHelp()
        {
            for (var i = 0; i < RulePopPanels.Length; i++)
            {
                var rule = RulePopPanels[i];
                rule.SetActive(false);
            }
            MainUI.SetActive(true);
        }
    }

}