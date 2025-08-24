using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class VendingMachine : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip chipInsertSound;
    public AudioClip purchaseSound;
    public FloatUnityEvent depositedAmountUpdate;
    public UnityEvent notEnoughDepositedAmount;
    public UnityEvent boughtStarCheese;
    public UnityEvent boughtRoundCheese;
    public UnityEvent boughtWedgeCheese;

    public Cheese starCheesePrefab;
    public Cheese roundCheesePrefab;
    public Cheese wedgeCheesePrefab;
    public Transform cheeseSpawnPoint;

    public TextMeshProUGUI depositedAmountDisplayText;

    public float starCheeseCost = 5f;
    public float roundCheeseCost = 3f;
    public float wedgeCheeseCost = 1f;

    public float depositedAmount = 0f;

    public void depositChips()
    {
        audioSource.PlayOneShot(chipInsertSound, 0.1f);
        float chipValue = Player.player.getChipValue();
        changeDepositedAmount(chipValue);
        depositedAmountUpdate.Invoke(chipValue);
        Player.player.ClearChips();
    }

    public void buyStarCheese()
    {
        if (hasDepositedAmount(starCheeseCost))
        {
            audioSource.PlayOneShot(purchaseSound, 0.07f);
            changeDepositedAmount(-starCheeseCost);
            SpawnCheese(starCheesePrefab);
        }
        else
        {
            notEnoughDepositedAmount.Invoke();
        }
    }

    public void buyRoundCheese()
    {
        if (hasDepositedAmount(roundCheeseCost))
        {
            audioSource.PlayOneShot(purchaseSound, 0.07f);
            changeDepositedAmount(-roundCheeseCost);
            SpawnCheese(roundCheesePrefab);
        }
        else
        {
            notEnoughDepositedAmount.Invoke();
        }
    }

    public void buyWedgeCheese()
    {
        if (hasDepositedAmount(wedgeCheeseCost))
        {
            audioSource.PlayOneShot(purchaseSound, 0.07f);
            changeDepositedAmount(-wedgeCheeseCost);
            SpawnCheese(wedgeCheesePrefab);
        }
        else
        {
            notEnoughDepositedAmount.Invoke();
        }
    }

    private void changeDepositedAmount(float amount)
    {
        depositedAmount += amount;
        depositedAmountDisplayText.text = $"${(int)depositedAmount}";
    }

    private bool hasDepositedAmount(float cost)
    {
        return depositedAmount >= cost;
    }
    
    private void SpawnCheese(Cheese cheesePrefab)
    {
        Instantiate(cheesePrefab, cheeseSpawnPoint.position, cheeseSpawnPoint.rotation);
    }
}
