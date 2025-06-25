using System;

public sealed class ReplicantFSM
{
    public ReplicantState CurrentState { get; private set; }

    private Dictionary<Type, ReplicantState> _states = new();

    public void AddState(ReplicantState state)
    {
        Type stateType = state.GetType();

        if (_states.ContainsKey(stateType))
            return;

        _states.Add(stateType, state);
    }

    public void SetState<T>() where T : ReplicantState
    {
        Type stateType = typeof(T);

        if (!_states.TryGetValue(stateType, out var state))
        {
            Log.Error($"State {stateType.Name} not found in replicant states.");
            return;
        }

        CurrentState?.Exit();
        CurrentState = state;
        CurrentState.Enter();
    }
}