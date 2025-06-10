using UnityEngine;

public class AnimationEventRelay : MonoBehaviour
{
    public OnboardingManager manager;

    public void OnDissolveEnd()
    {
        if (manager != null)
        {
            manager.OnDissolveEnd();
        }
    }

    public void OnReformEnd()
    {
        if (manager != null)
        {
            manager.OnReformEnd();
        }
    }
}
