using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BizzyBeeGames.DotConnect
{
	public class GameEventManager : EventManager<GameEventManager>
	{
        #region Member Variables
        // Event Ids
        public const string EventId_BundleSelected			     = "BundleSelected";
		public const string EventId_PackSelected			     = "PackSelected";
		public const string EventId_ActiveLevelCompleted	     = "ActiveLevelCompleted";
		public const string EventId_LevelStarted			     = "LevelStarted";
		public const string EventId_StarsIncreased			     = "StarsIncreased";
		public const string EventId_InventoryUpdated             = "InventoryUpdated";
		public const string EventId_QuestUserUpdated             = "QuestUserUpdated";
		public const string EventId_DailyRewardSelected          = "DailyRewardSelected";
		public const string EventId_MovesRechargeTimeUpdated     = "MovesRechargeTimeUpdated";
		public const string EventId_UserDataUpdated              = "UserDataUpdated";
		public const string EventId_ActiveLevelFailed            = "ActiveLevelFailed";
		public const string EventId_CloseContainerAnimComplete   = "CloseContainerAnimComplete";
		public const string EventId_ContentContainerAnimComplete = "ContentContainerAnimComplete";
		public const string EventId_TitleContainerAnimComplete   = "TitleContainerAnimComplete";

		// Event Id data types
		private static readonly Dictionary<string, List<System.Type>> eventDataTypes = new Dictionary<string, List<System.Type>>()
		{
			{ EventId_BundleSelected, new List<System.Type>() { typeof(BundleInfo) } },
			{ EventId_PackSelected, new List<System.Type>() { typeof(PackInfo) } },
			{ EventId_ActiveLevelCompleted, new List<System.Type>() { typeof(int), typeof(PuzzleData) } },
			{ EventId_LevelStarted, new List<System.Type>() {} },
			{ EventId_StarsIncreased, new List<System.Type>() {} },
			{ EventId_InventoryUpdated, new List<System.Type>() { typeof(string), typeof(int) } },
			{ EventId_QuestUserUpdated, new List<System.Type>() {} },
			{ EventId_DailyRewardSelected, new List<System.Type>() { typeof(int) } },
			{ EventId_MovesRechargeTimeUpdated, new List<System.Type>() { typeof(int) } },
			{ EventId_UserDataUpdated, new List<System.Type>() {} },
			{ EventId_ActiveLevelFailed, new List<System.Type>() { typeof(bool), typeof(bool) } },
			{ EventId_CloseContainerAnimComplete, new List<System.Type>() {} },
			{ EventId_ContentContainerAnimComplete, new List<System.Type>() {} },
			{ EventId_TitleContainerAnimComplete, new List<System.Type>() {} },

		};

		#endregion

		#region Protected Methods
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
		protected override Dictionary<string, List<Type>> GetEventDataTypes()
		{
			return eventDataTypes;
		}

		#endregion
	}
}
