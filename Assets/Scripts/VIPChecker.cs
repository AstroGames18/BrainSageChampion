using BizzyBeeGames;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class VIPChecker : MonoBehaviour
{
    [SerializeField] AnimatedScrollbar StarProgressBar;
    [SerializeField] GameObject CardAnimatedShine;
    [SerializeField] Sprite CompleteProgressBG, IncompleteProgressBG;
    [SerializeField] Image Background;

    // DarkMode Image Changes
    [SerializeField] Sprite DarkBackground = null;
    private int max_stars;

    private void Start()
    {
        max_stars = GameConfiguration.Instance.MiniGameMaxStars;
        UpdateStarsInfoDisplay();
    }
    public void claim()
    {
        PopupManager.Instance.Show("VipPopup");
    }
    void UpdateStarsInfoDisplay()
    {
        int stars = Math.Min(UserDataManager.Instance.GetData("mini_game_stars"), max_stars);
        StarProgressBar.SetValue(stars / (float)max_stars);
        CardAnimatedShine.SetActive(stars >= max_stars);
        if (UserDataManager.Instance.IsDarkModeOn()) { Background.sprite = DarkBackground; }
        else if (stars >= max_stars) { Background.sprite = CompleteProgressBG; }
        else { Background.sprite = IncompleteProgressBG; }
    }
}


