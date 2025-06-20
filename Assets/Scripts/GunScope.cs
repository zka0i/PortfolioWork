using UnityEngine;

public class GunScope : MonoBehaviour
{
    [Header("Scope Settings")]
    public Vector3 normalLocalPosition = Vector3.zero;
    public Vector3 scopedLocalPosition = new Vector3(0f, -0.05f, 0.2f);
    public float aimSmoothing = 10f;

    [HideInInspector] public bool isAiming = false;

    private void Update()
    {
        Vector3 targetPosition = isAiming ? scopedLocalPosition : normalLocalPosition;
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.deltaTime * aimSmoothing);
    }
}
