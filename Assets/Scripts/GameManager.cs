using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [SerializeField] private List<GameObject> players = new List<GameObject>();
    

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void GetPlayers()
    {
        this.players.Clear();
        var players = FindObjectsByType<UnityEngine.InputSystem.PlayerInput>(FindObjectsSortMode.None);
        foreach (var player in players)
        {
            this.players.Add(player.gameObject);
        }
    }

    public void RemovePlayer(GameObject player)
    {
        players.Remove(player);
        if (players.Count == 1)
        {
            GameObject winner = players[0];
            winner.GetComponent<PlayerMovement>().enabled = false;
            
            // Hacer el ganador un objeto raíz antes de DontDestroyOnLoad
            winner.transform.SetParent(null);
            DontDestroyOnLoad(winner);
            
            winner.transform.position = new Vector3(0.5f, -1.85f, 0);
            winner.transform.localScale = new Vector3(7, 7, 1);
            winner.transform.GetChild(0).GetChild(0).gameObject.SetActive(false); // Desactivar luz
            Destroy(Guard.Instance.gameObject);
            SceneManager.LoadScene("GameOverScene");
        }
    }

    public void ResetPlayers()
    {
        foreach (var player in players)
        {
            Destroy(player);
        }
        players.Clear();
    }

    public string GetWinnersName() {
        if (players.Count == 0) return "¡Nadie gana!";

        return "¡" + players[0].name + " gana!";
    }
}
