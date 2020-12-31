using BizzyBeeGames;
using BizzyBeeGames.DotConnect;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TopHUD : MonoBehaviour
{
    [SerializeField] GameObject coins, moves, trophy;

    // DarkMode Image Changes
    [SerializeField] Image TrophyCapsule = null, MovesCapsule = null, CoinsCapsule = null;
    [SerializeField] Sprite DarkCapsule = null;

    private void Start()
    {
        GameEventManager.Instance.RegisterEventHandler(GameEventManager.EventId_InventoryUpdated, OnInventoryUpdate);
        RefreshHUD();
        CheckDarkMode();
    }
    private void OnDestroy()
    {
        if (GameEventManager.Instance == null) { return; }
        GameEventManager.Instance.UnRegisterEventHandler(GameEventManager.EventId_InventoryUpdated, OnInventoryUpdate);
    }
    public void CheckDarkMode()
    {
        if (UserDataManager.Instance.IsDarkModeOn())
        {
            TrophyCapsule.sprite = DarkCapsule;
            MovesCapsule.sprite = DarkCapsule;
            CoinsCapsule.sprite = DarkCapsule;

            trophy.GetComponent<Text>().color = Color.white;
            coins.GetComponent<Text>().color = Color.white;
            moves.GetComponent<Text>().color = Color.white;
        }
    }
    private void OnInventoryUpdate(string eventId, object[] data)
    {
        RefreshHUD();
    }
    public void RefreshHUD()
    {
        trophy.GetComponent<Text>().text = InventoryManager.Instance.GetInventory(InventoryManager.Key_InventoryTrophies).ToString();
        coins.GetComponent<Text>().text = InventoryManager.Instance.GetInventory(InventoryManager.Key_InventoryCoins).ToString();
        moves.GetComponent<Text>().text = InventoryManager.Instance.GetInventory(InventoryManager.Key_InventoryMoves).ToString();
    }

    public void StartAddTrophyAnim(int amount)
    {
        StartCoroutine(AddTrophies(amount, InventoryManager.Instance.GetInventory(InventoryManager.Key_InventoryTrophies)));
    }
    IEnumerator AddTrophies(int amount, int current)
    {
        yield return new WaitForSeconds(0.2f);
        trophy.GetComponent<Text>().text = current++.ToString();
        amount--;
        if (amount > 0)
        {
            AddTrophies(amount, current);
        }
    }
}
