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
    public bool CalledOut;
    
    [SerializeField] private float cardDisplayWidth = 0.5f;

    [Space, SerializeField] private Transform handObject;
    [SerializeField] private CinemachineCamera playerCamera;
    [SerializeField] private TMP_Text playerNameText;

    [SerializeField] private int cardMaxLimitWithoutJoker = 4;
    [SerializeField] private int cardMaxLimitWithJoker = 2;
    
    private int _cardMaxLimit;
    
    private List<Card> _selectedCards;
    private List<CardInfo.CardRank> _playedRanks;
    
    private float _xBoundPosition => 0.01965f * (cardDisplayWidth + 0.05f);
    private const float Y_POSITION = -0.009f;
    private const float Z_POSITION = 0.0211f;
    private const float CARD_SCALE = 0.001f;
    private const int SORTING_ORDER = 52;
    private const float BASE_CARD_SPACING = 0.5f;
    private const int HIGHEST_CAMERA_PRIORITY = 100;
    private const float CARD_DISPLAY_WIDTH_NOT_CURRENT_PLAYER = 3f;
    
    public delegate void PlayerAction(PlayerRequestData data);
    public static PlayerAction OnPlayerAction;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        GameManager.OnChangePlayerCamera += SetCamera;
        GameSettings.OnTransitioning += () =>
        {
            GameManager.OnChangePlayerCamera -= SetCamera;
        };
    }

    private void Start()
    {
        _selectedCards = new List<Card>();
        _playedRanks = new List<CardInfo.CardRank>();
    }

    // Update is called once per frame
    void Update()
    {   
        UpdateHandPositions();
        playerNameText.text = playerName;

        cardDisplayWidth = GameManager.Instance.PlayerManager.CurrentPlayerIndex == playerIndex
            ? Mathf.Lerp(BASE_CARD_SPACING, 1f, hand.Count / 32f)
            : CARD_DISPLAY_WIDTH_NOT_CURRENT_PLAYER;

        if (playerCamera.Priority == HIGHEST_CAMERA_PRIORITY)
        {
            GameManager.Instance.PlaceButton.interactable = _selectedCards.Count > 0;
        }
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
        playerCamera.Priority = player == this ? HIGHEST_CAMERA_PRIORITY : 0;
        
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
            if(!filter(card) || _selectedCards.Contains(card)) continue;
            
            card.Selectable = false;
        }
    }
    
    private void SelectableCards()
    {
        foreach (Card card in hand)
        {
            if(_selectedCards.Contains(card)) continue;
            
            card.Selectable = true;
        }
    }
    
    public void AddToSelectedCard(Card card)
    {
        if (_selectedCards.Contains(card))
        {
            _selectedCards.Remove(card);
            SelectableCards();
            UpdatePlayedRank();
            return;
        }

        _cardMaxLimit = SelectedContainsJoker(card) && !GameManager.Instance.JustCalledOut
            ? cardMaxLimitWithJoker
            : cardMaxLimitWithoutJoker;
        
        if (_selectedCards.Count >= _cardMaxLimit) return;
        
        _selectedCards.Add(card);
        
        SelectableCardsConditions();
    }

    private void SelectableCardsConditions()
    {
        if (GameManager.Instance.JustCalledOut)
        {
            //No Jokers can be played after someone has called out
            UnSelectableCards(card => card.Suit == CardInfo.CardSuit.Jokers);

            //Only One rank of cards can be played at once after a call-out
            if(_selectedCards.Count >= 1) UnSelectableCards(card => card.Rank != _selectedCards[0].Rank);
            
            //Prevents going over card limit
            if(_selectedCards.Count >= _cardMaxLimit) UnSelectableCards();
            
            return;
        }
        
        UpdatePlayedRank();
        
        if(!GameManager.Instance.CanReverse)
            UnSelectableCards(card => card.Rank == CardInfo.CardRank.Jester);
        
        //If the hand count goes below 4 then only 1 rank of card can be selected after or any rank currently selected
        if (hand.Count <= cardMaxLimitWithoutJoker || hand.Count - _selectedCards.Count <= cardMaxLimitWithoutJoker)
            UnSelectableCards(card => !_playedRanks.Contains(card.Rank));

        //If 2 cards have been selected already then no more Jokers can be played after
        if (_selectedCards.Count >= cardMaxLimitWithJoker && !SelectedContainsJoker())
            UnSelectableCards(card => card.Suit == CardInfo.CardSuit.Jokers);
        
        //Prevents going over card limit
        if(_selectedCards.Count >= _cardMaxLimit) UnSelectableCards();
    }

    private void UpdatePlayedRank()
    {
        _playedRanks.Clear();
        
        foreach (Card card in hand)
        {
            if(_selectedCards.Contains(card) && !_playedRanks.Contains(card.Rank)) _playedRanks.Add(card.Rank);
        }
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
        return _selectedCards.Any(card => card.Suit == CardInfo.CardSuit.Jokers);
    }
    
    private bool SelectedContainsJoker(Card selectedCard)
    {
        return selectedCard.Suit == CardInfo.CardSuit.Jokers || _selectedCards.Any(card => card.Suit == CardInfo.CardSuit.Jokers);
    }
    
    public void SendPlaceRequestedData()
    {
        if (_selectedCards.Count == 0) return; 
        
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

        if (hand.Count >= 2)
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
