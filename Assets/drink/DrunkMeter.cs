using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class DrunkMeter : MonoBehaviour
{
    public float maxDrunkness;
    public TextMeshProUGUI display;
    private bool _done;
    public Transform door;
    
    private void Update()
    {
        var v = Player.player.drunkness/maxDrunkness*100;
        display.text = $"You Are: {v:F1}% Drunk";
        
        if (!_done && v >= 100) StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        var t = 0f;
        _done = true;
        
        while (t < 1)
        {
            t += Time.deltaTime;
            door.localRotation = Quaternion.Euler(0, 0, t * 70);
            yield return new WaitForSeconds(0);
        }
        door.localRotation = Quaternion.Euler(0, 0, 70);
    }
}
