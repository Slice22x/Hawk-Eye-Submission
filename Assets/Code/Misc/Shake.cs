using UnityEngine;

public class Shake : MonoBehaviour
{
    private Vector2 _origin;

    public bool Active;
    public bool Done => _intensity >= 0f;
    [SerializeField] private float intensity;
    [SerializeField] private float decayAmount;

    private float _intensity;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _origin = transform.position;
        _intensity = intensity;
    }

    // Update is called once per frame
    void Update()
    {
        if(Active)
        {
            transform.position = _origin + Random.insideUnitCircle * _intensity;
            _intensity = Mathf.Lerp(_intensity, -0.01f, decayAmount * Time.deltaTime);
        }
        else
        {
            transform.position = _origin;
            _intensity = intensity;
        }
    }
}
