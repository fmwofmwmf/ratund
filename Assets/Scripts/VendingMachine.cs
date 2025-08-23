using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class VendingMachine : MonoBehaviour
{
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

    float depositedAmount = 0f;

    public void depositChipAmount(float amount)
    {
        int playerChipCount = Player.player.GetChipCount();
        if (playerChipCount >= amount)
        {
            changeDepositedAmount(amount);
            depositedAmountUpdate.Invoke(depositedAmount);
            Player.player.RemoveChip((int)amount);
        }
    }

    public void buyStarCheese()
    {
        if (hasDepositedAmount(starCheeseCost))
        {
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
        depositedAmountDisplayText.text = ((int)depositedAmount).ToString();
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
