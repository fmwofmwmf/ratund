using System;
using UnityEngine;

public class RouletteWheelSlot : MonoBehaviour
{
    public int number;           // The roulette number (0–36 usually)
    public int slotColor;      // Red/Black/Green if needed
    public ParticleSystem particle;

    private void Awake()
    {
        particle = GetComponentInChildren<ParticleSystem>();
    }
}