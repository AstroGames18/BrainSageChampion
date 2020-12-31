using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

public class QuestEditor : EditorWindow
{
    private int version = 0;
    private float time = 0f;
    private UnityEngine.Object outputFolder = null;
    private UnityEngine.Object jsonObject = null;

    private Vector2 scrollPosition;

    private List<QuestTier> tier_list = new List<QuestTier>();
    private List<bool> tier_collapse = new List<bool>();
    private float total_reward_probability = 0;
    private float total_type_probability = 0;

    public enum collectable_type
    {
        COLLECT_STARS,
        COLLLECT_LOLLIPOP,
        PAIR_CANDIES
    }

    private string OutputFolderAssetPath
    {
        get { return EditorPrefs.GetString("OutputFolderAssetPath", ""); }
        set { EditorPrefs.SetString("OutputFolderAssetPath", value); }
    }

    private string JsonObjectAssetPath
    {
        get { return EditorPrefs.GetString("JsonObjectAssetPath", ""); }
        set { EditorPrefs.SetString("JsonObjectAssetPath", value); }
    }

    private void OnGUI()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        version = EditorGUILayout.IntField("Version", version);
        time = EditorGUILayout.FloatField("Time Allotted", time);

        outputFolder = EditorGUILayout.ObjectField("Output Folder", outputFolder, typeof(UnityEngine.Object), false);

        EditorGUILayout.BeginHorizontal();
        jsonObject = EditorGUILayout.ObjectField("Quest Data (json)", jsonObject, typeof(UnityEngine.Object), false);
        if (GUILayout.Button("Load Quest Data"))
        {
            JsonObjectAssetPath = (jsonObject != null) ? AssetDatabase.GetAssetPath(jsonObject) : null;
            string dataAsJson = System.IO.File.ReadAllText(JsonObjectAssetPath);
            EditorQuestData dataFromFile = JsonUtility.FromJson<EditorQuestData>(dataAsJson);
            tier_list = dataFromFile.all_tiers;
            tier_collapse = new List<bool>();
            for (int i = 0; i < tier_list.Count; i++)
            {
                tier_collapse.Add(false);
            }
        }
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(50);
        EditorGUILayout.BeginHorizontal();
        DrawTierConfigurationButtons();
        EditorGUILayout.EndHorizontal();

        DrawTierConfiguration();

