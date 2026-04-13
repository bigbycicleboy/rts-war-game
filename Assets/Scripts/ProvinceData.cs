// ProvinceData.cs
using UnityEngine;

[CreateAssetMenu(fileName = "Province", menuName = "WW2/Province Data")]
public class ProvinceData : ScriptableObject
{
    public int id;
    public string provinceName;
    public Color32 lookupColor;
    public Faction startingFaction;

    // runtime state - not serialized, resets each play
    [System.NonSerialized] public Faction currentFaction;

    void OnEnable()
    {
        // reset to starting state when loaded
        currentFaction = startingFaction;
    }
}