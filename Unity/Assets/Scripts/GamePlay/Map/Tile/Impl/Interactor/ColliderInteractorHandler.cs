using Cysharp.Threading.Tasks;
using UnityEngine;

namespace SunAndMoon
{
    [Interactor(InteractorType.Collider)]
    public class ColliderInteractorHandler : ITileHandler
    {
        public bool CanLeave(RoleType role, Vector2Int pos)
        {
            throw new System.NotImplementedException();
        }

        public bool CanMoveTo(RoleType role, Vector2Int pos)
        {
            return false;
        }

        public UniTask OnCollision(RoleType role, Vector2Int pos)
        {
            return UniTask.CompletedTask;
        }

        public UniTask OnEnter(RoleType role, Vector2Int pos)
        {
            throw new System.NotImplementedException();
        }

        public UniTask OnLeave(RoleType role, Vector2Int pos)
        {
            throw new System.NotImplementedException();
        }

        public UniTask OnStay(RoleType role, Vector2Int pos)
        {
            throw new System.NotImplementedException();
        }
    }
}