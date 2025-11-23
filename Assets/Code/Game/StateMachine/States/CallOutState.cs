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
    private bool _bailed;

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
        _containsJoker = false;
        _bailed = false;
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
    }

    public override void ExitState()
    {
        if(_containsJoker) return;
        
        int giveToPlayerIndex = _calledOutPositive ? GameContext.LastPlayerIndex : GameContext.CurrentPlayerIndex;
            
        //Fix Error with negative index
        giveToPlayerIndex = giveToPlayerIndex < 0 ? GameContext.Players.Count - 1 : giveToPlayerIndex;
        giveToPlayerIndex = giveToPlayerIndex > GameContext.Players.Count ? 0 : giveToPlayerIndex;
            
        if (!_bailed)
        {
            var giveCards = GameContext.PreviousState == GameStateManager.GameState.Joker ? new List<Card> { GameContext.PlacedJoker } : _lastPlayedCards;
        
            GameContext.Manager.CardManager.GiveStack(GameContext.Players[giveToPlayerIndex]);
            GameContext.Manager.CardManager.GiveCards(GameContext.Players[giveToPlayerIndex], giveCards);
            
            GameContext.Manager.CanCallOut = false;
            GameContext.Manager.JustCalledOut = true;
        }
        
        _lastPlayedCards = null;
        _calledOutPositive = false;
        _animating = true;

        GameContext.Players[GameContext.LastPlayerIndex].CalledOut = true;
        
        GameContext.PreviousState = GameStateManager.GameState.CallOut;
    }

    public override GameStateManager.GameState GetNextState(GameStateManager.GameState lastState)
    {
        if(_containsJoker && GameContext.PreviousState != GameStateManager.GameState.Joker && _processed) return GameStateManager.GameState.Joker;
        
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
        foreach (var card in _lastPlayedCards.Where(card => !card.RevealCompleted))
        {
            _animating = true;
            break;
        }
    }
    
    private void ProcessCallOut()
    {
        if(_processedCallOut) return;
        
        _lastPlayedCards = GameContext.PreviousState == GameStateManager.GameState.Joker ? GameContext.LastPlayedCardsBuffer : GameContext.Manager.CardManager.PopLastPlayedCards().ToList();

        if (_lastPlayedCards.Any(card => card.Suit == CardInfo.CardSuit.Jokers) && GameContext.PreviousState != GameStateManager.GameState.Joker)
        {
            _containsJoker = true;
            GameContext.LastPlayedCardsBuffer = _lastPlayedCards;
            GameContext.JokerActive = _lastPlayedCards.Find(card => card.Suit == CardInfo.CardSuit.Jokers).Rank;
            GameContext.PlacedJoker = _lastPlayedCards.Find(card => card.Suit == CardInfo.CardSuit.Jokers);
            _processedCallOut = true;
        }

        if (_containsJoker) return;
        
        _calledOutPositive = false;
        CardInfo.CardRank lastRank = GameContext.Manager.CardManager.LastRank();
                
        foreach (var card in _lastPlayedCards.Where(card => card.Rank != lastRank))
        {
            //Efficiently checks if the card is null
            if(!card) continue;
            
            if (card.Rank == CardInfo.CardRank.Bail)
            {
                _calledOutPositive = false;
                Object.Destroy(card.gameObject);
                _bailed = true;
                break;
            }
            if (card.Rank == CardInfo.CardRank.Spy)
            {
                _calledOutPositive = false;
                Object.Destroy(card.gameObject);
                break;
            }
            
            if(card.Suit == CardInfo.CardSuit.Jokers) continue;
            
            _calledOutPositive = true;
            break;
        }
        
        _processedCallOut = true;
    }
}
