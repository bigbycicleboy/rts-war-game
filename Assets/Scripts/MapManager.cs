using UnityEngine;
using System.Collections.Generic;

public class MapManager : MonoBehaviour
{
    [SerializeField] private Texture2D lookupTexture;
    [SerializeField] private Renderer mapRenderer;

    private Texture2D displayTexture;
    private Dictionary<Color32, ProvinceData> colorToProvince = new Dictionary<Color32, ProvinceData>();
    private Dictionary<int, Faction> provinceOwners = new Dictionary<int, Faction>();

    void Start()
    {
        displayTexture = new Texture2D(lookupTexture.width, lookupTexture.height, TextureFormat.RGBA32, false);
        displayTexture.filterMode = FilterMode.Point;
        mapRenderer.material.mainTexture = displayTexture;

        BuildProvinceLookup();
        RefreshDisplayTexture();
    }

    void BuildProvinceLookup()
    {
        var allProvinces = Resources.LoadAll<ProvinceData>("Provinces");
        Debug.Log("Loaded " + allProvinces.Length + " provinces");

        foreach (var p in allProvinces)
        {
            colorToProvince[p.lookupColor] = p;
            // no longer need provinceOwners dict, currentFaction lives on the asset
        }
    }

    public void RefreshDisplayTexture()
    {
        Color32[] lookup = lookupTexture.GetPixels32();
        Color32[] display = new Color32[lookup.Length];

        for (int i = 0; i < lookup.Length; i++)
        {
            if (colorToProvince.TryGetValue(lookup[i], out ProvinceData prov))
            {
                display[i] = GetFactionColor(prov.currentFaction);
            }
            else
            {
                display[i] = new Color32(20, 30, 60, 255);
            }
        }

        displayTexture.SetPixels32(display);
        displayTexture.Apply();
    }

    public void SetProvinceOwner(ProvinceData province, Faction newOwner)
    {
        province.currentFaction = newOwner;
        RefreshDisplayTexture();
    }

    public ProvinceData GetProvinceAtScreenPoint(Vector2 screenPos)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPos);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // convert UV to pixel coordinate
            Vector2 uv = hit.textureCoord;
            int px = Mathf.FloorToInt(uv.x * lookupTexture.width);
            int py = Mathf.FloorToInt(uv.y * lookupTexture.height);
            Color32 col = lookupTexture.GetPixel(px, py);

            if (colorToProvince.TryGetValue(col, out ProvinceData prov))
                return prov;
        }
        return null;
    }

    Color32 GetFactionColor(Faction f)
    {
        return f switch
        {
            Faction.Germany     => new Color32(60,  60,  60,  255), // dark gray
            Faction.France      => new Color32(50,  100, 200, 255), // blue
            Faction.UK          => new Color32(200, 50,  50,  255), // red
            Faction.USSR        => new Color32(180, 40,  40,  255), // dark red
            Faction.Italy       => new Color32(50, 150, 60,  255), // green
            Faction.Luxembourg  => new Color32(80,  140, 200, 255), // light blue
            Faction.Spain       => new Color32(200, 160, 40,  255), // yellow
            Faction.Portugal    => new Color32(40,  140, 80,  255), // green
            Faction.Sweden      => new Color32(180, 160, 60,  255), // gold
            Faction.Finland     => new Color32(200, 190, 170, 255), // cream
            Faction.Norway      => new Color32(160, 100, 140, 255), // mauve
            Faction.Denmark     => new Color32(200, 120, 100, 255), // salmon
            Faction.Poland      => new Color32(180, 80,  80,  255), // rose
            Faction.Romania     => new Color32(200, 180, 80,  255), // yellow
            Faction.Hungary     => new Color32(160, 120, 60,  255), // brown
            Faction.Yugoslavia  => new Color32(50, 75, 255, 255), // dark blue
            Faction.Greece      => new Color32(100, 160, 200, 255), // sky blue
            Faction.Turkey      => new Color32(180, 100, 60,  255), // orange
            Faction.Bulgaria    => new Color32(140, 100, 160, 255), // purple
            Faction.Belgium     => new Color32(200, 160, 80,  255), // amber
            Faction.Netherlands => new Color32(220, 120, 60,  255), // orange
            Faction.Switzerland => new Color32(180, 60,  60,  255), // crimson
            Faction.Neutral     => new Color32(120, 120, 100, 255), // gray
            Faction.Slovakia    => new Color32(160, 140, 120, 255), // taupe
            Faction.Czechia     => new Color32(140, 160, 180, 255), // steel blue
            Faction.Ireland     => new Color32(60,  160, 80,  255), // emerald
            Faction.Lithuania   => new Color32(160, 120, 100, 255), // tan
            Faction.Latvia      => new Color32(160, 100, 120, 255), // mauve
            Faction.Estonia     => new Color32(100, 120, 160, 255), // slate blue
            _                   => new Color32(80,  80,  70,  255),
        };
    }
}