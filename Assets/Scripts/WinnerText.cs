using UnityEngine;
using UnityEngine.SceneManagement;

public class WinnerText : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI winnerText;
    [SerializeField] private TMPro.TextMeshProUGUI winnerTextShadow;

    void Start()
    {
        var winner = GameManager.Instance.GetWinnersName();
        winnerText.text = winner;
        winnerTextShadow.text = winner;
        Invoke("ChangeToLobby", 5);
    }

    private void ChangeToLobby()
    {
        SceneManager.LoadScene("Lobby");
    }
}
