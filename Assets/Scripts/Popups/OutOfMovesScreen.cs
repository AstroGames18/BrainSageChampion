using BizzyBeeGames.DotConnect;
using UnityEngine;

namespace BizzyBeeGames
{
    public class OutOfMovesScreen : Popup
    {
        private void OnEnable()
        {
            SoundManager.Instance.Play("OutOfMovesAppear");
            SoundManager.Instance.PlayScreenBGM("OutOfMovesBGM");
            GameEventManager.Instance.RegisterEventHandler(GameEventManager.EventId_InventoryUpdated, OnInventoryUpdate);
        }
        private void OnDisable()
        {
            SoundManager.Instance.Play("OutOfMovesDissappear");

            if (GameEventManager.Instance == null) { return; }
            GameEventManager.Instance.UnRegisterEventHandler(GameEventManager.EventId_InventoryUpdated, OnInventoryUpdate);
        }
        public void ClosePopup()
        {
            Hide(true);
            PopupManager.Instance.Show("VipPopup", new object[] { true });
        }
        private void OnInventoryUpdate(string eventId, object[] data)
        {
            GameGrid grid = GameManager.Instance.Grid;
            string inventory_type = (string)data[0];
            int amount = (int)data[1];
            int moveToCheck = grid.GetMinimumMovesAllowedInLevel();
            if (inventory_type == InventoryManager.Key_InventoryMoves && amount >= moveToCheck)
            {
                SoundManager.Instance.Play("OutOfMovesCloseAfterPurchase");

                grid.AddNumMoves(amount);
                Hide(true);
            }
        }
    }
}

