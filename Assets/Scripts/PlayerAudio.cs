using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    static public PlayerAudio playerAudio;

    public AudioSource audioSource;

    public AudioClip landSound;
    public AudioClip nibbleSound;
    public AudioClip screamSound;
    public AudioClip squeak1Sound;
    public AudioClip squeak2Sound;
    public AudioClip squeak3Sound;
    public AudioClip walkSound;
    public AudioClip boingSound;

    public void Awake()
    {
        playerAudio = this;
    }

    public void PlayLand()
    {
        PlaySound(landSound);
    }

    public void PlayNibble()
    {
        PlaySound(nibbleSound);
    }

    public void PlayScream()
    {
        PlaySound(screamSound);
    }

    public void PlaySqueak1()
    {
        PlaySound(squeak1Sound);
    }

    public void PlaySqueak2()
    {
        PlaySound(squeak2Sound);
    }

    public void PlaySqueak3()
    {
        PlaySound(squeak3Sound);
    }

    public void PlayWalk()
    {
        PlaySound(walkSound);
    }

    public void PlayBoing()
    {
        PlaySound(boingSound, 0.05f);
    }

    public void PlayRandomSqueak()
    {
        int index = Random.Range(1, 4);
        switch (index)
        {
            case 1: PlaySqueak1(); break;
            case 2: PlaySqueak2(); break;
            case 3: PlaySqueak3(); break;
        }
    }

    public void StartWalking()
    {
        if (walkSound != null && (!audioSource.isPlaying || audioSource.clip != walkSound))
        {
            audioSource.clip = walkSound;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    public void StopWalking()
    {
        if (audioSource.isPlaying && audioSource.clip == walkSound)
        {
            audioSource.Stop();
            audioSource.loop = false;
            audioSource.clip = null;
        }
    }

    private void PlaySound(AudioClip clip, float volume = 1f)
    {
        if (clip != null)
        {
            audioSource.PlayOneShot(clip, volume);
        }
    }
}
