using UnityEngine;

public interface IDamagable
{
    void TakeDamage(float damage);
    void PlayShotClip(AudioSource audioSource);
}
