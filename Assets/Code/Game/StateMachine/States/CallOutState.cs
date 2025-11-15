using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CallOutState : GameState
{
    private bool _processed;
    
    private bool _processedCallOut;
    private bool _calledOutPositive;
    private List<Card> _lastPlayedCards;
    private bool _containsJoker;
    private bool _animating;

    private float _timer;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public CallOutState(GameStateManager.GameState key, GameContext gameContext) : base(key, gameContext)
    {
        GameContext = gameContext;
    }
    
    public override void EnterState()
    {
        _processed = false;
        _processedCallOut = false;
        _timer = GameContext.RevealTimer;
    }

    public override void UpdateState()
    {
        ProcessCallOut();
        AnimateCards();
        
        _timer -= !_animating ? Time.deltaTime : 0f;
        if (_timer <= 0)
        {
            _processed = true;
        }
        
        Debug.Log("Timer: " + _timer);
        Debug.Log("Animating: " + _animating);
        Debug.Log("Processed: " + _processed);
        Debug.Log("Processed Call Out: " + _processedCallOut);
    }

    public override void ExitState()
    {
        if(_containsJoker) return;
        
        int giveToPlayerIndex = _calledOutPositive ? GameContext.LastPlayerIndex : GameContext.CurrentPlayerIndex;
            
        //Fix Error with negative index
        giveToPlayerIndex = giveToPlayerIndex < 0 ? GameContext.Players.Count - 1 : giveToPlayerIndex;
            
        GameContext.Manager.CardManager.GiveStack(GameContext.Players[giveToPlayerIndex]);
        GameContext.Manager.CardManager.GiveCards(GameContext.Players[giveToPlayerIndex], _lastPlayedCards.ToList());

        GameContext.Manager.CanCallOut = false;
        GameContext.Manager.JustCalledOut = true;
        
        _lastPlayedCards = null;
        _calledOutPositive = false;
        _animating = true;
    }

    public override GameStateManager.GameState GetNextState(GameStateManager.GameState lastState)
    {
        if(_containsJoker) return GameStateManager.GameState.Joker;
        
        return _processed ? GameStateManager.GameState.ChangePlayer : GameStateManager.GameState.CallOut;
    }
    
    private void AnimateCards()
    {
        GameContext.Manager.CardManager.MaxCardRevealed = _lastPlayedCards.Count;
        
        _animating = false;
        
        CheckAnimating();
        
        for (int i = 0; i < _lastPlayedCards.Count; i++)
        {
            _lastPlayedCards[i].RevealCard = true;
            _lastPlayedCards[i].RevealIndex = i;
        }
    }

    private void CheckAnimating()
    {
        foreach (var card in _lastPlayedCards)
        {
            if (!card.RevealCompleted)
            {
                _animating = true;
                break;
            }
        }
    }
    
    private void ProcessCallOut()
    {
        if(_processedCallOut) return;
        
        _lastPlayedCards = GameContext.Manager.CardManager.PopLastPlayedCards().ToList();

        if (_lastPlayedCards.Any(card => card.suit == CardInfo.CardSuit.Jokers))
        {
            _containsJoker = true;
            GameContext.JokerActive = _lastPlayedCards.Find(card => card.suit == CardInfo.CardSuit.Jokers).rank;
            _processedCallOut = true;
        }

        if (_containsJoker) return;
        
        _calledOutPositive = false;
        CardInfo.CardRank lastRank = GameContext.Manager.CardManager.LastRank();
                
        foreach (Card card in _lastPlayedCards)
        {
            if (card.rank != lastRank)
            {
                _calledOutPositive = true;
                break;
            }
        }
        
        _processedCallOut = true;
    }
}
