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

        if (Input.GetButton("Fire1") && weapon.CanShoot())
        {
            weapon.Shoot();

            Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            if (Physics.Raycast(ray, out RaycastHit hit, weapon.range))
            {
                EnemyHitbox hitbox = hit.collider.GetComponent<EnemyHitbox>();
                if (hitbox != null && hitbox.enemy != null)
                {
                    float finalDamage = weapon.damage * (hitbox.isHead ? headshotMultiplier : bodyshotMultiplier);
                    Debug.Log((hitbox.isHead ? "💥 HEADSHOT" : "🔫 BODY SHOT") + $" for {finalDamage} damage");
                    hitbox.ApplyDamage(finalDamage);
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            weapon.Reload();
        }
    }
}
