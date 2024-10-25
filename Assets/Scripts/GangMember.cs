using UnityEngine;

[System.Serializable]
public class GangMember
{
    public string name;
    public string role;
    public string mugshotPath;
    public Vector3 position;
    public bool kickedOut;
    public string npcPrefabName; // Name of the NPC prefab to load
    public int health; // Example dynamic attribute
    public string weapon; // Example dynamic attribute

    [System.NonSerialized]
    public Sprite mugshot;

    public GangMember(string name, string role, Sprite mugshot, Vector3 position, string npcPrefabName)
    {
        this.name = name;
        this.role = role;
        this.position = position;
        this.kickedOut = false;
        this.npcPrefabName = npcPrefabName; // Save the prefab name
        SetMugshot(mugshot);
        this.health = 100; // Default health value
        this.weapon = "Pistol"; // Default weapon value
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
