using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using UnityEngine;
using static SunAndMoon.CommandRollbackManager;

namespace SunAndMoon
{
    public class RoleController<T> : MonoBehaviour, IRoleController, RollbackObject where T : RoleController<T>
    {
        private static T _instance;
        public static T Instance => _instance;

        public float Velocity;

        private Vector2Int _forceDirect;

        public Animator Controller;


        protected async virtual UniTask Awake()
        {
            _instance = (T)this;
            while (GameplayController.Instance == null || GameplayController.Instance.GetCommandRollbackManager() == null)
            {
                await UniTask.Yield(PlayerLoopTiming.EarlyUpdate);
            }
            GameplayController.Instance.GetCommandRollbackManager().RegistRollbackObject(this);
        }

        private void OnDestroy()
        {
            _instance = null;
        }

        public void SetPlayerPosition(InputManager.PlayerState playerState)
        {
            transform.position = playerState.position;
            transform.rotation = playerState.rotation;
        }

        public async UniTask Move(Vector2Int offset, Func<bool> switchCellCallback)
        {
            PlayAnimation(offset);
            var totalTime = 1f / Velocity;
            var x = 0f;
            var origin = transform.position;
            var offsetV3 = new Vector3(offset.x, 0, offset.y);
            
            var interruptFlag = false;
            var seq = DOTween.Sequence();
            await seq.Append(
                DOTween.To(() => x, p =>
                {
                    x = p;
                    transform.position = origin + (Velocity * x * totalTime) * offsetV3;
                }, 0.5f, totalTime / 2f)
                .OnComplete(() =>
                {
                    var rtn = switchCellCallback?.Invoke() ?? true;
                    interruptFlag = !rtn;
                    if (interruptFlag)
                    {
                        x = 1f;
                    }
                })
            )
            .Append(
                DOTween.To(() => x, p =>
                {
                    if (!interruptFlag)
                    {
                        transform.position = origin + (Velocity * x * totalTime) * offsetV3;
                    }
                    x = p;
                }, 1f, totalTime / 2f)
            ).OnComplete(() =>
            {
                if (!interruptFlag)
                {
                    transform.position = origin + (Velocity * totalTime) * offsetV3;
                }
            }).ToUniTask();
        }
        public void FinishMove()
        {
            Controller.Play("Idle");
        }

        private async UniTask PlayAnimation(Vector2Int offset)
        {
            var angle = Mathf.RoundToInt(Vector3.SignedAngle(
                new Vector3(transform.forward.x, 0, transform.forward.z), 
                new Vector3(offset.x, 0, offset.y),
                Vector3.up));
            var animation = "RunForward";
            if (angle == 90)
            {
                animation = "RollRight";
            }
            if (angle == 180)
            {
                animation = "RollBack";
            }
            if (angle == -90 || angle == 270)
            {
                animation = "RollLeft";
            }
            Controller.Play(animation);
            if (animation == "RunForward")
            {
                transform.forward = new Vector3(offset.x, 0, offset.y);
                Velocity = 2f;
            }
            else
            {
                Velocity = 1f;
                var time = UnityEngine.Time.realtimeSinceStartup;
                await UniTask.WaitUntil(() => UnityEngine.Time.realtimeSinceStartup - time >= 0.9f);
                transform.forward = new Vector3(offset.x, 0, offset.y);
            }

        }

        public SnapshotData GetCurrentSnapshot()
        {
            var snapshot = new RoleSnapshotData();
            snapshot.position = transform.position;
            snapshot.rotation = transform.rotation;
            return snapshot;
        }

        public void Apply(SnapshotData data)
        {
            var myData = data as RoleSnapshotData;
            transform.position = myData.position;
            transform.rotation = myData.rotation;
        }

        public bool HasForceDirect()
        {
            return _forceDirect != Vector2Int.zero;
        }

        public Vector2Int GetForceDirect()
        {
            return _forceDirect;
        }

        public void SetForceDirect(Vector2Int direct)
        {
            _forceDirect = direct;
        }

        public class RoleSnapshotData : SnapshotData
        {
            public Vector3 position;
            public Quaternion rotation;
        }

        public async UniTask PlayFallDeadAnimation()
        {
            var x = 0f;

            var origin = transform.position;
            var seq = DOTween.Sequence();
            await seq.AppendInterval(0.5f)
                .Append(DOTween.To(() => x, p =>
                {
                    x = p;
                    var y = 0.5f * 10 * x * x;
                    transform.position = new Vector3(origin.x, origin.y - y, origin.z);
                }, 2f, 1f)).ToUniTask();
        }
    }

}