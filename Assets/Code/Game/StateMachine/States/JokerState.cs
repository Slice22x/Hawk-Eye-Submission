using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class JokerState : GameState
{
    private bool _processed;
    private bool _callout;
    
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
            GameContext.Manager.CardManager.PlaceCards(GameContext.PlayerRequestDataBuffer.sentCards);
            _processed = true;
        }
        
        if (GameContext.PlayerRequestDataBuffer.actionType == PlayerActionType.CallOut)
        {
            JokerData data = GameContext.Manager.CardManager.PlayedJokers.Find(joker => joker.type == GameContext.JokerActive);

            switch (data.type)
            {
                case CardInfo.CardRank.Clown:
                    var cards = GameContext.Manager.CardManager.PlayedStack;
                    Player current = GameContext.Players[GameContext.CurrentPlayerIndex];
                    var players =
                        GameContext.Manager.PlayerManager.GetPlayersExcluding(current);
                    GameContext.Manager.CardManager.DistributeCards(cards, players, current);
                    break;
                case CardInfo.CardRank.Jester:
                    GameContext.Direction *= -1;
                    _callout = true;
                    break;
                case CardInfo.CardRank.Bail:
                default:
                    _callout = true;
                    break;
            }
            
        }
    }

    public override void UpdateState()
    {
        
    }

    public override void ExitState()
    {
        GameContext.PreviousState = GameStateManager.GameState.Joker;
    }
    
    public override GameStateManager.GameState GetNextState(GameStateManager.GameState lastState)
    {
        if (_callout) return GameStateManager.GameState.CallOut;
        
        return _processed ? GameStateManager.GameState.ChangePlayer : GameStateManager.GameState.Joker;
    }
}
