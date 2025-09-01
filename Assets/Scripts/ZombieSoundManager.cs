using UnityEngine;
using System.Collections;

public class ZombieSoundManager : MonoBehaviour
{
    public static ZombieSoundManager Instance;

    [Header("Zombie Audio Settings")]
    public AudioClip[] zombieClips;
    public int maxConcurrentSounds = 3;
    public float minDelayBetweenGroans = 3f;
    public float maxDelayBetweenGroans = 8f;
    [Range(0f, 1f)] public float volume = 0.7f;

    private int currentPlaying = 0;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // ✅ Non-blocking groan for dynamically spawned enemies
    public void RequestGroanNonBlocking(AudioSource zombieAudio, bool debug = false)
    {
        if (zombieAudio == null || zombieAudio.gameObject == null) return;
        StartCoroutine(PlayGroanRoutine(zombieAudio, debug));
    }

    private IEnumerator PlayGroanRoutine(AudioSource zombieAudio, bool debug)
    {
        while (zombieAudio != null && zombieAudio.gameObject != null)
        {
            if (currentPlaying < maxConcurrentSounds)
            {
                currentPlaying++;

                AudioClip clip = zombieClips[Random.Range(0, zombieClips.Length)];
                if (clip != null)
                {
                    zombieAudio.PlayOneShot(clip, volume);
                    if (debug) Debug.Log("🔊 Zombie groan played: " + clip.name);
                    yield return new WaitForSeconds(clip.length);
                }

                currentPlaying--;
            }

            float delay = Random.Range(minDelayBetweenGroans, maxDelayBetweenGroans);
            yield return new WaitForSeconds(delay);
        }
    }
}
