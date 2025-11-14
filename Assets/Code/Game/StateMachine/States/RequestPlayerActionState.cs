using System;
using System.Linq;
using UnityEngine;

public class RequestPlayerActionState : GameState
{
    private bool _processed;
    private bool _calledOutPositive;
    private bool _calledOut;
    private Card[] _lastPlayedCards;
    public RequestPlayerActionState(GameStateManager.GameState key, GameContext gameContext) : base(key, gameContext)
    {
        GameContext = gameContext;
    }

    public override void EnterState()
    {
        _processed = false;
    }

    public override void UpdateState()
    {
        if(GameContext.PlayerRequestDataBuffer == null) return;
        
        switch (GameContext.PlayerRequestDataBuffer.actionType)
        {
            case PlayerActionType.Place:

                if (GameContext.Manager.JustCalledOut)
                {
                    foreach (Card card in GameContext.PlayerRequestDataBuffer.sentCards)
                    {
                        card.ForceShow = true;
                    }
                }
                
                GameContext.Manager.PlaceCards(GameContext.PlayerRequestDataBuffer.sentCards);
                GameContext.Manager.JustCalledOut = false;
                break;
            case PlayerActionType.CallOut:
                ProcessCallOut();
                break;
            case PlayerActionType.PickUp:
                Card poppedCard = GameContext.Manager.PopCard();
                poppedCard.gameObject.SetActive(true);
                GameContext.Players[GameContext.PlayerRequestDataBuffer.playerIndex].AddCardToHand(poppedCard);
                poppedCard.AssignCardToPlayer(GameContext.Players[GameContext.PlayerRequestDataBuffer.playerIndex]);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        _processed = true;
    }

    public override void ExitState()
    {
        GameContext.PlayerRequestDataBuffer = null;
        
        if(_calledOut)
        {
            int giveToPlayerIndex = _calledOutPositive ? GameContext.LastPlayerIndex : GameContext.CurrentPlayerIndex;
            
            //Fix Error with negative index
            giveToPlayerIndex = giveToPlayerIndex < 0 ? GameContext.Players.Count - 1 : giveToPlayerIndex;
            
            GameContext.Manager.GiveStack(GameContext.Players[giveToPlayerIndex]);
            GameContext.Manager.GiveCards(GameContext.Players[giveToPlayerIndex], _lastPlayedCards.ToList());

            GameContext.Manager.JustCalledOut = true;
        }

        _lastPlayedCards = null;
        _calledOut = false;
        _calledOutPositive = false;
    }
    
    public override GameStateManager.GameState GetNextState(GameStateManager.GameState lastState)
    {
        return _processed ? GameStateManager.GameState.ChangePlayer : GameStateManager.GameState.RequestPlayerAction;
    }

    private void ProcessCallOut()
    {
        _calledOut = true;
        _calledOutPositive = false;
        _lastPlayedCards = GameContext.Manager.PopLastPlayedCards();
        CardInfo.CardRank lastRank = GameContext.Manager.LastRank();
                
        foreach (Card card in _lastPlayedCards)
        {
            if (card.rank != lastRank)
            {
                _calledOutPositive = true;
                break;
            }
        }
    }
}
