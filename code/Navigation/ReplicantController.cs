using Replicracy.Common;
using System;

public sealed class ReplicantController : Component
{
    private static readonly Logger Log = new("ReplicantController");

    [Property] public List<Replicant> Replicants { get; set; } = new();

    private Vector3 _targetPoint;
    private GameObject _targeObject;

    protected override void OnStart()
    {
        Subribe();
    }

    protected override void OnDestroy()
    {
        Unsubscribe();
    }

    private void PlayerSpecifie(SceneTraceResult traceResult)
    {
        if (Replicants.Count == 0) return;

        DisableHightlights();
        _targeObject = null;

        _targetPoint = traceResult.HitPosition;
        var traceObject = traceResult.GameObject;

        if (traceObject.Tags.Has("enemy"))
        {
            _targeObject = traceObject;

            ActiveHightlights();

            AttackEnemy();
            return;
        }

        if (traceObject.Tags.Has("building"))
        {
            _targeObject = traceObject;
            
            ActiveHightlights();

            AttackBuilding();
            return;
        }

        MoveToPoint();
    }

    private void AttackEnemy()
    {
        if (Replicants.Count == 0) return;

        foreach (var replicant in Replicants)
        {
            if (!replicant.IsValid()) continue;

            replicant.SetAttackEnemy(_targeObject);
        }

    }

    private void AttackBuilding()
    {
        if (Replicants.Count == 0) return;

        foreach (var replicant in Replicants)
        {
            if (!replicant.IsValid()) continue;

            replicant.SetAttackBuilding(_targeObject.WorldPosition);
        }
    }

    private void MoveToPoint()
    {
        if (Replicants.Count == 0) return;

        foreach (var replicant in Replicants)
        {
            if (!replicant.IsValid()) continue;

            replicant.SetMoveToPoint(_targetPoint);
        }
    }

    private void ActiveHightlights()
    {
        if (_targeObject == null || !_targeObject.IsValid())
            return;

        var targerHightlight = _targeObject.GetComponent<HighlightOutline>();

        if (targerHightlight != null)
            targerHightlight.Color = targerHightlight.Color.WithAlpha(1f);
    }

    private void DisableHightlights()
    {
        if (_targeObject == null || !_targeObject.IsValid())
            return;

        var targerHightlight = _targeObject.GetComponent<HighlightOutline>();

        if (targerHightlight != null)
            targerHightlight.Color = targerHightlight.Color.WithAlpha(0f);
    }

    public void AddReplicant(Replicant agent)
    {
        if (agent is null) return;
        if (Replicants.Contains(agent)) return;

        Replicants.Add(agent);

        Log.Info($"{agent} added ({Replicants.IndexOf(agent)})");
    }

    public void RemoveReplicant(Replicant agent)
    {
        if (agent is null) return;
        if (!Replicants.Contains(agent)) return;

        Log.Info($"{agent} removed ({Replicants.IndexOf(agent)})");

        Replicants.Remove(agent);
    }

    public int GetCountReplicants() => Replicants.Count;

    private void Subribe()
    {
        Player.Instance.OnSpecified += PlayerSpecifie;
    }

    private void Unsubscribe()
    {
        if (!Player.Instance.IsValid()) return;
        if (Player.Instance.OnSpecified is null) return;

        Player.Instance.OnSpecified -= PlayerSpecifie;
    }
}
