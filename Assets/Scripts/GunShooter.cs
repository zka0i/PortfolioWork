using UnityEngine;

public class GunShooter : MonoBehaviour
{
    private WeaponManager weaponManager;
    private Camera cam;

    [Header("Damage Multipliers")]
    public float headshotMultiplier = 2f;
    public float bodyshotMultiplier = 1f;

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
                    float finalDamage = weapon.damage;

                    string hitPart = hit.collider.name.ToLower();

                    if (hitPart.Contains("head"))
                    {
                        finalDamage *= headshotMultiplier;
                        Debug.Log("💥 HEADSHOT! Dealing " + finalDamage + " damage to " + enemy.gameObject.name);
                    }
                    else if (hitPart.Contains("body"))
                    {
                        finalDamage *= bodyshotMultiplier;
                        Debug.Log("🔫 BODY SHOT. Dealing " + finalDamage + " damage to " + enemy.gameObject.name);
                    }
                    else
                    {
                        // fallback: just log raw damage
                        Debug.Log("🟡 Hit unknown part '" + hit.collider.name + "'. Dealing " + finalDamage + " damage.");
                    }

                    enemy.TakeDamage(finalDamage);
                }
            }
        }

        // Reloading
        if (Input.GetKeyDown(KeyCode.R))
        {
            weapon.Reload();
        }
    }
}
