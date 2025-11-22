using System;
using System.Collections.Generic;
using UnityEngine;

public class Wave : MonoBehaviour
{
    public bool Active;
    [SerializeField] private List<Transform> targets;
    [SerializeField] private List<Vector3> startPositions;
    
    [Space, SerializeField] private float frequency;
    [SerializeField] private float amplitude;
    [SerializeField] private float space;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < targets.Count; i++)
        {
            startPositions.Add(targets[i].localPosition);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!Active) return;

        float xOffset = space / (targets.Count - 1);
        
        for (int i = 0; i < targets.Count; i++)
        {
            float offset = i * (2 * Mathf.PI / targets.Count);
            float xPosition = Mathf.Lerp(-space, space, i / (float)targets.Count) + xOffset / 2f;
            targets[i].localPosition =
                startPositions[i] + Vector3.up * (amplitude * Mathf.Sin(Time.time * frequency + offset));
            targets[i].localPosition = new Vector3(xPosition, targets[i].localPosition.y, targets[i].localPosition.z);
        }
    }
}
