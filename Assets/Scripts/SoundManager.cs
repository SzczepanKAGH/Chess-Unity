using Unity.VisualScripting;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
   public AudioClip checkSound;
   public AudioClip captureSound;
   public AudioClip castlingSound;
   public AudioClip moveWhiteSound;
   public AudioClip moveBlackSound;
   public AudioSource audioSource;

   private void Awake()
   {
      audioSource = GetComponent<AudioSource>();
   }

   public void PlayWhiteMoveSound() => audioSource.PlayOneShot(moveWhiteSound);

   public void PlayBlackMoveSound() => audioSource.PlayOneShot(moveBlackSound);

   public void PlayCaptureSound() => audioSource.PlayOneShot(captureSound);

   public void PlayCastlingSound() => audioSource.PlayOneShot(castlingSound);

   public void PlayCheckSound() => audioSource.PlayOneShot(checkSound);
}
