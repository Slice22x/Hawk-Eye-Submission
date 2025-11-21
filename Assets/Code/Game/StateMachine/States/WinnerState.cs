using System.Collections.Generic;
using UnityEngine;

public class WinnerState : GameState
{
    private bool _winnerDecided;
    private bool _processed;
    public WinnerState(GameStateManager.GameState key, GameContext gameContext) : base(key, gameContext)
    {
        GameContext = gameContext;
    }
    
    public override void EnterState()
    {
        if (GameContext.Manager.CardManager.CardStack.Count > 0)
        {
            var cards = GameContext.Manager.CardManager.PopCards(3);
            GameContext.Manager.CardManager.GiveCards(GameContext.Players[GameContext.CurrentPlayerIndex], cards);
            _winnerDecided = false;
            _processed = true;
        }
        else
        {
            _winnerDecided = true;
            _processed = false;
        }
    }

    public override void UpdateState()
    {
        throw new System.NotImplementedException();
    }

    public override void ExitState()
    {
        throw new System.NotImplementedException();
    }

    public override GameStateManager.GameState GetNextState(GameStateManager.GameState lastState)
    {
        return _processed ? GameStateManager.GameState.ChangePlayer : GameStateManager.GameState.Winner;
    }
}
