using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private int score = 0;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
    }

    public void StarGame()
    {
        Grid.Instance.CreateGrid();
        //Fill the grid with Jelly Objects
        for (int x = 0; x < Grid.Instance.sizeX; x++)
        {
            for (int y = 0; y < Grid.Instance.sizeY; y++)
            {
                Grid.Instance.AddJelly(x, y, ObjectPool.Instance.GetRandomJelly(), false);
            }
        }
    }

    public void AddScore(int score)
    {
        this.score += score;
        GameplayUI_main.Instance.UpdateScore(this.score);
    }
}
