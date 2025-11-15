using UnityEngine;

public class JokerState : GameState
{
    private bool _processed;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public JokerState(GameStateManager.GameState key, GameContext gameContext) : base(key, gameContext)
    {
        GameContext = gameContext;
    }

    public override void EnterState()
    {
        _processed = false;
        if (GameContext.PlayerRequestDataBuffer.actionType == PlayerActionType.Place)
        {
            JokerData data = new JokerData(GameContext.GetPlacedJoker(),
                GameContext.Manager.PlayerManager.Players[GameContext.PlayerRequestDataBuffer.playerIndex],
                false, 0);
        
            GameContext.Manager.CardManager.PlayedJokers.Add(data);
            _processed = true;
        }
    }

    public override void UpdateState()
    {

    }

    public override void ExitState()
    {

    }

    public override GameStateManager.GameState GetNextState(GameStateManager.GameState lastState)
    {
        return _processed ? GameStateManager.GameState.ChangePlayer : GameStateManager.GameState.Joker;
    }
}
