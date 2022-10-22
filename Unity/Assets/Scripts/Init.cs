using Cysharp.Threading.Tasks;
using UnityEngine;

namespace SunAndMoon
{
    public class Init : MonoBehaviour
    {
        public int TotalLevelCount;
#if UNITY_EDITOR
        public int StartLevel;
#endif
        private async void Awake()
        {
            UniTaskScheduler.UnobservedTaskException += (e) =>
            {
                UnityEngine.Debug.LogError(e);
            };
            await LevelManager.Instance.StartAsync(TotalLevelCount);
#if UNITY_EDITOR
            if (StartLevel > 0 && StartLevel <= 13)
            {
                LevelManager.Instance.JumpToLevel(StartLevel);
            }
#endif
            if (LevelManager.Instance.HasAllComplete())
            {
                await LevelManager.Instance.PlayAgain();
            }
        }
    }
}