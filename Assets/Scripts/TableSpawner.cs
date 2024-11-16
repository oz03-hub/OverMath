using UnityEngine;

public class TableSpawner : MonoBehaviour
{
    public GameObject tablePrefab;
    public int rows = 2;
    public int columns = 4;
    public float spacing = 2.0f;
    public Transform seatingPlane;
    public Vector3 tableScale = new Vector3(1.0f, 1.0f, 1.0f);

    void Start()
    {
        if (tablePrefab == null || seatingPlane == null) return;
        SpawnTablesInGrid();
    }

    void SpawnTablesInGrid()
    {
        float gridWidth = (columns - 1) * spacing;
        float gridHeight = (rows - 1) * spacing;
        Vector3 offset = new Vector3(-gridWidth / 2, 0, -gridHeight / 2);

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                Vector3 localPosition = offset + new Vector3(col * spacing, 0, row * spacing);
                GameObject table = Instantiate(tablePrefab, seatingPlane);
                table.transform.localPosition = localPosition;
                table.transform.localRotation = Quaternion.identity;
                table.transform.localScale = tableScale;
            }
        }
    }
}
