using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BizzyBeeGames
{
    public class SettingsPopup : Popup
    {
        [SerializeField] Button MusicToggle, SoundToggle;
        [SerializeField] Text MusicLabel, SoundLabel;
        [SerializeField] Image MusicImage, SoundImage, SoundIcon;
        [SerializeField] Color disableToggle, enableToggle;
        [SerializeField] RadioButton DarkModeOn, DarkModeOff;

        private bool darkModeEnabled = false;
        private void OnEnable()
        {
            RefreshToggles();
            darkModeEnabled = UserDataManager.Instance.IsDarkModeOn();
            SetRadioButtons(darkModeEnabled);
        }
        private void ResetDarkMode()
        {
            darkModeEnabled = UserDataManager.Instance.IsDarkModeOn();
            SetDarkMode(darkModeEnabled);
            SetRadioButtons(darkModeEnabled);
        }
        private void SetRadioButtons(bool enable)
        {

            if (enable)
            {
                DarkModeOn.SetValue(true);
                DarkModeOff.SetValue(false);
            }
            else
            {
                DarkModeOn.SetValue(false);
                DarkModeOff.SetValue(true);
            }
        }
        private void RefreshToggles()
        {
            bool soundOn = PlayerPrefs.GetInt("toggle_sound") == 1;
            bool musicOn = PlayerPrefs.GetInt("toggle_music") == 1;
            MusicImage.color = musicOn ? enableToggle : disableToggle;
            SoundImage.color = soundOn ? enableToggle : disableToggle;
            SoundIcon.color = soundOn ? enableToggle : disableToggle;
            SoundLabel.text = "Sound " + (soundOn ? "on" : "off");
            MusicLabel.text = "Music " + (musicOn ? "on" : "off");
        }
        public void onSoundToggle()
        {
            int prevStatus = PlayerPrefs.GetInt("toggle_sound");
            int nextStatus = prevStatus == 0 ? 1 : 0;
            PlayerPrefs.SetInt("toggle_sound", nextStatus);
            RefreshToggles();
        }
        public void onMusicToggle()
        {
            int prevStatus = PlayerPrefs.GetInt("toggle_music");
            int nextStatus = prevStatus == 0 ? 1 : 0;
            PlayerPrefs.SetInt("toggle_music", nextStatus);

            if (nextStatus == 0) { SoundManager.Instance.StopScreenBGM(); }
            else { SoundManager.Instance.PlayScreenBGM("StartScreenBGM"); }
            RefreshToggles();
        }

        public void SetDarkMode(bool enable)
        {
            if (enable == UserDataManager.Instance.IsDarkModeOn())
                return;
            darkModeEnabled = enable;
            if (darkModeEnabled)
                PopupManager.Instance.Show("ChangeToDarkmode", null, onApplySettingsClosed);
            else
                PopupManager.Instance.Show("ChangeFromDarkmode", null, onApplySettingsClosed);
        }


        public void onApplySettingsClosed(bool cancelled, object[] data)
        {
            if (data != null && data.Length > 0)
            {
                string action = (string)data[0];
                switch (action)
                {
                    case "YES":
                        UserDataManager.Instance.SetData("dark_mode_enabled", darkModeEnabled ? 1 : 0);
                        ResetDarkMode();
                        StartCoroutine(Utils.ExecuteAfterDelay(0.5f, (args) => { HideWithAction("Reload"); }));
                        break;
                    case "NO":
                        ResetDarkMode();
                        break;
                }
            }
        }
    }
}

