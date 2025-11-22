using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameStateManager : StateManager<GameStateManager.GameState>
{
    public enum GameState
    {
        Initialising,
        ChangePlayer,
        RequestPlayerAction,
        PlaceCard,
        CallOut,
        PickUp,
        Joker,
        Winner
    }

    public GameContext GameContext;

    [Space, SerializeField] private int startingCardAmount;
    
    [SerializeField] private Image switchPlayerPrompt;
    [SerializeField] private TMP_Text nextPlayerName;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private CanvasGroup winnerCanvasGroup;
    [SerializeField] private Image winnerCardImage;
    [SerializeField] private TMP_Text winnerCardTextName;
    [SerializeField] private Button returnToMenuButton;
    [SerializeField] private float alphaResponsiveness;
    
    [SerializeField] private float revealTimer;
    
    private void Awake()
    {
        States = new Dictionary<GameState, BaseState<GameState>>();
        Player.OnPlayerAction += ReceiveRequestedData;
    }
    
    private void InitialiseStates()
    {
       States.Add(GameState.Initialising, new InitialisationState(GameState.Initialising, GameContext));
       States.Add(GameState.ChangePlayer, new ChangePlayerState(GameState.ChangePlayer, GameContext));
       States.Add(GameState.RequestPlayerAction, new RequestPlayerActionState(GameState.RequestPlayerAction, GameContext));
       States.Add(GameState.PlaceCard, new PlaceCardState(GameState.PlaceCard, GameContext));
       States.Add(GameState.CallOut, new CallOutState(GameState.CallOut, GameContext));
       States.Add(GameState.PickUp, new PickUpState(GameState.PickUp, GameContext));
       States.Add(GameState.Joker, new JokerState(GameState.Joker, GameContext));
       States.Add(GameState.Winner, new WinnerState(GameState.Winner, GameContext));
       
       CurrentState = States[GameState.Initialising];
    }

    public void ConfirmedSwitchPlayer()
    {
        GameContext.CurrentPlayerIndex = GameContext.NextPlayerIndex;
    }
    
    public void PlacePlayerCards()
    {
        GameContext.Players[GameContext.CurrentPlayerIndex].SendPlaceRequestedData();
    }
    
    public void CallOutPlayerCards()
    {
        GameContext.Players[GameContext.CurrentPlayerIndex].SendCallOutRequestedData();
    }
    
    public void PickUpPlayerCards()
    {
        GameContext.Players[GameContext.CurrentPlayerIndex].SendPickUpRequestedData();
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    new void Start()
    {
        GameContext = new GameContext(GameManager.Instance, GameManager.Instance.PlayerManager.Players, switchPlayerPrompt,
            nextPlayerName, null, canvasGroup, revealTimer, winnerCanvasGroup, alphaResponsiveness,
            winnerCardImage, winnerCardTextName, returnToMenuButton, startingCardAmount);
        
        InitialiseStates();
        
        base.Start();
    }

    private void ReceiveRequestedData(PlayerRequestData data)
    {
        GameContext.PlayerRequestDataBuffer = data;
    }
    
    // Update is called once per frame
    new void Update()
    {
        base.Update();
    }
}
