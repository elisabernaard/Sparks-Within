using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using Unity.XR.CoreUtils;


public class SimpleVRMovement : MonoBehaviour
{
    public XRNode inputSource = XRNode.LeftHand;
    public float speed = 1.5f;

    private Vector2 inputAxis;
    private XROrigin rig;
    private CharacterController character;

    void Start()
    {
        rig = GetComponent<XROrigin>();
        character = GetComponent<CharacterController>();
    }

    void Update()
    {
        InputDevice device = InputDevices.GetDeviceAtXRNode(inputSource);
        device.TryGetFeatureValue(CommonUsages.primary2DAxis, out inputAxis);

        Vector3 direction = new Vector3(inputAxis.x, 0, inputAxis.y);
        direction = Quaternion.Euler(0, rig.Camera.transform.eulerAngles.y, 0) * direction;

        character.Move(direction * Time.deltaTime * speed);
    }
}
