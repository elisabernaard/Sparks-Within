using UnityEngine;

public class LightMotion : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("Seconds for one full oscillation cycle (lower = faster)")]
    [SerializeField] float cycleDuration = 5f;

    [Tooltip("Amplitude of movement in X and Z directions")]
    [SerializeField] Vector2 movementAmplitudeXZ = new Vector2(20f, 20f);

    [Tooltip("Time offset between X and Z motion (0 = synced, >0 = staggered)")]
    [SerializeField] float timeOffsetZ = 0.25f;

    private Vector3 initialPosition;

    void Awake()
    {
        initialPosition = transform.position;
    }

    void Update()
    {
        float t = Time.time / cycleDuration * Mathf.PI * 2f; // full sine wave cycle every X seconds

        float offsetX = Mathf.Sin(t) * movementAmplitudeXZ.x;
        float offsetZ = Mathf.Sin(t + timeOffsetZ * Mathf.PI * 2f) * movementAmplitudeXZ.y;

        transform.position = initialPosition + new Vector3(offsetX, 0, offsetZ);
    }
}
