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
        GameContext.Manager.SpawnPlayers();
        GameContext.Manager.SpawnCards();
        GameContext.Manager.Shuffle();
        GameContext.Manager.DistributeCards(GameContext.Players, 5);
    }

    public override void UpdateState()
    {
        
    }

    public override void ExitState()
    {
        GameContext.NextPlayerIndex = Random.Range(0, GameContext.Players.Count);
        Card firstCard = GameContext.Manager.PopCard();
        firstCard.BelongsTo = GameContext.Players[GameContext.NextPlayerIndex];
        firstCard.ForceShow = true;
        firstCard.Played = true;
        GameContext.Manager.PlaceCards(new List<Card> { firstCard }, true);
        GameContext.Manager.CanCallOut = false;
    }

    public override GameStateManager.GameState GetNextState(GameStateManager.GameState lastState)
    {
        return GameStateManager.GameState.ChangePlayer;
    }
}
