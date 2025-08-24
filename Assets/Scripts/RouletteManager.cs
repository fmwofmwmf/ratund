using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RouletteManager : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip spinSound;
    public AudioClip winSound;

    [Header("References")]
    public AxisRotator wheel;                 // The wheel object to spin
    public List<Rigidbody> balls;           // Roulette balls
    public List<RouletteWheelSlot> slots;   // Wheel slots (NOT chip slots)
    public Transform ballSpawn;
    public Rigidbody ballPrefab;
    
    [Header("Spin Settings")]
    public float spinDuration = 3f;         // How long the spin lasts
    public float spinSpeed = 720f;          // Degrees per second at start
    public float endDelay = 1.5f;           // Wait after spin before checking

    private bool isSpinning = false;
    public bool IsSpinning => isSpinning;

    public Roulette win;     // Event: winning number

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

        // Start playing tick sounds from the spin sound
        audioSource.clip = spinSound;
        audioSource.loop = true;
        audioSource.volume = 0.4f;
        audioSource.Play();

        var b = Instantiate(ballPrefab, ballSpawn.position, Quaternion.identity);
        balls.Add(b);
        if (Player.player.heft >= 30) balls.Add(Player.player.GetComponent<Rigidbody>());

        // Calculate when to stop ticking and let audio play naturally
        float audioClipLength = audioSource.clip.length;
        float tickingDuration = spinDuration - audioClipLength;
        
        // Ensure we have at least some ticking time
        if (tickingDuration < 0.5f)
        {
            tickingDuration = spinDuration * 0.7f; // Use 70% for ticking, 30% for natural audio ending
        }

        float time = 0f;
        bool isInTickingPhase = true;
        
        while (time < spinDuration)
        {
            float t = time / spinDuration;
            float easedSpeed = Mathf.Lerp(spinSpeed, 0f, t);
            wheel.rotationSpeed = easedSpeed;

            // Handle audio phases
            if (isInTickingPhase && time >= tickingDuration)
            {
                // Transition from ticking to natural audio playback
                audioSource.loop = false;
                audioSource.pitch = 1f; // Reset pitch for natural playback
                isInTickingPhase = false;
            }
            else if (isInTickingPhase)
            {
                // Adjust tick speed based on wheel rotation speed
                float speedRatio = easedSpeed / spinSpeed;
                
                // Create tick effect with pitch modulation
                // Faster ticks at the beginning, slower as it winds down
                audioSource.pitch = Mathf.Lerp(0.2f, 2.0f, speedRatio);
                
                // Optional: Adjust volume based on speed for more realistic effect
                audioSource.volume = Mathf.Lerp(0.2f, 0.4f, speedRatio);
            }

            time += Time.deltaTime;
            yield return null;
        }

        // If we're still in ticking phase, stop the loop now
        if (isInTickingPhase)
        {
            audioSource.loop = false;
            audioSource.pitch = 1f;
        }

        // Wait for any remaining audio to finish or until endDelay
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

        // Reset audio settings for future use
        audioSource.pitch = 1f;
        audioSource.volume = 0.4f;

        // Rest of your routine...
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
            audioSource.PlayOneShot(winSound, 0.1f);
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

    /// <summary>
    /// Checks which wheel slot(s) contain a ball.
    /// </summary>
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
