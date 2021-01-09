using BizzyBeeGames.DotConnect;
using UnityEngine;
using UnityEngine.UI;

namespace BizzyBeeGames
{
    public class InventoryScreen : Popup
    {
        [SerializeField] GameObject[] inventory_objs;
        [SerializeField] GameObject MovesOffer;

        private void OnInventoryUpdate(string eventId, object[] data)
        {
            DisplayInventoryStats();
        }
        private void OnDisable()
        {
            if (GameEventManager.Instance == null) { return; }
            GameEventManager.Instance.UnRegisterEventHandler(GameEventManager.EventId_InventoryUpdated, OnInventoryUpdate);
        }
        private void OnEnable()
        {
            GameEventManager.Instance.RegisterEventHandler(GameEventManager.EventId_InventoryUpdated, OnInventoryUpdate);
            DisplayInventoryStats();
        }
        public void DisplayInventoryStats()
        {
            inventory_objs[0].GetComponent<Text>().text = InventoryManager.Instance.GetInventory(InventoryManager.Key_InventoryResets).ToString();
            inventory_objs[1].GetComponent<Text>().text = InventoryManager.Instance.GetInventory(InventoryManager.Key_InventoryHints).ToString();
            inventory_objs[2].GetComponent<Text>().text = InventoryManager.Instance.GetInventory(InventoryManager.Key_InventoryUndos).ToString();
            inventory_objs[3].GetComponent<Text>().text = InventoryManager.Instance.GetInventory(InventoryManager.Key_InventoryCoins).ToString();
            inventory_objs[4].GetComponent<Text>().text = InventoryManager.Instance.GetInventory(InventoryManager.Key_InventoryMoves).ToString();
            MovesOffer.SetActive(InventoryManager.Instance.GetInventory(InventoryManager.Key_InventoryMoves) <= GameConfiguration.Instance.MovesLowerThreshold);
        }
        // public void add_hint()
        // {
        //     InventoryManager.Instance.UpdateInventory(InventoryManager.Key_InventoryHints, 5);
        // }
        // public void add_coin()
        // {
        //     InventoryManager.Instance.UpdateInventory(InventoryManager.Key_InventoryCoins, 100);
        // }

        // public void add_reset()
        // {
        //     InventoryManager.Instance.UpdateInventory(InventoryManager.Key_InventoryResets, 5);
        // }
        // public void add_moves()
        // {
        //     InventoryManager.Instance.UpdateInventory(InventoryManager.Key_InventoryMoves, 5);
        // }
        // public void add_undo()
        // {
        //     InventoryManager.Instance.UpdateInventory(InventoryManager.Key_InventoryUndos, 5);
        // }
    }
}


