using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [Header("Weapon Settings")]
    public Transform weaponHolder;
    public GameObject[] weaponPrefabs;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip[] equipSounds; // Should match weaponPrefabs size
    [Range(0f, 1f)] public float equipVolume = 0.5f;

    private Weapon currentWeapon;
    private GameObject currentWeaponObject;
    private int currentIndex = -1;

    // Ammo tracking
    private int[] weaponAmmo;

    void Start()
    {
        weaponAmmo = new int[weaponPrefabs.Length];

        for (int i = 0; i < weaponAmmo.Length; i++)
        {
            var wpn = weaponPrefabs[i].GetComponent<Weapon>();
            if (wpn != null) weaponAmmo[i] = wpn.maxAmmo;
        }

        EquipWeapon(0); // Equip primary by default
    }

    void Update()
    {
        if (Weapon.IsScoping) return; // Prevent swapping while scoping

        if (Input.GetKeyDown(KeyCode.Alpha1)) EquipWeapon(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) EquipWeapon(1);
    }

    void EquipWeapon(int index)
    {
        if (index >= weaponPrefabs.Length || index == currentIndex) return;

        // Save ammo before switching
        if (currentWeapon != null)
            weaponAmmo[currentIndex] = currentWeapon.currentAmmo;

        // Destroy current weapon
        if (currentWeaponObject != null)
            Destroy(currentWeaponObject);

        // Instantiate new weapon
        currentWeaponObject = Instantiate(weaponPrefabs[index], weaponHolder);
        currentWeaponObject.transform.localPosition = Vector3.zero;
        currentWeaponObject.transform.localRotation = Quaternion.identity;

        // Setup new weapon
        currentWeapon = currentWeaponObject.GetComponent<Weapon>();
        currentWeapon.currentAmmo = weaponAmmo[index];
        currentIndex = index;

        // Play equip sound
        if (audioSource != null && equipSounds != null && index < equipSounds.Length && equipSounds[index] != null)
        {
            audioSource.PlayOneShot(equipSounds[index], equipVolume);
        }
    }

    public Weapon GetCurrentWeapon()
    {
        return currentWeapon;
    }

    public GameObject GetCurrentWeaponObject()
    {
        return currentWeaponObject;
    }
}
