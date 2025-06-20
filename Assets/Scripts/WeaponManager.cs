using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public Transform weaponHolder; // Empty child object where weapon prefabs are spawned
    public GameObject[] weaponPrefabs; // Assign in Inspector (size = 2 for now)

    private Weapon currentWeapon;
    private GameObject currentWeaponObject;
    private int currentIndex = 0;

    void Start()
    {
        EquipWeapon(0); // Equip first weapon by default
    }

    void Update()
    {
        // Prevent switching while aiming (right-click held)
        if (Input.GetMouseButton(1)) return;

        if (Input.GetKeyDown(KeyCode.Alpha1)) EquipWeapon(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) EquipWeapon(1);
    }

    void EquipWeapon(int index)
    {
        if (index >= weaponPrefabs.Length) return;

        if (currentWeaponObject != null)
            Destroy(currentWeaponObject);

        currentWeaponObject = Instantiate(weaponPrefabs[index], weaponHolder);
        currentWeapon = currentWeaponObject.GetComponent<Weapon>();
        currentIndex = index;
    }

    public Weapon GetCurrentWeapon()
    {
        return currentWeapon;
    }
}
