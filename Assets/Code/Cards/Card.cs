using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class Card : MonoBehaviour
{
    private static readonly int BlendAmount = Shader.PropertyToID("_BlendAmount");
    private static readonly int ShowLines = Shader.PropertyToID("_ShowLines");

    public CardInfo.CardSuit Suit;
    public CardInfo.CardRank Rank;
    public bool Selectable;
    public bool ForceShow;
    public bool Played;
    public bool RevealCard;
    public int RevealIndex = -1;
    public bool RevealCompleted;
    public float RevealTimeout;

    private bool _revealTransformCalculated;
    
    private float _angle;
    private float _adder;
    private Vector3 _pos;
    private float _revealTimer;

    [SerializeField] private Material greyscaleMaterial;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private float cardResponsiveness;
    public SpriteRenderer SpriteRenderer => _spriteRenderer;

    private bool _selected;
    private Sprite _front;
    private Sprite _back;
    
    public Player BelongsTo { get; set; }

    private Camera mainCamera => Camera.main;

    private const float REVEAL_Y_POSITION = 6.3f;
    private const float CARD_SCALE_NOT_CURRENT_PLAYER = 6f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _spriteRenderer.material = new Material(greyscaleMaterial);
        _revealTimer = RevealTimeout;
        _front = GameManager.Instance.CardManager.GetSpriteImage(Suit, Rank, false);
        _back = GameManager.Instance.CardManager.GetSpriteImage(Suit, Rank, true);
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.PlayerManager.CurrentPlayer != BelongsTo && _spriteRenderer.sprite == _back &&
            !Played && GameManager.Instance.currentState != GameStateManager.GameState.ChangePlayer)
        {
            _spriteRenderer.transform.localScale = Vector3.Lerp(_spriteRenderer.transform.localScale,
                Vector3.one * CARD_SCALE_NOT_CURRENT_PLAYER, Time.deltaTime * cardResponsiveness);
        }

        if(!BelongsTo)
            _spriteRenderer.material.SetFloat(ShowLines, 0);
        
        Reveal();
        
        if((GameManager.Instance.PlayerManager.CurrentPlayer != BelongsTo || 
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
        _revealTimer -= Time.deltaTime;
        
        if (!RevealCard) return;

        Transform rendererTransform = _spriteRenderer.transform;
        ForceShow = true;
        
        if (!_revealTransformCalculated)
        {
            transform.localRotation = Quaternion.Euler(0, 0, 0);
            
            _angle = GameManager.Instance.PlayerManager.CurrentPlayerIndex * (360f / GameSettings.Instance.PlayerNames.Count);
            
            _pos = GameManager.Instance.CardManager.GetDisplayPosition(RevealIndex);
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
        
        RevealCompleted = (rendererTransform.localEulerAngles.x >= _angle - 1f && 
                                   rendererTransform.position.y >= _pos.y + REVEAL_Y_POSITION - 0.1f) || _revealTimer <= 0;
    }
    
    private void UpdateRenderer()
    {
        //Checks if the player has a player which it belongs to
        if(!BelongsTo)
        {
            _spriteRenderer.sprite = ForceShow ? _front : _back;
        }
        
        if(Played || RevealCard) return;
        
        _spriteRenderer.transform.localScale = Vector3.Lerp(_spriteRenderer.transform.localScale,
            Vector3.one, Time.deltaTime * cardResponsiveness);

        _spriteRenderer.material.SetFloat(BlendAmount,
            Mathf.Lerp(_spriteRenderer.material.GetFloat(BlendAmount), Selectable ? 0 : 1,
                Time.deltaTime * cardResponsiveness));

        bool show = (GameManager.Instance.CardManager.CurrentRank == Rank || GameManager.Instance.JustCalledOut) && Selectable && !Played;
        
        _spriteRenderer.material.SetFloat(ShowLines, show ? 1 : 0);
        
        _spriteRenderer.transform.localPosition = Vector3.Lerp(_spriteRenderer.transform.localPosition,
            _selected ? Vector3.up * 1.1f : Vector3.zero,
            Time.deltaTime * cardResponsiveness);
    }
    
    public void AssignCardToPlayer(Player assignTo)
    {
        BelongsTo = assignTo;
        UpdateSprite(BelongsTo);
        _spriteRenderer.material.SetFloat(ShowLines, 0);
        _selected = false;
        RevealCard = false;
        RevealIndex = -1;
        _revealTimer = RevealTimeout;
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

        _spriteRenderer.sprite = player == BelongsTo && player ? _front : _back;
    }
}
