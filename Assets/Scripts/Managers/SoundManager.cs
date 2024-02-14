using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum EffectSound
{
    BowString,
    BowFire,
    BowHit
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager instacne = null;

    public AudioSource bgmAudioSource;
    public AudioSource effectAudioSource;

    public AudioClip[] bgmClips;
    public AudioClip[] effectClips;

    private void Awake()
    {
        if(instacne == null)
        {
            instacne = this;
        }
        else
        {
            if(instacne !=  this)
            {
                Destroy(gameObject);
            }
        }
    }

    //배경음악 재생
    public void PlayBGMSound(AudioClip clip)
    {
        bgmAudioSource.clip = clip;
        bgmAudioSource.Play();
    }

    //효과음 재생
    public void PlayEffectSound(AudioClip clip) => effectAudioSource.PlayOneShot(clip);
    public void PlayEffectSound(EffectSound effectSound)
    {
        effectAudioSource.PlayOneShot(effectClips[(int)effectSound]);
    }
}
