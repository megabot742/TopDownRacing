using UnityEngine;

public class CameraAudioController : MonoBehaviour
{
    private void Awake()
    {
        AudioListener[] listeners = FindObjectsByType<AudioListener>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        if (listeners.Length > 1)
        {
            AudioListener currentListener = GetComponent<AudioListener>();
            if (currentListener != null && currentListener.enabled)
            {
                currentListener.enabled = false;
            }
        }
    }
}
