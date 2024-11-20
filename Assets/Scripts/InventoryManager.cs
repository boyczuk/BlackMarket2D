using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public List<Weapon> ownedWeapons = new List<Weapon>();

    public void AddWeapon(Weapon weapon)
    {
        if (!ownedWeapons.Contains(weapon))
        {
            ownedWeapons.Add(weapon);
            Debug.Log($"Added {weapon.weaponName} to inventory.");
        }
    }

    public bool HasWeapon(string weaponName)
    {
        return ownedWeapons.Exists(w => w.weaponName == weaponName);
    }

    public List<Weapon> GetOwnedWeapons()
    {
        return ownedWeapons;
    }
}
