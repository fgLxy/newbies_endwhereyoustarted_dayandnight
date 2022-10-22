using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

namespace SunAndMoon
{
    public abstract class WindmillInterfactorHandler : ITileHandler
    {
        public bool CanLeave(RoleType role, Vector2Int pos)
        {
            return true;
        }

        public bool CanMoveTo(RoleType role, Vector2Int pos)
        {
            var controller = GameplayController.Instance.GetRoleController(role);
            var currentPos = GameplayController.Instance.GetMap().GetTilemapPosition(controller.transform.position);
            var delta = pos - currentPos;
            return JudgeDirectCanMove(delta);
        }

        protected abstract bool JudgeDirectCanMove(Vector2Int delta);

        public async UniTask OnCollision(RoleType role, Vector2Int pos)
        {
            var view = GameplayController.Instance.GetInteractorViewManager().GetView<WindmillView>(pos);
            await view.OnCollision(role, pos);
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

    [Interactor(InteractorType.LeftUpWindmill)]
    public class LeftUpWindmillInterfactorHandler : WindmillInterfactorHandler
    {

        protected override bool JudgeDirectCanMove(Vector2Int delta)
        {
            return Vector2Int.right == delta || Vector2Int.down == delta;
        }
    }

    [Interactor(InteractorType.RightUpWindmill)]
    public class RightUpWindmillInterfactorHandler : WindmillInterfactorHandler
    {
        protected override bool JudgeDirectCanMove(Vector2Int delta)
        {
            return Vector2Int.left == delta || Vector2Int.down == delta;
        }
    }

    [Interactor(InteractorType.RightBottomWindmill)]
    public class RightBottomWindmillInterfactorHandler : WindmillInterfactorHandler
    {
        protected override bool JudgeDirectCanMove(Vector2Int delta)
        {
            return Vector2Int.left == delta || Vector2Int.up == delta;
        }
    }

    [Interactor(InteractorType.LeftBottomWindmill)]
    public class LeftBottomWindmillInterfactorHandler : WindmillInterfactorHandler
    {
        protected override bool JudgeDirectCanMove(Vector2Int delta)
        {
            return Vector2Int.right == delta || Vector2Int.up == delta;
        }
    }
}