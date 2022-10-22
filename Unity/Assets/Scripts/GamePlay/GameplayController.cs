using Cysharp.Threading.Tasks;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace SunAndMoon
{
    public class GameplayController : MonoBehaviour
    {
        private static GameplayController _instance;
        public static GameplayController Instance => _instance;
        public static async Task<GameplayController> Init(int level)
        {
            var go = new GameObject();
            go.name = "GamePlayController";
            var controller = go.AddComponent<GameplayController>();
            controller._level = level;
            //初始化地图
            var mapPrefab = await Resources.LoadAsync<GameObject>($"map/Level{level}_map");
            var map = Instantiate(mapPrefab as GameObject);

            return controller;
        }

        public IRoleController GetRoleController(RoleType role) => role switch
        {
            RoleType.Sun => SunController.Instance,
            RoleType.Moon => MoonController.Instance,
            _ => throw new Exception(),
        };

        private int _level;
        private LevelConfig _config;
        private CommandRollbackManager _commandRollbackManager;
        private InteractorViewManager _interactorViewManager;
        private InputManager _input;
        private Map _map;
        private bool _finishFlag;

        private Vector3 _mainTargetPosition;
        private Vector3 _reverseTargetPosition;

        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(_instance.gameObject);
            }
            _instance = this;
        }

        public async UniTask<MapType> GetLevelType()
        {
            await UniTask.WaitUntil(() => _config != null);
            return _config.MapType;
        }

        private LevelResult _result;

        public async Task<LevelResult> StartGame()
        {
            var manager = new GameObject();
            manager.name = "Manager";
            _interactorViewManager = manager.AddComponent<InteractorViewManager>();
            _commandRollbackManager = manager.AddComponent<CommandRollbackManager>();
            _input = manager.AddComponent<InputManager>();
            var mapInfo = await Resources.LoadAsync<MapInfo>($"map/Level{_level}_info");
            _map = new Map(mapInfo as MapInfo);
            _config = await Resources.LoadAsync<LevelConfig>($"map/Level{_level}_config") as LevelConfig;


            await UniTask.WaitUntil(() => CommandProgress.Instance != null);//稍等一下Mono们的Awake
            _commandRollbackManager.Init();
            CommandProgress.Instance.Init();
            await UniTask.WaitUntil(() => GetMainRoleController() != null && GetReverseController() != null);
            _reverseTargetPosition = GetMainRoleController().transform.position;
            _mainTargetPosition = GetReverseController().transform.position;
            GameplayUI.Instance.ShowMainUI();
            await UniTask.WaitUntil(() => _finishFlag);

            return _result;
        }

        public async UniTask OnCommandExecuted()
        {
            if (_map.GetTilemapPosition(_mainTargetPosition) == _map.GetTilemapPosition(GetMainRoleController().transform.position)
                && _map.GetTilemapPosition(_reverseTargetPosition) == _map.GetTilemapPosition(GetReverseController().transform.position))
            {
                _finishFlag = true;
                _input.SetEnable(false);
                _result = new LevelResult()
                {
                    Success = true,
                };
                return;
            }
            await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);
            CommandProgress.Instance.UpdateProgress();
            if (_commandRollbackManager.GetCurrentCommandCount() >= GetLimitCommandCount())
            {
                _finishFlag = true;
                _input.SetEnable(false);
                _result = new LevelResult()
                {
                    Success = false,
                };
                return;
            }
        }

        public void OnGameOver()
        {
            _result = new LevelResult()
            {
                Success = false,
            };
            _finishFlag = true;
            _input.SetEnable(false);
        }

        public Map GetMap()
        {
            return _map;
        }
        public RoleType GetMainRole()
        {
            return _config.MainRole;
        }

        public RoleType GetReverseRole()
        {
            return _config.MainRole == RoleType.Sun ? RoleType.Moon : RoleType.Sun;
        }

        public IRoleController GetMainRoleController()
        {
            return _config.MainRole == RoleType.Sun ? SunController.Instance : MoonController.Instance;
        }

        public IRoleController GetReverseController()
        {
            return _config.MainRole == RoleType.Moon ? SunController.Instance : MoonController.Instance;
        }

        public CommandRollbackManager GetCommandRollbackManager()
        {
            return _commandRollbackManager;
        }

        public int GetLimitCommandCount()
        {
            return _config.LimitCommandCount;
        }

        public InteractorViewManager GetInteractorViewManager()
        {
            return _interactorViewManager;
        }
    }
}