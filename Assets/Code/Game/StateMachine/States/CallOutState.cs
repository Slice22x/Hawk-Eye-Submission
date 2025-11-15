using System.Linq;
using UnityEngine;

public class CallOutState : GameState
{
    private bool _processed;
    
    private bool _processedCallOut;
    private bool _calledOutPositive;
    private bool _calledOut;
    private Card[] _lastPlayedCards;
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
    }

    public override void ExitState()
    {
        int giveToPlayerIndex = _calledOutPositive ? GameContext.LastPlayerIndex : GameContext.CurrentPlayerIndex;
            
        //Fix Error with negative index
        giveToPlayerIndex = giveToPlayerIndex < 0 ? GameContext.Players.Count - 1 : giveToPlayerIndex;
            
        GameContext.Manager.GiveStack(GameContext.Players[giveToPlayerIndex]);
        GameContext.Manager.GiveCards(GameContext.Players[giveToPlayerIndex], _lastPlayedCards.ToList());

        GameContext.Manager.JustCalledOut = true;
        
        _lastPlayedCards = null;
        _calledOut = false;
        _calledOutPositive = false;
        _animating = false;
    }

    public override GameStateManager.GameState GetNextState(GameStateManager.GameState lastState)
    {
        return _processed ? GameStateManager.GameState.ChangePlayer : GameStateManager.GameState.CallOut;
    }
    
    private void AnimateCards()
    {
        GameContext.Manager.MaxCardRevealed = _lastPlayedCards.Length;
        
        _animating = false;
        
        CheckAnimating();
        
        for (int i = 0; i < _lastPlayedCards.Length; i++)
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
        
        _processedCallOut = true;
    }
}
