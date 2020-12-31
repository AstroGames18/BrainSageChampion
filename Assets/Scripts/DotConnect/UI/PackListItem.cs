using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BizzyBeeGames.DotConnect
{
	public class PackListItem : ClickableListItem
	{
		#region Inspector Variables

		[SerializeField] private Text			nameText				= null;
		[SerializeField] private Text			descriptionText			= null;
		[SerializeField] private ProgressBar	progressBarContainer	= null;
		[SerializeField] private Text			progressText			= null;
		[Space]
		[SerializeField] private GameObject		lockedContainer			= null;
		[SerializeField] private GameObject		starsLockedContainer	= null;
		[SerializeField] private GameObject		iapLockedContainer		= null;
		[SerializeField] private Text			starAmountText			= null;
		[SerializeField] private Text			iapText					= null;

		#endregion

		#region Public Variables

		public void Setup(PackInfo packInfo)
		{
			nameText.text			= packInfo.packName;
			descriptionText.text	= packInfo.packDescription;

			// Check if the pack is locked and update the ui
			bool isPackLocked = GameManager.Instance.IsPackLocked(packInfo);

			lockedContainer.SetActive(isPackLocked);
			progressBarContainer.gameObject.SetActive(!isPackLocked);
			starsLockedContainer.SetActive(isPackLocked && packInfo.unlockType == PackUnlockType.Stars);
			iapLockedContainer.SetActive(isPackLocked && packInfo.unlockType == PackUnlockType.IAP);

			if (isPackLocked)
			{
				switch (packInfo.unlockType)
				{
					case PackUnlockType.Stars:
						starAmountText.text = "Collect " + packInfo.unlockStarsAmount + " ";
						break;
					case PackUnlockType.IAP:
						SetIAPText(packInfo.unlockIAPProductId);
						break;
				}
			}
			else
			{
				int numLevelsInPack		= packInfo.levelFiles.Count;
				int numCompletedLevels	= GameManager.Instance.GetNumCompletedLevels(packInfo);

				progressBarContainer.SetProgress((float)numCompletedLevels / (float)numLevelsInPack);
				progressText.text = string.Format("{0} / {1}", numCompletedLevels, numLevelsInPack);
			}
		}

		#endregion

		#region Private Methods

		private void SetIAPText(string productId)
		{
			string text = "";

			#if BBG_IAP
			UnityEngine.Purchasing.Product product = IAPManager.Instance.GetProductInformation(productId);

			if (product == null)
			{
				text = "Product does not exist";
			}
			else if (!product.availableToPurchase)
			{
				text = "Product not available to purchase";
			}
			else
			{
				text = "Purchase to unlock - " + product.metadata.localizedPriceString;
			}
			#else
			text = "IAP not enabled";
			#endif

			iapText.text = text;
		}

		#endregion
	}
}
