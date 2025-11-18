using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    [Header("Card")]
    public CardManager CardManager { get; private set; }
    
    [Header("Gameplay")]
    public GameStateManager.GameState currentState;
    public bool CanCallOut;
    public bool JustCalledOut;
    private GameStateManager _stateManager;

    [Header("Players")]
    public PlayerManager PlayerManager { get; private set; }

    [Header("World")] public Transform Mat;
    [SerializeField] private float radiusFromMat;
    [SerializeField] private bool showGizmos;

    [Header("UI")] public TMP_Text LastPlayedText;
    public TMP_Text RankText;
    public TMP_Text NextRankText;
    public TMP_Text AmountText;
    
    public float RadiusFromMat => radiusFromMat;
    
    public static GameManager Instance;

    public delegate void ChangePlayerCamera(Player player);
    public static ChangePlayerCamera OnChangePlayerCamera;
    
    private void Awake()
    {
        Instance = this;
        _stateManager = GetComponent<GameStateManager>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PlayerManager = GetComponent<PlayerManager>();
        CardManager = GetComponent<CardManager>();
    }
    
    // Update is called once per frame
    void Update()
    {
        currentState = _stateManager.CurrentStateKey;
        
        RankText.text = $"RANK: {GetLastPlayedRankText(CardManager.LastRank())}";
        NextRankText.text = JustCalledOut ? "RANK: Any" : $"RANK: {GetLastPlayedRankText(CardManager.LastRank() + 1)}";
        AmountText.text = $"{CardManager.AmountOfCardsPlayedLast}";
    }

    private string GetLastPlayedRankText(CardInfo.CardRank rank)
    {
        if (rank > CardInfo.CardRank.King) rank = CardInfo.CardRank.Ace;
        
        return rank switch
        {
            CardInfo.CardRank.Ace => "A",
            CardInfo.CardRank.Two => "2",
            CardInfo.CardRank.Three => "3",
            CardInfo.CardRank.Four => "4",
            CardInfo.CardRank.Five => "5",
            CardInfo.CardRank.Six => "6",
            CardInfo.CardRank.Seven => "7",
            CardInfo.CardRank.Eight => "8",
            CardInfo.CardRank.Nine => "9",
            CardInfo.CardRank.Ten => "10",
            CardInfo.CardRank.Jack => "J",
            CardInfo.CardRank.Queen => "Q",
            CardInfo.CardRank.King => "K",
            _ => ""
        };
    }
    
    private void OnDrawGizmos()
    {
        if (!showGizmos) return;
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(Mat.position, radiusFromMat);

        if (!Application.isPlaying) return;
        
        Gizmos.color = Color.black;

        Vector3 displayDirection = PlayerManager.Players[PlayerManager.CurrentPlayerIndex].transform.right;
        
        Gizmos.DrawSphere(Mat.position + displayDirection * Mat.localScale.x / 2f, 0.1f);
        Gizmos.DrawSphere(Mat.position - displayDirection * Mat.localScale.x / 2f, 0.1f);
    }
}