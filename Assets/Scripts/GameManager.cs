using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public TextMeshProUGUI turnText;
    public TextMeshProUGUI statusText;
    public Dice dice;
    public Player player1;
    public Player player2;
    public Button rollButton; // Reference for Roll button
    public float disableRollDuration = 1f; // Duration to disable Roll button

    private Player currentPlayer;
    private bool isRolling;
    private bool gameEnded = false; // Track whether the game has ended

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Optional: keeps instance alive across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        InitializeGame();
    }

    void InitializeGame()
    {
        player1.SetCellPositions(BoardManager.Instance.GetCellPositions());
        player2.SetCellPositions(BoardManager.Instance.GetCellPositions());
        currentPlayer = player1;
        turnText.text = currentPlayer.PlayerName + "'s Turn";
        isRolling = false;
        gameEnded = false; // Reset the game end status at the start
        rollButton.gameObject.SetActive(true); // Ensure the roll button is active
        turnText.gameObject.SetActive(true); // Ensure the turn text is active
    }

    void Update()
    {
        if (!gameEnded) // Only allow updates if the game hasn't ended
        {
            if (isRolling)
            {
                HandleDiceRolling();
            }
            else
            {
                HandlePlayerMovement();
            }
        }
    }

    public void OnRollButtonClicked()
    {
        if (!isRolling && !gameEnded) // Prevent rolling if the game has ended
        {
            isRolling = true;
            dice.Roll();
            rollButton.interactable = false; // Disable the Roll button
            Invoke("EnableRollButton", disableRollDuration); // Re-enable after the specified duration
        }
    }

    private void HandleDiceRolling()
    {
        if (dice.IsRollingCompleted)
        {
            isRolling = false;
            int diceValue = dice.GetDiceValue();
            statusText.text = currentPlayer.PlayerName + " rolled a " + diceValue;

            if (currentPlayer.IsOutsideBoard)
            {
                if (diceValue == 6)
                {
                    currentPlayer.EnterBoard();
                }
                else
                {
                    SwitchPlayer();
                    return;
                }
            }
            else
            {
                currentPlayer.Move(diceValue);
            }

            // Check if the current player has won immediately after moving
            if (currentPlayer.HasWon())
            {
                EndGame(currentPlayer);
                return; // Stop further actions like switching players
            }
            else
            {
                SwitchPlayer();
            }
        }
    }

    private void HandlePlayerMovement()
    {
        // Handle any additional logic for player movement if needed
    }

    private void SwitchPlayer()
    {
        currentPlayer = (currentPlayer == player1) ? player2 : player1;
        turnText.text = currentPlayer.PlayerName + "'s Turn";

        // Automatically trigger computer's turn
        if (currentPlayer.playerType == PlayerType.Computer && !gameEnded)
        {
            Invoke("TriggerComputerMove", 1.5f); // Add slight delay for smoothness
        }
    }

    private void TriggerComputerMove()
    {
        OnRollButtonClicked(); // Automatically roll the dice for the computer player
    }

    public void EndGame(Player winningPlayer)
    {
        gameEnded = true; // Mark the game as ended
        statusText.text = winningPlayer.PlayerName + " wins!";
        rollButton.gameObject.SetActive(false); // Hide the Roll button
        turnText.gameObject.SetActive(false); // Hide the turn text
        // Optionally: Trigger any additional win logic here
    }

    private void EnableRollButton()
    {
        rollButton.interactable = true; // Re-enable the Roll button
    }
}
