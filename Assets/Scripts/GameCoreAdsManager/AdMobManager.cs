using GoogleMobileAds.Api;
using UnityEngine;

public class AdMobManager 
{
    private RewardedAd _rewardedAd;
    private InterstitialAd _interstitialAd;
    private RewardedInterstitialAd _rewardedInterstitialAd;
    private BannerView _bannerView;
    private bool _isInitialized = false;

    private readonly static AdMobManager _instance = new();
    public static AdMobManager Instance { get; } = _instance;
    public void Initialize()
    {
        MobileAds.Initialize((InitializationStatus initstatus) =>
        {
            if (initstatus == null)
            {
                Debug.LogError("Google Mobile Ads initialization failed.");
                return;
            }

            _isInitialized = true;
            LoadAd();
            Debug.Log("Google Mobile Ads initialization complete.");
        });
    }

    private void LoadAd()
    {
        if (!IsInitialized("Main Load Ad")) return;
        LoadInterstitialAd();
        LoadRewardedAd();
        LoadRewardedInterstitialAd();
        LoadBannerAd();
    }


    private void LoadInterstitialAd()
    {
        if (!IsInitialized("Interstitial")) return;


        InterstitialAd.Load("your-interstitial-ad-unit-id", new AdRequest(), (InterstitialAd ad, LoadAdError error) =>
        {
            if (error != null)
            {
                Debug.LogError($"Failed to load interstitial ad: {error.GetMessage()}");
                return;
            }
            _interstitialAd = ad;
            Debug.Log("Interstitial ad loaded successfully.");
        });

    }

    private void LoadRewardedAd()
    {
        if (!IsInitialized("rewarded")) return;

        RewardedAd.Load("your-rewarded-ad-unit-id", new AdRequest(), (RewardedAd ad, LoadAdError error) =>
        {
            if (error != null)
            {
                Debug.LogError($"Failed to load rewarded ad: {error.GetMessage()}");
                return;
            }
            _rewardedAd = ad;
            Debug.Log("Rewarded ad loaded successfully.");
        });
    }

    private void LoadBannerAd()
    {
        if (!IsInitialized("Banner")) return;

        _bannerView = new BannerView("ca-app-pub-3940256099942544/6300978111", AdSize.Banner, AdPosition.Bottom);
        _bannerView.LoadAd(new AdRequest());
        HideBannerAd();
        Debug.Log("Banner ad loaded successfully.");
    }

    private void LoadRewardedInterstitialAd()
    {
        if (!IsInitialized("RewardedInterstital")) return;

        RewardedInterstitialAd.Load("your-rewarded-interstitial-ad-unit-id", new AdRequest(), (RewardedInterstitialAd ad, LoadAdError error) =>
        {
            if (error != null)
            {
                Debug.LogError($"Failed to load rewarded interstitial ad: {error.GetMessage()}");
                return;
            }
            _rewardedInterstitialAd = ad;
            Debug.Log("Rewarded interstitial ad loaded successfully.");
        });
    }


    private bool IsInitialized(string callerName)
    {
        if (!_isInitialized)
        {
            Debug.LogWarning($"Google Mobile Ads is not initialized yet. Cannot load {callerName} ad.");
            return false;
        }
        return true;
    }

    // Implement public methods to show ads of each type and attacht them to button events in the Unity Editor.

    public void ShowRewardedAd()
    {
        if (_rewardedAd != null && _rewardedAd.CanShowAd())
        {
            _rewardedAd.Show(reward =>
            {
                if (reward != null)
                {
                    Debug.Log($"User earned reward: {reward.Amount} {reward.Type}");
                    // Grant the reward to the user here.
                }
            });
        }
        else
        {
            Debug.LogWarning("Rewarded ad is not loaded yet.");
        }
    }

    public void ShowInterstitialAd()
    {
        if (_interstitialAd != null && _interstitialAd.CanShowAd())
        {
            _interstitialAd.Show();
        }
        else
        {
            Debug.LogWarning("Interstitial ad is not loaded yet.");
        }
    }

    public void ShowBannerAd()
    {
        if (_bannerView != null)
        {
            _bannerView.Show();
        }
        else
        {
            Debug.LogWarning("Banner ad is not loaded yet.");
        }
    }


    public void HideBannerAd()
    {
        if (_bannerView != null)
        {
            _bannerView.Hide();
        }
        else
        {
            Debug.LogWarning("Banner ad is not loaded yet.");
        }
    }
    public void ShowRewardedInterstitialAd()
    {
        if (_rewardedInterstitialAd != null && _rewardedInterstitialAd.CanShowAd())
        {
            _rewardedInterstitialAd.Show(reward =>
            {
                if (reward != null)
                {
                    Debug.Log($"User earned reward: {reward.Amount} {reward.Type}");
                    // Grant the reward to the user here.
                }
            });
        }
        else
        {
            Debug.LogWarning("Rewarded interstitial ad is not loaded yet.");
        }
    }


}
