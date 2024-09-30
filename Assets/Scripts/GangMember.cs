using UnityEngine;

[System.Serializable]
public class GangMember
{
    public string name;
    public string role;
    public string mugshotPath;
    public Vector3 position;

    [System.NonSerialized]
    public Sprite mugshot;

    public GangMember(string name, string role, Sprite mugshot, Vector3 position)
    {
        this.name = name;
        this.role = role;
        this.position = position;
        SetMugshot(mugshot);
    }

    public void SetMugshot(Sprite sprite)
    {
        mugshot = sprite;
        mugshotPath = sprite != null ? sprite.name : "";
        Debug.Log("Mugshot path set to: " + mugshotPath);
    }

    public void LoadMugshot()
    {
        if (!string.IsNullOrEmpty(mugshotPath))
        {
            mugshot = Resources.Load<Sprite>("Mugshots/" + mugshotPath);
            if (mugshot == null)
            {
                Debug.LogWarning("Failed to load mugshot: " + mugshotPath);
            }
            else
            {
                Debug.Log("Successfully loaded mugshot: " + mugshotPath);
            }
        }
    }
}
