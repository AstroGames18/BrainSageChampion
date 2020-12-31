using BizzyBeeGames.DotConnect;
using System;
using System.Collections.Generic;

namespace BizzyBeeGames
{
    public class InventoryManager : SingletonComponent<InventoryManager>
    {
        private int moves_time_gap = 0;
        private int max_moves_recharge = 0;
        private int max_free_moves = 0;

        public const string Key_InventoryMoves = "inventory_moves";
        public const string Key_InventoryUndos = "inventory_undos";
        public const string Key_InventoryHints = "inventory_hints";
        public const string Key_InventoryResets = "inventory_resets";
        public const string Key_InventoryCoins = "inventory_coins";
        public const string Key_InventoryStars = "inventory_stars";
        public const string Key_InventoryTrophies = "inventory_trophies";
        public const string Key_InventoryLollipops = "inventory_lollipops";
        public const string Key_InventoryPairedCandies = "inventory_paired_candies";

        public bool movesRecharged { get; set; }

        protected override void Awake()
        {
            if (Exists())
            {
                Destroy(gameObject);
            }
            else
            {
                base.Awake();
                DontDestroyOnLoad(this);
            }
        }
        private void Start()
        {
            max_free_moves = GameConfiguration.Instance.MovesRechargeMaximumFreeMoves;
            max_moves_recharge = GameConfiguration.Instance.MovesRechargedPerTimeGap;
            moves_time_gap = GameConfiguration.Instance.MovesRechargeTimeGapInMinutes;
            GameEventManager.Instance.RegisterEventHandler(GameEventManager.EventId_InventoryUpdated, OnInventoryUpdate);
            if (UserDataManager.Instance.GetDateTime("install_time") == new DateTime(1970, 1, 1, 0, 0, 0))
            {
                UpdateInventory(InventoryManager.Key_InventoryMoves, GameConfiguration.Instance.FirstTimeFreeMoves);
                UpdateInventory(InventoryManager.Key_InventoryHints, GameConfiguration.Instance.FirstTimeFreeHints);
                UpdateInventory(InventoryManager.Key_InventoryResets, GameConfiguration.Instance.FirstTimeFreeResets);
                UpdateInventory(InventoryManager.Key_InventoryUndos, GameConfiguration.Instance.FirstTimeFreeUndos);
                UpdateInventory(InventoryManager.Key_InventoryCoins, GameConfiguration.Instance.FirstTimeFreeCoins);
                UserDataManager.Instance.SetData("install_time", DateTime.Now);
            }
        }
        private void OnInventoryUpdate(string eventId, object[] data)
        {
            string inventory_type = (string)data[0];
            int amount = (int)data[1];

            if (inventory_type == InventoryManager.Key_InventoryStars)
            {
                UserDataManager.Instance.UpdateData("mini_game_stars", amount);
            }
        }
        public void UpdateInventory(string item, int amount = 0)
        {
            int prev_amount = GetInventory(item);
            int total = prev_amount + amount;

            SetInventory(item, total);
            GameEventManager.Instance.SendEvent(GameEventManager.EventId_InventoryUpdated, item, amount);
        }
        public int GetInventory(string item)
        {
            int value = UserDataManager.Instance.GetData(item);
            return value;
        }
        public void SetInventory(string item, int amount)
        {
            amount = amount < 0 ? 0 : amount;
            UserDataManager.Instance.SetData(item, amount);
        }

        public void UpdateAllQuestRewards(List<QuestReward> giftRewards)
        {
            for (int j = 0; j < giftRewards.Count; j++)
            {
                UpdateQuestReward(giftRewards[j]);
            }
        }

