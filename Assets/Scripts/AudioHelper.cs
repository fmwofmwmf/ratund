using UnityEngine;

public static class AudioHelper
{
    [System.Serializable]
    public class PitchSettings
    {
        [Range(0.1f, 1.0f)]
        public float pitchVariationRange = 0.2f;
        public bool enablePitchVariation = true;
        
        public PitchSettings(float range = 0.2f, bool enabled = true)
        {
            pitchVariationRange = range;
            enablePitchVariation = enabled;
        }
    }
    
    public static float GetRandomPitch(float basePitch = 1f, float variationRange = 0.2f)
    {
        return basePitch + Random.Range(-variationRange, variationRange);
    }
    
    public static void PlayOneShotWithRandomPitch(AudioSource audioSource, AudioClip clip, float volume = 1f, float pitchVariation = 0.2f)
    {
        if (audioSource == null || clip == null) return;
        
        float originalPitch = audioSource.pitch;
        audioSource.pitch = GetRandomPitch(originalPitch, pitchVariation);
        audioSource.PlayOneShot(clip, volume);
        audioSource.pitch = originalPitch;
    }
    
    public static void PlayWithRandomPitch(AudioSource audioSource, AudioClip clip, bool loop = false, float volume = 1f, float pitchVariation = 0.2f)
    {
        if (audioSource == null || clip == null) return;
        
        audioSource.clip = clip;
        audioSource.loop = loop;
        audioSource.volume = volume;
        audioSource.pitch = GetRandomPitch(1f, pitchVariation);
        audioSource.Play();
    }
}
