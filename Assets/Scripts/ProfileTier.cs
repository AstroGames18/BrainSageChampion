using System;

[Serializable]
public class ProfileTier
{
    public int min_value;
    public int max_value;
    public int medal_number;
    public string medal_tag = "Some Hunter";
    public ProbableRewards reward;
    public enum badge_type
    {
        BRONZE,
        SILVER,
        GOLD
    }

    public badge_type badge;
}
