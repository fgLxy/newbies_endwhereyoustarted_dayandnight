using Cysharp.Threading.Tasks;
using UnityEngine;

namespace SunAndMoon
{
    public class LevelHelpPanel : MonoBehaviour
    {
        public async UniTask OnShow()
        {
            await UniTask.CompletedTask;
        }
    }
}