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
    [HideInInspector] public int currentAmmo;

    [Header("UI")]
    public Sprite weaponIcon;

    [Header("Recoil")]
    public Transform modelTransform; // Assign this in Inspector
    public Vector3 recoilKick = new Vector3(0f, 0f, -0.05f);
    public float recoilReturnSpeed = 5f;
    private Vector3 recoilOffset;
    private Vector3 originalLocalPos;

    [Header("Scope")]
    public Vector3 scopeOffset = new Vector3(0f, -0.02f, 0.1f);
    public float scopeSpeed = 10f;
    public float zoomFOV = 30f;
    private float defaultFOV;
    private bool isScoping = false;

    [Header("Audio")]
    public AudioClip shootSound;
    public AudioClip equipSound;
    public float shootVolume = 0.6f;
    public float equipVolume = 0.7f;
    private AudioSource audioSource;

    private float nextFireTime = 0f;
    private bool isReloading = false;

    // Static flag for WeaponManager to check
    public static bool IsScoping = false;

    void Start()
    {
        originalLocalPos = modelTransform.localPosition;
        defaultFOV = Camera.main.fieldOfView;

        // Set up audio source
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        // Play equip sound on spawn
        if (equipSound != null)
        {
            audioSource.PlayOneShot(equipSound, equipVolume);
        }
    }

    void Update()
    {
        isScoping = Input.GetButton("Fire2");
        IsScoping = isScoping;

        // Apply scope and recoil offset
        Vector3 targetPos = originalLocalPos + (isScoping ? scopeOffset : Vector3.zero) + recoilOffset;
        modelTransform.localPosition = Vector3.Lerp(modelTransform.localPosition, targetPos, Time.deltaTime * scopeSpeed);

        // Recoil recovery
        recoilOffset = Vector3.Lerp(recoilOffset, Vector3.zero, Time.deltaTime * recoilReturnSpeed);

        // Camera zoom
        Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, isScoping ? zoomFOV : defaultFOV, Time.deltaTime * scopeSpeed);
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

        recoilOffset += recoilKick;

        if (shootSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(shootSound, shootVolume);
        }

        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(ray, out RaycastHit hit, range))
        {
            Enemy enemy = hit.collider.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
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
