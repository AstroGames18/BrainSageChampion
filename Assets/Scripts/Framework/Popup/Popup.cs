using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace BizzyBeeGames
{
    public class Popup : UIMonoBehaviour
    {
        #region Enums

        protected enum AnimType
        {
            Fade,
            Zoom
        }

        private enum State
        {
            Shown,
            Hidden,
            Showing,
            Hidding
        }

        #endregion

        #region Inspector Variables

        [SerializeField] protected bool canAndroidBackClosePopup;

        [Header("Anim Settings")]
        [SerializeField] protected RectTransform animContainer;

        [Header("Screen Settings")]
        [SerializeField] protected Screen TitleContainer;
        [SerializeField] protected Screen ContentContainer;
        [SerializeField] protected Screen CloseContainer;

        [Header("Audio Settings")]
        public bool IsCustomAudio = false;

        [Header("DarkMode Assets")]
        [SerializeField] Image CloseButton;
        [SerializeField] Image TitleHeader;
        [SerializeField] Image BackgroundCard;

        [SerializeField] Image[] MediumCardsBase, SmallerCardsBase;

        #endregion

        #region Member Variables

        private bool isInitialized;
        private State state;
        private PopupClosed callback;
        #endregion

        #region Properties

        public bool CanAndroidBackClosePopup { get { return canAndroidBackClosePopup; } }
        public Screen TitleScreen { get { return TitleContainer; } }
        public Screen ContentScreen { get { return ContentContainer; } }
        public Screen CloseScreen { get { return CloseContainer; } }

        #endregion

        #region Delegates

        public delegate void PopupClosed(bool cancelled, object[] outData);

        #endregion

        #region Public Methods
        public virtual void Initialize()
        {
            gameObject.SetActive(false);
            state = State.Hidden;
        }

        public void Show()
        {
            Show(null, null);
        }

        public bool Show(object[] inData, PopupClosed callback)
        {
            if (state != State.Hidden)
            {
                return false;
            }

            if (UserDataManager.Instance.IsDarkModeOn())
            {
                GameConfiguration.Instance.SetDarkModeOnPopups(CloseButton, TitleHeader, BackgroundCard);
                GameConfiguration.Instance.SetDarkModeOnCards(MediumCardsBase, SmallerCardsBase);
            }
            this.callback = callback;
            this.state = State.Showing;

            // Show the popup object
            gameObject.SetActive(true);

            OnShowing(inData);


            return true;
        }

        public void Hide(bool cancelled)
        {
            Hide(cancelled, null);
        }

        public void Hide(bool cancelled, object[] outData)
        {
            switch (state)
            {
                case State.Hidden:
                case State.Hidding:
                    return;
            }

            state = State.Hidding;

            if (callback != null)
            {
                callback(cancelled, outData);
            }

            state = State.Hidden;

            OnHiding();
        }

        public void HideWithAction(string action)
        {
            Hide(false, new object[] { action });
        }

        public virtual void OnShowing(object[] inData)
        {
            if (TitleContainer != null)
            {
                Animator anim = TitleContainer.gameObject.GetComponent<Animator>();
                if (anim != null) { anim.SetBool("appear", true); }
            }
            if (ContentContainer != null)
            {
                Animator anim = ContentContainer.gameObject.GetComponent<Animator>();
                if (anim != null) { anim.SetBool("appear", true); }

            }
            if (CloseContainer != null)
            {
                Animator anim = CloseContainer.gameObject.GetComponent<Animator>();
                if (anim != null) { anim.SetBool("appear", true); }
            }
        }

        public virtual void OnHiding()
        {
            if (!IsCustomAudio) { SoundManager.Instance.Play("PopupDissappear"); }

            PopupManager.Instance.OnPopupHiding(this);
            if (ContentContainer != null)
            {
                Animator anim = ContentContainer.gameObject.GetComponent<Animator>();
                if (anim != null) { anim.SetBool("appear", false); }
            }
            if (TitleContainer != null)
            {
                Animator anim = TitleContainer.gameObject.GetComponent<Animator>();
                if (anim != null) { anim.SetBool("appear", false); }
            }
            if (CloseContainer != null)
            {
                Animator anim = CloseContainer.gameObject.GetComponent<Animator>();
                if (anim != null) { anim.SetBool("appear", false); }
            }
            if (ContentContainer == null && CloseContainer == null && TitleContainer == null)
            {
                gameObject.SetActive(false);
            }
        }

        #endregion
    }
}