using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RouletteManager : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip spinSound;
    public AudioClip winSound;

    [Header("Audio Settings")]
    public AudioHelper.PitchSettings pitchSettings = new AudioHelper.PitchSettings();

    [Header("References")]
    public AxisRotator wheel;
    public List<Rigidbody> balls;
    public List<RouletteWheelSlot> slots;
    public Transform ballSpawn;
    public Rigidbody ballPrefab;
    
    [Header("Spin Settings")]
    public float spinDuration = 3f;
    public float spinSpeed = 720f;
    public float endDelay = 1.5f;

    private bool isSpinning = false;
    public bool IsSpinning => isSpinning;

    public Roulette win;

    public void SpinWheel()
    {
        if (isSpinning)
        {
            Debug.LogWarning("⚠️ Spin requested but already spinning.");
            return;
        }

        StartCoroutine(SpinRoutine());
    }

    private IEnumerator SpinRoutine()
    {
        isSpinning = true;

        // Start playing tick sounds from the spin sound with random pitch
        audioSource.clip = spinSound;
        audioSource.loop = true;
        audioSource.volume = 0.4f;
        
        if (pitchSettings.enablePitchVariation)
        {
            audioSource.pitch = AudioHelper.GetRandomPitch(1f, pitchSettings.pitchVariationRange);
        }
        audioSource.Play();

        var b = Instantiate(ballPrefab, ballSpawn.position, Quaternion.identity);
        balls.Add(b);
        if (Player.player.heft >= 30) balls.Add(Player.player.GetComponent<Rigidbody>());

        float audioClipLength = audioSource.clip.length;
        float tickingDuration = spinDuration - audioClipLength;
        
        if (tickingDuration < 0.5f)
        {
            tickingDuration = spinDuration * 0.7f;
        }

        float time = 0f;
        bool isInTickingPhase = true;
        
        while (time < spinDuration)
        {
            float t = time / spinDuration;
            float easedSpeed = Mathf.Lerp(spinSpeed, 0f, t);
            wheel.rotationSpeed = easedSpeed;

            if (isInTickingPhase && time >= tickingDuration)
            {
                audioSource.loop = false;
                audioSource.pitch = 1f;
                isInTickingPhase = false;
            }
            else if (isInTickingPhase)
            {
                float speedRatio = easedSpeed / spinSpeed;
                audioSource.pitch = Mathf.Lerp(0.2f, 2.0f, speedRatio);
                audioSource.volume = Mathf.Lerp(0.2f, 0.4f, speedRatio);
            }

            time += Time.deltaTime;
            yield return null;
        }

        if (isInTickingPhase)
        {
            audioSource.loop = false;
            audioSource.pitch = 1f;
        }

        float remainingAudioTime = 0f;
        if (audioSource.isPlaying)
        {
            remainingAudioTime = audioSource.clip.length - audioSource.time;
        }
        
        float waitTime = Mathf.Min(remainingAudioTime, endDelay);
        if (waitTime > 0)
        {
            yield return new WaitForSeconds(waitTime);
        }

        audioSource.pitch = 1f;
        audioSource.volume = 0.4f;

        List<(int, int)> result = DetectWinningSlot();
        HashSet<int> hit = new HashSet<int>();
        foreach (var item in result)
        {
            win.Payout(item.Item1);
            win.Payout(item.Item2);
            hit.Add(item.Item1);
            hit.Add(item.Item2);
        }

        if (result.Count > 0)
        {
            // Apply random pitch to win sound
            AudioHelper.PlayOneShotWithRandomPitch(audioSource, winSound, 0.1f, 
                pitchSettings.enablePitchVariation ? pitchSettings.pitchVariationRange : 0f);
        }

        for (int i = 1; i < 12; i++)
        {
            if (!hit.Contains(i)) win.CollapseSlot(i);
        }

        foreach (var ball in balls)
        {
            if (!ball.gameObject.GetComponent<Player>()) Destroy(ball.gameObject);
        }

        balls.Clear();
        isSpinning = false;
    }

    private List<(int, int)> DetectWinningSlot()
    {
        var result = new List<(int, int)>();
        foreach (var ball in balls)
        {
            Collider[] overlaps = Physics.OverlapSphere(ball.position, 0.05f);
            foreach (var col in overlaps)
            {
                RouletteWheelSlot slot = col.GetComponent<RouletteWheelSlot>();
                if (slot != null)
                {
                    result.Add((slot.number, slot.slotColor));
                    slot.particle.Emit(50);
                }
            }
        }
        
        return result;
    }
}
