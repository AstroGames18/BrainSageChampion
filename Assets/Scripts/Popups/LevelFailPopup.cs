using Lean.Localization;
using UnityEngine;
using UnityEngine.UI;

namespace BizzyBeeGames.DotConnect
{
    public class LevelFailPopup : Popup
    {
        [SerializeField] GameObject OfferInfoTime, OfferInfoMove, OfferInfoHint;
        [SerializeField] Sprite MovesIcon, TimerIcon;
        [SerializeField] Text BuyButtonPrice, OfferTextTime, OfferTextMoves;
        [SerializeField] RectTransform TitleRect, OfferRect, WatchAdRect, BuyButtonRect, CloseButtonRect, MotivationalTextRect;
        [SerializeField] LeanToken motivationPercentage;

        private bool out_of_time;
        private bool out_of_moves;
        private int buyButtonPrice = 0;
        private int movesToPurchase = 0;

        private float titlePos = -180f;
        private float offerPos = -760f;
        private float MotivationalTextPos = -262f;
        private float adButtonPos = 402f;
        private float buyButtonPos = 399f;
        private float closeButtonPos = -100f;


        private void OnEnable()
        {
            SoundManager.Instance.PlayScreenBGM("LevelLoseBGM");
            Debug.Log(CloseButtonRect.anchoredPosition.x);
            /* Utils.DoSwipeVerticalAnimation(TitleRect, titlePos + 500f, titlePos, 0.5f, 0f);
             Utils.DoSwipeVerticalAnimation(OfferRect, offerPos - 2500f, offerPos, 0.5f, 0.5f);
             Utils.DoSwipeVerticalAnimation(MotivationalTextRect, MotivationalTextPos - 1500f, MotivationalTextPos, 0.5f, 1f);
             Utils.DoSwipeVerticalAnimation(WatchAdRect, adButtonPos - 1500f, adButtonPos, 0.5f, 1.5f);
             Utils.DoSwipeVerticalAnimation(BuyButtonRect, buyButtonPos - 1500f, buyButtonPos, 0.5f, 1.5f);
             Utils.DoSwipeHorizontalAnimation(CloseButtonRect, closeButtonPos + 500f, closeButtonPos, 0.5f, 2f);*/

            SoundManager.Instance.Play("LevelFailedPopup");
        }
        public override void OnShowing(object[] inData)
        {
            base.OnShowing(inData);
            out_of_time = (bool)inData[0];
            out_of_moves = (bool)inData[1];
            movesToPurchase = 10;

            OfferInfoTime.SetActive(out_of_time);
            OfferInfoMove.SetActive(out_of_moves);
            OfferInfoHint.SetActive(GameManager.Instance.levelFailedCount >= 3);

            if (out_of_time) { OfferTextTime.text = "x 3 " + LeanLocalization.GetTranslationText("Minutes"); }
            if (out_of_moves) { OfferTextMoves.text = "x " + movesToPurchase; }

            buyButtonPrice = GetButtonPrice();
            Debug.LogWarning(GameManager.Instance.Grid.GetFillRate());
            motivationPercentage.SetValue(GameManager.Instance.Grid.GetFillRate().ToString());
            BuyButtonPrice.text = "x " + buyButtonPrice;
        }

        int GetButtonPrice()
        {
            int fail_count = GameManager.Instance.levelFailedCount;
            int[] failPrice = GameConfiguration.Instance.LevelFailPrice;

            if (fail_count >= failPrice.Length) { return failPrice[failPrice.Length - 1]; }
            else return failPrice[fail_count - 1];
        }
        public void addtime()
        {
            GameManager.Instance.PlayGameScreenBGM();
            GameManager.Instance.AddTimeToLevel(3 * 60);
            InventoryManager.Instance.UpdateInventory(InventoryManager.Key_InventoryCoins, -buyButtonPrice);
        }
        public void addMoves()
        {
            GameManager.Instance.PlayGameScreenBGM();
            GameManager.Instance.Grid.AddNumMoves(movesToPurchase);
            InventoryManager.Instance.UpdateInventory(InventoryManager.Key_InventoryCoins, -buyButtonPrice);
        }
        public void onBuyButtonPressed()
        {
            SoundManager.Instance.Play("CoinPurchase");
            if (InventoryManager.Instance.GetInventory(InventoryManager.Key_InventoryCoins) >= buyButtonPrice)
            {
                SoundManager.Instance.Play("CoinPurchaseConfirm");
                if (out_of_time) { addtime(); }
                if (out_of_moves) { addMoves(); }
                if (GameManager.Instance.levelFailedCount >= 3)
                {
                    InventoryManager.Instance.UpdateInventory(InventoryManager.Key_InventoryHints, 1);
                }
                onInventoryAdded();
            }
            else
            {
                PopupManager.Instance.Show("Shop");
            }
        }
        private void onInventoryAdded()
        {
            Utils.DoSwipeVerticalAnimation(TitleRect, titlePos, titlePos + 500f, 0.5f, 0f);
            Utils.DoSwipeVerticalAnimation(OfferRect, offerPos, offerPos - 2500f, 0.5f, 0f);
            Utils.DoSwipeVerticalAnimation(MotivationalTextRect, MotivationalTextPos, MotivationalTextPos - 1500f, 0.5f, 0f);
            Utils.DoSwipeVerticalAnimation(WatchAdRect, adButtonPos, adButtonPos - 1500f, 0.5f, 0f);
            Utils.DoSwipeVerticalAnimation(BuyButtonRect, buyButtonPos, buyButtonPos - 1500f, 0.5f, 0f);
            Utils.DoSwipeHorizontalAnimation(CloseButtonRect, closeButtonPos, closeButtonPos + 500f, 0.5f, 0f);

            StartCoroutine(Utils.ExecuteAfterDelay(0.5f, (args) => { HideWithAction("retry"); }));
            SoundManager.Instance.Play("LevelFailCloseAfterPurchase");
        }
        public void onWatchAdButtonPressed()
        {

        }
        public void closePopup()
        {
            HideWithAction("close");
        }
    }
}
