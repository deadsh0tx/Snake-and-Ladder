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
    public Button rollButton; // Add this for Roll button reference

    private Player currentPlayer;
    private bool isRolling;

    void Awake()
    {
        // Ensure there is only one instance of GameManager
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
        rollButton.gameObject.SetActive(true); // Ensure the roll button is active
        turnText.gameObject.SetActive(true); // Ensure the turn text is active
    }

    void Update()
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

    public void OnRollButtonClicked()
    {
        if (!isRolling)
        {
            isRolling = true;
            dice.Roll();
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

            if (currentPlayer.HasWon())
            {
                EndGame(currentPlayer);
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
    }

    public void EndGame(Player winningPlayer)
    {
        statusText.text = winningPlayer.PlayerName + " wins!";
        rollButton.gameObject.SetActive(false); // Hide the Roll button
        turnText.gameObject.SetActive(false); // Hide the turn text
        // Optionally: Trigger any additional win logic here
    }
}
