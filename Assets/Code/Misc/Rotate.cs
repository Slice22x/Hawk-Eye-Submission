using System;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float speed;
    [SerializeField] private bool invert;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private Transform _cachedTransform;
    
    void Start()
    {
        _cachedTransform = target ? target : transform;
        spriteRenderer.flipX = invert;
    }
    
    void Update()
    {
        _cachedTransform.localEulerAngles = new Vector3(
            _cachedTransform.localEulerAngles.x,
            _cachedTransform.localEulerAngles.y, 
            _cachedTransform.localEulerAngles.z + (invert ? 1 : -1) * speed * Time.deltaTime);
    }
    
    public void Invert()
    {
        invert = !invert;
        spriteRenderer.flipX = invert;
    }
}
