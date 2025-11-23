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
        int startingCardAmount, TMP_Text currentPlayerText, TMP_Text calledOutByText, Image calledOutByPlayerImage)
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
        CurrentPlayerText = currentPlayerText;
        CalledOutByText = calledOutByText;
        CalledOutByPlayerImage = calledOutByPlayerImage;
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
    public readonly TMP_Text CurrentPlayerText;
    public readonly TMP_Text CalledOutByText;
    public readonly Image CalledOutByPlayerImage;
    
    public int NextPlayerIndex;
    public int CurrentPlayerIndex;
    public int LastPlayerIndex;
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
