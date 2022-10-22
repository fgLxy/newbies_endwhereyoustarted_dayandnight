using Cysharp.Threading.Tasks;
using System;

namespace SunAndMoon
{
    public class LevelManager
    {
        private static LevelManager _instance;
        public static LevelManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new LevelManager();
                }
                return _instance;
            }
        }

        private int _currentLevelId;

        public async UniTask PlayAgain()
        {
            await SaveManager.Instance.Save(new SaveData());
            _currentLevelId = 0;
        }

        public int TotalLevelCount;
        public async UniTask StartAsync(int total)
        {
            TotalLevelCount = total;
            await SaveManager.Instance.StartAsync();
            _currentLevelId = SaveManager.Instance.GetCurrentSaveData().Level;
        }
#if UNITY_EDITOR

        public void JumpToLevel(int level)
        {
            _currentLevelId = level;
        }
#endif

        public int GetCurrentLevel()
        {
            return _currentLevelId <= 0 ? 1 : _currentLevelId;
        }

        public async UniTask LevelComplete()
        {
            _currentLevelId = GetCurrentLevel() + 1;
            await UpdateSaveData();
        }

        public async UniTask UpdateSaveData()
        {
            await SaveManager.Instance.Save(new SaveData()
            {
                Level = _currentLevelId
            });
        }

        public bool HasAllComplete()
        {
            return _currentLevelId > TotalLevelCount;
        }
    }
}