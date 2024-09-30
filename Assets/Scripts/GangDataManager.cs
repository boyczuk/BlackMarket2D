using UnityEngine;
using System.IO;

public class GangDataManager : MonoBehaviour
{
    private string savePath;
    public CriminalOrganization criminalOrganization;

    void Start()
    {
        savePath = Application.persistentDataPath + "/gangData.json";
        LoadGangData();
    }

    public void SaveGangData(CriminalOrganization organization)
    {
        string json = JsonUtility.ToJson(organization, true);
        File.WriteAllText(savePath, json);
    }

    public void LoadGangData()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            criminalOrganization = JsonUtility.FromJson<CriminalOrganization>(json);
        }
        else
        {
            criminalOrganization = new CriminalOrganization();
        }
    }
}
