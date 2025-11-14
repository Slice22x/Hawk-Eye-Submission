using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    [Header("Card Sprites")]
    [SerializeField] private CardInfo cardInfoHeart;
    [SerializeField] private CardInfo cardInfoDiamond;
    [SerializeField] private CardInfo cardInfoSpade;
    [SerializeField] private CardInfo cardInfoClub;

    [Header("Prefabs")]
    [SerializeField] private Card cardPrefab;
    [SerializeField] private Player playerPrefab;
    
    [Header("Gameplay")]
    [SerializeField] private List<Card> cardStack;
    public List<Card> PlayedStack;
    public int AmountOfCardsPlayedLast;
    public int MaxCardRevealed;
    public GameStateManager.GameState currentState;
    public CardInfo.CardRank[] CardQueue;
    public bool CanCallOut;
    public bool JustCalledOut;
    public bool CanPickCard => cardStack.Count > 0;
    private int _cardQueueIndex;
    private GameStateManager _stateManager;

    [Header("Players")]
    public int AmountOfPlayers;
    public List<Player> Players;
    private int _currentPlayerIndex;
    public Player CurrentPlayer => Players[_currentPlayerIndex];
    public int CurrentPlayerIndex => _currentPlayerIndex;

    [Header("World")] [SerializeField] private Transform mat;
    [SerializeField] private float radiusFromMat;
    [SerializeField] private bool showGizmos;

    [Header("UI")] public TMP_Text LastPlayedText;
    public TMP_Text RankText;
    public TMP_Text AmountText;
    
    public static GameManager Instance;

    public delegate void ChangePlayerCamera(Player player);
    public static ChangePlayerCamera OnChangePlayerCamera;

    private const float PLACED_CARD_POSITION_Z = 0.001f;
    private const float PLACED_CARD_SCALE = 0.05f;
    
    private void Awake()
    {
        Instance = this;
        _stateManager = GetComponent<GameStateManager>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void SpawnCards()
    {
        for (int suit = 0; suit < 4; suit++)
        {
            for (int rank = 0; rank < 13; rank++)
            {
                Card newCard = Instantiate(cardPrefab, transform);
                
                newCard.suit = (CardInfo.CardSuit)suit;
                newCard.rank = (CardInfo.CardRank)rank;

                newCard.name =
                    $"{System.Enum.GetName(typeof(CardInfo.CardRank), rank)} of {System.Enum.GetName(typeof(CardInfo.CardSuit), suit)}";
                
                newCard.gameObject.SetActive(false);
                
                cardStack.Add(newCard);
            }
        }
    }
    
    public void SpawnPlayers()
    {
        for (int i = 0; i < AmountOfPlayers; i++)
        {
            Player newPlayer = Instantiate(playerPrefab, null);
            
            newPlayer.transform.position = mat.position + new Vector3(
                Mathf.Cos(i * (2 * Mathf.PI / AmountOfPlayers)) * radiusFromMat, 
                0, 
                Mathf.Sin(i * (2 * Mathf.PI / AmountOfPlayers)) * radiusFromMat);
            
            newPlayer.transform.LookAt(mat);
            
            newPlayer.transform.eulerAngles = new Vector3(0, newPlayer.transform.eulerAngles.y, 0);
            newPlayer.playerIndex = i;
            
            Players.Add(newPlayer);
        }
        
        _currentPlayerIndex = Random.Range(0, AmountOfPlayers);
        OnChangePlayerCamera?.Invoke(Players[_currentPlayerIndex]);
    }

    public void PlaceCards(List<Card> cards, bool firstCard = false)
    {
        foreach (var card in cards.Where(card => card))
        {
            card.gameObject.SetActive(true);
            
            card.transform.parent = mat;
            card.transform.localPosition = new Vector3(0f, 0f, PLACED_CARD_POSITION_Z);
            
            card.transform.localEulerAngles = new Vector3(0, 0, card.BelongsTo.transform.localEulerAngles.y + Random.Range(-15f,15f));
            
            card.transform.localScale = Vector3.one * PLACED_CARD_SCALE;

            card.Played = true;
            
            card.AssignCardToPlayer(null);
            PlayedStack.Add(card);
            
            card.SpriteRenderer.sortingOrder = PlayedStack.Count;
            card.SpriteRenderer.transform.localPosition = Vector3.zero;
        }
        
        if (firstCard || JustCalledOut)
        {
            CardInfo.CardRank rank = cards[0].rank;
            
            for (int i = 0; i < CardQueue.Length; i++)
            {
                if (CardQueue[i] == rank)
                {
                    _cardQueueIndex = i;
                    break;
                }
            }
            
            return;
        }

        CanCallOut = !JustCalledOut;
        JustCalledOut = false;
        _cardQueueIndex = (_cardQueueIndex + 1) % CardQueue.Length;
        AmountOfCardsPlayedLast = cards.Count;
    }
    
    public void Shuffle() 
    {
        var count = cardStack.Count;
        var last = count - 1;
        
        for (var i = 0; i < last; ++i) 
        {
            var r = Random.Range(i, count);
            (cardStack[i], cardStack[r]) = (cardStack[r], cardStack[i]);
        }
    }

    public Sprite GetSpriteImage(CardInfo.CardSuit suit, CardInfo.CardRank rank, bool back)
    {
        return suit switch
        {
            CardInfo.CardSuit.Hearts => back ? cardInfoHeart.cardBack : cardInfoHeart.GetCardImage(rank),
            CardInfo.CardSuit.Diamonds => back ? cardInfoDiamond.cardBack : cardInfoDiamond.GetCardImage(rank),
            CardInfo.CardSuit.Spades => back ? cardInfoSpade.cardBack : cardInfoSpade.GetCardImage(rank),
            CardInfo.CardSuit.Clubs => back ? cardInfoClub.cardBack : cardInfoClub.GetCardImage(rank),
            _ => throw new System.ArgumentOutOfRangeException(nameof(suit), suit, null)
        };
    }

    public void UpdateCurrentPlayerIndex(int index)
    {
        _currentPlayerIndex = index;
    }
    
    public Card PopCard()
    {
        Card poppedCard = cardStack[^1];
        cardStack.RemoveAt(cardStack.Count - 1);
        
        return poppedCard;
    }

    public Card[] PopLastPlayedCards()
    {
        var cards = new Card[AmountOfCardsPlayedLast];
        
        for (int i = 0; i < AmountOfCardsPlayedLast; i++)
        {
            cards[i] = PlayedStack[^1];
            PlayedStack.RemoveAt(PlayedStack.Count - 1);
        }
        
        return cards;
    }

    public void GiveStack(Player player)
    {
        foreach (Card card in PlayedStack)
        {
            card.AssignCardToPlayer(player);
            player.AddCardToHand(card);
        }
        
        PlayedStack.Clear();
    }
    
    public CardInfo.CardRank LastRank()
    {
        int index = _cardQueueIndex - 1 == -1 ? 0 : _cardQueueIndex;
        return CardQueue[index];
    }

    public void GiveCards(Player giveTo, List<Card> cards)
    {
        foreach (Card card in cards)
        {
            card.gameObject.SetActive(true);
            card.AssignCardToPlayer(giveTo);
            giveTo.AddCardToHand(card);
        }
    }
    
    public void DistributeCards(List<Player> distributeTo, int eachHave)
    {
        for (int i = 0; i < eachHave; i++)
        {
            foreach (var player in distributeTo)
            {
                Card card = PopCard();
                card.gameObject.SetActive(true);
                player.AddCardToHand(card);
                card.AssignCardToPlayer(player);
            }
        }
    }
    
    public void DistributeCards(List<Card> cards, List<Player> distributeTo, Player bias)
    {
        int eachHave = cards.Count / distributeTo.Count;
        int extra = cards.Count % distributeTo.Count;
        
        for (int i = 0; i < extra; i++)
        {
            cards[i].AssignCardToPlayer(bias);
            bias.AddCardToHand(cards[i]);
            cards.RemoveAt(i);
        }
        
        for (int i = 0; i < eachHave; i++)
        {
            foreach (var player in distributeTo)
            {
                Card card = cards[i];
                card.gameObject.SetActive(true);
                player.AddCardToHand(card);
                card.AssignCardToPlayer(player);
            }
        }
        
        cards.Clear();
    }
    
    // Update is called once per frame
    void Update()
    {
        currentState = _stateManager.CurrentStateKey;
        
        RankText.text = $"RANK: {GetLastPlayedRankText()}";
        AmountText.text = $"{AmountOfCardsPlayedLast}";
    }

    private string GetLastPlayedRankText()
    {
        return LastRank() switch
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
    
    public Vector3 GetDisplayPosition(int index)
    {
        if (MaxCardRevealed == -1) return Vector3.zero;
        
        var displayDirection = Players[_currentPlayerIndex].transform.right;

        var max = mat.position + displayDirection * mat.localScale.x / 2f;
        var min = mat.position - displayDirection * mat.localScale.x / 2f;
        
        var position = Vector3.zero;
        
        if(MaxCardRevealed > 1)
            position = Vector3.Lerp(min, max, (float)index / MaxCardRevealed);
        else if (MaxCardRevealed == 1)
        {
            position = mat.position;
        }
        
        return position;
    }
    
    private void OnDrawGizmos()
    {
        if (!showGizmos) return;
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(mat.position, radiusFromMat);

        if (!Application.isPlaying) return;
        
        Gizmos.color = Color.black;

        Vector3 displayDirection = Players[_currentPlayerIndex].transform.right;
        
        Gizmos.DrawSphere(mat.position + displayDirection * mat.localScale.x / 2f, 0.1f);
        Gizmos.DrawSphere(mat.position - displayDirection * mat.localScale.x / 2f, 0.1f);
    }
}