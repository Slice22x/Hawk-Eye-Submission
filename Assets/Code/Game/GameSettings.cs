using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class GameSettings : MonoBehaviour
{
    public static GameSettings Instance;
    [SerializeField] private int minRemainingCards;

    public List<string> PlayerNames;
    public int StartingCards;
    public int GainCardsAfterPlacing;
    
    private const int PLAYABLE_CARDS = 56;
    
    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    public void UpdateStartingCards(int amount)
    {
        StartingCards = amount;
    }

    public bool WarnStartingCards()
    {
        return PLAYABLE_CARDS - (StartingCards * PlayerNames.Count) < minRemainingCards;
    }
    
    public int AddPlayer(string name)
    {
        for(int i = 0; i < PlayerNames.Count; i++)
        {
            if(PlayerNames[i] == name)
            {
                return i;
            }

            if (string.IsNullOrEmpty(PlayerNames[i]))
            {
                PlayerNames[i] = name;
                return i;
            }
        }
        
        PlayerNames.Add(name);
        return PlayerNames.Count;
    }
}
