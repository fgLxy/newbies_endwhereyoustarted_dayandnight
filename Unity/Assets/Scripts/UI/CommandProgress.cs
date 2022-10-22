using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SunAndMoon
{
    public class CommandProgress : MonoBehaviour
    {
        public static CommandProgress Instance { get; private set; }

        public GameObject ProgressItemPrefab;

        private List<Image> _images;

        private void Awake()
        {
            Instance = this;
        }
        public void Init()
        {
            var count = GameplayController.Instance.GetLimitCommandCount();
            _images = new List<Image>();
            for (var i = 0; i < count; i++)
            {
                var go = Instantiate(ProgressItemPrefab, transform);
                var img = go.GetComponent<Image>();
                _images.Add(img);
            }
        }

        public void UpdateProgress()
        {
            var count = GameplayController.Instance.GetCommandRollbackManager().GetCurrentCommandCount();
            for (var i = 0; i < _images.Count; i++)
            {
                var img = _images[i];
                img.color = i < count ? new Color(img.color.r, img.color.g, img.color.b, 0f) : new Color(img.color.r, img.color.g, img.color.b, 1f);
            }
        }
    }
}