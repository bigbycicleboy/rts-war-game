// Editor/ProvinceAutoGenerator.cs
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class ProvinceAutoGenerator : EditorWindow
{
    private Texture2D lookupTexture;
    private string outputFolder = "Assets/Resources/Provinces";
    private int minPixelCount = 50; // ignore tiny specks

    [MenuItem("WW2/Generate Province Assets")]
    public static void ShowWindow()
    {
        GetWindow<ProvinceAutoGenerator>("Province Generator");
    }

    void OnGUI()
    {
        GUILayout.Label("Province Auto Generator", EditorStyles.boldLabel);
        lookupTexture = (Texture2D)EditorGUILayout.ObjectField("Lookup Texture", lookupTexture, typeof(Texture2D), false);
        outputFolder = EditorGUILayout.TextField("Output Folder", outputFolder);
        minPixelCount = EditorGUILayout.IntField("Min Pixel Count", minPixelCount);

        if (GUILayout.Button("Generate Province Assets"))
        {
            GenerateProvinces();
        }
    }

    void GenerateProvinces()
    {
        if (lookupTexture == null)
        {
            Debug.LogError("No texture assigned.");
            return;
        }

        // make sure output folder exists
        if (!Directory.Exists(outputFolder))
            Directory.CreateDirectory(outputFolder);

        Color32[] pixels = lookupTexture.GetPixels32();
        Dictionary<Color32, int> colorCounts = new Dictionary<Color32, int>();

        // count how many pixels each unique color has
        foreach (Color32 c in pixels)
        {
            // skip black (borders) and teal/blue (water)
            if (IsBorder(c) || IsWater(c))
                continue;

            if (!colorCounts.ContainsKey(c))
                colorCounts[c] = 0;
            colorCounts[c]++;
        }

        int id = 1;
        int created = 0;

        foreach (var kvp in colorCounts)
        {
            // skip colors with too few pixels (noise, anti-aliasing)
            if (kvp.Value < minPixelCount)
                continue;

            Color32 col = kvp.Key;
            string assetName = $"Province_{id:000}";
            string assetPath = $"{outputFolder}/{assetName}.asset";

            // skip if asset already exists
            if (File.Exists(assetPath))
            {
                id++;
                continue;
            }

            ProvinceData province = ScriptableObject.CreateInstance<ProvinceData>();
            province.id = id;
            province.provinceName = assetName;
            province.lookupColor = col;
            province.startingFaction = Faction.Neutral; // set manually after

            AssetDatabase.CreateAsset(province, assetPath);
            id++;
            created++;
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"Generated {created} province assets in {outputFolder}");
    }

    bool IsBorder(Color32 c)
    {
        // black or very dark pixels are border outlines
        return c.r < 30 && c.g < 30 && c.b < 30;
    }

    bool IsWater(Color32 c)
    {
        // teal water color on your map
        return c.g > 150 && c.r < 100 && c.b > 130;
    }
}