using System.Collections.Generic;
using UnityEngine;

namespace SunAndMoon
{
    [CreateAssetMenu(menuName = "Serialize/MapInfo")]
    public class MapInfo : ScriptableObject
    {
        //地面位置
        public List<Vector3Int> GroundsPosition;
        //地面类型
        public List<GroundType> GroundsType;
        //地图边界
        public List<Vector3Int> Bounds;
        //交互物坐标
        public List<Vector3Int> InteractorsPosition;
        //交互物类型
        public List<InteractorType> InteractorsType;

        public Vector3 CellSize;
    }
}