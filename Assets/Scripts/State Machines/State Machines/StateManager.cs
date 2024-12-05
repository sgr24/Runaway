using System.Collections.Generic;
using System;
using UnityEngine;

public abstract class StateManager<EState> : MonoBehaviour where EState : Enum
{
    protected Dictionary<EState, BaseState<EState>> States = new Dictionary<EState, BaseState<EState>>();
    protected bool IsTransitioningState = false;
    public BaseState<EState> CurrentState { get; protected set; }

    public void Start()
    {
        CurrentState.EnterState();
    }

    void Update()
    {
        EState nextState = CurrentState.GetNextState();

        if (!IsTransitioningState && nextState.Equals(CurrentState.StateKey))
        {
            CurrentState.UpdateState();
        }
        else if (!IsTransitioningState)
        {
            TransitionToState(nextState);
        }
    }

    public void TransitionToState(EState state)
    {
        IsTransitioningState = true;
        CurrentState.ExitState();
        CurrentState = States[state];
        CurrentState.EnterState();
        IsTransitioningState = false;
    }

    void OnTriggerEnter(Collider other)
    {
        CurrentState.OnTriggerEnter(other);
    }

    void OnTriggerStay(Collider other)
    {
        CurrentState.OnTriggerStay(other);
    }

    void OnTriggerExit(Collider other)
    {
        CurrentState.OnTriggerExit(other);
    }
}
