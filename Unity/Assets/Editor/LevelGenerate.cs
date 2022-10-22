using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace SunAndMoon
{
    public class LevelGenerate
    { 
        [MenuItem("Tools/¹Ø¿¨Éú³É")]
        public static void Generator()
        {
            var go = Selection.activeObject as GameObject;
            if (go == null)
            {
                return;
            }
            var grid = go.GetComponent<Grid>();
            if (grid == null)
            {
                return;
            }
            var info = ScriptableObject.CreateInstance<MapInfo>();
            info.CellSize = grid.cellSize;
            RecordGroundInfo(info, grid);
            RecordBoundsInfo(info, grid);
            RecordInteractorInfo(info, grid);
            AssetDatabase.CreateAsset(info, $"Assets/Resources/map/{grid.name}_info.asset");
            var prefab = CreateMapPrefab(grid);
            PrefabUtility.SaveAsPrefabAsset(prefab, $"Assets/Resources/map/{grid.name}_map.prefab");
            GameObject.DestroyImmediate(prefab);
        }

        private static GameObject CreateMapPrefab(Grid grid)
        {
            var go = new GameObject();
            go.name = grid.name;
            go.AddComponent<Grid>();

            for (var i = 0; i < grid.transform.childCount; i++)
            {
                var child = grid.transform.GetChild(i);
                CreateAndMoveChildren(go, grid, child.name);
            }
            return go;
        }

        private static void CreateAndMoveChildren(GameObject root, Grid grid, string name)
        {
            var trans = grid.transform;
            var go = new GameObject();
            go.name = name;
            go.transform.parent = root.transform;
            var origin = trans.Find(name);
            var hashset = new HashSet<Vector3Int>();
            for (var i = origin.childCount - 1; i >= 0 ; i--)
            {
                var child = origin.GetChild(i);
                var pos = ToVector3Int(child.position, grid.cellSize);
                
                if (IsDuplicate(hashset, pos))
                {
                    GameObject.DestroyImmediate(child.gameObject);
                    continue;
                }

                child.parent = go.transform;
            }
        }

        private static void RecordInteractorInfo(MapInfo info, Grid grid)
        {
            info.InteractorsPosition = new System.Collections.Generic.List<Vector3Int>();
            info.InteractorsType = new System.Collections.Generic.List<InteractorType>();
            var interactor = grid.transform.Find("Interactor");
            var hashset = new HashSet<Vector3Int>();
            for (var i = 0; i < interactor.childCount; i++)
            {
                var child = interactor.GetChild(i);
                var pos = ToVector3Int(child.position, grid.cellSize);
                if (IsDuplicate(hashset, pos))
                {
                    continue;
                }

                info.InteractorsPosition.Add(pos);
                var nameSplits = child.name.Split('_');
                info.InteractorsType.Add(Enum.Parse<InteractorType>(nameSplits[0]));
            }
        }

        private static Vector3Int ToVector3Int(Vector3 position, Vector3 size)
        {
            return new Vector3Int()
            {
                x = Mathf.RoundToInt((position.x + size.x / 2) / size.x),
                y = Mathf.RoundToInt((position.y + size.y / 2) / size.y),
                z = Mathf.RoundToInt((position.z + size.z / 2) / size.z),
            };
        }

        private static void RecordBoundsInfo(MapInfo info, Grid grid)
        {
            info.Bounds = new System.Collections.Generic.List<Vector3Int>();
            var bound = grid.transform.Find("Bound");
            var hashset = new HashSet<Vector3Int>();
            for (var i = 0; i < bound.childCount; i++)
            {
                var child = bound.GetChild(i);
                var pos = ToVector3Int(child.position, grid.cellSize);
                if (IsDuplicate(hashset, pos))
                {
                    continue;
                }

                info.Bounds.Add(pos);
            }
        }

        private static void RecordGroundInfo(MapInfo info, Grid grid)
        {
            info.GroundsPosition = new System.Collections.Generic.List<Vector3Int>();
            info.GroundsType = new System.Collections.Generic.List<GroundType>();
            var hashset = new HashSet<Vector3Int>();
            var ground = grid.transform.Find("Ground");
            for (var i = 0; i < ground.childCount; i++)
            {
                var child = ground.GetChild(i);
                var pos = ToVector3Int(child.position, grid.cellSize);
                if (IsDuplicate(hashset, pos))
                {
                    continue;
                }
                info.GroundsPosition.Add(pos);
                var nameSplits = child.name.Split('_');
                info.GroundsType.Add(Enum.Parse<GroundType>(nameSplits[0]));
            }
        }

        private static bool IsDuplicate(HashSet<Vector3Int> hashset, Vector3Int pos)
        {
            if (hashset.Contains(pos))
            {
                return true;
            }
            hashset.Add(pos);
            return false;
        }
    }

}