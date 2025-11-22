using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameContext
{
    public GameContext(GameManager manager, List<Player> players, Image switchPlayerPrompt,
        TMP_Text switchPlayerPromptText, PlayerRequestData playerRequestData, CanvasGroup canvasGroup,
        float revealTimer, CanvasGroup winnerCanvasGroup, float alphaResponsiveness,
        Image winnerCardImage, TMP_Text winnerCardTextName, Button returnToMenuButton,
        int startingCardAmount)
    {
        Manager = manager;
        Players = players;
        SwitchPlayerPrompt = switchPlayerPrompt;
        NextPlayerName = switchPlayerPromptText;
        PlayerRequestDataBuffer = playerRequestData;
        CanvasGroup = canvasGroup;
        RevealTimer = revealTimer;
        WinnerCanvasGroup = winnerCanvasGroup;
        AlphaResponsiveness = alphaResponsiveness;
        WinnerCardImage = winnerCardImage;
        WinnerCardTextName = winnerCardTextName;
        ReturnToMenuButton = returnToMenuButton;
        StartingCardAmount = startingCardAmount;
    }

    public readonly GameManager Manager;
    public readonly List<Player> Players;
    public readonly Image SwitchPlayerPrompt;
    public readonly TMP_Text NextPlayerName;
    public PlayerRequestData PlayerRequestDataBuffer;
    public readonly CanvasGroup CanvasGroup;
    public readonly CanvasGroup WinnerCanvasGroup;
    public readonly Image WinnerCardImage;
    public readonly TMP_Text WinnerCardTextName;
    public readonly Button ReturnToMenuButton;
    
    public int NextPlayerIndex;
    public int CurrentPlayerIndex = -1;
    public int LastPlayerIndex = -1;
    public int StartingCardAmount;
    
    /// <summary>
    /// 1 for clockwise, -1 for counter-clockwise
    /// </summary>
    public int Direction = 1;

    public float AlphaResponsiveness;
    
    public readonly float RevealTimer;

    public CardInfo.CardRank JokerActive;
    public Card PlacedJoker;
    public List<Card> LastPlayedCardsBuffer;

    public GameStateManager.GameState PreviousState;
    
    public CardInfo.CardRank GetPlacedJoker()
    {
        if (PlayerRequestDataBuffer == null) return CardInfo.CardRank.None;
        
        return PlayerRequestDataBuffer.sentCards.Find(card => card.Suit == CardInfo.CardSuit.Jokers).Rank;
    }
}
