using UnityEngine;

public class Dice : MonoBehaviour
{
    public SpriteRenderer spriteRenderer; // Assign in Inspector
    public Sprite[] diceFaces; // Assign dice face sprites in Inspector

    private int diceValue;
    private bool isRollingCompleted;

    public void Roll()
    {
        // Example roll logic, update according to your implementation
        diceValue = Random.Range(1, 7); // Roll a dice value between 1 and 6
        UpdateDiceSprite(diceValue);
        isRollingCompleted = true; // Set to true after roll is complete
    }

    public int GetDiceValue()
    {
        return diceValue;
    }

    public bool IsRollingCompleted => isRollingCompleted;

    private void UpdateDiceSprite(int value)
    {
        if (spriteRenderer != null && value >= 1 && value <= diceFaces.Length)
        {
            spriteRenderer.sprite = diceFaces[value - 1];
        }
        else
        {
            Debug.LogError("Dice value out of range or spriteRenderer not assigned.");
        }
    }
}
