using Cysharp.Threading.Tasks;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace SunAndMoon
{
    public abstract class WalkCommand : ICommand
    {
        public abstract Vector2Int GetDirect();
        public async Task Execute()
        {
            var main = ExecuteMainController();
            var reverse = ExecuteReverseController();
            _ = AudioResourceManager.Instance.PlayAudioEffect("music/Audio/Move_audio01", 0.1f);
            await UniTask.WhenAll(new UniTask[]
            {
                main, reverse
            });

            var map = GameplayController.Instance.GetMap();
            if (CheckRoleInSameCell() 
                || map.IsFall(GameplayController.Instance.GetMainRole()) 
                || map.IsFall(GameplayController.Instance.GetReverseRole()))
            {
                GameplayController.Instance.OnGameOver();
                return;
            }
            GameplayController.Instance.GetCommandRollbackManager().RecordSnapshot();
        }

        public Vector2Int GetCurrentDirect(IRoleController role)
        {
            if (role.HasForceDirect())
            {
                return role.GetForceDirect();
            }
            var isMain = role == GameplayController.Instance.GetMainRoleController();
            return isMain ? GetDirect() : -GetDirect();
        }

        private async UniTask ExecuteReverseController()
        {
            var map = GameplayController.Instance.GetMap();
            var controller = GameplayController.Instance.GetReverseController();
            var role = GameplayController.Instance.GetReverseRole();
            while(true)
            {
                var direct = GetCurrentDirect(controller);
                if (map.CanMove(role, direct))
                {
                    var nextPosition = map.GetNextPosition(role, direct);
                    await controller.Move(direct, () =>
                    {
                        map.OnEnter(role, nextPosition, direct);
                        return !CheckRoleInSameCell();
                    });
                    if (map.IsFall(role))
                    {
                        await controller.PlayFallDeadAnimation();
                        controller.FinishMove();
                        return;
                    }
                    if (CheckRoleInSameCell())
                    {
                        controller.FinishMove();
                        return;
                    }
                    GameplayController.Instance.GetCommandRollbackManager().SetDirty();
                    continue;
                }
                else
                {
                    controller.transform.forward = new Vector3(direct.x, 0, direct.y);
                    await map.OnCollision(role, direct);
                }
                break;
            }
            controller.SetForceDirect(Vector2Int.zero);
            map.OnStay(role);
            controller.FinishMove();
        }

        private bool CheckRoleInSameCell()
        {
            var map = GameplayController.Instance.GetMap();
            var mainWPos = GameplayController.Instance.GetMainRoleController().transform.position;
            var reverseWPos = GameplayController.Instance.GetReverseController().transform.position;
            var mainPos = map.GetTilemapPosition(mainWPos);
            var reversePos = map.GetTilemapPosition(reverseWPos);
            return mainPos == reversePos || (mainWPos - reverseWPos).magnitude < 0.5f;
        }

        private async UniTask ExecuteMainController()
        {
            var map = GameplayController.Instance.GetMap();
            var controller = GameplayController.Instance.GetMainRoleController();
            var role = GameplayController.Instance.GetMainRole();
            while (true)
            {
                var direct = GetCurrentDirect(controller);
                if (map.CanMove(role, direct))
                {
                    var nextPosition = map.GetNextPosition(role, direct);
                    await controller.Move(direct, () =>
                    {
                        map.OnEnter(role, nextPosition, direct);
                        return !CheckRoleInSameCell();
                    });
                    if (map.IsFall(role))
                    {
                        await controller.PlayFallDeadAnimation();
                        controller.FinishMove();
                        return;
                    }
                    if (CheckRoleInSameCell())
                    {
                        controller.FinishMove();
                        return;
                    }
                    GameplayController.Instance.GetCommandRollbackManager().SetDirty();
                    continue;
                }
                else
                {
                    controller.transform.forward = new Vector3(direct.x, 0, direct.y);
                    await map.OnCollision(role, direct);
                }
                break;
            }
            controller.SetForceDirect(Vector2Int.zero);
            map.OnStay(role);
            controller.FinishMove();
        }
    }
}