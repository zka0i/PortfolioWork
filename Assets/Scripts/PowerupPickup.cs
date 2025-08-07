using UnityEngine;
using System;

public class PowerupPickup : MonoBehaviour
{
    public enum PowerupType { Medkit, Bandage, EnergyDrink, AmmoBox }
    public PowerupType type;
    public float pickupRange = 3f;

    [Header("Audio")]
    public AudioClip useSound;
    public float volume = 1f;

    public Action OnPickedUp;

    private Transform player;
    private PlayerStats stats;
    private AudioSource playerAudio;
    private WeaponManager weaponManager;

    public int ammoAmount = 30; // ✅ How much reserve ammo to give

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (player != null)
        {
            stats = player.GetComponent<PlayerStats>();
            playerAudio = player.GetComponent<AudioSource>();
            weaponManager = player.GetComponent<WeaponManager>();

            if (playerAudio == null)
                playerAudio = player.gameObject.AddComponent<AudioSource>();
        }
    }

    void Update()
    {
        if (player == null || stats == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= pickupRange && Input.GetKeyDown(KeyCode.E))
        {
            ApplyEffect();
        }
    }

    void ApplyEffect()
    {
        switch (type)
        {
            case PowerupType.Medkit:
                stats.Heal(stats.maxHealth);
                PlaySound();
                break;

            case PowerupType.Bandage:
                stats.Heal(30f);
                PlaySound();
                break;

            case PowerupType.EnergyDrink:
                stats.UseEnergyDrink();
                PlaySound();
                break;

            case PowerupType.AmmoBox:
                if (weaponManager != null)
                {
                    Weapon current = weaponManager.GetCurrentWeapon();
                    if (current != null)
                    {
                        int reserve = current.reserveAmmo;
                        int maxReserve = current.maxReserveAmmo;

                        if (reserve >= maxReserve)
                        {
                            Debug.Log("⚠️ Reserve ammo is full. Ammo pickup ignored.");
                            return; // ✅ Don't use or destroy if ammo full
                        }

                        current.reserveAmmo = Mathf.Min(reserve + ammoAmount, maxReserve);
                        Debug.Log($"🟢 Picked up ammo! Reserve now: {current.reserveAmmo}");
                        PlaySound();
                    }
                }
                break;
        }

        OnPickedUp?.Invoke();
        Destroy(gameObject);
    }

    void PlaySound()
    {
        if (useSound != null && playerAudio != null)
        {
            playerAudio.PlayOneShot(useSound, volume);
        }
    }
}
