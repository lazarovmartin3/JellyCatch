using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameplayUI_main : MonoBehaviour
{
    public static GameplayUI_main Instance;

    [SerializeField] private TextMeshProUGUI scoreTxt;
    [SerializeField] private TextMeshProUGUI titleTxt;
    [SerializeField] private Button playBtn;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        UpdateScore(0);
        playBtn.onClick.AddListener(StartGame);
    }

    private void StartGame()
    {
        GameManager.Instance.StarGame();
        playBtn.gameObject.SetActive(false);
        titleTxt.gameObject.SetActive(false);
        scoreTxt.transform.parent.gameObject.SetActive(true);
    }

    public void UpdateScore(int score)
    {
        scoreTxt.text = score.ToString();
    }
}
