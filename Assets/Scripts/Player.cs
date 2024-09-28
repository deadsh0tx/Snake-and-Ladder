using UnityEngine;

public class Player : MonoBehaviour
{
    public SpriteRenderer playerSprite;
    public Vector3[] cellPositions;

    private int currentCellIndex;
    private bool isOutsideBoard = true;
    private bool isMoving = false;
    private int targetCellIndex;
    private float moveSpeed = 5f; // Speed of the player movement
    private float moveStartTime;
    private float moveDuration;
    private int cellsToMove;

    public bool IsOutsideBoard => isOutsideBoard;
    public string PlayerName;

    // Define ladders and snakes using game board numbers
    private readonly int[] ladderEntries = { 5, 9, 22, 28, 44, 53, 66, 71, 85 };
    private readonly int[] ladderEnds = { 27, 51, 60, 54, 79, 69, 88, 92, 97 };
    private readonly int[] snakeHeads = { 13, 37, 80, 86, 91, 99 };
    private readonly int[] snakeTails = { 7, 19, 43, 46, 49, 4 };

    void Update()
    {
        if (isMoving)
        {
            MovePlayer();
        }
    }

    public void EnterBoard()
    {
        if (playerSprite != null)
        {
            playerSprite.transform.position = cellPositions[0];
            currentCellIndex = 0;
            isOutsideBoard = false;
        }
        else
        {
            Debug.LogError("playerSprite is not assigned.");
        }
    }

    public void Move(int diceValue)
    {
        if (!isOutsideBoard)
        {
            int startCellIndex = currentCellIndex;
            int targetIndex = currentCellIndex + diceValue;

            // Handle case where the target cell index is beyond the board
            if (targetIndex >= cellPositions.Length)
            {
                // Don't move if the dice roll goes beyond the final cell
                targetIndex = currentCellIndex;
            }
            else
            {
                // Check for ladders
                if (System.Array.Exists(ladderEntries, entry => entry == targetIndex + 1))
                {
                    int ladderIndex = System.Array.IndexOf(ladderEntries, targetIndex + 1);
                    targetIndex = ladderEnds[ladderIndex] - 1;
                }

                // Check for snakes
                if (System.Array.Exists(snakeHeads, head => head == targetIndex + 1))
                {
                    int snakeIndex = System.Array.IndexOf(snakeHeads, targetIndex + 1);
                    targetIndex = snakeTails[snakeIndex] - 1;
                }

                // If targetIndex is at or beyond the final cell
                if (targetIndex >= cellPositions.Length - 1)
                {
                    targetIndex = cellPositions.Length - 1;
                }
            }

            // Check if the player can move to the targetIndex
            if (currentCellIndex != targetIndex)
            {
                cellsToMove = Mathf.Abs(targetIndex - currentCellIndex);
                StartMoveToCells(startCellIndex, targetIndex);
            }
        }
    }

    private void StartMoveToCells(int startCellIndex, int targetCellIndex)
    {
        this.targetCellIndex = targetCellIndex;
        moveStartTime = Time.time;
        moveDuration = Vector3.Distance(cellPositions[startCellIndex], cellPositions[targetCellIndex]) / moveSpeed;
        isMoving = true;
    }

    private void MovePlayer()
    {
        if (cellsToMove > 0)
        {
            float elapsedTime = (Time.time - moveStartTime) / moveDuration;
            int cellIndex = Mathf.RoundToInt(Mathf.Lerp(currentCellIndex, targetCellIndex, elapsedTime));
            cellIndex = Mathf.Clamp(cellIndex, currentCellIndex, targetCellIndex);
            playerSprite.transform.position = cellPositions[cellIndex];

            if (elapsedTime >= 1f)
            {
                currentCellIndex = targetCellIndex;
                isMoving = false;
                cellsToMove--;

                // Check win condition immediately
                if (currentCellIndex == cellPositions.Length - 1)
                {
                    Debug.Log($"{PlayerName} wins!");
                    // Call the GameManager's EndGame method to end the game
                    GameManager.Instance.EndGame(this);
                }
                else if (cellsToMove > 0)
                {
                    // Move to the next cell if there are remaining cells to move
                    StartMoveToCells(currentCellIndex, targetCellIndex);
                }
            }
        }
    }

    public bool HasWon()
    {
        return currentCellIndex == cellPositions.Length - 1;
    }

    public void SetCellPositions(Vector3[] positions)
    {
        cellPositions = positions;
    }
}
