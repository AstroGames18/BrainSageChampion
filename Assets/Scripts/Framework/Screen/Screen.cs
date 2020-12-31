using UnityEngine;
using System.Collections.Generic;
using BizzyBeeGames.DotConnect;

namespace BizzyBeeGames
{
	public class Screen : AdjustRectTransformForSafeArea
	{
		#region Classes

		[System.Serializable]
		private class OnTransitionEvent : UnityEngine.Events.UnityEvent { }

		[System.Serializable]
		private class TransitionInfo
		{
			public enum Type
			{
				Fade,
				Swipe
			}
			public enum SwipeDirection
			{
				SWIPE_FROM_TOP,
				SWIPE_FROM_BOTTOM,
				SWIPE_FROM_LEFT,
				SWIPE_FROM_RIGHT
			}

			public bool animate = false;
			public Type animationType = Type.Fade;
			public SwipeDirection swipeDirection = SwipeDirection.SWIPE_FROM_TOP;
			public float animationDuration = 0;
			public UIAnimation.Style animationStyle = UIAnimation.Style.Linear;
			public AnimationCurve animationCurve = null;
			public OnTransitionEvent onTransition = null;
		}

		#endregion

		#region Inspector Variables

		[SerializeField] private string id = "";
		[Space]
		[SerializeField] private TransitionInfo showTransition = null;
		[SerializeField] private TransitionInfo hideTransition = null;
		[SerializeField] private GameObject parentPopup;
		#endregion

		#region Properties

		public string Id { get { return id; } }

		#endregion

		#region Public Methods

		public virtual void Initialize()
		{
		}

		public virtual void Show(bool back, bool immediate, System.Action action = null)
		{
			Transition(showTransition, back, immediate, true, action);
		}

		public virtual void Hide(bool back, bool immediate, System.Action action = null)
		{
			Transition(hideTransition, back, immediate, false, action);
		}
		public void onCloseButtonAnimationComplete()
        {
			GameEventManager.Instance.SendEvent(GameEventManager.EventId_CloseContainerAnimComplete);
		}
		public void onContentAnimationComplete()
		{
			GameEventManager.Instance.SendEvent(GameEventManager.EventId_ContentContainerAnimComplete);
		}
		public void onTitleAnimationComplete()
		{
			GameEventManager.Instance.SendEvent(GameEventManager.EventId_TitleContainerAnimComplete);
		}

		private void Transition(TransitionInfo transitionInfo, bool back, bool immediate, bool show, System.Action action)
		{
			if (transitionInfo.animate)
			{
				// Make sure the screen is showing for the animation
				SetVisibility(true);

				float animationDuration = immediate ? 0 : transitionInfo.animationDuration;

				switch (transitionInfo.animationType)
				{
					case TransitionInfo.Type.Fade:
						StartFadeAnimation(transitionInfo, show, animationDuration);
						break;
					case TransitionInfo.Type.Swipe:
						StartSwipeAnimation(transitionInfo, show, back, animationDuration, action);
						break;
				}

				if (!show)
				{
					if (immediate)
					{
						SetVisibility(false);
					}
				}
			}
			else
			{
				// No animations, set the screen to active or de-active
				SetVisibility(show);
			}

			transitionInfo.onTransition.Invoke();
		}

		/// <summary>
		/// Starts the fade screen transition animation
		/// </summary>
		private void StartFadeAnimation(TransitionInfo transitionInfo, bool show, float duration)
		{
			float fromAlpha = show ? 0f : 1f;
			float toAlpha = show ? 1f : 0f;

			UIAnimation anim = UIAnimation.ScaleY(RectT, fromAlpha, toAlpha, duration);

			anim.style = transitionInfo.animationStyle;
			anim.animationCurve = transitionInfo.animationCurve;
			anim.startOnFirstFrame = true;

			if (!show)
			{
				anim.OnAnimationFinished = (GameObject obj) =>
				{
					SetVisibility(false);
				};
			}
			else
			{
				anim.OnAnimationFinished = null;
			}

			anim.Play();
		}

		/// <summary>
		/// Starts the swipe screen transition animation
		/// </summary>
		private void StartSwipeAnimation(TransitionInfo transitionInfo, bool show, bool back, float duration, System.Action action)
		{
			float from = 0f;
			float to = 0f;
			UIAnimation anim = UIAnimation.PositionY(RectT, from, to, duration);

			switch (transitionInfo.swipeDirection)
			{
				case TransitionInfo.SwipeDirection.SWIPE_FROM_BOTTOM:
					from = RectT.rect.height;
					to = 0f;
					anim = show ? UIAnimation.PositionY(RectT, -from, to, duration) :
						UIAnimation.PositionY(RectT, to, -from, duration);
					break;
				case TransitionInfo.SwipeDirection.SWIPE_FROM_TOP:
					from = -RectT.rect.height;
					to = 0f;
					anim = show ? UIAnimation.PositionY(RectT, -from, to, duration) :
						UIAnimation.PositionY(RectT, to, -from, duration);
					break;
				case TransitionInfo.SwipeDirection.SWIPE_FROM_LEFT:
					from = -RectT.rect.width;
					to = 0f; ;
					anim = show ? UIAnimation.PositionX(RectT, from, to, duration) :
						UIAnimation.PositionX(RectT, to, from, duration);
					break;
				case TransitionInfo.SwipeDirection.SWIPE_FROM_RIGHT:
					from = RectT.rect.height;
					to = 0f;
					anim = show ? UIAnimation.PositionX(RectT, from, to, duration) :
						UIAnimation.PositionX(RectT, to, from, duration);
					break;
			}

			anim.style = transitionInfo.animationStyle;
			anim.animationCurve = transitionInfo.animationCurve;
			anim.startOnFirstFrame = true;

			anim.OnAnimationFinished = (GameObject obj) =>
			{
				if (action != null) { action(); }
				else { SetVisibility(false); }
			};


			anim.Play();
		}

		/// <summary>
		/// Sets the visibility.
		/// </summary>
		private void SetVisibility(bool isVisible)
		{
			CG.alpha = isVisible ? 1f : 0f;
			CG.interactable = isVisible ? true : false;
			CG.blocksRaycasts = isVisible ? true : false;
		}

		#endregion
	}
}
