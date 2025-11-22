using UnityEngine;

public class Grow : MonoBehaviour
{
    public bool Active;
    [SerializeField] private Transform target;
    [SerializeField] private float growResponsiveness = 1f;
    [SerializeField] private Vector3 growTarget;
    
    private Vector3 _startScale;

    private Transform _cachedTransform;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _cachedTransform = target ? target : transform;
        
        _startScale = _cachedTransform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        _cachedTransform.localScale = Vector3.Lerp(
            _cachedTransform.localScale, 
            Active ? growTarget : _startScale,
            Time.deltaTime * growResponsiveness);
    }

    public void Activate()
    {
        Active = true;
    }
    
    public void Deactivate()
    {
        Active = false;
    }
}
