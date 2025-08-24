using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RouletteManager : MonoBehaviour
{
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
        var b = Instantiate(ballPrefab, ballSpawn.position, Quaternion.identity);
        balls.Add(b);
        if (Player.player.heft >= 30) balls.Add(Player.player.GetComponent<Rigidbody>());
        
        float time = 0f;
        while (time < spinDuration)
        {
            float t = time / spinDuration;
            float easedSpeed = Mathf.Lerp(spinSpeed, 0f, t); // slow down
            wheel.rotationSpeed = easedSpeed;

            time += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(endDelay);

        List<(int, int)> result = DetectWinningSlot();
        HashSet<int> hit = new HashSet<int>();
        foreach (var item in result)
        {
            win.Payout(item.Item1);
            win.Payout(item.Item2);
            hit.Add(item.Item1);
            hit.Add(item.Item2);
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
