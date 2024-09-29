using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Grid : MonoBehaviour
{
    public static Grid Instance;

    [SerializeField] private GameObject gridBackgroundPrefab;
    public int sizeX = 4, sizeY = 5;
    private GameObject[,] grid;
    private float offset = 1.1f;
    private bool isGridReady = false;


    private void Awake()
    {
        Instance = this;
    }


    public void CreateGrid()
    {
        grid = new GameObject[sizeX, sizeY];

        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                GameObject tile = Instantiate(gridBackgroundPrefab, new Vector3(transform.position.x + x * offset, transform.position.y - y * offset, 0), Quaternion.identity);
                tile.name = x + "_" + y;
                tile.transform.parent = transform;
                grid[x, y] = tile;
            }
        }
        isGridReady = true;
    }

    public void AddJelly(int x, int y, GameObject jelly, bool playSpawn)
    {
        if (grid[x, y].transform.childCount > 0)
        {
            Destroy(grid[x, y].transform.GetChild(0).gameObject); // Destroy the existing object if present
        }

        GameObject newJelly = Instantiate(jelly);
        newJelly.transform.parent = grid[x, y].transform;
        newJelly.transform.position = new Vector3(grid[x, y].transform.position.x, grid[x, y].transform.position.y - 0.38f, 0);

        newJelly.GetComponent<Jelly>().gridPos = new Vector2(x, y);
        if(playSpawn)
            newJelly.GetComponent<Jelly>().TriggerAnimation("Spawn");
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && isGridReady)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

            // Check if the ray hits any collider in the scene
            if (hit.collider != null)
            {
                // Check if the hit object is a grid cell
                GameObject hitObject = hit.collider.gameObject;
                if (hitObject.CompareTag("Jelly"))
                {
                    Vector2 gridPos = hitObject.GetComponent<Jelly>().gridPos;
                    // Get matching neighbors
                    List<GameObject> matchingNeighbors = GetMatchingJellies((int)gridPos.x, (int)gridPos.y, hitObject.GetComponent<Jelly>().GetID());

                    if (matchingNeighbors.Count >= 3)
                    {
                        // Do something with the matching neighbors
                        foreach (GameObject neighbor in matchingNeighbors)
                        {
                            // Do something with the neighbor
                            neighbor.GetComponent<Jelly>().Collided();
                        }
                        int score = 10 * matchingNeighbors.Count;

                        GameManager.Instance.AddScore(score);

                        StartCoroutine(Wait(1.5f, ReorderGrid));

                        // Check for combos
                        //CheckForCombo((int)gridPos.x, (int)gridPos.y, hitObject.GetComponent<Jelly>().GetID());
                        //AutoClear();
                    }
                }
            }
            bool hasMatch = CheckForMatches();
            if (!hasMatch)
            {
                print("GAME OVER");
            }
        }
    }


    public List<GameObject> GetMatchingJellies(int x, int y, int id)
    {
        List<GameObject> matchingJellies = new List<GameObject>();
        GameObject targetJelly = grid[x, y].transform.GetChild(0).gameObject;
        matchingJellies.Add(targetJelly);

        // Check horizontal
        int horizontalCount = 1;
        for (int i = x - 1; i >= 0 && grid[i, y].transform.childCount > 0 && grid[i, y].transform.GetChild(0).GetComponent<Jelly>().id == id; i--)
        {
            print("horizontal check " + grid[i, y].name + " target id " + id + " compare with " + grid[i, y].transform.GetChild(0).GetComponent<Jelly>().id);
            horizontalCount++;
            matchingJellies.Add(grid[i, y].transform.GetChild(0).gameObject);
        }
        for (int i = x + 1; i < sizeX && grid[i, y].transform.childCount > 0 && grid[i, y].transform.GetChild(0).GetComponent<Jelly>().id == id; i++)
        {
            print("horizontal check 2 " + grid[i, y].name + " target id " + id + " compare with " + grid[i, y].transform.GetChild(0).GetComponent<Jelly>().id);
            horizontalCount++;
            matchingJellies.Add(grid[i, y].transform.GetChild(0).gameObject);
        }

        // Check vertical
        int verticalCount = 1;

        // Check up
        for (int j = y - 1; j >= 0 && grid[x, j].transform.childCount > 0 && grid[x, j].transform.GetChild(0).GetComponent<Jelly>().id == id; j--)
        {
            print("vertical check up " + grid[j, y].name + " target id " + id + " compare with " + grid[j, y].transform.GetChild(0).GetComponent<Jelly>().id);
            verticalCount++;
            matchingJellies.Add(grid[x, j].transform.GetChild(0).gameObject);
        }

        // Check down
        for (int j = y + 1; j < sizeY && grid[x, j].transform.childCount > 0 && grid[x, j].transform.GetChild(0).GetComponent<Jelly>().id == id; j++)
        {
            print("vertical check down " + grid[x, y].name + " target id " + id + " compare with " + grid[x, y].transform.GetChild(0).GetComponent<Jelly>().id);
            verticalCount++;
            matchingJellies.Add(grid[x, j].transform.GetChild(0).gameObject);
        }

        print("vertical list " + verticalCount + " horizontal list " + horizontalCount + " list " + matchingJellies.Count);
        // If neither horizontal nor vertical counts are enough, clear the list
        if (horizontalCount < 3 && verticalCount < 3)
        {
            matchingJellies.Clear();
        }
        print("return list " + matchingJellies.Count);

        return matchingJellies;
    }

    public void RefillGrid()
    {
        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                if (grid[x, y].transform.childCount == 0)
                {
                    AddJelly(x, y, ObjectPool.Instance.GetRandomJelly(), true);
                }
            }
        }
    }

     public void ReorderGrid()
     {
         int[] emptySpaces = new int[sizeX];

         // Count empty spaces in each column
         for (int x = 0; x < sizeX; x++)
         {
             for (int y = 0; y < sizeY; y++)
             {
                 if (grid[x, y].transform.childCount == 0)
                 {
                     emptySpaces[x]++;
                 }
             }
         }

         // Shift jellies down
         for (int x = 0; x < sizeX; x++)
         {
             // If the column has empty spaces
             if (emptySpaces[x] > 0)
             {
                 for (int y = sizeY - 1; y >= 0; y--)
                 {
                     if (grid[x, y].transform.childCount > 0)
                     {
                         // Move the jelly to the bottom of the column
                         int newY = y + emptySpaces[x]; // Update the new y position based on empty spaces
                         newY = Mathf.Max(newY, 0); // Ensure newY is at least 0 to avoid negative values
                         newY = Mathf.Min(newY, sizeY - 1); // Ensure newY is within the grid bounds
                         if (grid[x, newY].transform.childCount == 0)
                         {
                             GameObject jellyObject = grid[x, y].transform.GetChild(0).gameObject;
                             jellyObject.GetComponent<Jelly>().gridPos = new Vector2(x, newY);
                             jellyObject.transform.DOMove(new Vector3(jellyObject.transform.position.x, grid[x, newY].transform.position.y - 0.38f, 0), 0.5f).SetEase(Ease.OutBounce);
                             jellyObject.transform.SetParent(grid[x, newY].transform);
                         }
                     }
                 }
             }
         }
         RefillGrid();
     }

    /*private void ReorderGrid()
    {
        // Shift jellies down
        for (int x = 0; x < sizeX; x++)
        {
            int emptySpaces = 0; // Counter for empty spaces
            for (int y = 0; y < sizeY; y++)
            {
                if (grid[x, y].transform.childCount == 0)
                {
                    emptySpaces++; // Increment empty space counter
                }
                else if (emptySpaces > 0)
                {
                    // Move jelly down by the number of empty spaces
                    GameObject jellyObject = grid[x, y].transform.GetChild(0).gameObject;
                    int newY = y - emptySpaces; // Calculate new y position
                    jellyObject.GetComponent<Jelly>().gridPos = new Vector2(x, newY);
                    jellyObject.transform.DOMove(new Vector3(jellyObject.transform.position.x, grid[x, newY].transform.position.y - 0.38f, 0), 0.5f).SetEase(Ease.OutBounce);
                    jellyObject.transform.SetParent(grid[x, newY].transform);
                }
            }
        }

        // Refill any remaining empty spaces
        RefillGrid();
    }*/



    private void CheckForCombo(int x, int y, int id)
    {
        HashSet<GameObject> visited = new HashSet<GameObject>(); // Keep track of visited Jellies to avoid duplicates
        List<GameObject> queue = new List<GameObject>(); // Queue to store Jellies to be checked

        queue.Add(grid[x, y].transform.GetChild(0).gameObject); // Add the initial Jelly to the queue

        while (queue.Count > 0)
        {
            GameObject jelly = queue[0];
            queue.RemoveAt(0);

            if (visited.Contains(jelly)) continue; // Skip if the Jelly is already visited
            visited.Add(jelly);

            List<GameObject> matchingJellies = GetMatchingJellies((int)jelly.GetComponent<Jelly>().gridPos.x, (int)jelly.GetComponent<Jelly>().gridPos.y, id);

            foreach (GameObject neighbor in matchingJellies)
            {
                if (!visited.Contains(neighbor))
                {
                    queue.Add(neighbor);
                }
            }
        }

        // If the number of visited Jellies is greater than or equal to 3, remove them
        if (visited.Count >= 3)
        {
            print("Will reorder");
            foreach (GameObject jelly in visited)
            {
                jelly.GetComponent<Jelly>().Collided();
            }
            int score = 10 * visited.Count;
            GameManager.Instance.AddScore(score);
            StartCoroutine(Wait(1.5f, ReorderGrid));
        }
    }

    private void AutoClear()
    {
        // Check horizontally
        for (int y = 0; y < sizeY; y++)
        {
            for (int x = 0; x < sizeX - 2; x++)
            {
                int id = grid[x, y].GetComponentInChildren<Jelly>().GetID();
                if (id != 0 && grid[x + 1, y].GetComponentInChildren<Jelly>().GetID() == id &&
                    grid[x + 2, y].GetComponentInChildren<Jelly>().GetID() == id)
                {
                    // Match found, mark jellies as matched
                    MarkAsMatched(x, y, 1, 0);
                }
            }
        }

        // Check vertically
        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY - 2; y++)
            {
                int id = grid[x, y].GetComponentInChildren<Jelly>().GetID();
                if (id != 0 && grid[x, y + 1].GetComponentInChildren<Jelly>().GetID() == id &&
                    grid[x, y + 2].GetComponentInChildren<Jelly>().GetID() == id)
                {
                    // Match found, mark jellies as matched
                    MarkAsMatched(x, y, 0, 1);
                }
            }
        }
    }

    private void MarkAsMatched(int startX, int startY, int dx, int dy)
    {
        int id = grid[startX, startY].GetComponentInChildren<Jelly>().GetID();
        int x = startX;
        int y = startY;
        List<GameObject> matchedJellies = new List<GameObject>();

        // Collect all matching jellies
        while (x < sizeX && y < sizeY &&
               grid[x, y].GetComponentInChildren<Jelly>().GetID() == id)
        {
            matchedJellies.Add(grid[x, y]);
            x += dx;
            y += dy;
        }

        // Destroy all matched jellies
        foreach (GameObject jelly in matchedJellies)
        {
            jelly.GetComponentInChildren<Jelly>().Collided();
        }

        // Increment score
        int score = 10 * matchedJellies.Count;
        GameManager.Instance.AddScore(score);

        // Reorder the grid after a delay
        StartCoroutine(Wait(1.5f, ReorderGrid));
    }

    private bool CheckForMatches()
    {
        // Check horizontally
        for (int y = 0; y < sizeY; y++)
        {
            for (int x = 0; x < sizeX - 2; x++)
            {
                int id = grid[x, y].GetComponentInChildren<Jelly>().GetID();
                if (id != 0 && grid[x + 1, y].GetComponentInChildren<Jelly>().GetID() == id &&
                    grid[x + 2, y].GetComponentInChildren<Jelly>().GetID() == id)
                {
                    // Horizontal match found
                    return true;
                }
            }
        }

        // Check vertically
        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY - 2; y++)
            {
                int id = grid[x, y].GetComponentInChildren<Jelly>().GetID();
                if (id != 0 && grid[x, y + 1].GetComponentInChildren<Jelly>().GetID() == id &&
                    grid[x, y + 2].GetComponentInChildren<Jelly>().GetID() == id)
                {
                    // Vertical match found
                    return true;
                }
            }
        }

        // No match found
        return false;
    }

    public List<GameObject> GetObjectsWithID(int id)
    {
        List<GameObject> objects = new List<GameObject>();
        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                if (grid[x, y].GetComponentInChildren<Jelly>().GetID() == id)
                {
                    objects.Add(grid[x, y].transform.GetChild(0).gameObject);
                }
            }
        }
        return objects;
    }

    private IEnumerator Wait(float time, Action action)
    {
        yield return new WaitForSeconds(time);

        action?.Invoke();

        yield return null;
    }
}
