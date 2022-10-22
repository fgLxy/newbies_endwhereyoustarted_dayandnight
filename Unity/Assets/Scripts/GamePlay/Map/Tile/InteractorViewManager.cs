using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

namespace SunAndMoon
{
    public class InteractorViewManager : MonoBehaviour
    {
        private Dictionary<Vector2Int, BaseInteractorView> _regists = new Dictionary<Vector2Int, BaseInteractorView>();
        public void RegistInteractorView(BaseInteractorView view)
        {
            var pos = GameplayController.Instance.GetMap().GetTilemapPosition(view.transform.position);
            _regists[pos] = view;
        }

        public void RegistInteractorView(Vector2Int pos, BaseInteractorView view)
        {
            _regists[pos] = view;
        }

        public T GetView<T>(Vector2Int pos) where T : BaseInteractorView
        {
            _regists.TryGetValue(pos, out var interactor);
            return interactor == null ? null : (T)interactor;
        }
    }

    public abstract class BaseInteractorView : MonoBehaviour
    {

        protected async virtual UniTask Awake()
        {
            while (GameplayController.Instance == null || GameplayController.Instance.GetMap() == null)
            {
                await UniTask.Yield(PlayerLoopTiming.EarlyUpdate);
            }
            GameplayController.Instance.GetInteractorViewManager().RegistInteractorView(this);
        }
    }
}