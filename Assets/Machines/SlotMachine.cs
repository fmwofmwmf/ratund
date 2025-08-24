using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class SlotMachineDirect : MonoBehaviour
{
    [System.Serializable]
    public class SymbolChance
    {
        public Sprite sprite;
        [Range(0f, 1f)] public float probability; // chance of triple win
        public int win;
    }

    [Header("Setup")]
    public SymbolChance[] symbols; // assign sprites + odds
    public Image[] reels;          // assign 3 UI Images in Inspector
    public ChipSpawner spawner;

    [Header("Spin Settings")]
    public float spinTime = 1.5f;
    public float cycleSpeed = 0.05f;
    public float settleDelay = 0.5f; // staggered stop delay per reel

    private System.Random rand = new System.Random();

    public float money;
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI betText;
    public int betAmount;
    private int _winnings;
    
    private void Start()
    {
        Spin();
    }

    public void SetBet1()
    {
        betAmount = 1;
    }
    
    public void SetBet3()
    {
        betAmount = 3;
    }
    
    public void SetBet5()
    {
        betAmount = 5;
    }

    private void Update()
    {
        moneyText.text = $"Credit: {(int)money}";
        betText.text = $"Bet: {betAmount}";
    }

    public void Spin()
    {
        StopAllCoroutines();
        StartCoroutine(SpinRoutine());
    }

    IEnumerator SpinRoutine()
    {
        Sprite[] finalResults = ChooseResults();

        // Start all reels spinning
        for (int i = 0; i < reels.Length; i++)
        {
            StartCoroutine(SpinSingleReel(i, finalResults[i], spinTime + settleDelay * i));
        }

        // Wait until the last reel finishes
        yield return new WaitForSeconds(spinTime + settleDelay * (reels.Length - 1));

        CheckWin(finalResults);
    }

    IEnumerator SpinSingleReel(int index, Sprite finalSprite, float stopAfter)
    {
        float t = 0f;
        while (t < stopAfter)
        {
            t += cycleSpeed;
            reels[index].sprite = symbols[rand.Next(symbols.Length)].sprite;
            yield return new WaitForSeconds(cycleSpeed);
        }

        reels[index].sprite = finalSprite; // settle on chosen result
    }

    Sprite[] ChooseResults()
    {
        float roll = (float)rand.NextDouble();
        float cumulative = 0f;

        // Try each symbol's win chance
        foreach (var s in symbols)
        {
            cumulative += s.probability;
            if (roll <= cumulative)
            {
                // Triple win
                _winnings = s.win;
                return new Sprite[] { s.sprite, s.sprite, s.sprite };
            }
        }

        // Otherwise: random, but not all three the same
        Sprite[] randoms = new Sprite[reels.Length];
        for (int i = 0; i < reels.Length; i++)
        {
            randoms[i] = symbols[rand.Next(symbols.Length)].sprite;
        }

        // Ensure not all 3 match
        if (randoms[0] == randoms[1] && randoms[1] == randoms[2])
        {
            Sprite replacement;
            do
            {
                replacement = symbols[rand.Next(symbols.Length)].sprite;
            } while (replacement == randoms[0]);
            randoms[2] = replacement;
        }

        return randoms;
    }

    void CheckWin(Sprite[] results)
    {
        if (results[0] == results[1] && results[1] == results[2])
        {
            Debug.Log("WIN!");
            spawner.SpawnChips(betAmount * _winnings);
        }
        else
        {
            spawner.SpawnChips(1);
        }
    }
}
