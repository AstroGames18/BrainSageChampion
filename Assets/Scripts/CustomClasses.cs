using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ChapterTier
{
    public int min_level;
    public int max_level;
    public string banner_message;
    public Sprite chapter_image;
    public Sprite chapter_moves_counter_bg;
    public Sprite chapter_moves_progress;
    public Sprite chapter_top_hud_bg;
    public Sprite chapter_pause_image;
    public ProbableRewards reward;
    public ChapterPack chapter_pack;
}
[Serializable]
public class UserQuest
{
    public int tier;
    public int max_amount;
    public int amount_collected;
    public QuestType.collectable_type type;
    public DateTime quest_started;
    public int moves_rewarded = 0;

    public UserQuest (int t, int m_a, int a_c, QuestType.collectable_type q_t, DateTime q_s, int moves)
    {
        tier = t;
        max_amount = m_a;
        amount_collected = a_c;
        type = q_t;
        quest_started = q_s;
        moves_rewarded = moves;
    }
}
[Serializable]
public class LevelFile
{
    public TextAsset File;
    public List<TextAsset> Iterations;
}
[Serializable]
public class RangedValue
{
    public int min_value;
    public int max_value;
}
[Serializable]
public class ProbableRewards
{
    public List<QuestReward> list;
    public int max_reward;
    public GiftType giftType;
}
[Serializable]
public class DefiniteRewards
{
    public int moves;
    public int trophies;
    public int hint;
    public int undo;
    public int reset;
    public int coins;
}
[Serializable]
public class QuestOfferItem
{
    public RangedValue item;
    public DefiniteRewards item_reward;
}
[Serializable]
public class QuestOfferPack
{
    public int coins;
    public QuestOfferItem stars;
    public QuestOfferItem lollipops;
    public QuestOfferItem paired_candies;
    [HideInInspector]
    public QuestOfferItem selected_offer;
}
public class ProbabilityData
{
    public float percentatage;
    public object item;

    public ProbabilityData(float p, object i)
    {
        percentatage = p;
        item = i;
    }
}
public class ProbabilityResult
{
    public float min_value;
    public float max_value;
    public object item;
    public ProbabilityResult(float min_v, float max_v, object i)
    {
        min_value = min_v;
        max_value = max_v;
        item = i;
    }
}

public class EditorQuestData
{
    public int version;
    public float time_allotted = 0;
    public List<QuestTier> all_tiers;
}

[Serializable]
public class QuestTier
{
    public List<QuestType> quest_type = new List<QuestType>();
    public int tier = 0;
    public int trophy_reward = 0;
    public int extra_rewards_amount = 0;
    public GiftType TypeOfGift = GiftType.BLUE;
    public List<QuestReward> extra_rewards = new List<QuestReward>();
}
[Serializable]
public class LevelMessageOffers
{
    public SelectableReward OfferOne = null;
    public SelectableReward OfferTwo = null;
    public float PriceOfOffer = 0f;
}

[Serializable]
public class QuestType
{
    public enum collectable_type
    {
        COLLECT_STARS,
        COLLLECT_LOLLIPOP,
        PAIR_CANDIES
    }
    public collectable_type type = collectable_type.COLLECT_STARS;
    public int amount = 0;
    public float percentage;
    public int moves_reward = 0;
}

[Serializable]
public class QuestReward
{
    public enum reward_types
    {
        TROPHIES,
        HINT,
        RESET,
        MOVES,
        UNDO,
        COINS
    }
    public int amount = 0;
    public float percentage = 0;
    public reward_types type = reward_types.HINT;
}
[Serializable]
public class DailyRewardConfig
{
    public List<SelectableReward> reward = new List<SelectableReward>();
    public float probability = 0f;
}
[Serializable]
public class SelectableReward
{
    public int amount = 0;
    public RewardTypes type = RewardTypes.HINT;
}
[Serializable]
public enum GiftType
{
    BLUE,
    PINK,
    GREEN,
    RED,
    YELLOW
}
[Serializable]
public enum RewardTypes
{
    TROPHIES,
    HINT,
    RESET,
    MOVES,
    UNDO,
    COINS
}
[Serializable]
public class GiftToReward
{
    public List<QuestReward> rewardsToGive;
    public GiftType rewardType;
    public int moves = 0;
    public string id;
    public bool isUnclaimed = false;
    public String title = "";
}
[Serializable]
public class ChapterPack
{
    public DefiniteRewards pack;
    public int time;
    public float price;
}
