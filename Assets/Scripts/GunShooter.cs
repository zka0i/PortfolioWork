using UnityEngine;

public class GunShooter : MonoBehaviour
{
    private WeaponManager weaponManager;
    private Camera cam;

    [Header("Damage Multipliers")]
    public float headshotMultiplier = 2f;

    void Start()
    {
        weaponManager = GetComponent<WeaponManager>();
        cam = Camera.main;
    }

    void Update()
    {
        Weapon weapon = weaponManager.GetCurrentWeapon();
        if (weapon == null || cam == null) return;

        // Shooting
        if (Input.GetButton("Fire1") && weapon.CanShoot())
        {
            weapon.Shoot();

            Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            if (Physics.Raycast(ray, out RaycastHit hit, weapon.range))
            {
                Enemy enemy = hit.collider.GetComponentInParent<Enemy>();
                if (enemy != null)
                {
                    float appliedDamage = weapon.damage;

                    // Check if the hit collider is the head
                    if (hit.collider.name.ToLower().Contains("head"))
                    {
                        appliedDamage *= headshotMultiplier;
                        Debug.Log("Headshot!");
                    }

                    enemy.TakeDamage(appliedDamage);
                }

                // Optional: hit effect, sound, etc.
            }
        }

        // Reloading
        if (Input.GetKeyDown(KeyCode.R))
        {
            weapon.Reload();
        }
    }
}
