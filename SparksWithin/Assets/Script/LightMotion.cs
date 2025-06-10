using UnityEngine;

public class LightMotion : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("Seconds for one full oscillation cycle (lower = faster)")]
    [SerializeField] float cycleDuration = 10f; // 느리게 만들기 위해 값을 늘림

    [Tooltip("Amplitude of movement in X direction")]
    [SerializeField] float movementAmplitudeX = 20f;

    private Vector3 initialPosition;

    void Awake()
    {
        initialPosition = transform.position;
    }

    void Update()
    {
        float t = Time.time / cycleDuration * Mathf.PI * 2f; // full sine wave cycle

        float offsetX = Mathf.Sin(t) * movementAmplitudeX;

        transform.position = initialPosition + new Vector3(offsetX, 0, 0);
    }
}
