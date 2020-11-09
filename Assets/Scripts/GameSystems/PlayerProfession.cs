[System.Serializable]
public class PlayerProfession
{
    public string ProfessionName = "Profession";

    public string AbilityName = "Ability";
    public string AbilityDescription = "Does an action";

    //Each profession comes with a set of default stats that you can make add upon
    public BaseStats[] defaultStats;
}
