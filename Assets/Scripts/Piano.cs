using UnityEngine;

public class Piano : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
}
