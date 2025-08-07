using UnityEngine;

public class HelicopterExtractionZone : MonoBehaviour
{
    public float extractionTime = 5f;
    private float timer = 0f;
    private bool playerInside = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
            timer = 0f;
            Debug.Log("🟢 Player entered extraction zone.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
            timer = 0f;
            Debug.Log("🔴 Player left extraction zone.");
        }
    }

    private void Update()
    {
        if (playerInside)
        {
            timer += Time.deltaTime;
            if (timer >= extractionTime)
            {
                Debug.Log("✅ Extraction complete!");
                GameManager.Instance.ShowWinScreen();
                enabled = false;
            }
        }
    }
}
