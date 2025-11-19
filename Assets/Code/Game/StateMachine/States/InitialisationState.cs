using System.Collections.Generic;
using UnityEngine;

public class InitialisationState : GameState
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public InitialisationState(GameStateManager.GameState key, GameContext gameContext) : base(key, gameContext)
    {
        GameContext = gameContext;
    }
    
    public override void EnterState()
    {
        GameContext.Manager.PlayerManager.SpawnPlayers();
        GameContext.Manager.CardManager.SpawnCards();
        GameContext.Manager.CardManager.Shuffle();
        GameContext.Manager.CardManager.DistributeCards(GameContext.Players, 5);
    }

    public override void UpdateState()
    {
        
    }

    public override void ExitState()
    {
        GameContext.NextPlayerIndex = Random.Range(0, GameContext.Players.Count);
        Card firstCard = GameContext.Manager.CardManager.PopCard();

        int iterations = 0;
        
        while (firstCard.suit == CardInfo.CardSuit.Jokers && iterations < 10)
        {
            GameContext.Manager.CardManager.AddCardToStack(firstCard);
            firstCard = GameContext.Manager.CardManager.PopCard();
            iterations++;
        }
        
        if(iterations >= 10) Debug.LogError("Failed to find a non-joker card");
        
        firstCard.BelongsTo = GameContext.Players[GameContext.NextPlayerIndex];
        firstCard.ForceShow = true;
        firstCard.Played = true;
        GameContext.Manager.CardManager.PlaceCards(new List<Card> { firstCard }, true);
        GameContext.Manager.CanCallOut = false;
        
        GameContext.PreviousState = GameStateManager.GameState.Initialising;
    }

    public override GameStateManager.GameState GetNextState(GameStateManager.GameState lastState)
    {
        return GameStateManager.GameState.ChangePlayer;
    }
}
