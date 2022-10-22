using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

namespace SunAndMoon
{
    public abstract class RotateInteractorHandler : ITileHandler
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
            throw new System.NotImplementedException();
        }

        public UniTask OnEnter(RoleType role, Vector2Int pos)
        {
            var controller  = GameplayController.Instance.GetRoleController(role);
            controller.SetForceDirect(GetDirect());
            return UniTask.CompletedTask;
        }

        protected abstract Vector2Int GetDirect();

        public UniTask OnLeave(RoleType role, Vector2Int pos)
        {
            return UniTask.CompletedTask;
        }

        public UniTask OnStay(RoleType role, Vector2Int pos)
        {
            return UniTask.CompletedTask;
        }
    }
    
    [Interactor(InteractorType.LeftRotate)]
    public class LeftRotateInteractorHandler : RotateInteractorHandler
    {
        protected override Vector2Int GetDirect()
        {
            return Vector2Int.left;
        }
    }
    [Interactor(InteractorType.RightRotate)]
    public class RightRotateInteractorHandler : RotateInteractorHandler
    {
        protected override Vector2Int GetDirect()
        {
            return Vector2Int.right;
        }
    }
    [Interactor(InteractorType.UpRotate)]
    public class UpRotateInteractorHandler : RotateInteractorHandler
    {
        protected override Vector2Int GetDirect()
        {
            return Vector2Int.up;
        }
    }
    [Interactor(InteractorType.DownRotate)]
    public class DownRotateInteractorHandler : RotateInteractorHandler
    {
        protected override Vector2Int GetDirect()
        {
            return Vector2Int.down;
        }
    }
}