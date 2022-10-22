using UnityEngine;

namespace SunAndMoon
{ 
    public class RulePanel : MonoBehaviour
    {
        public void OnClickClose()
        {
            var activeFlag = gameObject.activeSelf;
            gameObject.SetActive(!activeFlag);
        }
    }
}