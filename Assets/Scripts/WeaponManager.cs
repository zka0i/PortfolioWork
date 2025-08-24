using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [Header("Weapon Settings")]
    public Transform weaponHolder;
    public GameObject[] weaponPrefabs;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip[] equipSounds;
    [Range(0f, 1f)] public float equipVolume = 0.5f;

    private Weapon currentWeapon;
    private GameObject currentWeaponObject;
    private int currentIndex = -1;

    private int[] weaponAmmo;
    private int[] weaponReserveAmmo; // ✅ store reserve ammo
    private bool disableSwitching = false;

    void Start()
    {
        weaponAmmo = new int[weaponPrefabs.Length];
        weaponReserveAmmo = new int[weaponPrefabs.Length];

        for (int i = 0; i < weaponPrefabs.Length; i++)
        {
            var wpn = weaponPrefabs[i].GetComponent<Weapon>();
            if (wpn != null)
            {
                weaponAmmo[i] = wpn.maxAmmo;              // start full mag
                weaponReserveAmmo[i] = wpn.maxReserveAmmo; // start full reserve
            }
        }

        EquipWeapon(0);
    }

    void Update()
    {
        if (disableSwitching || Weapon.IsScoping) return;

        if (Input.GetKeyDown(KeyCode.Alpha1)) EquipWeapon(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) EquipWeapon(1);
    }

    void EquipWeapon(int index)
    {
        if (index >= weaponPrefabs.Length || index == currentIndex) return;

        if (currentWeapon != null)
        {
            // Save ammo before switching
            currentWeapon.CancelReload();
            weaponAmmo[currentIndex] = currentWeapon.currentAmmo;
            weaponReserveAmmo[currentIndex] = currentWeapon.reserveAmmo; // ✅ save reserve
        }

        if (currentWeaponObject != null)
            Destroy(currentWeaponObject);

        currentWeaponObject = Instantiate(weaponPrefabs[index], weaponHolder);
        currentWeaponObject.transform.localPosition = Vector3.zero;
        currentWeaponObject.transform.localRotation = Quaternion.identity;

        currentWeapon = currentWeaponObject.GetComponent<Weapon>();

        // ✅ Restore ammo values
        currentWeapon.currentAmmo = weaponAmmo[index];
        currentWeapon.reserveAmmo = weaponReserveAmmo[index];

        currentIndex = index;

        if (audioSource != null && equipSounds != null && index < equipSounds.Length && equipSounds[index] != null)
        {
            audioSource.PlayOneShot(equipSounds[index], equipVolume);
        }
    }

    public Weapon GetCurrentWeapon() => currentWeapon;

    public GameObject GetCurrentWeaponObject() => currentWeaponObject;

    public void DisableWeaponSwitching(bool value) => disableSwitching = value;
}
