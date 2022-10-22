using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace SunAndMoon
{
    public class StartPanel : MonoBehaviour
    {
        private bool _destroyFlag;

        public Animator Sun;
        public Animator Moon;
        public Camera PlayCamera;
        public GameObject RT;

        private async UniTask Awake()
        {
            Sun.Play("RunForward");
            Moon.Play("RunForward");
            var musics = new string[]
            {
                "music/Bgm/Begin/Chunky_Monkey",
                "music/Bgm/Begin/Good Fel",
                "music/Bgm/Begin/Happy",
                "music/Bgm/Begin/InfiniteDoors",
                "music/Bgm/Begin/JumpyGame",
                "music/Bgm/Begin/Memor",
                "music/Bgm/Begin/Potato",
                "music/Bgm/Begin/StreetLove",
                "music/Bgm/Begin/Stupid_Dancer",
                "music/Bgm/Begin/SunnyDay",
                "music/Bgm/Begin/Tiny_Blocks",
                "music/Bgm/Begin/Zephyr",
            };
            while (!_destroyFlag)
            {
                await AudioResourceManager.Instance.PlayBGM(RandomHelper.RandomInArray<string>(musics));
            }
            
        }

        private void OnDestroy()
        {
            _destroyFlag = true;
        }
        public async void OnClickStartButton()
        {
            while (!LevelManager.Instance.HasAllComplete())
            {
                await StartLoading();
                if (LevelManager.Instance.HasAllComplete())
                {
                    await SceneManager.LoadSceneAsync("Start");
                }
            }
        }

        private async UniTask StartLoading()
        {
            await SceneManager.LoadSceneAsync("Loading", LoadSceneMode.Additive);
            if (PlayCamera)
            {
                PlayCamera.gameObject.SetActive(false);
                RT.gameObject.SetActive(false);
                Sun.Play("RunRight");
                Moon.Play("RollForward");
                var originSun = Sun.transform.position;
                var originMoon = Moon.transform.position;
                var targetSun = new Vector3(13, 0, 0);
                var targetMoon = new Vector3(7.5f, 0, -1f);
                await UniTask.WaitUntil(() => LoadingPanel.Instance != null);
                var x = 0f;
                await DOTween.To(() => x, p =>
                {
                    x = p;
                    Sun.transform.position = Vector3.Lerp(originSun, targetSun, p);
                    if (x < 0.1f)
                    {
                        Moon.transform.position = Vector3.Lerp(originMoon, targetMoon, p);
                    }
                    else
                    {
                        Moon.Play("JumpUp");
                    }
                    LoadingPanel.Instance.UpdatePreLoading(p);
                }, 1f, 3f).OnComplete(() => LoadingPanel.Instance.UpdatePreLoading(1f));
                DontDestroyOnLoad(Moon.gameObject);
                await SceneManager.UnloadSceneAsync("Start");
                Moon.Play("RunLeft");

                var level = LevelManager.Instance.GetCurrentLevel();
                await GameplayLauncher.StartLevel(level, op =>
                {
                    _ = AfterLoadLevel(op);
                });
            }
            else
            {
                var level = LevelManager.Instance.GetCurrentLevel();
                await GameplayLauncher.StartLevel(level, op =>
                {
                    
                }, LoadSceneMode.Single);
            }
        }

        private async UniTask AfterLoadLevel(AsyncOperation op)
        {
            var x = 0f;
            await DOTween.To(() => x, p =>
            {
                x = Math.Min(p, op.progress);
                LoadingPanel.Instance.UpdateLoading(p);
                Moon.transform.position = Vector3.Lerp(new Vector3(7.5f, 0, -1f), new Vector3(-13, 0, 0), x);
            }, 1f, 3f);
            await op;
            GameObject.Destroy(Moon);
            await SceneManager.UnloadSceneAsync("Loading");
            GameplayUI.Instance.ShowMainUI();
        }
    }
}