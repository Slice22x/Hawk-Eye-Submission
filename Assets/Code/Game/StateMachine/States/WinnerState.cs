using System.Collections.Generic;
using UnityEngine;

public class WinnerState : GameState
{
    private bool _winnerDecided;
    private bool _processed;

    private float _timer = 0.75f;
    private Shake _nameShake;
    private string _winnerName;
    
    public WinnerState(GameStateManager.GameState key, GameContext gameContext) : base(key, gameContext)
    {
        GameContext = gameContext;
    }
    
    public override void EnterState()
    {
        if (GameContext.Manager.CardManager.CardStack.Count > 0)
        {
            var cards = GameContext.Manager.CardManager.PopCards(3);
            Debug.Log(cards);
            GameContext.Manager.CardManager.GiveCards(GameContext.Players[GameContext.CurrentPlayerIndex], cards);
            _winnerDecided = false;
            _processed = true;
        }
        else
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
            
            _winnerDecided = true;
            _processed = false;
            _winnerName = GameContext.Players[GameContext.CurrentPlayerIndex].playerName;
        }
        
        if(_winnerDecided) _nameShake = GameContext.WinnerCardTextName.GetComponent<Shake>();
    }

    public override void UpdateState()
    {
        if(!_winnerDecided) return;

        var cardTransform = GameContext.WinnerCardImage.transform;
        
        GameContext.WinnerCanvasGroup.alpha = Mathf.Lerp(GameContext.WinnerCanvasGroup.alpha, 1,
            Time.deltaTime * GameContext.AlphaResponsiveness);
        GameContext.WinnerCanvasGroup.blocksRaycasts = true;
        GameContext.WinnerCanvasGroup.interactable = true;
        
        cardTransform.localPosition = Vector3.Lerp(cardTransform.localPosition, Vector3.zero, Time.deltaTime * GameContext.AlphaResponsiveness);

        if (cardTransform.localPosition.y <= -1f) return;

        _timer -= Time.deltaTime;
        GameContext.WinnerCardTextName.text = _winnerName;
        
        if(_timer > 0f) return;

        _nameShake.gameObject.SetActive(true);
        _nameShake.Active = true;
        
        if(!_nameShake.Done) return;
        
        GameContext.ReturnToMenuButton.transform.parent.gameObject.SetActive(true);
    }

    public override void ExitState()
    {
        if (!_winnerDecided)
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
        }
        
        GameContext.PreviousState = GameStateManager.GameState.Winner;
    }

    public override GameStateManager.GameState GetNextState(GameStateManager.GameState lastState)
    {
        if (_winnerDecided) return GameStateManager.GameState.Winner;
        
        return _processed ? GameStateManager.GameState.ChangePlayer : GameStateManager.GameState.Winner;
    }
}
