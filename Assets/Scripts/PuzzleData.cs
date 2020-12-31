using UnityEngine;

[System.Serializable]
public class PuzzleData
{
    public int level_index;
    public int stars;
    public bool is_challenge;

    public PuzzleData (int index, int stars_earned, bool challenge)
    {
        level_index = index;
        stars = stars_earned;
        is_challenge = challenge;
    }
}
