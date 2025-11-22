using System;
using UnityEngine;
using UnityEngine.Events;

public class Rotate : MonoBehaviour
{
    public enum Axis { X, Y, Z }
    
    public bool Active;
    [SerializeField] private Transform target;
    [SerializeField] private float rotationResponsiveness = 1f;
    [SerializeField] private Vector3 rotationTarget;
    [SerializeField] private float activationDegree;
    [SerializeField] private Axis axis;
    
    [SerializeField] private UnityEvent OnReachActivationDegree;
    
    private Vector3 _startRotation;

    private Transform _cachedTransform;
    private bool _activated;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _cachedTransform = target ? target : transform;
        
        _startRotation = _cachedTransform.localRotation.eulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        _cachedTransform.localRotation = Quaternion.Euler(Vector3.Lerp(
            _cachedTransform.localRotation.eulerAngles,
            Active ? rotationTarget : _startRotation,
            Time.deltaTime * rotationResponsiveness));

        if (_activated) return;
        
        switch (axis)
        {
            case Axis.X:
                if (_cachedTransform.localEulerAngles.x > activationDegree)
                {
                    OnReachActivationDegree?.Invoke();
                    _activated = true;
                }
                break;
            case Axis.Y:
                if (_cachedTransform.localEulerAngles.y > activationDegree)
                {
                    OnReachActivationDegree?.Invoke();
                    _activated = true;
                }
                break;
            case Axis.Z:
                if (_cachedTransform.localEulerAngles.z > activationDegree)
                {
                    OnReachActivationDegree?.Invoke();
                    _activated = true;
                }
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    public void Activate()
    {
        Active = true;
        _activated = false;
    }
    
    public void Deactivate()
    {
        Active = false;
    }
}
