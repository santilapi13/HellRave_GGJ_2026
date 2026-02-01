using UnityEngine;

public class WinnerText : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI winnerText;

    void Start()
    {
        var winner = GameManager.Instance.GetWinnersName();
        winnerText.text = winner;
    }
}