        public void UpdateQuestReward(QuestReward reward)
        {
            switch (reward.type)
            {
                case QuestReward.reward_types.HINT:
                    UpdateInventory(Key_InventoryHints, reward.amount);

                    break;
                case QuestReward.reward_types.MOVES:
                    UpdateInventory(Key_InventoryMoves, reward.amount);

                    break;
                case QuestReward.reward_types.RESET:
                    UpdateInventory(Key_InventoryResets, reward.amount);

                    break;
                case QuestReward.reward_types.UNDO:
                    UpdateInventory(Key_InventoryUndos, reward.amount);

                    break;
                case QuestReward.reward_types.COINS:
                    UpdateInventory(Key_InventoryCoins, reward.amount);

                    break;

                case QuestReward.reward_types.TROPHIES:
                    UpdateInventory(Key_InventoryTrophies, reward.amount);

                    break;
            }
        }
        public void BuyWithCoins(string keyInventory, int amount, int price)
        {
            SoundManager.Instance.Play("CoinPurchase");
            if (GetInventory(Key_InventoryCoins) >= price)
            {
                SoundManager.Instance.Play("CoinPurchaseConfirm");
                UpdateInventory(Key_InventoryCoins, -price);
                UpdateInventory(keyInventory, amount);
            }
            else
            {
                PopupManager.Instance.Show("Shop");
            }
        }
        public void ClaimProbableRewards(ProbableRewards all_rewards)
        {
            List<ProbabilityData> reward_list = new List<ProbabilityData>();
            foreach (QuestReward item in all_rewards.list)
            {
                reward_list.Add(new ProbabilityData(item.percentage, item));
            }
            for (int i = 0; i < all_rewards.max_reward; i++)
            {
                QuestReward reward_item = (QuestReward)Utils.GetRandomNumberInWeights(reward_list).item;
                UpdateQuestReward(reward_item);
            }
        }
        public List<QuestReward> GetProbableRewards(ProbableRewards all_rewards)
        {
            List<ProbabilityData> reward_list = new List<ProbabilityData>();
            List<QuestReward> allRewards = new List<QuestReward>();

            foreach (QuestReward item in all_rewards.list)
            {
                reward_list.Add(new ProbabilityData(item.percentage, item));
            }
            for (int i = 0; i < all_rewards.max_reward; i++)
            {
                QuestReward reward_item = (QuestReward)Utils.GetRandomNumberInWeights(reward_list).item;
                allRewards.Add(reward_item);
            }
            return allRewards;
        }
        private void Update()
        {
            int inventory_moves = GetInventory(Key_InventoryMoves);
            if (inventory_moves < max_free_moves)
            {
                DateTime next_recharge = UserDataManager.Instance.GetDateTime("moves_recharge_time");
                if (next_recharge == new DateTime(1970, 1, 1, 0, 0, 0))
                {
                    UserDataManager.Instance.SetData("moves_recharge_time", DateTime.Now.AddMinutes(moves_time_gap));
                    next_recharge = UserDataManager.Instance.GetDateTime("moves_recharge_time");
                }
                int remaining_time = (int)(next_recharge - DateTime.Now).TotalSeconds;
                if (remaining_time < 0)
                {
                    int difference_time = Math.Abs((int)(next_recharge - DateTime.Now).TotalMinutes);
                    int multiplier = (difference_time / moves_time_gap) + 1;
                    int moves_reward = GetRechargeableMoves() * multiplier;
                    int remaining_time_till_next = moves_time_gap * 60 - (Math.Abs((int)(next_recharge - DateTime.Now).TotalSeconds) % (moves_time_gap * 60));

                    moves_reward = (inventory_moves + moves_reward > max_free_moves) ? max_free_moves - inventory_moves :
                        moves_reward;

                    UpdateInventory(Key_InventoryMoves, moves_reward);
                    if (remaining_time_till_next > 0)
                    {
                        UserDataManager.Instance.SetData("moves_recharge_time", DateTime.Now.AddSeconds(remaining_time_till_next));
                    }
                }
                GameEventManager.Instance.SendEvent(GameEventManager.EventId_MovesRechargeTimeUpdated, remaining_time);
            }
            else
            {
                UserDataManager.Instance.SetData("moves_recharge_time", new DateTime(1970, 1, 1, 0, 0, 0));
            }
            movesRecharged = inventory_moves >= max_free_moves;
        }
        public int GetRechargeableMoves()
        {
            int inventory_moves = GetInventory(Key_InventoryMoves);
            int moves_required = inventory_moves <= max_free_moves ? Math.Min(max_free_moves - inventory_moves, max_moves_recharge) : 0;

            return moves_required;
        }
        public int GetMaxFreeMoves()
        {
            return max_free_moves;
        }
        public ProbableRewards CheckProfileScreenAcheivements(int inventory_item, int update_amount, List<ProfileTier> profiles)
        {
            ProfileTier profile_tier;

            for (int i = 0; i < profiles.Count; i++)
            {
                profile_tier = profiles[i];
                if (inventory_item >= profile_tier.min_value &&
                    inventory_item < profile_tier.max_value &&
                    inventory_item + update_amount >= profile_tier.max_value)
                {
                    UserDataManager.Instance.UpdateData("notification_profilescreen", 1);
                    return profile_tier.reward;
                }
            }
            return null;
        }
        public bool CheckChapterScreenAcheivements(int level)
        {
            ChapterTier chapter_tier;
            List<ChapterTier> chapters = GameConfiguration.Instance.ChapterTiers;

            for (int i = 0; i < chapters.Count; i++)
            {
                chapter_tier = chapters[i];
                if (level >= chapter_tier.min_level &&
                    level < chapter_tier.max_level &&
                    level + 1 >= chapter_tier.max_level)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
