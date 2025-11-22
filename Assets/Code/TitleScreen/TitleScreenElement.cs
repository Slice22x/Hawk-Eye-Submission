using System;
using UnityEngine;
using UnityEngine.UI;

public class TitleScreenElement : MonoBehaviour
{
    public enum ElementActionType { Position, Scale, Width, Toggle, Colour, Gravity }
    
    [System.Serializable]
    private struct ElementSetting
    {
        public TitleScreen.TitleScreenState state;
        public ElementActionType actionType;
        public Transform position;
        public Transform parent;
        public Vector3 scale;
        public Color colour;
        public Image image;
        public GameObject toggle;
        public Rigidbody2D rigidbody;
        public bool toggleState;
        public float value;
        public float extraValue;
        public float responsiveness;
    }
    
    [SerializeField] private ElementSetting[] settings;

    private RectTransform _rectTransform;
    
    private Vector3 _targetPosition;
    private Vector3 _targetScale;
    private Transform _targetParent;
    private Color _targetColour;
    private Image _targetImage;
    private Rigidbody2D _targetRigidbody;
    
    private float _responsiveness;
    private float _targetWidth;
    private float _targetValue;
    private float _targetExtraValue;
    private float _targetGravity;
    
    private bool _useWidth;
    private bool _useScale;
    private bool _usePosition;
    private bool _useColour;
    private bool _useGravity;
    
    void Start()
    {
        TitleScreen.OnMenuChange += UpdateElement;
        _rectTransform = GetComponent<RectTransform>();
    }
    
    void Update()
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition,
            _usePosition ? _targetPosition : transform.localPosition, Time.deltaTime * _responsiveness);
        
        transform.localScale = Vector3.Lerp(transform.localScale, _useScale ? _targetScale : transform.localScale,
            Time.deltaTime * _responsiveness);
        
        transform.parent = _targetParent ? _targetParent : transform.parent;
        
        _rectTransform.sizeDelta = Vector3.Lerp(_rectTransform.sizeDelta,
            new Vector3(_useWidth ? _targetWidth : _rectTransform.sizeDelta.x, _rectTransform.sizeDelta.y),
            Time.deltaTime * _responsiveness);
        
        _targetImage.color = Color.Lerp(_targetImage.color, _useColour ? _targetColour : _targetImage.color,
            Time.deltaTime * _responsiveness);
        
        _targetRigidbody.gravityScale = _useGravity ? _targetGravity : _targetRigidbody.gravityScale;
    }

    public void ExtendBox()
    {
        _targetWidth = _targetExtraValue;
    }
    
    public void ContractBox()
    {
        _targetWidth = _targetValue;
    }
    
    private void UpdateElement(TitleScreen.TitleScreenState state)
    {
        foreach (ElementSetting setting in settings)
        {
            print(setting.state);
            print(state);
            
            if (setting.state == state)
            {
                switch (setting.actionType)
                {
                    case ElementActionType.Position:
                        _targetPosition = setting.position ? setting.position.localPosition : transform.localPosition;
                        _usePosition = true;
                        break;
                    case ElementActionType.Scale:
                        _targetScale = setting.scale;
                        _useScale = true;
                        break;
                    case ElementActionType.Width:
                        _targetValue = setting.value;
                        _targetWidth = setting.value;
                        _targetExtraValue = setting.extraValue;
                        _useWidth = true;
                        break;
                    case ElementActionType.Toggle:
                        setting.toggle.SetActive(setting.toggleState);
                        break;
                    case ElementActionType.Colour:
                        _targetImage = setting.image;
                        _targetColour = setting.colour;
                        break;
                    case ElementActionType.Gravity:
                        _targetGravity = setting.value;
                        _targetGravity = setting.value;
                        break;
                }
                
                _responsiveness = setting.responsiveness;
                _targetParent = setting.parent;
            }
        }
    }
}
