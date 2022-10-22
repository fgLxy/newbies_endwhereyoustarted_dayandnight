using System.Collections.Generic;
using UnityEngine;

namespace SunAndMoon
{
    [CreateAssetMenu(menuName = "Serialize/MapInfo")]
    public class MapInfo : ScriptableObject
    {
        //����λ��
        public List<Vector3Int> GroundsPosition;
        //��������
        public List<GroundType> GroundsType;
        //��ͼ�߽�
        public List<Vector3Int> Bounds;
        //����������
        public List<Vector3Int> InteractorsPosition;
        //����������
        public List<InteractorType> InteractorsType;

        public Vector3 CellSize;
    }
}