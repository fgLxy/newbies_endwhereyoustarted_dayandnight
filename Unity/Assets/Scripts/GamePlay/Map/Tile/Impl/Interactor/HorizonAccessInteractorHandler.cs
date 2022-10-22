using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

namespace SunAndMoon
{
    public abstract class HorizonAccessInteractorHandler : ITileHandler
    {
        public abstract bool CanLeave(RoleType role, Vector2Int pos);

        public bool CanMoveTo(RoleType role, Vector2Int pos)
        {
            return JudgeCanMoveTo(role, pos);
        }

        protected abstract bool JudgeCanMoveTo(RoleType role, Vector2Int pos);

        protected Vector2Int GetMoveDelta(RoleType role, Vector2Int nextPos)
        {
            var controller = GameplayController.Instance.GetRoleController(role);
            var rolePosition = GameplayController.Instance.GetMap().GetTilemapPosition(controller.transform.position);
            return nextPos - rolePosition;
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

    [Interactor(InteractorType.HorizonAccessUp)]
    public class HorizonAccessUpInteractorHandler : HorizonAccessInteractorHandler
    {
        public override bool CanLeave(RoleType role, Vector2Int pos)
        {
            var delta = GetMoveDelta(role, pos);
            return delta.y == 0 || delta == Vector2Int.down;
        }

        protected override bool JudgeCanMoveTo(RoleType role, Vector2Int pos)
        {
            var moveDelta = GetMoveDelta(role, pos);
            return moveDelta.x != 0 || moveDelta == Vector2Int.up;
        }
    }

    [Interactor(InteractorType.HorizonAccessBottom)]
    public class HorizonAccessBottomInteractorHandler : HorizonAccessInteractorHandler
    {
        public override bool CanLeave(RoleType role, Vector2Int pos)
        {
            var delta = GetMoveDelta(role, pos);
            return delta.y == 0 || delta == Vector2Int.up;
        }

        protected override bool JudgeCanMoveTo(RoleType role, Vector2Int pos)
        {
            var moveDelta = GetMoveDelta(role, pos);
            return moveDelta.x != 0 || moveDelta == Vector2Int.down;
        }
    }
}