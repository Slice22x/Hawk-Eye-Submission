using UnityEngine;
using UnityEngine.UI;

public class Fade : MonoBehaviour
{
    public bool Active;

    public bool Done;
    
    [SerializeField] private Image image;
    [SerializeField] private float responsiveness;

    [SerializeField] private Color endColour;
    private Color _startColour, _targetColour; 
    
    void Start()
    {
        _startColour = image.color;
        _targetColour = endColour;
    }
    
    void Update()
    {
        _targetColour = Active ? endColour : _startColour;
        image.color = Color.Lerp(image.color, _targetColour, responsiveness * Time.deltaTime);
        
        Done = Vector4.Distance(image.color, _targetColour) <= 0.05f;
    }
}
