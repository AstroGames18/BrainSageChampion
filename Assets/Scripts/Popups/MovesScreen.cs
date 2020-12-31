using BizzyBeeGames.DotConnect;
using Lean.Localization;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace BizzyBeeGames
{
    public class MovesScreen : Popup
    {
        [SerializeField] GameObject MovesTimer, Scrollbar, MovesInfoText, MovesText, MovesIconHUD, InstantRecharge, CardAnimatedShine;
        [SerializeField] Text InstantRechargeMoves, InstantRechargePrice;
        [SerializeField] Image Background;
        [SerializeField] Sprite CompleteProgressCardBG, IncompleteProgressCardBG;
        [SerializeField] Animator WhiteGlow;
        [SerializeField] LeanToken rewardMoves;
        [SerializeField] LeanToken addMoves;
        [SerializeField] GameObject[] moveoffers;

        [SerializeField] Image[] MediumCards, SmallCards;
        private void OnEnable()
        {
            GameEventManager.Instance.RegisterEventHandler(GameEventManager.EventId_MovesRechargeTimeUpdated, onMovesRechargeTimeUpdate);
            GameEventManager.Instance.RegisterEventHandler(GameEventManager.EventId_InventoryUpdated, OnInventoryUpdate);
            DisplayStats();
            GameConfiguration.Instance.SetDarkModeOnCards(MediumCards, SmallCards);
        }
        private void OnInventoryUpdate(string eventId, object[] data)
        {
            DisplayStats();
        }
        private void OnDisable()
        {
            if (GameEventManager.Instance == null) { return; }
            GameEventManager.Instance.UnRegisterEventHandler(GameEventManager.EventId_MovesRechargeTimeUpdated, onMovesRechargeTimeUpdate);
            GameEventManager.Instance.UnRegisterEventHandler(GameEventManager.EventId_InventoryUpdated, OnInventoryUpdate);
        }
        private void DisplayStats()
        {
            int inventory_moves = InventoryManager.Instance.GetInventory(InventoryManager.Key_InventoryMoves);
            int moves_required = InventoryManager.Instance.GetRechargeableMoves();
            int max_free_moves = InventoryManager.Instance.GetMaxFreeMoves();

            rewardMoves.SetValue(moves_required);
            InstantRechargeMoves.text = "x " + (max_free_moves - inventory_moves);
            InstantRechargePrice.text = "x " + (max_free_moves - inventory_moves) * GameConfiguration.Instance.InstantRechargeCostPerMove;
            MovesInfoText.GetComponent<Text>().text = Math.Min(inventory_moves, max_free_moves) + "/" + max_free_moves;
            MovesText.SetActive(inventory_moves > max_free_moves);
            addMoves.SetValue((inventory_moves - max_free_moves).ToString());
            Scrollbar.GetComponent<Scrollbar>().size = Math.Min(inventory_moves, max_free_moves) / (float)max_free_moves;
            moveoffers[0].SetActive(inventory_moves <= GameConfiguration.Instance.MovesLowerThreshold);
            InstantRecharge.SetActive(inventory_moves < max_free_moves);
            MovesIconHUD.GetComponent<Animator>().SetBool("recharged", inventory_moves >= max_free_moves);
            WhiteGlow.SetBool("shine", inventory_moves < max_free_moves);
            MovesTimer.GetComponent<Text>().text = Utils.ConvertSecondsToString(0);
            CardAnimatedShine.SetActive(inventory_moves >= max_free_moves);
            if (inventory_moves >= max_free_moves) { Background.sprite = CompleteProgressCardBG; }
            else { Background.sprite = IncompleteProgressCardBG; }
        }
        public void OnInstantRecharge()
        {
            int inventory_moves = InventoryManager.Instance.GetInventory(InventoryManager.Key_InventoryMoves);
            int max_free_moves = InventoryManager.Instance.GetMaxFreeMoves();
            InventoryManager.Instance.BuyWithCoins(InventoryManager.Key_InventoryMoves, (max_free_moves - inventory_moves), (max_free_moves - inventory_moves) * GameConfiguration.Instance.InstantRechargeCostPerMove);
        }
        private void onMovesRechargeTimeUpdate(string eventId, object[] data)
        {
            int time_remaining = (int)data[0];
            MovesTimer.GetComponent<Text>().text = Utils.ConvertSecondsToString(time_remaining);
        }
    }
}
