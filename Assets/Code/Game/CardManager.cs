using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class JokerData
{
    public CardInfo.CardRank type;
    public Player playerCaster;
    public bool safe;
    public int turnsSinceCast;
    
    public JokerData(CardInfo.CardRank type, Player playerCaster, bool safe, int turnsSinceCast)
    {
        this.type = type;
        this.playerCaster = playerCaster;
        this.safe = safe;
        this.turnsSinceCast = turnsSinceCast;
    }

    public void UpdateJoker(int safeTurns)
    {
        safe = turnsSinceCast >= safeTurns;
        turnsSinceCast++;
    }
}

public class CardManager : MonoBehaviour
{
    [SerializeField] private Card cardPrefab;
    
    [Space, SerializeField] private CardInfo cardInfoHeart;
    [SerializeField] private CardInfo cardInfoDiamond;
    [SerializeField] private CardInfo cardInfoSpade;
    [SerializeField] private CardInfo cardInfoClub;
    [SerializeField] private CardInfo cardInfoJokers;
    
    private int _cardQueueIndex;
    
    [Space]
    
    [SerializeField] private List<Card> cardStack;
    public List<Card> PlayedStack;
    public List<JokerData> PlayedJokers;
    public int AmountOfCardsPlayedLast;
    public int MaxCardRevealed;
    public CardInfo.CardRank[] CardQueue;
    public int JokerTurnsTillSafe = 1;
    public bool CanPickCard => cardStack.Count > 0;
    
    private PlayerManager _playerManager;
    private Transform _mat;
    
    private const float PLACED_CARD_POSITION_Z = 0.001f;
    private const float PLACED_CARD_SCALE = 0.05f;
    
    public delegate void UpdateJokers();
    public static UpdateJokers OnUpdateJokers;
    
    private void Start()
    {
        _playerManager = GetComponent<PlayerManager>();
        _mat = GameManager.Instance.Mat;
        OnUpdateJokers += UpdatePlayedJokers;
    }

    private void UpdatePlayedJokers()
    {
        foreach (var joker in PlayedJokers)
        {
            joker.UpdateJoker(JokerTurnsTillSafe);
        }
    }

    public void SpawnCards()
    {
        for (int suit = 0; suit < 5; suit++)
        {
            for (int rank = 0; rank < 17; rank++)
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
    
    public void PlaceCards(List<Card> cards, bool firstCard = false)
    {
        foreach (var card in cards.Where(card => card))
        {
            card.gameObject.SetActive(true);
            
            card.transform.parent = _mat;
            card.transform.localPosition = new Vector3(0f, 0f, PLACED_CARD_POSITION_Z);
            
            card.transform.localEulerAngles = new Vector3(0, 0, card.BelongsTo.transform.localEulerAngles.y + Random.Range(-15f,15f));
            
            card.transform.localScale = Vector3.one * PLACED_CARD_SCALE;

            card.Played = true;
            
            card.AssignCardToPlayer(null);
            PlayedStack.Add(card);
            
            card.SpriteRenderer.sortingOrder = PlayedStack.Count;
            card.SpriteRenderer.transform.localPosition = Vector3.zero;
        }
        
        if (firstCard || GameManager.Instance.JustCalledOut)
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

        if(!GameManager.Instance.JustCalledOut)
            GameManager.Instance.CanCallOut = !GameManager.Instance.JustCalledOut;
        GameManager.Instance.JustCalledOut = false;
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
    
    public Vector3 GetDisplayPosition(int index)
    {
        if (MaxCardRevealed == -1) return Vector3.zero;
        
        var displayDirection = _playerManager.Players[_playerManager.CurrentPlayerIndex].transform.right;

        var max = _mat.position + displayDirection * _mat.localScale.x / 2f;
        var min = _mat.position - displayDirection * _mat.localScale.x / 2f;
        
        var position = Vector3.zero;
        
        if(MaxCardRevealed > 1)
            position = Vector3.Lerp(min, max, (float)index / MaxCardRevealed);
        else if (MaxCardRevealed == 1)
        {
            position = _mat.position;
        }
        
        return position;
    }
}
