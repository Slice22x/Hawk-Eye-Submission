using System;
using System.Linq;
using UnityEngine;

public class RequestPlayerActionState : GameState
{
    private bool _processed;
    private bool _processedCallOut;
    private bool _calledOutPositive;
    private bool _calledOut;
    private Card[] _lastPlayedCards;
    private bool _animating;

    private float _timer;
    
    
    public RequestPlayerActionState(GameStateManager.GameState key, GameContext gameContext) : base(key, gameContext)
    {
        GameContext = gameContext;
    }

    public override void EnterState()
    {
        GameContext.PlayerRequestDataBuffer = null;
        CardManager.OnUpdateJokers?.Invoke();
    }

    public override void UpdateState()
    {

    }

    public override void ExitState()
    {
        GameContext.PreviousState = GameStateManager.GameState.RequestPlayerAction;
    }
    
    public override GameStateManager.GameState GetNextState(GameStateManager.GameState lastState)
    {
        if (GameContext.PlayerRequestDataBuffer == null)
        {
            return GameStateManager.GameState.RequestPlayerAction;
        }

        return GameContext.PlayerRequestDataBuffer.actionType switch
        {
            PlayerActionType.Place => GameStateManager.GameState.PlaceCard,
            PlayerActionType.CallOut => GameStateManager.GameState.CallOut,
            PlayerActionType.PickUp => GameStateManager.GameState.PickUp,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}
