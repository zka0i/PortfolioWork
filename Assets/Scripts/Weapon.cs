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
    public Transform modelTransform;
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
    private Coroutine reloadCoroutine;

    public static bool IsScoping = false;
    private WeaponBuildTool buildTool;

    [Header("Muzzle Flash")]
    public Light muzzleFlashLight;
    public float flashDuration = 0.05f;
    public Sprite[] muzzleFlashSprites;
    public SpriteRenderer muzzleFlashRenderer;

    [Header("Bullet Tracer")]
    public LineRenderer bulletLinePrefab;
    public Transform muzzlePoint;
    public float tracerDuration = 0.05f;

    void Start()
    {
        originalLocalPos = modelTransform.localPosition;
        defaultFOV = Camera.main.fieldOfView;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        if (equipSound != null)
            audioSource.PlayOneShot(equipSound, equipVolume);

        buildTool = FindObjectOfType<WeaponBuildTool>();

        if (muzzleFlashLight != null)
            muzzleFlashLight.enabled = false;

        if (muzzleFlashRenderer != null)
            muzzleFlashRenderer.enabled = false;

        if (currentAmmo <= 0)
            currentAmmo = maxAmmo;
    }

    void Update()
    {
        isScoping = Input.GetButton("Fire2");
        IsScoping = isScoping;

        Vector3 targetPos = originalLocalPos + (isScoping ? scopeOffset : Vector3.zero) + recoilOffset;
        modelTransform.localPosition = Vector3.Lerp(modelTransform.localPosition, targetPos, Time.deltaTime * scopeSpeed);

        recoilOffset = Vector3.Lerp(recoilOffset, Vector3.zero, Time.deltaTime * recoilReturnSpeed);

        Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, isScoping ? zoomFOV : defaultFOV, Time.deltaTime * scopeSpeed);

        if (buildTool != null && buildTool.IsBuilding()) return;

        if (Input.GetButton("Fire1") && CanShoot())
        {
            Shoot();
        }
    }

    public bool CanShoot()
    {
        if (buildTool != null && buildTool.IsBuilding()) return false;
        return Time.time >= nextFireTime && !isReloading && currentAmmo > 0;
    }

    public void Shoot()
    {
        if (!CanShoot()) return;

        nextFireTime = Time.time + fireRate;
        currentAmmo--;

        recoilOffset += recoilKick;

        if (shootSound != null)
            audioSource.PlayOneShot(shootSound, shootVolume);

        Vector3 hitPoint = Camera.main.transform.position + Camera.main.transform.forward * range;

        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(ray, out RaycastHit hit, range))
        {
            hitPoint = hit.point;

            Enemy enemy = hit.collider.GetComponent<Enemy>();
            if (enemy != null)
                enemy.TakeDamage(damage);
        }

        Debug.Log($"{weaponName} fired! Remaining ammo: {currentAmmo}");

        StartCoroutine(DoMuzzleFlash());
        StartCoroutine(SpawnBulletTracer(muzzlePoint.position, hitPoint));
    }

    IEnumerator DoMuzzleFlash()
    {
        if (muzzleFlashLight != null) muzzleFlashLight.enabled = true;

        if (muzzleFlashRenderer != null && muzzleFlashSprites.Length > 0)
        {
            muzzleFlashRenderer.sprite = muzzleFlashSprites[Random.Range(0, muzzleFlashSprites.Length)];
            muzzleFlashRenderer.enabled = true;
        }

        yield return new WaitForSeconds(flashDuration);

        if (muzzleFlashLight != null) muzzleFlashLight.enabled = false;
        if (muzzleFlashRenderer != null) muzzleFlashRenderer.enabled = false;
    }

    IEnumerator SpawnBulletTracer(Vector3 start, Vector3 end)
    {
        LineRenderer line = Instantiate(bulletLinePrefab);
        line.SetPosition(0, start);
        line.SetPosition(1, end);

        yield return new WaitForSeconds(tracerDuration);

        Destroy(line.gameObject);
    }

    public void Reload()
    {
        if (!isReloading && currentAmmo < maxAmmo)
        {
            isReloading = true;
            Debug.Log($"Reloading {weaponName}...");
            reloadCoroutine = StartCoroutine(ReloadCoroutine());
        }
    }

    IEnumerator ReloadCoroutine()
    {
        yield return new WaitForSeconds(reloadTime);
        currentAmmo = maxAmmo;
        isReloading = false;
        reloadCoroutine = null;
        Debug.Log($"{weaponName} reloaded.");
    }

    public void CancelReload()
    {
        if (reloadCoroutine != null)
        {
            StopCoroutine(reloadCoroutine);
            reloadCoroutine = null;
        }
        isReloading = false;
    }

    public bool IsReloading() => isReloading;

    public float GetDamage() => damage;
}
