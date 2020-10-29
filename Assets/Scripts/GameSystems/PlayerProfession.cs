using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerProfession
{
    public string ProfessionName = "Profession";

    public string AbilityName = "Ability";
    public string AbilityDescription = "Does an action";

    public BaseStats[] defaultStats;
}
