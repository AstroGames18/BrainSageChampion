using BizzyBeeGames;
using BizzyBeeGames.DotConnect;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DailyRewardButton : MonoBehaviour
{
    public GameObject QuestionMark, RewardIcon, RewardAmount;
    public Sprite Hint, Undo, Reset, Coins, Moves;
    public ParticleSystem trail, blast, border;

    private string reward_item;
    private int reward_amount;
    private bool card_shown;
    private bool rewards_claimed = false;
    private int index = -1;
    private Dictionary<string, Sprite> ImageDictionary = new Dictionary<string, Sprite>();

    private void onEnable()
    {
        HideCard();
    }
    private void OnDisable()
    {
        if (GameEventManager.Instance == null) { return; }
        GameEventManager.Instance.UnRegisterEventHandler(GameEventManager.EventId_DailyRewardSelected, onDailyRewardSelected);
    }
    private void onDailyRewardSelected(string eventId, object[] data)
    {
        rewards_claimed = true;
        if (!card_shown)
        {
            StartCoroutine(ExecuteAfterTime(1));
        }
    }


    public void ShowTrail(bool show)
    {
        trail.gameObject.SetActive(show);
    }

    public void ShowBlast()
    {
        blast.Play();
    }

    public void ShowBorderParticles()
    {
        border.Play();
    }
    IEnumerator ExecuteAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        ShowCard();
    }
    public int getIndex()
    {
        return index;
    }
    public void SetDailyRewardButton(RewardTypes type, int amount, int i)
    {
        reward_amount = amount;
        GameEventManager.Instance.RegisterEventHandler(GameEventManager.EventId_DailyRewardSelected, onDailyRewardSelected);
        index = i;
        switch (type)
        {
            case RewardTypes.MOVES:
                RewardIcon.GetComponent<Image>().sprite = Moves;
                reward_item = InventoryManager.Key_InventoryMoves;

                break;
            case RewardTypes.HINT:
                RewardIcon.GetComponent<Image>().sprite = Hint;
                reward_item = InventoryManager.Key_InventoryHints;

                break;
            case RewardTypes.UNDO:
                RewardIcon.GetComponent<Image>().sprite = Undo;
                reward_item = InventoryManager.Key_InventoryUndos;

                break;
            case RewardTypes.RESET:
                RewardIcon.GetComponent<Image>().sprite = Reset;
                reward_item = InventoryManager.Key_InventoryResets;

                break;
            case RewardTypes.COINS:
                RewardIcon.GetComponent<Image>().sprite = Coins;
                reward_item = InventoryManager.Key_InventoryCoins;

                break;
        }
        RewardAmount.GetComponent<Text>().text = amount.ToString();
    }
    public void HideCard()
    {
        QuestionMark.SetActive(true);
        RewardIcon.SetActive(false);
        RewardAmount.SetActive(false);
        card_shown = false;
    }
    public void ShowCard()
    {
        GetComponent<Animator>().SetBool("show", true);
        card_shown = true;
    }
    public void SetCardVisible()
    {
        QuestionMark.SetActive(false);
        RewardIcon.SetActive(true);
        RewardAmount.SetActive(true);
    }
    public void ClaimReward()
    {
        if (rewards_claimed) { return; }
        if (card_shown || reward_item == null) { return; }
        InventoryManager.Instance.UpdateInventory(reward_item, reward_amount);
        ShowCard();
        float posX = gameObject.transform.localPosition.x;
        float posY = gameObject.transform.localPosition.y;

        gameObject.transform.localPosition = new Vector3(posX, posY, -2f);
        GameEventManager.Instance.SendEvent(GameEventManager.EventId_DailyRewardSelected, new object[] { index });
    }
}
