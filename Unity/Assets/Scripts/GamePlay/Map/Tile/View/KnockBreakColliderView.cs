using Cysharp.Threading.Tasks;
using UnityEngine;
using static SunAndMoon.CommandRollbackManager;

namespace SunAndMoon
{
    public class KnockBreakColliderView : BaseInteractorView, RollbackObject
    {
        public bool IsBreak;

        public GameObject Unbreak;
        public GameObject Breaked;

        protected override async UniTask Awake()
        {
            await base.Awake();
            while (GameplayController.Instance == null || GameplayController.Instance.GetCommandRollbackManager() == null)
            {
                await UniTask.Yield(PlayerLoopTiming.EarlyUpdate);
            }
            GameplayController.Instance.GetCommandRollbackManager().RegistRollbackObject(this);
        }


        public bool CanMove()
        {
            return IsBreak;
        }

        public void OnCollision()
        {
            _ = AudioResourceManager.Instance.PlayAudio3DEffect("music/Audio/DM-CGS-10", transform.position);
            IsBreak = true;
            Unbreak.SetActive(false);
            Breaked.SetActive(true);
        }

        public void Refresh()
        {
            Unbreak.SetActive(!IsBreak);
            Breaked.SetActive(IsBreak);
        }

        public SnapshotData GetCurrentSnapshot()
        {
            return new KnockBreakColliderSnapshot
            {
                IsBreak = IsBreak,
            };
        }

        public void Apply(SnapshotData data)
        {
            IsBreak = (data as KnockBreakColliderSnapshot).IsBreak;
            Refresh();
        }

        public class KnockBreakColliderSnapshot : SnapshotData
        {
            public bool IsBreak;
        }

    }
}