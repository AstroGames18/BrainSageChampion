using BizzyBeeGames;
using BizzyBeeGames.DotConnect;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GiftTopHUD : MonoBehaviour
{
    [SerializeField] GameObject coins, moves, trophy;

    // DarkMode Image Changes
    [SerializeField] Image TrophyCapsule = null, MovesCapsule = null, CoinsCapsule = null;
    [SerializeField] Sprite DarkCapsule = null;

    private void Start()
    {
        trophy.GetComponent<Text>().text = InventoryManager.Instance.GetInventory(InventoryManager.Key_InventoryTrophies).ToString();
        coins.GetComponent<Text>().text = InventoryManager.Instance.GetInventory(InventoryManager.Key_InventoryCoins).ToString();
        moves.GetComponent<Text>().text = InventoryManager.Instance.GetInventory(InventoryManager.Key_InventoryMoves).ToString();
        CheckDarkMode();
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

    public void RefreshHUD()
    {
        int current = int.Parse(trophy.GetComponent<Text>().text);
        int amount = InventoryManager.Instance.GetInventory(InventoryManager.Key_InventoryTrophies) - current;
        delay = 1f / amount;
        StartAddTrophyAnim(amount);
    }
    float delay = 0;

    public void StartAddTrophyAnim(int amount)
    {
        StartCoroutine(AddTrophies(amount));
    }
    IEnumerator AddTrophies(int amount)
    {
        yield return new WaitForSeconds(delay);
        Debug.LogWarning(amount);
        string t = trophy.GetComponent<Text>().text;
        int a = int.Parse(t);
        Debug.LogWarning(a);
        a++;
        Debug.LogWarning(a);
        trophy.GetComponent<Text>().text = a.ToString();
        amount--;
        Debug.LogWarning(amount);
        if (amount > 0)
        {
            StartAddTrophyAnim(amount);
        }
    }
}
