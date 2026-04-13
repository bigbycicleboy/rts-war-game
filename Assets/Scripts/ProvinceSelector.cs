// ProvinceSelector.cs
using UnityEngine;
using TMPro;

public class ProvinceSelector : MonoBehaviour
{
    [SerializeField] private MapManager mapManager;
    [SerializeField] private GameObject provincePanel;
    [SerializeField] private TextMeshProUGUI provinceNameText;
    [SerializeField] private TextMeshProUGUI factionNameText;
    [SerializeField] private TextMeshProUGUI pixelCountText;

    private ProvinceData selectedProvince;

    void Start()
    {
        provincePanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ProvinceData clicked = mapManager.GetProvinceAtScreenPoint(Input.mousePosition);

            if (clicked != null)
            {
                SelectProvince(clicked);
            }
            else
            {
                Deselect();
            }
        }
    }

    void SelectProvince(ProvinceData province)
    {
        selectedProvince = province;
        provincePanel.SetActive(true);

        provinceNameText.text = province.provinceName;
        factionNameText.text = province.currentFaction.ToString();
        pixelCountText.text = "Faction: " + province.currentFaction.ToString();
    }

    void Deselect()
    {
        selectedProvince = null;
        provincePanel.SetActive(false);
    }
}