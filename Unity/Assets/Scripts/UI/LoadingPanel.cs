using UnityEngine;
using UnityEngine.UI;

namespace SunAndMoon
{
    public class LoadingPanel : MonoBehaviour
    {
        public Image SunPanel;
        public Image MoonPanel;

        public static LoadingPanel Instance;
        private Vector3 originSun;
        private Vector3 targetSun;
        private Vector3 originMoon;
        private Vector3 targetMoon;
        private void Awake()
        {
            Instance = this;
            originSun = SunPanel.transform.position;
            targetSun = SunPanel.transform.position + new Vector3(1920, 0, 0);
            originMoon = MoonPanel.transform.position;
            targetMoon = MoonPanel.transform.position + new Vector3(-1920, 0, 0);
        }
        public void UpdatePreLoading(float progress)
        {
            UnityEngine.Debug.Log($"{progress}");
            SunPanel.transform.position = Vector3.Lerp(originSun, targetSun, progress);
        }

        public void UpdateLoading(float progress)
        {
            MoonPanel.transform.position = Vector3.Lerp(originMoon, targetMoon, progress);
        }
    }
}