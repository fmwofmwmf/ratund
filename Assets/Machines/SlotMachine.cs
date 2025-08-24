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
    public int betAmount = 1;
    private int _winnings;
    public GameObject handle;
    public bool isSpinning = false;
    public Camera slotMachineCamera;
    private Camera previousCamera;

    private void Awake()
    {
        if (slotMachineCamera != null)
        {
            slotMachineCamera.enabled = false;
        }
    }

    private void Start()
    {
        Spin();
    }

    public void trySpin()
    {
        if (!isSpinning && money >= betAmount && betAmount > 0)
        {
            if (Player.player.HeftStage == 1 || Player.player.HeftStage == 2)
            {
                isSpinning = true;
                previousCamera = Camera.main;

                if (previousCamera != null)
                    previousCamera.enabled = false;

                if (slotMachineCamera != null)
                    slotMachineCamera.enabled = true;
                StartCoroutine(HandleSpinRoutine());
            }
            else
            {
                Rigidbody rigidBody = Player.player.GetComponent<Rigidbody>();
                float speed = rigidBody.linearVelocity.magnitude;
                float impulse = Player.player.heft * speed;

                Debug.Log("Impulse: " + impulse);
                if (impulse > 7.5f)
                {
                    isSpinning = true;
                    previousCamera = Camera.main;

                    if (previousCamera != null)
                        previousCamera.enabled = false;

                    if (slotMachineCamera != null)
                        slotMachineCamera.enabled = true;
                    StartCoroutine(HandleSpinRoutine());
                }   
            }
        }
    }

    IEnumerator HandleSpinRoutine()
    {
        // Store the original rotation
        Vector3 originalRotation = handle.transform.localEulerAngles;
        
        // Calculate target rotation (40 degrees down on X axis)
        Vector3 targetRotation = originalRotation + new Vector3(40f, 0f, 0f);
        
        // Animate handle down
        float animationTime = 0.3f;
        float elapsedTime = 0f;
        
        while (elapsedTime < animationTime)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / animationTime;
            progress = Mathf.SmoothStep(0f, 1f, progress); // Smooth animation curve
            
            handle.transform.localEulerAngles = Vector3.Lerp(originalRotation, targetRotation, progress);
            yield return null;
        }
        
        // Ensure handle is at exact target position
        handle.transform.localEulerAngles = targetRotation;
        
        // Animate handle back up
        elapsedTime = 0f;
        
        while (elapsedTime < animationTime)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / animationTime;
            progress = Mathf.SmoothStep(0f, 1f, progress); // Smooth animation curve
            
            handle.transform.localEulerAngles = Vector3.Lerp(targetRotation, originalRotation, progress);
            yield return null;
        }
        // Ensure handle returns to exact original position
        handle.transform.localEulerAngles = originalRotation;

        // Start the spin
        Spin();
    }

    public void depositChips()
    {
        float chipValue = Player.player.getChipValue();
        changeMoney(chipValue);
        Update();
        Player.player.ClearChips();
    }

    public void changeMoney(float amount)
    {
        money += amount;
    }

    public void SetBet1()
    {
        if (money >= 1)
        { 
            betAmount = 1;
        }
    }
    
    public void SetBet3()
    {
        if (money >= 3)
        {
            betAmount = 3;
        }
    }
    
    public void SetBet5()
    {
        if (money >= 5)
        {
            betAmount = 5;
        }
    }

    private void Update()
    {
        moneyText.text = $"Credit: {(int)money}";
        betText.text = $"Bet: {betAmount}";
    }

    public void Spin()
    {
        isSpinning = true;
        money -= betAmount;
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
        isSpinning = false;
        if (slotMachineCamera != null)
            slotMachineCamera.enabled = false;

        if (previousCamera != null)
            previousCamera.enabled = true;
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
