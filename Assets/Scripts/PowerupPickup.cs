using UnityEngine;

public class PowerupPickup : MonoBehaviour
{
    public enum PowerupType { Medkit, Bandage, EnergyDrink }
    public PowerupType type;
    public float pickupRange = 3f;

    [Header("Audio")]
    public AudioClip useSound; // Assign unique clip on each prefab
    public float volume = 1f;

    private Transform player;
    private PlayerStats stats;
    private AudioSource playerAudio;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (player != null)
        {
            stats = player.GetComponent<PlayerStats>();
            playerAudio = player.GetComponent<AudioSource>();

            // Add AudioSource if not already there
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
        // Play sound from player's AudioSource
        if (useSound != null && playerAudio != null)
        {
            playerAudio.PlayOneShot(useSound, volume);
        }

        switch (type)
        {
            case PowerupType.Medkit:
                stats.Heal(stats.maxHealth);
                break;

            case PowerupType.Bandage:
                stats.Heal(30f);
                break;

            case PowerupType.EnergyDrink:
                stats.UseEnergyDrink();
                break;
        }

        Destroy(gameObject);
    }
}
