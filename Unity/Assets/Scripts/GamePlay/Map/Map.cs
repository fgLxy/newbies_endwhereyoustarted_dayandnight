using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SunAndMoon
{
    public class Map
    {
        private HashSet<Vector2Int> _bounds = new HashSet<Vector2Int>();
        private Dictionary<Vector2Int, InteractorType> _interactors = new Dictionary<Vector2Int, InteractorType>();
        private Dictionary<Vector2Int, GroundType> _grounds = new Dictionary<Vector2Int, GroundType>();
        private Vector3 cellSize;

        public void RegistInteractor(InteractorType type, Vector2Int pos)
        {
            _interactors[pos] = type;
        }

        private Dictionary<InteractorType, ITileHandler> _interactorsHandler = new Dictionary<InteractorType, ITileHandler>();
        private Dictionary<GroundType, ITileHandler> _groundsHandler = new Dictionary<GroundType, ITileHandler>();
        public Map(MapInfo info)
        {
            cellSize = info.CellSize;
            for(var i = 0; i < info.Bounds.Count; i++)
            {
                _bounds.Add(new Vector2Int()
                {
                    x = info.Bounds[i].x,
                    y = info.Bounds[i].z,
                });
            }
            for (var i = 0; i < info.GroundsPosition.Count; i++)
            {
                _grounds.Add(new Vector2Int()
                {
                    x = info.GroundsPosition[i].x,
                    y = info.GroundsPosition[i].z,
                }, info.GroundsType[i]);
            }
            for (var i = 0; i < info.InteractorsPosition.Count; i++)
            {
                _interactors.Add(new Vector2Int()
                {
                    x = info.InteractorsPosition[i].x,
                    y = info.InteractorsPosition[i].z,
                }, info.InteractorsType[i]);
            }

            ReflectHelper.FindImplements<ITileHandler>(handlerType =>
            {
                var groundAttr = ReflectHelper.GetAttribute<GroundAttribute>(handlerType);
                var handler = Activator.CreateInstance(handlerType) as ITileHandler;
                if (groundAttr != null)
                {
                    _groundsHandler[groundAttr.GroundType] = handler;
                }
                else
                {
                    var interactorAttr = ReflectHelper.GetAttribute<InteractorAttribute>(handlerType);
                    _interactorsHandler[interactorAttr.InteractorType] = handler;
                }
            });

        }


        public Vector2Int GetTilemapPosition(Vector3 position)
        {
            return ToTilePosition(position);
        }
        public Vector2Int GetNextPosition(RoleType role, Vector2Int direct)
        {
            return ToTilePosition(role switch
            {
                RoleType.Moon => MoonController.Instance.transform.position,
                RoleType.Sun => SunController.Instance.transform.position,
                _ => throw new System.Exception("Unknown Player Role"),
            }) + direct;
        }

        public bool IsFall(RoleType role)
        {
            var position = role switch
            {
                RoleType.Moon => MoonController.Instance.transform.position,
                RoleType.Sun => SunController.Instance.transform.position,
                _ => throw new System.Exception("Unknown Player Role"),
            };
            var posInt = ToTilePosition(position);
            if (_grounds.TryGetValue(posInt, out var type))
            {
                return false;
            }
            return true;
        }

        public bool CanMove(RoleType role, Vector2Int direct)
        {
            var position = role switch
            {
                RoleType.Moon => MoonController.Instance.transform.position,
                RoleType.Sun => SunController.Instance.transform.position,
                _ => throw new System.Exception("Unknown Player Role"),
            };
            var posInt = ToTilePosition(position);
            var nextPos = posInt + direct;
            if(_interactors.TryGetValue(posInt, out var ilType) && !CanInteractorLeave(role, posInt, nextPos, ilType))
            {
                return false;
            }
            if (_grounds.TryGetValue(posInt, out var glType) && !CanGroundLeave(role, posInt, nextPos, glType))
            {
                return false;
            }
            if (_bounds.Contains(nextPos))
            {
                return false;
            }
            if (_interactors.TryGetValue(nextPos, out var iType) && !CanInteractorMove(role, nextPos, iType))
            {
                return false;
            }
            if (_grounds.TryGetValue(nextPos, out var gType) && !CanGroundMove(role, nextPos, gType))
            {
                return false;
            }
            return true;
        }


        public async UniTask OnCollision(RoleType role, Vector2Int direct)
        {
            var position = role switch
            {
                RoleType.Moon => MoonController.Instance.transform.position,
                RoleType.Sun => SunController.Instance.transform.position,
                _ => throw new System.Exception("Unknown Player Role"),
            };
            var posInt = ToTilePosition(position);
            var nextPos = posInt + direct;
            if (_bounds.Contains(nextPos))
            {
                return;
            }
            if (_interactors.TryGetValue(nextPos, out var iType))
            {
                await OnInteractorCollision(role, nextPos, iType);
                return;
            }
            if (_grounds.TryGetValue(nextPos, out var gType))
            {
                await OnGroundCollision(role, nextPos, gType);
                return;
            }
        }

        public void OnStay(RoleType role)
        {
            var position = role switch
            {
                RoleType.Moon => MoonController.Instance.transform.position,
                RoleType.Sun => SunController.Instance.transform.position,
                _ => throw new System.Exception("Unknown Player Role"),
            };
            var posInt = ToTilePosition(position);
            if (_interactors.TryGetValue(posInt, out var iType))
            {
                OnInteractorStay(role, posInt, iType);
                return;
            }
            if (_grounds.TryGetValue(posInt, out var gType))
            {
                OnGroundStay(role, posInt, gType);
                return;
            }
        }

        public void OnEnter(RoleType role, Vector2Int pos, Vector2Int direct)
        {
            var posInt = pos - direct;
            if (_interactors.TryGetValue(posInt, out var iType))
            {
                OnInteractorLeave(role, posInt, iType);
                return;
            }
            if (_grounds.TryGetValue(posInt, out var gType))
            {
                OnGroundLeave(role, posInt, gType);
                return;
            }
            var nextPos = pos;
            if (_interactors.TryGetValue(nextPos, out var ieType))
            {
                OnInteractorEnter(role, nextPos, iType);
                return;
            }
            if (_grounds.TryGetValue(nextPos, out var geType))
            {
                OnGroundEnter(role, nextPos, gType);
                return;
            }
        }

        private void OnGroundStay(RoleType role, Vector2Int pos, GroundType type)
        {
            _groundsHandler[type].OnStay(role, pos);
        }

        private void OnInteractorStay(RoleType role, Vector2Int pos, InteractorType type)
        {
            _interactorsHandler[type].OnStay(role, pos);
        }

        private void OnGroundLeave(RoleType role, Vector2Int pos, GroundType type)
        {
            _groundsHandler[type].OnLeave(role, pos);
        }

        private void OnInteractorLeave(RoleType role, Vector2Int pos, InteractorType type)
        {
            _interactorsHandler[type].OnLeave(role, pos);
        }

        private void OnGroundEnter(RoleType role, Vector2Int pos, GroundType type)
        {
            _groundsHandler[type].OnEnter(role, pos);
        }

        private void OnInteractorEnter(RoleType role, Vector2Int pos, InteractorType type)
        {
            _interactorsHandler[type].OnEnter(role, pos);
        }

        private bool CanGroundLeave(RoleType role, Vector2Int pos, Vector2Int nextPos, GroundType type)
        {
            return _groundsHandler[type].CanLeave(role, nextPos);
        }

        private bool CanInteractorLeave(RoleType role, Vector2Int pos, Vector2Int nextPos, InteractorType type)
        {
            return _interactorsHandler[type].CanLeave(role, nextPos);
        }

        private bool CanGroundMove(RoleType role, Vector2Int pos, GroundType type)
        {
            return _groundsHandler[type].CanMoveTo(role, pos);
        }

        private bool CanInteractorMove(RoleType role, Vector2Int pos, InteractorType type)
        {
            return _interactorsHandler[type].CanMoveTo(role, pos);
        }

        
        private async UniTask OnGroundCollision(RoleType role, Vector2Int pos, GroundType type)
        {
            await _groundsHandler[type].OnCollision(role, pos);
        }

        private async UniTask OnInteractorCollision(RoleType role, Vector2Int pos, InteractorType type)
        {
            await _interactorsHandler[type].OnCollision(role, pos);
        }

        private Vector2Int ToTilePosition(Vector3 position)
        {
            return new Vector2Int()
            { 
                x = Mathf.RoundToInt((position.x + cellSize.x / 2) / (cellSize.x)),
                y = Mathf.RoundToInt((position.z + cellSize.z / 2) / (cellSize.z)),
            };
        }

        public Vector3 ToWorldPosition(Vector2Int pos)
        {
            return new Vector3()
            {
                x = pos.x * cellSize.x - (cellSize.x / 2f),
                y = 0,
                z = pos.y * cellSize.z - (cellSize.z / 2f),
            };
        }
    }
}