using UnityEngine;
using UnityEngine.UI;

public class Fade : MonoBehaviour
{
    public bool Active;
    
    [SerializeField] private Image image;
    [SerializeField] private float responsiveness;

    [SerializeField] private Color endColour;
    private Color _startColour, _targetColour; 
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _startColour = image.color;
        _targetColour = endColour;
    }

    // Update is called once per frame
    void Update()
    {
        _targetColour = Active ? endColour : _startColour;
        image.color = Color.Lerp(image.color, _targetColour, responsiveness * Time.deltaTime);
    }
}
