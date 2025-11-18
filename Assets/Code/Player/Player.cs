using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Cinemachine;
using UnityEngine;
using TMPro;

public class PlayerRequestData
{
    public PlayerActionType actionType;
    public int playerIndex;
    public List<Card> sentCards;
    public bool containsJoker;
    
    public PlayerRequestData(PlayerActionType actionType, int playerIndex, List<Card> sentCards, bool containsJoker = false)
    {
        this.actionType = actionType;
        this.playerIndex = playerIndex;
        this.sentCards = sentCards;
        this.containsJoker = containsJoker;
    }
}

public enum PlayerActionType
{
    Place,
    CallOut,
    PickUp,
}

public class Player : MonoBehaviour
{
    public int playerIndex;
    public List<Card> hand;
    public string playerName;
    
    [SerializeField] private float cardDisplayWidth = 0.5f;

    [Space, SerializeField] private Transform handObject;
    [SerializeField] private CinemachineCamera playerCamera;
    [SerializeField] private TMP_Text playerNameText;

    [SerializeField] private int cardMaxLimitWithoutJoker = 4;
    [SerializeField] private int cardMaxLimitWithJoker = 2;
    
    private int _cardMaxLimit;
    
    private List<Card> _selectedCards;
    
    private float _xBoundPosition => 0.01965f * cardDisplayWidth;
    private const float Y_POSITION = -0.009f;
    private const float Z_POSITION = 0.0211f;
    private const float CARD_SCALE = 0.001f;
    private const int SORTING_ORDER = 52;
    private const float BASE_CARD_SPACING = 0.5f;
    
    public delegate void PlayerAction(PlayerRequestData data);
    public static PlayerAction OnPlayerAction;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        GameManager.OnChangePlayerCamera += SetCamera;
    }

    private void Start()
    {
        _selectedCards = new List<Card>();
    }

    // Update is called once per frame
    void Update()
    {   
        UpdateHandPositions();
        playerNameText.text = playerName;
        
        cardDisplayWidth = Mathf.Lerp(BASE_CARD_SPACING, 1f, hand.Count / 52f);
    }

    public void AddCardToHand(Card card)
    {
        hand.Add(card);
        card.transform.parent = handObject;
        card.transform.localRotation = Quaternion.identity;
        card.transform.localScale = Vector3.one * CARD_SCALE;
        card.Selectable = true;
        card.Played = false;
        card.ForceShow = false;
    }
    
    private void SetCamera(Player player)
    {
        playerCamera.Priority = player == this ? 100 : 0;
        
        foreach (Card card in hand)
        {
            card.UpdateSprite(player);
        }
    }

    private void UnSelectableCards()
    {
        foreach (Card card in hand)
        {
            if(_selectedCards.Contains(card)) continue;
            
            card.Selectable = false;
        }
    }
    
    private void UnSelectableCards(Predicate<Card> filter)
    {
        foreach (Card card in hand)
        {
            if(!filter(card)) continue;
            
            card.Selectable = false;
        }
    }
    
    private void SelectableCards()
    {
        foreach (Card card in hand)
        {
            card.Selectable = true;
        }
    }
    
    public void AddToSelectedCard(Card card)
    {
        if (_selectedCards.Contains(card))
        {
            _selectedCards.Remove(card);
            SelectableCards();
            return;
        }

        _cardMaxLimit = SelectedContainsJoker() ? 2 : 4;
        
        if (_selectedCards.Count >= _cardMaxLimit) return;
        
        _selectedCards.Add(card);

        if (GameManager.Instance.JustCalledOut && _selectedCards.Count >= 1 || hand.Count <= 4)
            UnSelectableCards(card1 => card1.rank != _selectedCards[0].rank);
        
        if(SelectedContainsJoker() && _selectedCards.Count >= _cardMaxLimit) UnSelectableCards(card1 => card1.suit != CardInfo.CardSuit.Jokers);
        
        if(_selectedCards.Count >= _cardMaxLimit) UnSelectableCards();
    }

    private void UnselectAll()
    {
        foreach (Card card in _selectedCards)
        {
            card.Deselect();
        }
        
        _selectedCards.Clear();
    }

    private bool SelectedContainsJoker()
    {
        return _selectedCards.Any(card => card.suit == CardInfo.CardSuit.Jokers);
    }
    
    public void SendPlaceRequestedData()
    {
        var buffer = new Card[_selectedCards.Count];
        
        _selectedCards.CopyTo(buffer);
        PlayerRequestData data = new PlayerRequestData(PlayerActionType.Place, playerIndex, buffer.ToList(), SelectedContainsJoker());
        
        OnPlayerAction?.Invoke(data);
        
        foreach (Card card in _selectedCards)
        {
            hand.Remove(card);
        }
        
        _selectedCards.Clear();
        SelectableCards();
    }
    
    public void SendCallOutRequestedData()
    {
        if (!GameManager.Instance.CanCallOut || GameManager.Instance.JustCalledOut) return;
        
        UnselectAll();
        PlayerRequestData data = new PlayerRequestData(PlayerActionType.CallOut, playerIndex, null);
        OnPlayerAction?.Invoke(data);
        SelectableCards();
    }
    
    public void SendPickUpRequestedData()
    {
        if(!GameManager.Instance.CardManager.CanPickCard) return;
        
        PlayerRequestData data = new PlayerRequestData(PlayerActionType.PickUp, playerIndex, null);
        OnPlayerAction?.Invoke(data);
    }
    
    private void UpdateHandPositions()
    {
        switch (hand.Count)
        {
            case 0:
                return;
            case 1:
                hand[0].transform.localPosition = new Vector3(0, Y_POSITION, Z_POSITION);
                hand[0].SpriteRenderer.sortingOrder = SORTING_ORDER + hand.Count;
                return;
        }


        hand[0].transform.localPosition = new Vector3(-_xBoundPosition, Y_POSITION, Z_POSITION);
        hand[0].SpriteRenderer.sortingOrder = SORTING_ORDER + hand.Count;

        if (hand.Count > 2)
        {
            for (int i = 1; i < hand.Count; i++)
            {
                hand[i].transform.localPosition = new Vector3(Mathf.Lerp(-_xBoundPosition, _xBoundPosition, i / (hand.Count - 1f)), Y_POSITION, Z_POSITION);
                hand[i].SpriteRenderer.sortingOrder = SORTING_ORDER + hand.Count - i;
            }
        }
        
        // hand[^1].transform.localPosition = new Vector3(_xBoundPosition, Y_POSITION, Z_POSITION);
        // hand[^1].SpriteRenderer.sortingOrder = SORTING_ORDER;
    }
}
