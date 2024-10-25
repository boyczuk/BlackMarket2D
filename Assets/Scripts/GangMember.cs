using UnityEngine;

[System.Serializable]
public class GangMember
{
    public string name;
    public string role;
    public string mugshotPath;
    public Vector3 position;
    public bool kickedOut;
    public string npcPrefabName;
    public int health;
    public string weapon;

    [System.NonSerialized]
    public Sprite mugshot;

    public GangMember(string name, string role, Sprite mugshot, Vector3 position, string npcPrefabName)
    {
        this.name = name;
        this.role = role;
        this.position = position;
        this.kickedOut = false;
        this.npcPrefabName = npcPrefabName;
        SetMugshot(mugshot);
        this.health = 100;
        this.weapon = "Pistol";
    }

    public void SetMugshot(Sprite sprite)
    {
        mugshot = sprite;
        mugshotPath = sprite != null ? sprite.name : "";
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
        }
    }
}
