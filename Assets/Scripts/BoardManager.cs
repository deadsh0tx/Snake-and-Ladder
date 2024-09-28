using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public static BoardManager Instance { get; private set; }

    public GameObject cellPrefab; // Reference to the Cell prefab
    public Transform boardParent; // Parent object to organize the cells in the hierarchy

    private int columns = 10;
    private int rows = 10;
    private float cellSize = 1.0f; // Adjust based on your cell size
    private Vector3[] cellPositions;

    void Awake()
    {
        // Ensure there is only one instance of BoardManager
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
        cellPositions = new Vector3[100];
        GenerateBoard();
    }

    void GenerateBoard()
    {
        int cellNumber = 1;

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                // Calculate the column index and row for zigzag pattern
                int rowIndex = i; // Reverse row order
                int columnIndex = (i % 2 == 0) ? j : (columns - 1 - j); // Zigzag pattern

                // Calculate the position for the current cell
                Vector3 cellPosition = new Vector3(columnIndex * cellSize, rowIndex * cellSize, 0);
                cellPositions[cellNumber - 1] = cellPosition;

                // Instantiate the cell at the calculated position
                GameObject newCell = Instantiate(cellPrefab, cellPosition, Quaternion.identity, boardParent);

                // Assign the cell number to the TextMesh component
                newCell.GetComponentInChildren<TextMesh>().text = cellNumber.ToString();

                // Increment the cell number
                cellNumber++;
            }
        }
    }

    public Vector3[] GetCellPositions()
    {
        return cellPositions;
    }
}
