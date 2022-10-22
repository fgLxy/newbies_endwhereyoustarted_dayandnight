using Cysharp.Threading.Tasks;
using UnityEngine;

namespace SunAndMoon
{
    [Ground(GroundType.Ice)]
    public class IceGroundHandler : ITileHandler
    {
        public bool CanLeave(RoleType role, Vector2Int pos)
        {
            return true;
        }

        public bool CanMoveTo(RoleType role, Vector2Int pos)
        {
            return true;
        }

        public UniTask OnCollision(RoleType role, Vector2Int pos)
        {
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