using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    static public PlayerAudio playerAudio;

    public AudioSource audioSource;

    [Header("Audio Settings")]
    public AudioHelper.PitchSettings pitchSettings = new AudioHelper.PitchSettings();
    
    public AudioClip landSound;
    public AudioClip nibbleSound;
    public AudioClip screamSound;
    public AudioClip squeak1Sound;
    public AudioClip squeak2Sound;
    public AudioClip squeak3Sound;
    public AudioClip walkSound;
    public AudioClip boingSound;

    private float originalPitch;

    public void Awake()
    {
        playerAudio = this;
        originalPitch = audioSource.pitch;
    }

    public void PlayLand()
    {
        PlaySoundWithRandomPitch(landSound);
    }

    public void PlayNibble()
    {
        PlaySoundWithRandomPitch(nibbleSound);
    }

    public void PlayScream()
    {
        PlaySoundWithRandomPitch(screamSound);
    }

    public void PlaySqueak1()
    {
        PlaySoundWithRandomPitch(squeak1Sound);
    }

    public void PlaySqueak2()
    {
        PlaySoundWithRandomPitch(squeak2Sound);
    }

    public void PlaySqueak3()
    {
        PlaySoundWithRandomPitch(squeak3Sound);
    }

    public void PlayWalk()
    {
        PlaySoundWithRandomPitch(walkSound);
    }

    public void PlayBoing()
    {
        PlaySoundWithRandomPitch(boingSound, 0.05f);
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
            AudioHelper.PlayWithRandomPitch(audioSource, walkSound, true, 1f, 
                pitchSettings.enablePitchVariation ? pitchSettings.pitchVariationRange : 0f);
        }
    }

    public void StopWalking()
    {
        if (audioSource.isPlaying && audioSource.clip == walkSound)
        {
            audioSource.Stop();
            audioSource.loop = false;
            audioSource.clip = null;
            audioSource.pitch = originalPitch;
        }
    }

    private void PlaySoundWithRandomPitch(AudioClip clip, float volume = 1f)
    {
        if (!pitchSettings.enablePitchVariation)
        {
            audioSource.PlayOneShot(clip, volume);
            return;
        }
        
        AudioHelper.PlayOneShotWithRandomPitch(audioSource, clip, volume, pitchSettings.pitchVariationRange);
    }
}
