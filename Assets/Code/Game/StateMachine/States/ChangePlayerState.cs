using UnityEngine;

public class ChangePlayerState : GameState
{
    private bool switched;
    
    public ChangePlayerState(GameStateManager.GameState key, GameContext gameContext) : base(key, gameContext)
    {
        GameContext = gameContext;
    }
    
    public override void EnterState()
    {
        GameManager.OnChangePlayerCamera?.Invoke(null);
        GameContext.SwitchPlayerPrompt.gameObject.SetActive(true);
        GameContext.NextPlayerName.text = $"Switch To Player: {GameContext.Players[GameContext.NextPlayerIndex].playerName}";
        GameContext.CanvasGroup.blocksRaycasts = false;
        GameContext.CanvasGroup.alpha = 0f;
    }

    public override void UpdateState()
    {
        switched = GameContext.CurrentPlayerIndex == GameContext.NextPlayerIndex;
    }

    public override void ExitState()
    {
        GameManager.OnChangePlayerCamera?.Invoke(GameContext.Players[GameContext.NextPlayerIndex]);
        GameContext.Manager.PlayerManager.UpdateCurrentPlayerIndex(GameContext.NextPlayerIndex);
        
        GameContext.LastPlayerIndex = (GameContext.CurrentPlayerIndex - GameContext.Direction) % GameContext.Players.Count;
        GameContext.NextPlayerIndex = (GameContext.NextPlayerIndex + GameContext.Direction) % GameContext.Players.Count;
        
        GameContext.SwitchPlayerPrompt.gameObject.SetActive(false);
        GameContext.CanvasGroup.blocksRaycasts = true;
        GameContext.CanvasGroup.alpha = 1f;
        switched = false;
        
        GameContext.PreviousState = GameStateManager.GameState.ChangePlayer;
    }
    
    public override GameStateManager.GameState GetNextState(GameStateManager.GameState lastState)
    {
        return switched switch
        {
            true => GameStateManager.GameState.RequestPlayerAction,
            false => GameStateManager.GameState.ChangePlayer
        };
    }
}
