using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BizzyBeeGames.DotConnect
{
    public class GameManager : SingletonComponent<GameManager>, ISaveable
    {
        #region Inspector Variables

        [Header("Data")]
        [SerializeField] private List<BundleInfo> bundleInfos = null;
        [SerializeField] private int startingHints = 5;
        [SerializeField] private int numLevelsForGift = 25;
        [SerializeField] private Text levelTimer;
        [SerializeField] private LevelTimer timerObj;
        public GameObject options_screen;
        public bool challenge_failed = false;
        public bool challenge_started = false;
        public bool challenge_finished = false;
        [Header("Ads")]
        [SerializeField] private int numLevelsBetweenAds = 0;

        [Header("UI Components")]
        [SerializeField] private GameGrid gameGrid = null;
        [SerializeField] private Text hintAmountText = null;

        [Header("Debug")]
        [SerializeField] private bool unlockAllPacks = false;   // Sets all packs to be unlocked
        [SerializeField] private bool unlockAllLevels = false;  // Sets all levels to be unlocked (does not unlock packs)
        [SerializeField] private bool freeHints = false;    // You can used hints regardless of the amount of hints you have
        [SerializeField] private int startingStars = 0;     // Sets the amount of stars you have when the game runs, overrides saved value

        [Header("Animation Components")]
        [SerializeField] private List<AnimationItem> AnimationList = new List<AnimationItem>();
        [SerializeField] private List<AnimatedText> AnimatedTextList = new List<AnimatedText>();

        [SerializeField] LevelTimer LevelTimerObject = null;
        #endregion

        #region Classes
        [System.Serializable]
        class AnimationItem
        {
            public string id = "";
            public GameObject obj = null;
        }
        #endregion

        #region Member Variables

        private Dictionary<string, int> packNumStarsEarned;
        private Dictionary<string, int> packLastCompletedLevel;
        private Dictionary<string, Dictionary<int, int>> packLevelStatuses;
        private Dictionary<string, LevelSaveData> levelSaveDatas;
        bool loaded = false;
        private float timeAllotted = 0;
        private bool pauseGame = false;
        private bool focusOntimeInitiated = false;
        public bool isPlaying = false;
        private float timeAllotedFocusMin = 15f;
        #endregion

        #region Properties
        public float TimeAllotted { get { return timeAllotted; } set { timeAllotted = value; } }
        public int levelFailedCount { get; private set; }
        public GameGrid Grid { get { return gameGrid; } }
        public List<BundleInfo> BundleInfos { get { return bundleInfos; } }
        public PackInfo ActivePackInfo { get; private set; }
        public LevelData ActiveLevelData { get; private set; }
        public int StarAmount { get; private set; }
        public int HintAmount { get; private set; }
        public int NumLevelsTillAd { get; private set; }

        public string SaveId { get { return "game"; } }

        #endregion

        #region Unity Methods

        protected override void Awake()
        {
            base.Awake();

            GameEventManager.Instance.RegisterEventHandler(GameEventManager.EventId_ActiveLevelCompleted, OnActiveLevelComplete);
            GameEventManager.Instance.RegisterEventHandler(GameEventManager.EventId_ActiveLevelFailed, OnActiveLevelFailed);

            SaveManager.Instance.Register(this);
            loaded = false;
            packNumStarsEarned = new Dictionary<string, int>();
            packLastCompletedLevel = new Dictionary<string, int>();
            packLevelStatuses = new Dictionary<string, Dictionary<int, int>>();
            levelSaveDatas = new Dictionary<string, LevelSaveData>();

            if (!LoadSave())
            {
                HintAmount = startingHints;
                NumLevelsTillAd = numLevelsBetweenAds;
            }

            gameGrid.Initialize();

            if (startingStars > 0)
            {
                StarAmount = startingStars;
            }
        }

        private void OnDestroy()
        {
            SoundManager.Instance.Stop("LevelTimerLowOnTime");
            Save();
            if (GameEventManager.Instance == null) { return; }
            GameEventManager.Instance.UnRegisterEventHandler(GameEventManager.EventId_ActiveLevelCompleted, OnActiveLevelComplete);
            GameEventManager.Instance.UnRegisterEventHandler(GameEventManager.EventId_ActiveLevelFailed, OnActiveLevelFailed);
        }

        private void Update()
        {
            if (!loaded)
            {
                loaded = true;
                int bundle = 0, pack = 0;

                BundleInfo bundleInfo = GameManager.Instance.BundleInfos[bundle];
                PackInfo packInfo = bundleInfo.PackInfos[pack];

                StartLevel(packInfo);
            }
            if (timeAllotted != -1)
            {
                if (timeAllotted <= 0)
                {
                    SoundManager.Instance.Stop("LevelTimerLowOnTime");
                    SoundManager.Instance.Play("TimerZero");
                    timeAllotted = -1;
                    GameEventManager.Instance.SendEvent(GameEventManager.EventId_ActiveLevelFailed, true, false);
                }
                else if (timeAllotted > 0 && timeAllotted < timeAllotedFocusMin && !focusOntimeInitiated)
                {
                    FocusOnLevelTimer();
                }
                else if (timeAllotted > 0 && !pauseGame)
                {
                    timeAllotted -= Time.deltaTime;
                    levelTimer.text = Utils.ConvertSecondsToString((int)timeAllotted, false);
                }
            }
        }


        /// <summary>
        /// Gives a heart beat animation when low on time
        /// </summary>
        private void FocusOnLevelTimer()
        {
            Debug.Log("Focus");
            SoundManager.Instance.Play("LevelTimerLowOnTime", true);
            focusOntimeInitiated = true;
            LevelTimerObject.FocusOnTimer(15f, true);
        }

        private void OnApplicationPause(bool pause)
        {
            pauseGame = pause;
            if (pause)
            {
                Save();
            }
        }

        #endregion

        #region Public Variables
        public void SetPause(bool enable)
        {
            pauseGame = enable;

            Debug.Log("Pause: " + pauseGame);
            Debug.Log("isPlaying: " + isPlaying);

            if (focusOntimeInitiated)
            {
                if (pauseGame)
                {
                    SoundManager.Instance.Stop("LevelTimerLowOnTime");
                }
                else
                {
                    SoundManager.Instance.Play("LevelTimerLowOnTime", true);
                }
            }
            if (isPlaying)
            {
                if (pauseGame)
                {
                    SoundManager.Instance.Stop("LevelTimerFocus");
                }
                else
                {
                    SoundManager.Instance.Play("LevelTimerFocus", true);
                }
            }
        }
        /// <summary>
        /// Adds extra time to level.
        /// </summary>
        public void AddTimeToLevel(int timeAdded)
        {
            timeAllotted += timeAdded;
            if (timeAllotted > timeAllotedFocusMin) { focusOntimeInitiated = false; }
        }
        /// <summary>
        /// Starts the level.
        /// </summary>
        public void StartLevel(PackInfo packInfo)
        {
            // Check if the level has not been started and if there is load save data for it
            int bundle = 0, pack = 0, level_index = UserDataManager.Instance.GetData("current_level") - 1;

            BundleInfo bundleInfo = GameManager.Instance.BundleInfos[bundle];
            packInfo = bundleInfo.PackInfos[pack];
            levelFailedCount = 0;

            LevelData levelData = packInfo.LevelDatas[level_index];
            ActivePackInfo = packInfo;
            ActiveLevelData = levelData;
            if (!levelSaveDatas.ContainsKey(levelData.Id))
            {
                levelSaveDatas[levelData.Id] = new LevelSaveData();
            }

            // if the level is challenge level take the next iteration
            if (ActiveLevelData.is_challenge_library)
            {
                List<LevelData> challenge_level_datas = ActivePackInfo.GetChallengeLevelDatas(level_index);
                int iterator = ActivePackInfo.GetChallengeLevelIterator(level_index);
                ActiveLevelData = challenge_level_datas[iterator];
            }
            gameGrid.SetupLevel(ActiveLevelData, levelSaveDatas[levelData.Id]);
            SetPause(false);
            PlayGameScreenBGM();

            focusOntimeInitiated = false;
            timeAllotted = ActiveLevelData.time == 0 ? -1 : ActiveLevelData.time * 60;

            UpdateHintAmountText();
            GameEventManager.Instance.SendEvent(GameEventManager.EventId_LevelStarted);

            // Check if it's time to show an interstitial ad
            if (NumLevelsTillAd <= 0)
            {
                NumLevelsTillAd = numLevelsBetweenAds;
            }

            // if the level has hardness defined in the level creator editor show the
            // LevelIntroductionPopup. Else start the game play animation.
            if (ActiveLevelData.level_hardness != 0) { PopupManager.Instance.Show("LevelIntroduction"); }
            else { StartGamePlayAnimation(); }
        }

        public void StartGamePlayAnimation()
        {
            ExecuteAllGamePlayAnimation();
            SoundManager.Instance.Play("GridAppears");
            ScreenManager.Instance.Show("game", () => { }, true);
            gameGrid.ResetNumMoves();
        }
        public void ExecuteAllGamePlayAnimation()
        {
            for (int i = 0; i < AnimationList.Count; i++)
            {
                // Dont animate level timer if its not a time level
                if (AnimationList[i].id == "LevelTimer" && timeAllotted == -1) { continue; }

                GameObject obj = AnimationList[i].obj;
                if (obj != null)
                {
                    Animator anim = obj.GetComponent<Animator>();
                    if (anim != null) { anim.SetBool("appear", true); }
                }
            }
        }

        public void AnimateObjectInList(string id, bool status)
        {
            for (int i = 0; i < AnimationList.Count; i++)
            {
                GameObject obj = AnimationList[i].obj;
                if (obj != null && AnimationList[i].id == id)
                {
                    Animator anim = obj.GetComponent<Animator>();
                    if (anim != null) { anim.SetBool("appear", status); }
                    break;
                }
            }
        }

        /// <summary>
        /// Reset all objects to its initial position
        /// </summary>
        public void ResetAllGamePlayAnimation()
        {
            for (int i = 0; i < AnimationList.Count; i++)
            {
                GameObject obj = AnimationList[i].obj;
                if (obj != null)
                {
                    Animator anim = obj.GetComponent<Animator>();
                    if (anim != null) { anim.SetBool("appear", false); }
                }
            }
            for (int j = 0; j < AnimatedTextList.Count; j++)
            {
                AnimatedTextList[j].setContainerAnim(false);
            }
            gameGrid.ResetProgressMeter();
            ScreenManager.Instance.Hide("game", false, false);
        }

        /// <summary>
        /// Plays the next level based on the current active PackInfo and LevelData
        /// </summary>
        ///
        public void NextLevel()
        {
            int nextLevelIndex = ActiveLevelData.LevelIndex + 1;
            int bundle = 0, pack = 0;

            BundleInfo bundleInfo = GameManager.Instance.BundleInfos[bundle];
            ActivePackInfo = bundleInfo.PackInfos[pack];

            if (nextLevelIndex < ActivePackInfo.LevelDatas.Count)
            {
                StartLevel(ActivePackInfo);
            }
        }

        /// <summary>
        /// Plays the last level based on the current active level
        /// </summary>
        public void LastLevel()
        {
            int lastLevelIndex = ActiveLevelData.LevelIndex - 1;

            if (lastLevelIndex >= 0)
            {
                StartLevel(ActivePackInfo);
            }
        }

        /// <summary>
        /// Returns true if the level has been completed atleast once
        /// </summary>
        public bool IsLevelCompleted(LevelData levelData)
        {
            if (!packLevelStatuses.ContainsKey(levelData.PackId))
            {
                return false;
            }

            Dictionary<int, int> levelStatuses = packLevelStatuses[levelData.PackId];

            if (!levelStatuses.ContainsKey(levelData.LevelIndex))
            {
                return false;
            }

            // If it has an entry in levelStatuses then it must have been completed 
            return true;
        }

        /// <summary>
        /// Returns true if a star has been enarned for the given level
        /// </summary>
        public bool HasEarnedStar(LevelData levelData)
        {
            return IsLevelCompleted(levelData) && packLevelStatuses[levelData.PackId][levelData.LevelIndex] == 1;
        }

        /// <summary>
        /// Returns true if the level is locked, false if it can be played
        /// </summary>
        public bool IsLevelLocked(LevelData levelData)
        {
            if (unlockAllLevels) return false;
            return levelData.LevelIndex > 0 && (!packLastCompletedLevel.ContainsKey(levelData.PackId) || levelData.LevelIndex > packLastCompletedLevel[levelData.PackId] + 1);
        }

        /// <summary>
        /// Returns true if the pack is locked
        /// </summary>
        public bool IsPackLocked(PackInfo packInfo)
        {
            if (unlockAllPacks) return false;
            switch (packInfo.unlockType)
            {
                case PackUnlockType.Stars:
                    return StarAmount < packInfo.unlockStarsAmount;
            }
            return false;
        }

        /// <summary>
        /// Gets the pack progress percentage
        /// </summary>
        public int GetNumCompletedLevels(PackInfo packInfo)
        {
            if (!packLastCompletedLevel.ContainsKey(packInfo.packId))
            {
                return 0;
            }

            return packLastCompletedLevel[packInfo.packId] + 1;
        }

        /// <summary>
        /// Gets the pack progress percentage
        /// </summary>
        public float GetPackProgress(PackInfo packInfo)
        {
            return (float)(GetNumCompletedLevels(packInfo)) / (float)packInfo.levelFiles.Count;
        }

        #endregion

        #region Private Variables

        /// <summary>
        /// Invoked by GameGrid when the active level has all the lines placed on the grid
        /// </summary>
        private void OnActiveLevelComplete(string eventId, object[] data)
        {
            // Get the number of moves it took to complete the level
            int numMoves = (int)data[0];
            PuzzleData puzzle_data = (PuzzleData)data[1];

            UserDataManager.Instance.SavePuzzleData(puzzle_data);

            // Check if the user gets a star for completeing the level in the minimum number of moves
            bool earnedStar = (numMoves <= ActiveLevelData.LinePositions.Count);
            bool alreadyEarnedStar = HasEarnedStar(ActiveLevelData);

            // Check if they just earned a new star
            if (earnedStar && !alreadyEarnedStar)
            {
                IncreaseStarAmount(1);
            }

            // Get gift progress information
            int lastLevelCompleted = (packLastCompletedLevel.ContainsKey(ActiveLevelData.PackId) ? packLastCompletedLevel[ActiveLevelData.PackId] : -1);
            bool giftProgressed = (ActiveLevelData.LevelIndex > lastLevelCompleted);
            int fromGiftProgress = (lastLevelCompleted + 1);
            int toGiftProgress = (ActiveLevelData.LevelIndex + 1);
            bool giftAwarded = (giftProgressed && toGiftProgress % numLevelsForGift == 0);

            // Give one hint if a gift should be awarded
            if (giftAwarded)
            {
                HintAmount += 1;
            }

            // Set the active level as completed
            SetLevelComplete(ActiveLevelData, earnedStar ? 1 : 0);

            // Remove the save data since it's only for levels which have been started but not completed
            levelSaveDatas.Remove(ActiveLevelData.Id);

            bool isLastLevel = (ActiveLevelData.LevelIndex == ActivePackInfo.LevelDatas.Count - 1);

            // Create the data object array to pass to the level complete popup
            object[] popupData =
            {
                isLastLevel,
                numMoves,
                ActiveLevelData.LinePositions.Count, // Number of moves to earn a star
				earnedStar,
                alreadyEarnedStar,
                giftProgressed,
                giftAwarded,
                fromGiftProgress,
                toGiftProgress,
                numLevelsForGift
            };

            // Show the level completed popup
            PopupManager.Instance.Show("LevelCompleteMessage", popupData);
            SetPause(true);

            NumLevelsTillAd--;
        }
        private void OnLevelFailPopupClosed(bool cancelled, object[] data)
        {
            string action = data[0] as string;

            if (action == "close")
            {
                ResetAllGamePlayAnimation();
                InventoryManager.Instance.SetInventory(InventoryManager.Key_InventoryMoves, Grid.currentLevelSaveData.numMoves);
                PopupManager.Instance.Show("ChallengeRetryScreen");
            }
        }
        private void OnActiveLevelFailed(string eventId, object[] data)
        {
            levelFailedCount += 1;
            PopupManager.Instance.Show("LevelFailPopup", data, OnLevelFailPopupClosed);
        }

        /// <summary>
        /// Sets the level status
        /// </summary>
        private void SetLevelComplete(LevelData levelData, int status)
        {
            // Set the last completed level in the pack
            int curLastCompletedLevel = packLastCompletedLevel.ContainsKey(levelData.PackId) ? packLastCompletedLevel[levelData.PackId] : -1;

            if (levelData.LevelIndex > curLastCompletedLevel)
            {
                packLastCompletedLevel[levelData.PackId] = levelData.LevelIndex;
            }

            // Set the level status
            if (!packLevelStatuses.ContainsKey(levelData.PackId))
            {
                packLevelStatuses.Add(levelData.PackId, new Dictionary<int, int>());
            }

            Dictionary<int, int> levelStatuses = packLevelStatuses[levelData.PackId];

            int curStatus = levelStatuses.ContainsKey(levelData.LevelIndex) ? levelStatuses[levelData.LevelIndex] : -1;

            if (status > curStatus)
            {
                levelStatuses[levelData.LevelIndex] = status;
            }
        }

        /// <summary>
        /// Increases the amount of stars the player has
        /// </summary>
        private void IncreaseStarAmount(int amt)
        {
            StarAmount += amt;
            GameEventManager.Instance.SendEvent(GameEventManager.EventId_StarsIncreased);
        }

        private void UpdateHintAmountText()
        {
            hintAmountText.text = InventoryManager.Instance.GetInventory(InventoryManager.Key_InventoryHints).ToString();
        }

        public Dictionary<string, object> Save()
        {
            Dictionary<string, object> json = new Dictionary<string, object>();

            json["num_stars_earned"] = SaveNumStarsEarned();
            json["last_completed"] = SaveLastCompleteLevels();
            json["level_statuses"] = SaveLevelStatuses();
            json["level_save_datas"] = SaveLevelDatas();
            json["star_amount"] = StarAmount;
            json["hint_amount"] = HintAmount;
            json["num_levels_till_ad"] = NumLevelsTillAd;

            return json;
        }

        private List<object> SaveNumStarsEarned()
        {
            List<object> json = new List<object>();

            foreach (KeyValuePair<string, int> pair in packNumStarsEarned)
            {
                Dictionary<string, object> packJson = new Dictionary<string, object>();

                packJson["pack_id"] = pair.Key;
                packJson["num_stars_earned"] = pair.Value;

                json.Add(packJson);
            }

            return json;
        }

        private List<object> SaveLastCompleteLevels()
        {
            List<object> json = new List<object>();

            foreach (KeyValuePair<string, int> pair in packLastCompletedLevel)
            {
                Dictionary<string, object> packJson = new Dictionary<string, object>();

                packJson["pack_id"] = pair.Key;
                packJson["last_completed_level"] = pair.Value;

                json.Add(packJson);
            }

            return json;
        }

        public void PlayGameScreenBGM()
        {
            if (ActiveLevelData.is_challenge_library) { SoundManager.Instance.PlayScreenBGM("ChallengeLevelBGM"); }
            else { SoundManager.Instance.PlayScreenBGM("GameScreenBGM"); }
        }

        private List<object> SaveLevelStatuses()
        {
            List<object> json = new List<object>();

            foreach (KeyValuePair<string, Dictionary<int, int>> pair in packLevelStatuses)
            {
                Dictionary<string, object> packJson = new Dictionary<string, object>();

                packJson["pack_id"] = pair.Key;

                string levelStr = "";

                foreach (KeyValuePair<int, int> levelPair in pair.Value)
                {
                    if (!string.IsNullOrEmpty(levelStr)) levelStr += "_";
                    levelStr += levelPair.Key + "_" + levelPair.Value;
                }

                packJson["level_statuses"] = levelStr;

                json.Add(packJson);
            }

            return json;
        }

        private List<object> SaveLevelDatas()
        {
            List<object> savedLevelDatas = new List<object>();

            foreach (KeyValuePair<string, LevelSaveData> pair in levelSaveDatas)
            {
                Dictionary<string, object> levelSaveDataJson = pair.Value.Save();

                levelSaveDataJson["id"] = pair.Key;

                savedLevelDatas.Add(levelSaveDataJson);
            }

            return savedLevelDatas;
        }

        private bool LoadSave()
        {
            JSONNode json = SaveManager.Instance.LoadSave(this);

            if (json == null)
            {
                return false;
            }

            LoadNumStarsEarned(json["num_stars_earned"].AsArray);
            LoadLastCompleteLevels(json["last_completed"].AsArray);
            LoadLevelStatuses(json["level_statuses"].AsArray);
            LoadLevelSaveDatas(json["level_save_datas"].AsArray);

            StarAmount = json["star_amount"].AsInt;
            HintAmount = json["hint_amount"].AsInt;
            NumLevelsTillAd = json["num_levels_till_ad"].AsInt;

            return true;
        }

        private void LoadNumStarsEarned(JSONArray json)
        {
            for (int i = 0; i < json.Count; i++)
            {
                JSONNode childJson = json[i];

                string packId = childJson["pack_id"].Value;
                int numStarsEarned = childJson["num_stars_earned"].AsInt;

                packNumStarsEarned.Add(packId, numStarsEarned);
            }
        }

        private void LoadLastCompleteLevels(JSONArray json)
        {
            for (int i = 0; i < json.Count; i++)
            {
                JSONNode childJson = json[i];

                string packId = childJson["pack_id"].Value;
                int lastCompletedLevel = childJson["last_completed_level"].AsInt;

                packLastCompletedLevel.Add(packId, lastCompletedLevel);
            }
        }

        private void LoadLevelStatuses(JSONArray json)
        {
            for (int i = 0; i < json.Count; i++)
            {
                JSONNode childJson = json[i];

                string packId = childJson["pack_id"].Value;
                string[] levelStatusStrs = childJson["level_statuses"].Value.Split('_');

                Dictionary<int, int> levelStatuses = new Dictionary<int, int>();

                for (int j = 0; j < levelStatusStrs.Length; j += 2)
                {
                    int levelIndex = System.Convert.ToInt32(levelStatusStrs[j]);
                    int status = System.Convert.ToInt32(levelStatusStrs[j + 1]);

                    levelStatuses.Add(levelIndex, status);
                }

                packLevelStatuses.Add(packId, levelStatuses);
            }
        }

        /// <summary>
        /// Loads the game from the saved json file
        /// </summary>
        private void LoadLevelSaveDatas(JSONArray savedLevelDatasJson)
        {
            // Load all the placed line segments for levels that have progress
            for (int i = 0; i < savedLevelDatasJson.Count; i++)
            {
                JSONNode savedLevelDataJson = savedLevelDatasJson[i];
                JSONArray savedPlacedLineSegments = savedLevelDataJson["placed_line_segments"].AsArray;
                JSONArray savedHints = savedLevelDataJson["hints"].AsArray;

                List<List<CellPos>> placedLineSegments = new List<List<CellPos>>();

                for (int j = 0; j < savedPlacedLineSegments.Count; j++)
                {
                    placedLineSegments.Add(new List<CellPos>());

                    for (int k = 0; k < savedPlacedLineSegments[j].Count; k += 2)
                    {
                        placedLineSegments[j].Add(new CellPos(savedPlacedLineSegments[j][k].AsInt, savedPlacedLineSegments[j][k + 1].AsInt));
                    }
                }

                List<int> hintLineIndices = new List<int>();

                for (int j = 0; j < savedHints.Count; j++)
                {
                    hintLineIndices.Add(savedHints[j].AsInt);
                }

                string levelId = savedLevelDataJson["id"].Value;
                int numMoves = savedLevelDataJson["num_moves"].AsInt;

                LevelSaveData levelSaveData = new LevelSaveData();

                levelSaveData.placedLineSegments = placedLineSegments;
                levelSaveData.numMoves = numMoves;
                levelSaveData.hintLineIndices = hintLineIndices;

                if (!levelSaveDatas.ContainsKey(levelId))
                {
                    levelSaveDatas.Add(levelId, levelSaveData);
                }
            }
        }

        #endregion
    }
}
