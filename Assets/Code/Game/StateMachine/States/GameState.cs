using UnityEngine;

[System.Serializable]
public abstract class GameState : BaseState<GameStateManager.GameState>
{
    protected GameState(GameStateManager.GameState key, GameContext gameContext) : base(key)
    {
        GameContext = gameContext;
    }

    protected GameContext GameContext;
}

