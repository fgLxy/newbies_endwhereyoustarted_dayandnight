using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace SunAndMoon
{
    public class SettlementPanel : MonoBehaviour
    {
        private static SettlementPanel _instance;
        public static SettlementPanel Instance => _instance;

        public GameObject WinPanel;
        public GameObject LosePanel;
        public GameObject AllCompletePanel;

        private Dictionary<GameObject, Func<LevelResult, bool>> _checkShowCondition;

        private Dictionary<GameObject, string> _effectMusics;
        private bool _settlementFinishFlag;

        void Awake()
        {
            _instance = this;
            _checkShowCondition = new Dictionary<GameObject, Func<LevelResult, bool>>()
            {
                { WinPanel, (result) => result.Success && !LevelManager.Instance.HasAllComplete()},
                { LosePanel, (result) => !result.Success},
                { AllCompletePanel, (result) => LevelManager.Instance.HasAllComplete()},
            };
            _effectMusics = new Dictionary<GameObject, string>()
            {
                { WinPanel, "music/Audio/succeed_audio"},
                { LosePanel, "music/Audio/lose_audio"},
                { AllCompletePanel, "music/Audio/Pass-audio"},
            };
        }

        public void OnClickContinue()
        {
            _settlementFinishFlag = true;
        }

        public void OnClickExit()
        {
            Application.Quit();
        }

        public async Task ShowResult(int level, LevelResult result)
        {
            foreach (var wrap in _checkShowCondition)
            {
                wrap.Key.SetActive(wrap.Value.Invoke(result));
                if (wrap.Key.activeSelf)
                {
                    _ = AudioResourceManager.Instance.PlayAudioEffect(_effectMusics[wrap.Key]);
                }
            }

            await UniTask.WaitUntil(() => _settlementFinishFlag);
        }
    }

}