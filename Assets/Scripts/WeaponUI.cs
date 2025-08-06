using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI ammoText;
    public Image weaponIconImage;

    [Header("UI")]
    public Sprite weaponIcon;

    private WeaponManager weaponManager;

    void Start()
    {
        weaponManager = GetComponent<WeaponManager>();

        if (weaponManager == null)
        {
            Debug.LogError("WeaponManager not found on this GameObject.");
        }
    }

    void Update()
    {
        Weapon currentWeapon = weaponManager.GetCurrentWeapon();

        if (currentWeapon != null)
        {
            // ✅ Show current mag ammo and reserve ammo properly
            ammoText.text = $"{currentWeapon.currentAmmo} / {currentWeapon.reserveAmmo}";

            if (currentWeapon.weaponIcon != null)
            {
                weaponIconImage.sprite = currentWeapon.weaponIcon;
                weaponIconImage.enabled = true;
            }
            else
            {
                weaponIconImage.enabled = false;
            }
        }
        else
        {
            ammoText.text = "-- / --";
            weaponIconImage.enabled = false;
        }
    }
}
