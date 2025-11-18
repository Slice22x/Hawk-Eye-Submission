using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private Player playerPrefab;
    
    public int AmountOfPlayers;
    public List<Player> Players;
    private int _currentPlayerIndex;
    public Player CurrentPlayer => Players[_currentPlayerIndex];
    public int CurrentPlayerIndex => _currentPlayerIndex;
    
    private Transform _mat;
    private GameManager _manager;

    private void Awake()
    {
        _manager = GameManager.Instance;
        _mat = _manager.Mat;
    }

    public void SpawnPlayers()
    {
        for (int i = 0; i < AmountOfPlayers; i++)
        {
            Player newPlayer = Instantiate(playerPrefab, null);
            
            newPlayer.transform.position = _manager.Mat.position + new Vector3(
                Mathf.Cos(i * (2 * Mathf.PI / AmountOfPlayers)) * _manager.RadiusFromMat, 
                0, 
                Mathf.Sin(i * (2 * Mathf.PI / AmountOfPlayers)) * _manager.RadiusFromMat);
            
            newPlayer.transform.LookAt(_mat);
            
            newPlayer.transform.eulerAngles = new Vector3(0, newPlayer.transform.eulerAngles.y, 0);
            newPlayer.playerIndex = i;
            
            Players.Add(newPlayer);
        }
        
        _currentPlayerIndex = Random.Range(0, AmountOfPlayers);
        GameManager.OnChangePlayerCamera?.Invoke(Players[_currentPlayerIndex]);
    }

    public List<Player> GetPlayersExcluding(Player exclude)
    {
        var players = Players.Where(player => player != exclude).ToList();
        return players;
    }
    
    public void UpdateCurrentPlayerIndex(int index)
    {
        _currentPlayerIndex = index;
    }
}
