using UnityEngine;

public class SoundProfile : MonoBehaviour
{
    public string beingName;
    public AudioClip soundClip;
    public Color topColor = Color.white;
    public Color bottomColor = Color.gray;

    void Awake()
    {
        if (soundClip == null)
        {
            var src = GetComponent<AudioSource>();
            if (src != null)
            {
                soundClip = src.clip;

                if (soundClip == null)
                    Debug.LogWarning("⚠️ " + gameObject.name + " has AudioSource but no clip!");
            }
            else
            {
                Debug.LogWarning("⚠️ " + gameObject.name + " has no AudioSource attached!");
            }
        }
    }
}
