using UnityEngine;

public class PlaceCardState : GameState
{
    public PlaceCardState(GameStateManager.GameState key, GameContext gameContext) : base(key, gameContext)
    {
        GameContext = gameContext;
    }
    
    public override void EnterState()
    {
        throw new System.NotImplementedException();
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
        throw new System.NotImplementedException();
    }
}
