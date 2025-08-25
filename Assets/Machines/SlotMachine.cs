using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class SlotMachineDirect : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip leverSound;
    public AudioClip winSound;
    public AudioClip spinSound;
    public AudioClip chipInsertSound;

    [Header("Audio Settings")]
    public AudioHelper.PitchSettings pitchSettings = new AudioHelper.PitchSettings();

    [System.Serializable]
    public class SymbolChance
    {
        public Sprite sprite;
        [Range(0f, 1f)] public float probability;
        public int win;
    }

    [Header("Setup")]
    public SymbolChance[] symbols;
    public Image[] reels;
    public ChipSpawner spawner;

    [Header("Spin Settings")]
    public float spinTime = 1.5f;
    public float cycleSpeed = 0.05f;
    public float settleDelay = 0.5f;

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

    public void trySpin()
    {
        isSpinning = true;
        previousCamera = Camera.main;

        if (previousCamera != null)
            previousCamera.enabled = false;

        if (slotMachineCamera != null)
            slotMachineCamera.enabled = true;
        StartCoroutine(HandleSpinRoutine());
    }

    IEnumerator HandleSpinRoutine()
    {
        Vector3 originalRotation = handle.transform.localEulerAngles;
        Vector3 targetRotation = originalRotation + new Vector3(40f, 0f, 0f);
        
        float animationTime = 0.3f;
        float elapsedTime = 0f;
        
        AudioHelper.PlayOneShotWithRandomPitch(audioSource, leverSound, 0.1f, 
            pitchSettings.enablePitchVariation ? pitchSettings.pitchVariationRange : 0f);
            
        while (elapsedTime < animationTime)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / animationTime;
            progress = Mathf.SmoothStep(0f, 1f, progress);

            handle.transform.localEulerAngles = Vector3.Lerp(originalRotation, targetRotation, progress);
            yield return null;
        }
        
        handle.transform.localEulerAngles = targetRotation;
        elapsedTime = 0f;
        
        while (elapsedTime < animationTime)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / animationTime;
            progress = Mathf.SmoothStep(0f, 1f, progress);
            
            handle.transform.localEulerAngles = Vector3.Lerp(targetRotation, originalRotation, progress);
            yield return null;
        }
        handle.transform.localEulerAngles = originalRotation;

        Spin();
    }

    public void depositChips()
    {
        AudioHelper.PlayOneShotWithRandomPitch(audioSource, chipInsertSound, 0.1f, 
            pitchSettings.enablePitchVariation ? pitchSettings.pitchVariationRange : 0f);
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
        if (money >= 1) betAmount = 1;
    }
    
    public void SetBet3()
    {
        if (money >= 3) betAmount = 3;
    }
    
    public void SetBet5()
    {
        if (money >= 5) betAmount = 5;
    }

    private void Update()
    {
        moneyText.text = $"Credit: {(int)money}";
        betText.text = $"Bet: {betAmount}";
    }

    public void Spin()
    {
        AudioHelper.PlayOneShotWithRandomPitch(audioSource, spinSound, 0.1f, 
            pitchSettings.enablePitchVariation ? pitchSettings.pitchVariationRange : 0f);
        isSpinning = true;
        money -= betAmount;
        StopAllCoroutines();
        StartCoroutine(SpinRoutine());
    }

    IEnumerator SpinRoutine()
    {
        Sprite[] finalResults = ChooseResults();

        for (int i = 0; i < reels.Length; i++)
        {
            StartCoroutine(SpinSingleReel(i, finalResults[i], spinTime + settleDelay * i));
        }

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

        reels[index].sprite = finalSprite;
    }

    Sprite[] ChooseResults()
    {
        float roll = (float)rand.NextDouble();
        float cumulative = 0f;

        foreach (var s in symbols)
        {
            cumulative += s.probability;
            if (roll <= cumulative)
            {
                _winnings = s.win;
                return new Sprite[] { s.sprite, s.sprite, s.sprite };
            }
        }

        Sprite[] randoms = new Sprite[reels.Length];
        for (int i = 0; i < reels.Length; i++)
        {
            randoms[i] = symbols[rand.Next(symbols.Length)].sprite;
        }

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
        AudioHelper.PlayOneShotWithRandomPitch(audioSource, winSound, 0.08f, 
            pitchSettings.enablePitchVariation ? pitchSettings.pitchVariationRange : 0f);
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
