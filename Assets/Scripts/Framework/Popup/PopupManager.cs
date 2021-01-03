using BizzyBeeGames.DotConnect;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace BizzyBeeGames
{
    public class PopupManager : SingletonComponent<PopupManager>
    {
        #region Classes

        [System.Serializable]
        private class PopupInfo
        {
            [Tooltip("The popups id, used to show the popup. Should be unique between all other popups.")]
            public string popupId = "";

            [Tooltip("The Popup component to show.")]
            public Popup popup = null;
        }

        #endregion

        #region Inspector Variables

        [SerializeField] private List<PopupInfo> popupInfos = null;
        [SerializeField] private Volume volume;

        #endregion

        #region Member Variables

        private List<Popup> activePopups;
        private Popup activePopup;
        #endregion

        #region Unity Methods

        protected override void Awake()
        {
            base.Awake();
            activePopups = new List<Popup>();

            for (int i = 0; i < popupInfos.Count; i++)
            {
                popupInfos[i].popup.Initialize();
            }
        }
        private void Start()
        {
            GameEventManager.Instance.RegisterEventHandler(GameEventManager.EventId_TitleContainerAnimComplete, onContainerAnimComplete);
            GameEventManager.Instance.RegisterEventHandler(GameEventManager.EventId_ContentContainerAnimComplete, onContainerAnimComplete);
            GameEventManager.Instance.RegisterEventHandler(GameEventManager.EventId_CloseContainerAnimComplete, onContainerAnimComplete);
        }

        #endregion

        #region Public Methods

        public void Show(string id)
        {
            Show(id, null, null);
        }

        public void Show(string id, object[] inData)
        {
            Show(id, inData, null);
        }

        public void Show(string id, object[] inData, Popup.PopupClosed popupClosed)
        {
            // if (volume != null)
            // {
            //     volume.enabled = true;
            // }
            Popup popup = GetPopupById(id);

            if (popup != null)
            {
                // for (int i = 0; i < activePopups.Count; i++)
                // {
                //     if (activePopups[i].canvas)
                //         ShowChild(activePopups[i].canvas.gameObject, false);
                //}
                if (popup.Show(inData, popupClosed))
                {
                    if (!popup.IsCustomAudio) { SoundManager.Instance.Play("PopupAppear"); }
                    activePopups.Add(popup);
                }
            }
            else
            {
                Debug.LogErrorFormat("[PopupController] Popup with id {0} does not exist", id);
            }
        }

        public bool CloseActivePopup()
        {
            if (activePopups.Count > 0)
            {
                int index = activePopups.Count - 1;

                Popup popup = activePopups[index];

                if (popup.CanAndroidBackClosePopup)
                {
                    popup.Hide(true);
                }

                return true;
            }

            return false;
        }

        public void CloseAllActivePopup()
        {
            if (activePopups.Count > 0)
            {
                for (int i = 0; i < activePopups.Count; i++)
                {
                    Popup popup = activePopups[i];

                    if (popup.CanAndroidBackClosePopup)
                    {
                        popup.Hide(true);
                    }

                }
            }

        }

        public void OnPopupHiding(Popup popup)
        {
            for (int i = activePopups.Count - 1; i >= 0; i--)
            {
                if (popup == activePopups[i])
                {
                    activePopup = activePopups[i];
                    activePopups.RemoveAt(i);
                    break;
                }
            }
            //  if (activePopups.Count > 0 && activePopups[activePopups.Count - 1].canvas)
            //     ShowChild(activePopups[activePopups.Count - 1].canvas.gameObject, true);
        }

        #endregion

        #region Private Methods
        private void onContainerAnimComplete(string eventId, object[] data)
        {
            Popup popup = activePopup;
            if (eventId == GameEventManager.EventId_ContentContainerAnimComplete)
            {
                if (popup != null && popup.CloseScreen == null) { popup.gameObject.SetActive(false); };
            }
            if (eventId == GameEventManager.EventId_CloseContainerAnimComplete)
            {
                if (popup != null) { popup.gameObject.SetActive(false); }
            }

            //  if (activePopups.Count <= 0 && volume != null)
            // {
            // volume.enabled = false;
            // }


        }

        private void ShowChild(GameObject obj, bool show)
        {
            obj.layer = LayerMask.NameToLayer(show ? "Popup" : "Main Canvas");
            Debug.Log("SetLayer");
            Debug.Log(obj.name);
            Debug.Log(show ? "Popup" : "Main Canvas");
            if (obj.transform.childCount > 0)
                foreach (Transform child in obj.transform)
                {
                    ShowChild(child.gameObject, show);
                }
        }
        public Popup GetPopupById(string id)
        {
            for (int i = 0; i < popupInfos.Count; i++)
            {
                if (id == popupInfos[i].popupId)
                {
                    return popupInfos[i].popup;
                }
            }

            return null;
        }

        #endregion
    }
}