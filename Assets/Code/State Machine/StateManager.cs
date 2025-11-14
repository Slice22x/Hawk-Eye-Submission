using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateManager<EState> : MonoBehaviour where EState : Enum
{
    protected Dictionary<EState, BaseState<EState>> States;

    protected BaseState<EState> CurrentState;
    protected bool IsTransitioning = false;
    protected EState PreviousState;
    
    public EState CurrentStateKey => CurrentState.StateKey;
    
    protected void Start()
    {
        CurrentState.EnterState();
    }
    
    protected virtual void Update()
    {
        //Debug.Log($"Current State: {CurrentState.StateKey.ToString()}");
        
        EState nextStateKey = CurrentState.GetNextState(PreviousState);
        if(nextStateKey.Equals(CurrentState.StateKey))
            CurrentState.UpdateState();
        else
        {
            TransitionToState(nextStateKey);
        }
    }

    
    private void TransitionToState(EState key)
    {
        if(IsTransitioning) return;
        
        IsTransitioning = true;
        
        PreviousState = CurrentState.StateKey;
        
        CurrentState.ExitState();
        CurrentState = States[key];
        CurrentState.EnterState();
        
        IsTransitioning = false;
    }
}
