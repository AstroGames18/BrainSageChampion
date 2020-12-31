using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace BizzyBeeGames
{
    public class MovesOffer : MonoBehaviour
    {
        [SerializeField] Text MiniOfferMoves, MegaOfferMoves, MiniOfferCoins, MegaOfferCoins;

        private int megaOfferPrice = 0;
        private int miniOfferPrice = 0;
        private int megaOfferReward = 0;
        private int miniOfferReward = 0;
        [SerializeField] Image[] MediumCardsBase, SmallerCardsBase;
        private void OnEnable()
        {
            if (UserDataManager.Instance.IsDarkModeOn())
            {
                GameConfiguration.Instance.SetDarkModeOnCards(MediumCardsBase, SmallerCardsBase);
            }
            miniOfferPrice = GameConfiguration.Instance.MiniOfferCoins;
            megaOfferPrice = GameConfiguration.Instance.MegaOfferCoins;

            miniOfferReward = GameConfiguration.Instance.MiniOfferMoves;
            megaOfferReward = GameConfiguration.Instance.MegaOfferMoves;

            MiniOfferCoins.text = "x " + miniOfferPrice;
            MiniOfferMoves.text = "x " + miniOfferReward;

            MegaOfferCoins.text = "x " + megaOfferPrice;
            MegaOfferMoves.text = "x " + megaOfferReward;
        }
        public void ClaimMiniOffer()
        {
            InventoryManager.Instance.BuyWithCoins(InventoryManager.Key_InventoryMoves, miniOfferReward, miniOfferPrice);
        }
        public void ClaimMegaOffer()
        {
            InventoryManager.Instance.BuyWithCoins(InventoryManager.Key_InventoryMoves, megaOfferReward, megaOfferPrice);
        }
    }
}
