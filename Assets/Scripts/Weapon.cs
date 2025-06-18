using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour
{
    [Header("Weapon Settings")]
    public string weaponName = "Pistol";
    public float damage = 10f;
    public float fireRate = 0.2f;
    public float range = 100f;

    [Header("Ammo")]
    public int maxAmmo = 12;
    public float reloadTime = 1.5f;
    public int currentAmmo;

    [Header("UI")]
    public Sprite weaponIcon;

    private float nextFireTime = 0f;
    private bool isReloading = false;

    private void Start()
    {
        currentAmmo = maxAmmo;
    }

    public bool CanShoot()
    {
        return Time.time >= nextFireTime && !isReloading && currentAmmo > 0;
    }

    public void Shoot()
    {
        if (!CanShoot()) return;

        nextFireTime = Time.time + fireRate;
        currentAmmo--;

        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(ray, out RaycastHit hit, range))
        {
            // Check if the hit object is an enemy
            Enemy enemy = hit.collider.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }

            // Optional: Add hit effects or decals here
        }

        Debug.Log(weaponName + " fired! Remaining ammo: " + currentAmmo);
    }

    public void Reload()
    {
        if (!isReloading && currentAmmo < maxAmmo)
        {
            isReloading = true;
            Debug.Log("Reloading " + weaponName + "...");
            StartCoroutine(ReloadCoroutine());
        }
    }

    private IEnumerator ReloadCoroutine()
    {
        yield return new WaitForSeconds(reloadTime);
        currentAmmo = maxAmmo;
        isReloading = false;
        Debug.Log(weaponName + " reloaded.");
    }

    public bool IsReloading()
    {
        return isReloading;
    }

    public float GetDamage()
    {
        return damage;
    }
}
