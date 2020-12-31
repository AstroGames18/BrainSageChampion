using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BizzyBeeGames
{
    public class ShopScreen : Popup
    {
        private bool outOfMoves = false;
        public void ClosePopup()
        {
            Hide(true);
            if (outOfMoves) { PopupManager.Instance.Show("QuestScreen", new object[] { true }); }
        }
        public override void OnShowing(object[] inData)
        {
            base.OnShowing(inData);
            outOfMoves = false;
            if (inData != null && inData.Length > 0)
            {
                outOfMoves = (bool)inData[0];
            }
        }
        public void onBuyButton()
        {
            SoundManager.Instance.Play("CashPurchase");
        }
    }
}
