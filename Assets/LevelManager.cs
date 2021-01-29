using System.Collections;
using System.Collections.Generic;
using BizzyBeeGames.DotConnect;
using UnityEngine;

namespace BizzyBeeGames
{
    public class LevelManager : SingletonComponent<LevelManager>
    {

        public List<BundleInfo> bundleInfos = null;

        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(this);
        }
    }
}
