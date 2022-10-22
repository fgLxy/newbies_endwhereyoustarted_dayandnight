using Cysharp.Threading.Tasks;
using UnityEngine;

namespace SunAndMoon
{
    [Interactor(InteractorType.KnockBreakCollider)]
    public class KnockBreakColliderHandler : ITileHandler
    {

        private KnockBreakColliderView GetView(Vector2Int pos)
        {
            return GameplayController.Instance.GetInteractorViewManager().GetView<KnockBreakColliderView>(pos);
        }
        public bool CanLeave(RoleType role, Vector2Int pos)
        {
            return true;
        }

        public bool CanMoveTo(RoleType role, Vector2Int pos)
        {
            return GetView(pos).CanMove();
        }

        public UniTask OnCollision(RoleType role, Vector2Int pos)
        {
            GetView(pos).OnCollision();
            return UniTask.CompletedTask;
        }

        public UniTask OnEnter(RoleType role, Vector2Int pos)
        {
            return UniTask.CompletedTask;
        }

        public UniTask OnLeave(RoleType role, Vector2Int pos)
        {
            return UniTask.CompletedTask;
        }

        public UniTask OnStay(RoleType role, Vector2Int pos)
        {
            return UniTask.CompletedTask;
        }
    }
}