using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameContext
{
    public GameContext(GameManager manager, List<Player> players, Image switchPlayerPrompt,
        TMP_Text switchPlayerPromptText, PlayerRequestData playerRequestData, CanvasGroup canvasGroup, float revealTimer)
    {
        Manager = manager;
        Players = players;
        SwitchPlayerPrompt = switchPlayerPrompt;
        NextPlayerName = switchPlayerPromptText;
        PlayerRequestDataBuffer = playerRequestData;
        CanvasGroup = canvasGroup;
        RevealTimer = revealTimer;
    }
    
    public GameManager Manager;
    public List<Player> Players;
    public Image SwitchPlayerPrompt;
    public TMP_Text NextPlayerName;
    public PlayerRequestData PlayerRequestDataBuffer;
    public CanvasGroup CanvasGroup;
    
    public int NextPlayerIndex;
    public int CurrentPlayerIndex = -1;
    public int LastPlayerIndex = -1;

    public float RevealTimer;

    public CardInfo.CardRank JokerActive;

    public CardInfo.CardRank GetPlacedJoker()
    {
        if (PlayerRequestDataBuffer == null) return CardInfo.CardRank.None;
        
        return PlayerRequestDataBuffer.sentCards.Find(card => card.suit == CardInfo.CardSuit.Jokers).rank;
    }
}
