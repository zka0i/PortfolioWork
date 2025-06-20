using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public Transform weaponHolder;
    public GameObject[] weaponPrefabs;

    private Weapon currentWeapon;
    private GameObject currentWeaponObject;
    private int currentIndex = -1;

    // Track ammo for each weapon
    private int[] weaponAmmo;

    void Start()
    {
        weaponAmmo = new int[weaponPrefabs.Length];

        // Store default max ammo
        for (int i = 0; i < weaponAmmo.Length; i++)
            weaponAmmo[i] = weaponPrefabs[i].GetComponent<Weapon>().maxAmmo;

        EquipWeapon(0); // Equip primary weapon at start
    }

    void Update()
    {
        if (Weapon.IsScoping) return; // Prevent swapping while scoped

        if (Input.GetKeyDown(KeyCode.Alpha1)) EquipWeapon(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) EquipWeapon(1);
    }

    void EquipWeapon(int index)
    {
        if (index >= weaponPrefabs.Length || index == currentIndex) return;

        // Save current weapon's ammo
        if (currentWeapon != null)
            weaponAmmo[currentIndex] = currentWeapon.currentAmmo;

        // Destroy old weapon
        if (currentWeaponObject != null)
            Destroy(currentWeaponObject);

        // Instantiate new weapon
        currentWeaponObject = Instantiate(weaponPrefabs[index], weaponHolder);
        currentWeaponObject.transform.localPosition = Vector3.zero; // Reset position
        currentWeaponObject.transform.localRotation = Quaternion.identity; // Reset rotation

        currentWeapon = currentWeaponObject.GetComponent<Weapon>();
        currentWeapon.currentAmmo = weaponAmmo[index]; // Restore saved ammo
        currentIndex = index;
    }

    public Weapon GetCurrentWeapon()
    {
        return currentWeapon;
    }
}
