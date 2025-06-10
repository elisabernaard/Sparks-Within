using UnityEngine;

public class ReticleController : MonoBehaviour
{
    public float defaultDistance = 2.0f;
    public float sizeInDegrees = 1.0f;
    public LayerMask raycastLayers;
    public Transform cameraTransform;

    void Start()
    {
        // Defauts the cameraTransform to the main camera if not set
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    void Update()
    {
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        RaycastHit hit;

        Vector3 targetPosition;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, raycastLayers))
        {
            targetPosition = hit.point;
            Debug.Log("Raycast hit: " + hit.collider.name);
        }
        // Si le raycast ne touche rien, on utilise la distance par défaut
        else
        {
            targetPosition = ray.GetPoint(defaultDistance);
        }

        transform.position = targetPosition;
        transform.LookAt(cameraTransform);
        transform.Rotate(0, 180f, 0); // Pour garder le bon sens

        // Garde une taille constante (taille angulaire fixe)
        float scale = 2 * Mathf.Tan(sizeInDegrees * Mathf.Deg2Rad / 2) * Vector3.Distance(cameraTransform.position, targetPosition);
        transform.localScale = Vector3.one * scale;
    }
}
