using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BizzyBeeGames.DotConnect
{
    #region Enums

    public enum PackUnlockType
    {
        None,
        Stars,
        IAP
    }

    #endregion

    [System.Serializable]
    public class PackInfo
    {
        #region Inspector Variables

        public string packId;
        public string packName;
        public string packDescription;
        public PackUnlockType unlockType;
        public int unlockStarsAmount;
        public string unlockIAPProductId;
        public List<LevelFile> levelFiles;

        #endregion

        #region Member Variables

        private List<LevelData> levelDatas;
        private Dictionary<int, int> challenge_iterator = new Dictionary<int, int>();

        #endregion

        #region Properties

        public bool HasLevelDats { get { return levelDatas != null; } }
        public List<LevelData> LevelDatas
        {
            get
            {
                if (levelDatas == null)
                {
                    CreateLevelDatas();
                }

                return levelDatas;
            }
        }

        public List<LevelData> GetChallengeLevelDatas(int level_idx)
        {
            return CreateChallengeLevelDatas(level_idx);
        }
        public int GetChallengeLevelIterator(int level_idx)
        {
            int iterator = 0;
            if (challenge_iterator.ContainsKey(level_idx))
            {
                iterator = challenge_iterator[level_idx] + 1;
                iterator %= levelFiles[level_idx].Iterations.Count;
            }
            challenge_iterator[level_idx] = iterator;
            return iterator;
        }

        #endregion

        #region Private Methods

        private void CreateLevelDatas()
        {
            levelDatas = new List<LevelData>();

            for (int i = 0; i < levelFiles.Count; i++)
            {
                levelDatas.Add(new LevelData(levelFiles[i].File, packId, i));
            }
        }
        private List<LevelData> CreateChallengeLevelDatas(int level_idx)
        {
            LevelFile level_file = levelFiles[level_idx];
            List<LevelData> challenge_level_datas = new List<LevelData>();

            for (int i = 0; i < level_file.Iterations.Count; i++)
            {
                challenge_level_datas.Add(new LevelData(level_file.Iterations[i], packId, i));
            }
            return challenge_level_datas;
        }

        #endregion
    }
}
