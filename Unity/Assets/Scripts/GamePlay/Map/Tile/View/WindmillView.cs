using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using UnityEngine;

namespace SunAndMoon
{
    public class WindmillView : BaseInteractorView
    {
        private bool _inRotate;
        protected async override UniTask Awake()
        {
            while (GameplayController.Instance == null || GameplayController.Instance.GetMap() == null)
            {
                await UniTask.Yield(PlayerLoopTiming.EarlyUpdate);
            }
            var map = GameplayController.Instance.GetMap();
            var lt = map.GetTilemapPosition(transform.position + new Vector3(-0.5f, 0, 0.5f));
            var rt = map.GetTilemapPosition(transform.position + new Vector3(0.5f, 0, 0.5f));
            var lb = map.GetTilemapPosition(transform.position + new Vector3(-0.5f, 0, -0.5f));
            var rb = map.GetTilemapPosition(transform.position + new Vector3(0.5f, 0, -0.5f));
            map.RegistInteractor(InteractorType.LeftUpWindmill, lt);
            map.RegistInteractor(InteractorType.RightUpWindmill, rt);
            map.RegistInteractor(InteractorType.LeftBottomWindmill, lb);
            map.RegistInteractor(InteractorType.RightBottomWindmill, rb);

            var interactorViewRegister = GameplayController.Instance.GetInteractorViewManager();
            interactorViewRegister.RegistInteractorView(lt, this);
            interactorViewRegister.RegistInteractorView(rt, this);
            interactorViewRegister.RegistInteractorView(lb, this);
            interactorViewRegister.RegistInteractorView(rb, this);
        }
        private Sequence _seq;
        public async UniTask OnCollision(RoleType role, Vector2Int pos)
        {
            var controller = GameplayController.Instance.GetRoleController(role);
            var map = GameplayController.Instance.GetMap();
            var currentPos = map.ToWorldPosition(pos);
            var origin = controller.transform.position;
            var nextPos = new Vector3(currentPos.x, origin.y, currentPos.z);

            var targetAngle = GetTargetAngleDelta(nextPos - origin, nextPos - transform.position);
            if (_inRotate)
            {
                _seq.Kill();
                var x1 = 0f;
                var mainController = GameplayController.Instance.GetMainRoleController();
                var reverseController = GameplayController.Instance.GetReverseController();

                var mainTarget = new Vector3(UnityEngine.Random.Range(10f, 50f), mainController.transform.position.y, UnityEngine.Random.Range(10f, 50f));
                var reverseTarget = new Vector3(UnityEngine.Random.Range(10f, 50f), reverseController.transform.position.y, UnityEngine.Random.Range(10f, 50f));
                var mainOrigin = mainController.transform.position;
                var reverseOrigin = reverseController.transform.position;
                await DOTween.To(() => x1, p1 =>
                {
                    x1 = p1;
                    transform.Rotate(new Vector3(0, x1 * 90f, 0));
                    mainController.transform.position = Vector3.Lerp(mainOrigin, mainTarget, x1);
                    reverseController.transform.position = Vector3.Lerp(reverseOrigin, reverseTarget, x1);
                }, 1f, 1f);
                GameplayController.Instance.OnGameOver();
                return;
            }
            var x = 0f;
            var originAngles = transform.rotation.eulerAngles;
            _seq = DOTween.Sequence().Append(DOTween.To(() => x, p =>
                {
                    x = p;
                    var angle = originAngles.y + targetAngle * x;
                    transform.rotation = Quaternion.Euler(originAngles.x, angle, originAngles.z);
                    var position = Vector3.Slerp(origin, nextPos, x);
                    position.y = controller.transform.position.y;
                    controller.transform.position = position;
                }, 1f, 0.5f));
            _inRotate = true;
            await _seq.ToUniTask();
            controller.transform.LookAt(nextPos);
            controller.transform.position = nextPos;
            GameplayController.Instance.GetCommandRollbackManager().SetDirty();
            _inRotate = false;
        }

        private float GetTargetAngleDelta(Vector3 d, Vector3 offset)
        {
            if (offset.x < 0 && offset.z > 0)//lt
            {
                return (d == Vector3.left || d == Vector3.back) ? -90f : 90f;
            }
            else if(offset.x > 0 && offset.z > 0)//rt
            {
                return (d == Vector3.left || d == Vector3.forward) ? -90f : 90f;
            }
            else if (offset.x > 0 && offset.z < 0)//rb
            {
                return (d == Vector3.right || d == Vector3.forward) ? -90f : 90f;
            }
            else//lb
            {
                return (d == Vector3.right || d == Vector3.back) ? -90f : 90f;
            }    
        }
    }
}