using UnityEngine;

public class PlaceCardState : GameState
{
    private bool _processed;
    
    public PlaceCardState(GameStateManager.GameState key, GameContext gameContext) : base(key, gameContext)
    {
        GameContext = gameContext;
    }
    
    public override void EnterState()
    {
        _processed = false;
    }

    public override void UpdateState()
    {
        if (GameContext.Manager.JustCalledOut)
        {
            foreach (Card card in GameContext.PlayerRequestDataBuffer.sentCards)
            {
                card.ForceShow = true;
            }
        }
                
        GameContext.Manager.CardManager.PlaceCards(GameContext.PlayerRequestDataBuffer.sentCards);
        GameContext.Manager.JustCalledOut = false;
                
        _processed = true;
    }

    public override void ExitState()
    {
        
    }

    public override GameStateManager.GameState GetNextState(GameStateManager.GameState lastState)
    {
        if(GameContext.PlayerRequestDataBuffer.containsJoker) return GameStateManager.GameState.Joker;
        
        return _processed ? GameStateManager.GameState.ChangePlayer : GameStateManager.GameState.PlaceCard;
    }
}
