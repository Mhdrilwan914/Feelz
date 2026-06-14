using UnityEngine;

public class AdMobHelper : MonoBehaviour
{
    private void OnEnable()
    {
        AdMobManager.Instance.Initialize();
    }

    public void ShowInterstitialAd()
    {
        AdMobManager.Instance.ShowInterstitialAd();
    }

    public void ShowRewardedAd()
    {
        AdMobManager.Instance.ShowRewardedAd();
    }

    public void ShowRewardedInterstitialAd()
    {
        AdMobManager.Instance.ShowRewardedInterstitialAd();
    }

    public void ShowBannerAd()
    {
        AdMobManager.Instance.ShowBannerAd();
    }

    public void HideBannerAd()
    {
        AdMobManager.Instance.HideBannerAd();
    }
}
