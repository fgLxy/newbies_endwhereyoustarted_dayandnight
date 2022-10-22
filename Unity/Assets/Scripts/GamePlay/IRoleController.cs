using Cysharp.Threading.Tasks;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace SunAndMoon
{
    public interface IRoleController
    {
        UniTask Move(Vector2Int offset, Func<bool> switchCellCallback);

        void SetPlayerPosition(InputManager.PlayerState playerState);
        
        Transform transform { get; }

        bool HasForceDirect();
        Vector2Int GetForceDirect();
        void SetForceDirect(Vector2Int direct);
        UniTask PlayFallDeadAnimation();

        void FinishMove();
    }
}