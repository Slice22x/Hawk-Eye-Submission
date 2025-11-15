using UnityEngine;

public class PickUpState : GameState
{
    private bool _processed;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public PickUpState(GameStateManager.GameState key, GameContext gameContext) : base(key, gameContext)
    {
        GameContext = gameContext;
    }

    public override void EnterState()
    {
        _processed = false;
    }

    public override void UpdateState()
    {
        Card poppedCard = GameContext.Manager.CardManager.PopCard();
        poppedCard.gameObject.SetActive(true);
        GameContext.Players[GameContext.PlayerRequestDataBuffer.playerIndex].AddCardToHand(poppedCard);
        poppedCard.AssignCardToPlayer(GameContext.Players[GameContext.PlayerRequestDataBuffer.playerIndex]);
        _processed = true;
    }

    public override void ExitState()
    {
        
    }

    public override GameStateManager.GameState GetNextState(GameStateManager.GameState lastState)
    {
        return _processed ? GameStateManager.GameState.ChangePlayer : GameStateManager.GameState.PickUp;
    }
}
