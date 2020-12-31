using BizzyBeeGames.DotConnect;
using UnityEngine;
using UnityEngine.UI;

namespace BizzyBeeGames
{
    public class PauseScreen : Popup
    {
        [SerializeField] Button MusicToggle, SoundToggle;
        [SerializeField] Text MusicLabel, SoundLabel;
        [SerializeField] Image MusicImage, SoundImage, SoundIcon;
        [SerializeField] Color disableToggle, enableToggle;

        private void OnEnable()
        {
            RefreshToggles();
        }
        public void ClosePopup()
        {
            SoundManager.Instance.Play("PauseScreenResume");
            HideWithAction("resume");
        }

        public void GoHome()
        {
            InventoryManager.Instance.SetInventory(InventoryManager.Key_InventoryMoves,
                GameManager.Instance.Grid.currentLevelSaveData.numMoves);
            SoundManager.Instance.Play("PauseScreenHome");

            HideWithAction("home");
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
    }
}

