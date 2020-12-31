using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using System;
using BizzyBeeGames.DotConnect;
using static BizzyBeeGames.DotConnect.GameGrid;
using static UserData;

namespace BizzyBeeGames
{
    public class UserDataManager : SingletonComponent<UserDataManager>
    {
        string save_path;
        static BinaryFormatter formatter;
        static UserData user_data;

        [HideInInspector]
        public bool session_started = false;
        [HideInInspector]
        public bool dark_mode_popup_shown = false;
        [HideInInspector]
        public bool show_rate_us_popup = false;

        protected override void Awake()
        {
            save_path = Application.persistentDataPath + "/bsc_puzzle.astro";
            formatter = new BinaryFormatter();
            user_data = Load();

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
        public UserData GetUserData()
        {
            if (user_data == null) { user_data = Load(); }
            return user_data;
        }
        public void SetUserData(UserData ud)
        {
            user_data = ud;
            Save();
        }
        public void SetCurrentChapterPack(ChapterPack pack)
        {
            user_data.CurrentChapterPack = pack;
            Save();
        }
        public ChapterPack GetCurrentChapterPack()
        {
            return user_data.CurrentChapterPack;
        }
        public void SetUserGift(string id = "", GiftToReward ug = null)
        {
            switch (id)
            {
                case InventoryManager.Key_InventoryStars:
                    user_data.UserGiftProfileStar = ug;
                    break;
                case InventoryManager.Key_InventoryTrophies:
                    user_data.UserGiftProfileTrophy = ug;
                    break;
                case InventoryManager.Key_InventoryLollipops:
                    user_data.UserGiftProfileLollipop = ug;
                    break;
                case "QuestScreen":
                    user_data.UserQuestReward = ug;
                    break;
                default:
                    user_data.UserGiftReward = ug;
                    break;
            }
            Save();
        }

        public bool IsDarkModeOn()
        {
            return GetData("dark_mode_enabled") == 1;
        }
        public GiftToReward GetUserGift(string id = "")
        {
            GiftToReward ug;
            switch (id)
            {
                case InventoryManager.Key_InventoryStars:
                    ug = user_data.UserGiftProfileStar;
                    break;
                case InventoryManager.Key_InventoryTrophies:
                    ug = user_data.UserGiftProfileTrophy;
                    break;
                case InventoryManager.Key_InventoryLollipops:
                    ug = user_data.UserGiftProfileLollipop;
                    break;
                case "QuestScreen":
                    ug = user_data.UserQuestReward;
                    break;
                default:
                    ug = user_data.UserGiftReward;
                    break;
            }
            return ug;
        }
        public void SetUserGiftReward(List<QuestReward> rewards, GiftType type, string id, String title = "", bool unclaimed = false)
        {
            GiftToReward UserGiftReward = new GiftToReward();
            UserGiftReward.rewardsToGive = rewards;
            UserGiftReward.rewardType = type;
            UserGiftReward.id = id;
            UserGiftReward.isUnclaimed = unclaimed;
            UserGiftReward.title = title;
            user_data.UserGiftReward = UserGiftReward;
            Save();
        }

        public void AddAchivements(AcheivementData acheivementData)
        {
            Debug.LogError("Add Achievemt: " + acheivementData.acheivement_type);
            if (user_data.acheivementsList == null)
                user_data.acheivementsList = new List<AcheivementData>();
            user_data.acheivementsList.Add(acheivementData);
            Save();
        }

        public void AddAchivements(List<AcheivementData> acheivementDataList)
        {
            foreach (AcheivementData acheivementData in acheivementDataList)
            {
                AddAchivements(acheivementData);
            }
        }
        public void RemoveAchivement(int pos)
        {
            Debug.LogError("Remove Achievemt");
            if (user_data.acheivementsList != null && user_data.acheivementsList.Count > 0)
            {
                Debug.LogError("Remove Achievemt: " + user_data.acheivementsList[0].acheivement_type);
                user_data.acheivementsList.RemoveAt(0);
                Save();
            }
        }

        public List<AcheivementData> GetAchivements()
        {
            Debug.Log("Get All Achievement");
            if (user_data.acheivementsList != null)
                return user_data.acheivementsList;
            else
                return new List<AcheivementData>();
        }

        public void SetUserProfileStarGift(List<QuestReward> rewards, GiftType type)
        {
            GiftToReward UserGiftProfileStar = new GiftToReward();
            UserGiftProfileStar.rewardsToGive = rewards;
            UserGiftProfileStar.rewardType = type;
            UserGiftProfileStar.id = InventoryManager.Key_InventoryStars;
            user_data.UserGiftProfileStar = UserGiftProfileStar;
            Save();
        }
        public void SetUserProfileTrophyGift(List<QuestReward> rewards, GiftType type)
        {
            GiftToReward UserGiftProfileTrophy = new GiftToReward();
            UserGiftProfileTrophy.rewardsToGive = rewards;
            UserGiftProfileTrophy.rewardType = type;
            UserGiftProfileTrophy.id = InventoryManager.Key_InventoryTrophies;
            user_data.UserGiftProfileTrophy = UserGiftProfileTrophy;
            Save();
        }
        public void SetUserProfileLollipopGift(List<QuestReward> rewards, GiftType type)
        {
            GiftToReward UserGiftProfileLollipop = new GiftToReward();
            UserGiftProfileLollipop.rewardsToGive = rewards;
            UserGiftProfileLollipop.rewardType = type;
            UserGiftProfileLollipop.id = InventoryManager.Key_InventoryLollipops;
            user_data.UserGiftProfileLollipop = UserGiftProfileLollipop;
            Save();
        }
        public void UpdateData(string key, int value)
        {
            int prev_value = GetData(key);

            SetData(key, prev_value + value);
        }
        public void SetData(string key, int value)
        {
            Dictionary<string, int> dictionary = user_data.UserDataHistory_Int;

            if (dictionary.ContainsKey(key))
            {
                dictionary.Remove(key);
            }
            dictionary.Add(key, value);
            user_data.UserDataHistory_Int = dictionary;
            GameEventManager.Instance.SendEvent(GameEventManager.EventId_UserDataUpdated);
            Save();
        }
        public void SetData(string key, DateTime value)
        {
            Dictionary<string, DateTime> dictionary = user_data.UserDataHistory_DateTime;

            if (dictionary.ContainsKey(key))
            {
                dictionary.Remove(key);
            }
            dictionary.Add(key, value);
            user_data.UserDataHistory_DateTime = dictionary;
            Save();
        }
        public int GetData(string key)
        {
            Dictionary<string, int> dictionary = GetUserData().UserDataHistory_Int;
            if (dictionary.ContainsKey(key))
            {
                return dictionary[key];
            }

            return GetDefaults(key);
        }
        private int GetDefaults(string key)
        {
            int value = 0;
            if (key == "current_level") { value = 1; }
            SetData(key, value);
            return value;
        }
        public DateTime GetDateTime(string key)
        {
            Dictionary<string, DateTime> dictionary = GetUserData().UserDataHistory_DateTime;
            if (dictionary.ContainsKey(key))
            {
                return dictionary[key];
            }
            return new DateTime(1970, 1, 1, 0, 0, 0);
        }
        public bool SavePuzzleData(PuzzleData data)
        {
            if (!user_data.PuzzleDataHistory.ContainsKey(data.level_index.ToString()))
            {
                user_data.PuzzleDataHistory.Add(data.level_index.ToString(), data);
                Save();
                return true;
            }
            return false;
        }
        public PuzzleData LoadPuzzleData(int level_index)
        {
            int level = level_index - 1;
            Dictionary<string, PuzzleData> puzzle_data = user_data.PuzzleDataHistory;
            if (puzzle_data.ContainsKey(level.ToString()))
            {
                return puzzle_data[level.ToString()];
            }
            return null;
        }
        public UserQuest LoadUserQuest()
        {
            if (user_data.UserQuest != null)
            {
                return user_data.UserQuest;
            }
            return null;
        }
        public void SaveUserQuest(UserQuest user_quest)
        {
            user_data.UserQuest = user_quest;
            Save();
            GameEventManager.Instance.SendEvent(GameEventManager.EventId_QuestUserUpdated);
        }
        public void SaveUserQuestTime()
        {
            DateTime time = DateTime.Now.AddHours(GameConfiguration.Instance.QuestAllottedTime);
            SetData("user_quest_time", time);
        }
        public DateTime GetUserQuestTime()
        {
            return GetDateTime("user_quest_time");
        }
        private void Save()
        {
            FileStream stream = File.Create(save_path);
            formatter.Serialize(stream, user_data);
            stream.Close();
        }
        public UserData Load()
        {
            if (!File.Exists(save_path))
            {
                return new UserData();
            }
            FileStream stream = File.Open(save_path, FileMode.Open);
            try
            {
                UserData data = (UserData)formatter.Deserialize(stream);
                stream.Close();
                return data;
            }
            catch (Exception e)
            {
                Debug.LogErrorFormat("Could not load puzzle data at {0}", save_path);
                Debug.LogException(e);
                stream.Close();
                return new UserData();
            }
        }
        public void DeleteUserData()
        {
            try
            {
                File.Delete(save_path);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }
    }
}
