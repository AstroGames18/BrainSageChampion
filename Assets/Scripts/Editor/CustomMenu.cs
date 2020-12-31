using System;
using System.IO;
using UnityEditor;
using UnityEngine;
public class CustomMenu : MonoBehaviour
{
    [MenuItem("Brain Sage/Delete User Data")]
    static void DeleteUserData()
    {
        try
        {
            File.Delete(Application.persistentDataPath + "/bsc_puzzle.astro");
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
        PlayerPrefs.DeleteAll();
    }

    [MenuItem("Brain Sage/ Quest Editor")]
    static void ShowQuestEditor()
    {
        EditorWindow.GetWindow<QuestEditor>("Quest Editor");
    }
}