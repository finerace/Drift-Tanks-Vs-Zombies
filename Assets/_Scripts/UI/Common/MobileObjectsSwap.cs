using UnityEngine;
using YG;

public class MobileObjectsSwap : MonoBehaviour
{
    [SerializeField] private GameObject pcObject;
    [SerializeField] private GameObject mobileObject;

    private void OnEnable()
    {
        if (YandexGame.SDKEnabled)
            init();
        else
            YandexGame.GetDataEvent += init;

        void init()
        {
            var isMobile = YandexGame.EnvironmentData.isMobile;
            pcObject.SetActive(!isMobile);
            mobileObject.SetActive(isMobile);
        }
    }
}
