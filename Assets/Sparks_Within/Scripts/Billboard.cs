using UnityEngine;

public class Billboard : MonoBehaviour {
  [SerializeField] private BillboardType billboardType;





  public enum BillboardType { LookAtCamera, CameraForward };


  // Use Late update so everything should have finished moving.
  void LateUpdate() {
    // There are two ways people billboard things.
    switch (billboardType) {
      case BillboardType.LookAtCamera:
        transform.LookAt(Camera.main.transform.position, Vector3.up);
        break;
      case BillboardType.CameraForward:
        
        break;
      default:
        break;
    }
   
}
}