using System;
using BizzyBeeGames;
using UnityEngine;

public class GameUIManager : MonoBehaviour
{
    public static GameUIManager instance;

    public GameObject LoginScreen, TopHUD, MainScreenElements, BottomHUD;
    public GameObject popupBG;

    public GameObject GuestLoginScreenScreen;

    public GameObject[] MainScreenPopup;

    [SerializeField] StartScreen startScreen;
    void Awake()
    {
        if (instance != null)
        {
            Destroy(instance.gameObject);
        }
        instance = this;
    }

    void ShowLoginScreen()
    {
        LoginScreen.SetActive(true);
        TopHUD.SetActive(false);
        BottomHUD.SetActive(false);
        MainScreenElements.SetActive(false);
    }

    void ShowMainScreen()
    {
        SoundManager.Instance.PlayScreenBGM("StartScreenBGM");
        LoginScreen.SetActive(false);
        TopHUD.SetActive(true);
        BottomHUD.SetActive(true);
        MainScreenElements.SetActive(true);
        startScreen.ShowChapterPack();
        startScreen.AnimateSocialMediaIcons();
        if (UserDataManager.Instance.show_rate_us_popup)
        {
            UserDataManager.Instance.show_rate_us_popup = false;
            ShowPopup("RateUsPopup");
        }
        if (GameConfiguration.Instance.IsGameCompleted())
        {
            ShowPopup("GameCompleted");
        }
        startScreen.ShowGiftScreens();
    }

    public void ShowQuestScreen()
    {
        if (GameConfiguration.Instance.IsGameCompleted())
        {
            ShowPopup("GameCompleted");
        }
        else
        {
            ShowPopup("QuestScreen");
        }
    }

    public void OnFBPlayButtonPressed()
    {
        SoundManager.Instance.Play("FBPlayButtonPressed");
        ShowMainScreen();
    }

    void Start()
    {
        if (!UserDataManager.Instance.session_started)
        {
            ShowLoginScreen();
            UserDataManager.Instance.session_started = true;
        }
        else
        {
            ShowMainScreen();
        }

        for (int i = 0; i < MainScreenPopup.Length; i++)
        {
            MainScreenPopup[i].SetActive(false);
        }
    }

    //void Update()
    //{
    //    if (Input.GetKey(KeyCode.Escape))
    //    {
    //        Debug.Log(KeyCode.Escape);
    //        if (PopupManager.Instance.Showing())
    //        {
    //            PopupManager.Instance.OnBackPressed();
    //            return;
    //        }
    //        PopupManager.Instance.Show("QuitScreen");
    //    }
    //}

    public void QuitYesBtn()
    {
        Application.Quit();
    }

    public void FBClaimBtn()
    {

    }

    public void FBClaimRewardsBtn() { }

    public void FBClaimCancel()
    {

    }

    public void ShowDailyPopup()
    {
        PopupManager.Instance.Show("DailyReward", null, OnDailyRewardClose);
    }

    private void OnDailyRewardClose(bool cancelled, object[] outData)
    {
        startScreen.AnimateSocialMediaIcons();
    }

    public void ShowPopup(string id)
    {
        PopupManager.Instance.Show(id);
    }

    public void CloseHelpPopupBtn(string id)
    {
        for (int i = 0; i < MainScreenPopup.Length; i++)
        {
            MainScreenPopup[i].SetActive(MainScreenPopup[i].gameObject.name == id);
        }
    }
}