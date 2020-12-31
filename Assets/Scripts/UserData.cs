using System;
using System.Collections.Generic;
using static BizzyBeeGames.DotConnect.GameGrid;

[Serializable]
public class UserData
{
    public Dictionary<string, PuzzleData> PuzzleDataHistory = new Dictionary<string, PuzzleData>();
    public UserQuest UserQuest;
    public Dictionary<string, int> UserDataHistory_Int = new Dictionary<string, int>();
    public Dictionary<string, DateTime> UserDataHistory_DateTime = new Dictionary<string, DateTime>();
    public GiftToReward UserGiftReward = null;
    public GiftToReward UserGiftProfileStar = null;
    public GiftToReward UserGiftProfileTrophy = null;
    public GiftToReward UserGiftProfileLollipop = null;
    public GiftToReward UserQuestReward = null;
    public ChapterPack CurrentChapterPack = null;
    public List<AcheivementData> acheivementsList = new List<AcheivementData>();


    [Serializable]
    public class Gift
    {
        public List<QuestReward> gifts;
        public GiftType giftType;
    }
}
