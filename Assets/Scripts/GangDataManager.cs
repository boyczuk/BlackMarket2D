using UnityEngine;
using System.IO;

public class GangDataManager : MonoBehaviour
{
    private string savePath;
    public CriminalOrganization criminalOrganization;

    void Start()
    {
        savePath = Application.persistentDataPath + "/gangData.json";
        Debug.Log("Gang data path: " + savePath);
        LoadGangData();
    }

    public void SaveGangData(CriminalOrganization organization)
    {
        string json = JsonUtility.ToJson(organization, true);
        File.WriteAllText(savePath, json);
        Debug.Log("Gang data saved to: " + savePath);
    }

    public void LoadGangData()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            criminalOrganization = JsonUtility.FromJson<CriminalOrganization>(json);
            Debug.Log("Gang data loaded.");
        }
        else
        {
            Debug.LogWarning("No gang data found!");
            criminalOrganization = new CriminalOrganization();
        }
    }
}
