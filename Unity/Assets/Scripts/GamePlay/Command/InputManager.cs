using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace SunAndMoon
{
    public class InputManager : MonoBehaviour
    {
        private Dictionary<KeyCode, ICommand> _inputHandlers = new Dictionary<KeyCode, ICommand>();

        private bool _enableFlag = true;

        public struct PlayerState
        {
            public Vector3 position;
            public Quaternion rotation;
        }

        private void Awake()
        {
            ReflectHelper.FindImplements<ICommand>(type =>
            {
                var attr = ReflectHelper.GetAttribute<CommandAttribute>(type);
                var impl = Activator.CreateInstance(type) as ICommand;
                _inputHandlers[attr.Code] = impl;
            });
        }

        private void Update()
        {
            if (!_enableFlag)
            {
                return;
            }
            foreach (var wrap in _inputHandlers)
            {
                if (Input.GetKeyDown(wrap.Key))
                {
                    _ = PlayCurrentCommand(wrap.Key);
                    break;
                }
            }
        }

        private async Task PlayCurrentCommand(KeyCode key)
        {
            _enableFlag = false;
            await _inputHandlers[key].Execute();
            GameplayController.Instance.GetCommandRollbackManager().RecordSnapshot();
            _enableFlag = true;
            _ = GameplayController.Instance.OnCommandExecuted();
        }

        public void SetEnable(bool flag)
        {
            _enableFlag = flag;
        }
    }
}