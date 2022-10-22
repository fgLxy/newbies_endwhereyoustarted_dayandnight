using System.Collections.Generic;
using UnityEngine;

namespace SunAndMoon
{
    public class CommandRollbackManager : MonoBehaviour
    {
        private RollbackStack<Snapshot> _snapshots = new RollbackStack<Snapshot>();

        private HashSet<RollbackObject> _rollbacks = new HashSet<RollbackObject>();

        private bool _needRecord;
        private bool _dirtyFlag;

        public class Snapshot
        {
            public Dictionary<object, SnapshotData> Datas = new Dictionary<object, SnapshotData>();
        }

        public interface SnapshotData { }

        public interface RollbackObject
        {
            SnapshotData GetCurrentSnapshot();

            void Apply(SnapshotData data);
        }

        public int GetCurrentCommandCount()
        {
            return _snapshots.Count - 1;
        }

        public void RegistRollbackObject(RollbackObject obj)
        {
            if (_rollbacks.Contains(obj))
            {
                return;
            }
            _rollbacks.Add(obj);
        }

        public void Init()
        {
            SetDirty();
            RecordSnapshot();
        }

        public void SetDirty()
        {
            _dirtyFlag = true;
        }
        public void RecordSnapshot()
        {
            _needRecord = true;
        }

        public void PreviewRevert(int n)
        {
            var snapshot = _snapshots.Peek(n);
            if (snapshot == null)
            {
                return;
            }
            foreach (var obj in _rollbacks)
            {
                obj.Apply(snapshot.Datas[obj]);
            }
        }

        public void RevertTo(int n)
        {
            var snapshot = _snapshots.Rollback(n);
            if (snapshot == null)
            {
                return;
            }
            foreach (var obj in _rollbacks)
            {
                obj.Apply(snapshot.Datas[obj]);
            }
        }

        private void LateUpdate()
        {
            if (_rollbacks.Count <= 0)
            {
                return;
            }
            if (!_needRecord)
            {
                return;
            }
            _needRecord = false;
            if (!_dirtyFlag)
            {
                return;
            }
            _dirtyFlag = false;

            var snapshot = new Snapshot();
            _snapshots.Push(snapshot);
            foreach (var obj in _rollbacks)
            {
                snapshot.Datas[obj] = obj.GetCurrentSnapshot();
            }
        }
    }
}