        GUILayout.Space(50);
        if (GUILayout.Button("Generate JSON"))
        {
            EditorQuestData data = new EditorQuestData
            {
                version = version,
                time_allotted = time,
                all_tiers = tier_list
            };
            onGenerateButtonPressed(data);
        }
        EditorGUILayout.EndScrollView();
    }
    void DrawTierConfigurationButtons()
    {
        if (GUILayout.Button("Add Tier"))
        {
            // Add Tier
            tier_list.Add(new QuestTier());
            tier_collapse.Add(true);
        }
        if (GUILayout.Button("Remove Tier"))
        {
            // Remove Tier
            tier_list.RemoveAt(tier_list.Count - 1);
            tier_collapse.RemoveAt(tier_collapse.Count - 1);
        }
        if (GUILayout.Button("Reset All tiers"))
        {
            // Reset tiers
            tier_list.Clear();
            tier_collapse.Clear();
        }
    }
    void DrawTierConfiguration()
    {
        for (int i = 0; i < tier_list.Count; i++)
        {
            List<QuestType> quest_types = tier_list[i].quest_type;
            List<QuestReward> extra_rewards = tier_list[i].extra_rewards;

            tier_collapse[i] = EditorGUILayout.Foldout(tier_collapse[i], "Configure for Tier " + (i + 1));
            if (!tier_collapse[i]) { continue; }
            EditorGUI.indentLevel++; // Foldout

            GUILayout.Space(10);
            tier_list[i].tier = EditorGUILayout.IntField("Tier :", tier_list[i].tier);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Add Quest Type", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("(" + total_type_probability + "%)", GUILayout.MaxWidth(100));

            DrawTypeConfigurationButtons(quest_types);
            EditorGUILayout.EndHorizontal();

            EditorGUI.indentLevel++; // Quest type

            DrawTypeConfiguration(quest_types);
            GUILayout.Space(10);
            tier_list[i].TypeOfGift = (GiftType)EditorGUILayout.EnumPopup("Gift Type :", tier_list[i].TypeOfGift);
            tier_list[i].trophy_reward = EditorGUILayout.IntField("Trophy Awarded :", tier_list[i].trophy_reward);
            tier_list[i].extra_rewards_amount = EditorGUILayout.IntField("Loots to give :", tier_list[i].extra_rewards_amount);

            GUILayout.Space(10);

            EditorGUILayout.BeginHorizontal();
            EditorGUI.indentLevel++; // Rewards
            EditorGUILayout.LabelField("Add Rewards", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("(" + total_reward_probability + "%)", GUILayout.MaxWidth(100));

            DrawRewardConfigurationButtons(extra_rewards);
            EditorGUI.indentLevel--; // Rewards

            EditorGUILayout.EndHorizontal();

            DrawRewardConfiguration(extra_rewards);

            EditorGUI.indentLevel--; // Quest type
            EditorGUI.indentLevel--; // Foldout

            GUILayout.Space(30);
        }
    }
    void DrawRewardConfigurationButtons(List<QuestReward> extra_rewards)
    {
        if (GUILayout.Button("Add Reward"))
        {
            // Add type
            extra_rewards.Add(new QuestReward());
        }
        if (GUILayout.Button("Remove Reward"))
        {
            // Remove type
            extra_rewards.RemoveAt(extra_rewards.Count - 1);
        }
        if (GUILayout.Button("Reset All Reward"))
        {
            // Reset type
            extra_rewards.Clear();
        }
    }
    void DrawRewardConfiguration(List<QuestReward> extra_rewards)
    {
        GUILayout.Space(10);
        total_reward_probability = 0;
        for (int i = 0; i < extra_rewards.Count; i++)
        {
            EditorGUI.indentLevel++; // extra rewards[i]

            EditorGUILayout.BeginHorizontal();
            extra_rewards[i].amount = EditorGUILayout.IntField("Amount :", extra_rewards[i].amount);
            extra_rewards[i].type = (QuestReward.reward_types)EditorGUILayout.EnumPopup("Collectable type :", extra_rewards[i].type);
            extra_rewards[i].percentage = EditorGUILayout.FloatField("Probability :", extra_rewards[i].percentage);
            total_reward_probability += extra_rewards[i].percentage;
            EditorGUILayout.EndHorizontal();

            EditorGUI.indentLevel--; // extra rewards[i]
        }
        GUILayout.Space(10);
    }
    void DrawTypeConfigurationButtons(List<QuestType> quest_types)
    {
        if (GUILayout.Button("Add type"))
        {
            // Add type
            quest_types.Add(new QuestType());
        }
        if (GUILayout.Button("Remove type"))
        {
            // Remove type
            quest_types.RemoveAt(quest_types.Count - 1);
        }
        if (GUILayout.Button("Reset All types"))
        {
            // Reset type
            quest_types.Clear();
        }
    }
    void DrawTypeConfiguration(List<QuestType> quest_type)
    {
        GUILayout.Space(10);
        total_type_probability = 0;
        for (int i = 0; i < quest_type.Count; i++)
        {
            EditorGUI.indentLevel++; // quest type[i]
            EditorGUILayout.BeginVertical();

            EditorGUILayout.BeginHorizontal();
            quest_type[i].type = (QuestType.collectable_type)EditorGUILayout.EnumPopup("Collectable type :", quest_type[i].type);
            quest_type[i].amount = EditorGUILayout.IntField("Amount :", quest_type[i].amount);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            quest_type[i].moves_reward = EditorGUILayout.IntField("Moves :", quest_type[i].moves_reward);
            quest_type[i].percentage = EditorGUILayout.FloatField("Probability :", quest_type[i].percentage);
            total_type_probability += quest_type[i].percentage;
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            GUILayout.Space(10);
            EditorGUI.indentLevel--; // quest type[i]
        }
        GUILayout.Space(10);
    }
    void onGenerateButtonPressed(EditorQuestData data)
    {
        string quest_data = JsonUtility.ToJson(data);
        OutputFolderAssetPath = (outputFolder != null) ? AssetDatabase.GetAssetPath(outputFolder) : null;

        System.IO.File.WriteAllText(OutputFolderAssetPath + "/quest_data.json", quest_data);
        Debug.Log(OutputFolderAssetPath);
        AssetDatabase.Refresh();
    }
}