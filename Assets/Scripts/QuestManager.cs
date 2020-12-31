using BizzyBeeGames.DotConnect;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BizzyBeeGames
{
    public class QuestManager : SingletonComponent<QuestManager>
    {
        private List<QuestTier> all_tiers;

        [HideInInspector] public bool waitForClaim = false;
        [HideInInspector] public float questDelayTime = 5.5f;


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
            UserQuest user_quest = UserDataManager.Instance.LoadUserQuest();

            // Load the quest data from the json file
            EditorQuestData data = JsonUtility.FromJson<EditorQuestData>(GameConfiguration.Instance.QuestData.text);
            GameConfiguration.Instance.QuestAllottedTime = data.time_allotted;
            all_tiers = data.all_tiers;

            if (user_quest == null)
            {
                // if no previously saved user_quest is available,
                // then start from beginning.
                ResetQuest();
            }
            GameEventManager.Instance.RegisterEventHandler(GameEventManager.EventId_InventoryUpdated, OnInventoryUpdate);
        }

        private void Update()
        {
            int remaining_time = (int)QuestManager.Instance.GetRemainingTimeForQuest();
            if (remaining_time <= 0 && !waitForClaim)
            {

                if (questDelayTime > 0)
                {
                    questDelayTime -= Time.deltaTime;
                    return;
                }
                questDelayTime = 5.5f;
                UserQuest user_quest = UserDataManager.Instance.LoadUserQuest();

                Debug.Log("user_quest.tier");
                Debug.Log(user_quest.tier);
                if (user_quest.tier > 1) { UserDataManager.Instance.UpdateData("quest_level", 1); }
                ResetQuest();
            }
        }
        private void OnDestroy()
        {
            if (GameEventManager.Instance == null) { return; }
            GameEventManager.Instance.UnRegisterEventHandler(GameEventManager.EventId_InventoryUpdated, OnInventoryUpdate);
        }
        private void OnInventoryUpdate(string eventId, object[] data)
        {
            UserQuest user_quest = UserDataManager.Instance.LoadUserQuest();

            // Whenever inventory is updated it checks if that inventory is
            // related to the current quest and updates the quest data.
            string inventory_type = (string)data[0];
            int amount = (int)data[1];
            if ((user_quest.type == QuestType.collectable_type.COLLECT_STARS && inventory_type == InventoryManager.Key_InventoryStars) ||
                (user_quest.type == QuestType.collectable_type.COLLLECT_LOLLIPOP && inventory_type == InventoryManager.Key_InventoryLollipops) ||
                (user_quest.type == QuestType.collectable_type.PAIR_CANDIES && inventory_type == InventoryManager.Key_InventoryPairedCandies))
            {
                user_quest.amount_collected += amount;
                UserDataManager.Instance.SaveUserQuest(user_quest);
            }
        }
        public void SetUserGiftReward()
        {
            QuestTier current_tier = GetCurrentQuestTierInfo();
            List<QuestReward> reward = current_tier.extra_rewards;
            int trophy_reward = current_tier.trophy_reward;
            int extra_reward_amount = current_tier.extra_rewards_amount;

            List<QuestReward> rewardedListAfterRandom = GetQuestRewards(reward, extra_reward_amount);
            QuestReward trophyReward = new QuestReward
            {
                amount = trophy_reward,
                type = QuestReward.reward_types.TROPHIES
            };
            rewardedListAfterRandom.Add(trophyReward);

            //
            // rewards are given instantly here
            //InventoryManager.Instance.UpdateAllQuestRewards(rewardedListAfterRandom);

            // Set the user gift to show in gift screen later
            GiftToReward UserGiftReward = new GiftToReward
            {
                rewardsToGive = rewardedListAfterRandom,
                rewardType = GetCurrentQuestTierInfo().TypeOfGift,
                id = "QuestScreen"
            };
            UserDataManager.Instance.SetUserGift("QuestScreen", UserGiftReward);
        }
        public List<QuestReward> GetQuestRewards(List<QuestReward> reward, int extra_reward_amount)
        {
            List<ProbabilityData> reward_list = new List<ProbabilityData>();
            List<QuestReward> rewardedListAfterRandom = new List<QuestReward>();
            foreach (QuestReward r in reward)
            {
                reward_list.Add(new ProbabilityData(r.percentage, r));
            }
            for (int i = 0; i < extra_reward_amount; i++)
            {
                QuestReward reward_to_give = (QuestReward)Utils.GetRandomNumberInWeights(reward_list).item;
                rewardedListAfterRandom.Add(reward_to_give);
            }
            return rewardedListAfterRandom;
        }
        public QuestTier GetCurrentQuestTierInfo()
        {
            QuestTier quest_tier;
            UserQuest user_quest = UserDataManager.Instance.LoadUserQuest();

            for (int i = 0; i < all_tiers.Count; i++)
            {
                quest_tier = all_tiers[i];
                if (quest_tier.tier == user_quest.tier)
                {
                    return quest_tier;
                }
            }
            return null;
        }

        public double GetRemainingTimeForQuest()
        {
            DateTime quest_end_time = UserDataManager.Instance.GetUserQuestTime();

            return (quest_end_time - DateTime.Now).TotalSeconds;
        }
        public void ResetQuest()
        {
            QuestTier tier = FindNextTier(1);
            SetQuest(tier);
            UserDataManager.Instance.SaveUserQuestTime();
        }
        public void UpdateQuest()
        {
            UserQuest user_quest = UserDataManager.Instance.LoadUserQuest();
            int target_tier = user_quest.tier + 1;
            if (target_tier >= all_tiers.Count) { target_tier = all_tiers.Count; }
            QuestTier tier = FindNextTier(target_tier);
            SetQuest(tier);
        }
        public void SetQuest(QuestTier tier)
        {
            QuestType quest_type = GetQuestType(tier);
            QuestType.collectable_type type = quest_type.type;
            int max_amount = quest_type.amount;
            UserQuest user_quest = new UserQuest(tier.tier, max_amount, 0, type, DateTime.Now, quest_type.moves_reward);
            UserDataManager.Instance.SaveUserQuest(user_quest);
        }
        private QuestType GetQuestType(QuestTier tier)
        {
            List<QuestType> quest_types = tier.quest_type;
            List<ProbabilityData> type_list = new List<ProbabilityData>();

            foreach (QuestType r in quest_types)
            {
                type_list.Add(new ProbabilityData(r.percentage, r));
            }

            QuestType next_quest = (QuestType)Utils.GetRandomNumberInWeights(type_list).item;
            return next_quest;
        }
        private QuestTier FindNextTier(int target_tier)
        {
            QuestTier tier = all_tiers[0];
            UserQuest user_quest = UserDataManager.Instance.LoadUserQuest();

            if (user_quest != null)
            {
                for (int i = 0; i < all_tiers.Count; i++)
                {
                    if (target_tier == all_tiers[i].tier)
                    {
                        tier = all_tiers[i];
                    }
                }
            }
            return tier;
        }
    }
}
