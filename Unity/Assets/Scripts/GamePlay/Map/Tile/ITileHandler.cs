using Cysharp.Threading.Tasks;
using UnityEngine;

namespace SunAndMoon
{
    public interface ITileHandler
    {
        bool CanMoveTo(RoleType role, Vector2Int pos);

        bool CanLeave(RoleType role, Vector2Int pos);

        UniTask OnEnter(RoleType role, Vector2Int pos);

        UniTask OnStay(RoleType role, Vector2Int pos);

        UniTask OnLeave(RoleType role, Vector2Int pos);

        UniTask OnCollision(RoleType role, Vector2Int pos);
    }
}