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
    public bool RevealCard;
    public int RevealIndex = -1;
    public bool RevealCompleted;

    private bool _revealTransformCalculated;
    
    private float _angle;
    private float _adder;
    private Vector3 _pos;
    private bool _rotating;

    [SerializeField] private Material greyscaleMaterial;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private float cardResponsiveness;
    public SpriteRenderer SpriteRenderer => _spriteRenderer;

    private bool _selected;
    
    public Player BelongsTo { get; set; }

    private Camera mainCamera => Camera.main;

    private const float REVEAL_Y_POSITION = 6.3f;
    private const float REVEAL_ANGLE = 180f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _spriteRenderer.material = new Material(greyscaleMaterial);
    }

    // Update is called once per frame
    void Update()
    {
        Reveal();
        
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

    private void Reveal()
    {
        if (!RevealCard) return;

        Transform rendererTransform = _spriteRenderer.transform;
        ForceShow = true;
        
        if (!_revealTransformCalculated)
        {
            transform.localRotation = Quaternion.Euler(0, 0, 0);
            
            _angle = GameManager.Instance.CurrentPlayerIndex * (360f / GameManager.Instance.AmountOfPlayers);
            
            print(_angle);
            
            _pos = GameManager.Instance.GetDisplayPosition(RevealIndex);
            _revealTransformCalculated = true;
        }
        

        rendererTransform.position = Vector3.Lerp(
            rendererTransform.position,
            new Vector3(_pos.x, _pos.y + REVEAL_Y_POSITION, _pos.z), 
            Time.deltaTime * cardResponsiveness);
        
        rendererTransform.localRotation = Quaternion.Lerp(
            rendererTransform.localRotation,
            Quaternion.Euler(_angle, 90, 90),
            Time.deltaTime * cardResponsiveness);
        
        RevealCompleted = rendererTransform.localEulerAngles.x >= _angle - 1f && 
                                   rendererTransform.position.y >= _pos.y + REVEAL_Y_POSITION - 0.1f;
    }

    // private void RevealRotation()
    // {
    //     _rotating = true;
    //     Transform rendererTransform = _spriteRenderer.transform;
    //     _adder = Mathf.Lerp(_adder, REVEAL_ANGLE, Time.deltaTime * cardResponsiveness);
    //
    //     rendererTransform.localEulerAngles = new Vector3(_angle + _adder,
    //         rendererTransform.localEulerAngles.y, rendererTransform.localEulerAngles.z);
    //
    //     if (_adder >= REVEAL_ANGLE / 2f)
    //     {
    //         ForceShow = true;
    //     }
    //     if (_adder >= REVEAL_ANGLE - 1f)
    //     {
    //         RevealCompleted = true;
    //     }
    // }
    
    private void UpdateRenderer()
    {
        //Checks if the player has a player which it belongs to
        if(!BelongsTo)
        {
            _spriteRenderer.sprite = ForceShow 
                ? GameManager.Instance.GetSpriteImage(suit, rank, false) 
                : GameManager.Instance.GetSpriteImage(suit, rank, true);
        }
        
        if(Played || RevealCard) return;
        
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
        RevealCard = false;
        RevealIndex = -1;
        _rotating = false;
        _revealTransformCalculated = false;
        Transform rendererTransform = _spriteRenderer.transform;
        rendererTransform.localPosition = Vector3.zero;
        rendererTransform.localRotation = Quaternion.Euler(0, 0, 0);
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
