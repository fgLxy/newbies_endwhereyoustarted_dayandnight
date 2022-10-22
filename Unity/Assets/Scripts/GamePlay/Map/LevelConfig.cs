using UnityEngine;

namespace SunAndMoon
{
    [CreateAssetMenu(menuName = "Serialize/LevelConfig")]
    public class LevelConfig : ScriptableObject
    {
        //关卡名称
        public string LevelName;
        //主操作对象
        public RoleType MainRole;
        //指令数限制
        public int LimitCommandCount;

        public MapType MapType;
    }
}