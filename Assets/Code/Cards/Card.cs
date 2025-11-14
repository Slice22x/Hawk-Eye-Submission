using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Card : MonoBehaviour
{
    private static readonly int BlendAmount = Shader.PropertyToID("_BlendAmount");

    public Card(CardInfo.CardSuit suit, CardInfo.CardRank rank)
    {
        this.suit = suit;
        this.rank = rank;
    }

    public CardInfo.CardSuit suit;
    public CardInfo.CardRank rank;
    public bool Selectable;
    public bool ForceShow;
    public bool Played;

    [SerializeField] private Material greyscaleMaterial;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private float cardResponsiveness;
    public SpriteRenderer SpriteRenderer => _spriteRenderer;

    private bool _selected;
    
    public Player BelongsTo { get; set; }

    private Camera mainCamera => Camera.main;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _spriteRenderer.material = new Material(greyscaleMaterial);
    }

    // Update is called once per frame
    void Update()
    {
        if((GameManager.Instance.CurrentPlayer != BelongsTo || 
           GameManager.Instance.currentState != GameStateManager.GameState.RequestPlayerAction) && !Played)
        {
            return;
        }
        
        UpdateRenderer();
        
        if (!Selectable || Played) return;
        
        CheckSelected();
    }

    private void CheckSelected()
    {
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (!Physics.Raycast(ray, out RaycastHit hit)) return;
            
        if(hit.collider.gameObject.name != gameObject.name) return;
            
        _spriteRenderer.transform.localScale = Vector3.Lerp(_spriteRenderer.transform.localScale,
            hit.transform == transform ? Vector3.one * 1.2f : Vector3.one, Time.deltaTime * cardResponsiveness);
            
        if (Mouse.current.leftButton.wasPressedThisFrame)
        { 
            BelongsTo.AddToSelectedCard(this); 
            _selected = !_selected;
        }
    }

    private void UpdateRenderer()
    {
        //Checks if the player has a player which it belongs to
        if(!BelongsTo)
        {
            _spriteRenderer.sprite = ForceShow 
                ? GameManager.Instance.GetSpriteImage(suit, rank, false) 
                : GameManager.Instance.GetSpriteImage(suit, rank, true);
        }
        
        if(Played) return;
        
        _spriteRenderer.transform.localScale = Vector3.Lerp(_spriteRenderer.transform.localScale,
            Vector3.one, Time.deltaTime * cardResponsiveness);

        _spriteRenderer.material.SetFloat(BlendAmount,
            Mathf.Lerp(_spriteRenderer.material.GetFloat(BlendAmount), Selectable ? 0 : 1,
                Time.deltaTime * cardResponsiveness));
        
        _spriteRenderer.transform.localPosition = Vector3.Lerp(_spriteRenderer.transform.localPosition, _selected ? Vector3.up * 1.1f : Vector3.zero,
            Time.deltaTime * cardResponsiveness);
    }
    
    public void AssignCardToPlayer(Player assignTo)
    {
        BelongsTo = assignTo;
        UpdateSprite(BelongsTo);
        _selected = false;
    }
    
    public void Deselect()
    {
        _selected = false;
    }
    
    public void UpdateSprite(Player player)
    {
        if(ForceShow) return;
        
        _spriteRenderer.sprite = player == BelongsTo && player ? 
            GameManager.Instance.GetSpriteImage(suit, rank, false): 
            GameManager.Instance.GetSpriteImage(suit, rank, true);
    }
}
