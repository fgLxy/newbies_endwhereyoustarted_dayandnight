using UnityEngine;

namespace SunAndMoon
{
    [Command(KeyCode.W)]
    public class UpWalkCommand : WalkCommand
    {
        public override Vector2Int GetDirect()
        {
            return Vector2Int.up;
        }
    }
    [Command(KeyCode.S)]
    public class DownWalkCommand : WalkCommand
    {
        public override Vector2Int GetDirect()
        {
            return Vector2Int.down;
        }
    }

    [Command(KeyCode.A)]
    public class LeftWalkCommand : WalkCommand
    {
        public override Vector2Int GetDirect()
        {
            return Vector2Int.left;
        }
    }

    [Command(KeyCode.D)]
    public class RightWalkCommand : WalkCommand
    {
        public override Vector2Int GetDirect()
        {
            return Vector2Int.right;
        }
    }
}