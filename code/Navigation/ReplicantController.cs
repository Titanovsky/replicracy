using Replicracy.Common;
using System;

public sealed class ReplicantController : Component
{
    private static readonly Logger Log = new("ReplicantController");

    [Property] public List<Replicant> Replicants { get; set; } = new();

    private Vector3 _targetPoint;
    private GameObject _targeObject;

    private Vector3 _targetObjectPosition;

    protected override void OnStart()
    {
        Subribe();
    }

    protected override void OnUpdate()
    {
        UpdateTargetObjectPosition();
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

            MoveToEnemy();
            return;
        }

        if (traceObject.Tags.Has("building"))
        {
            _targeObject = traceObject;

            MoveToBuilding();
            return;
        }

        MoveToPoint();
    }

    private void MoveToEnemy()
    {
        ActiveHightlights();


    }

    private void MoveToBuilding()
    {
        ActiveHightlights();

        MoveAroundBuilding();
    }

    private void MoveToPoint()
    {
        if (Replicants.Count == 0) return;

        foreach (var replicant in Replicants)
        {
            if (!replicant.IsValid()) continue;

            replicant.SetTargetPoint(_targetPoint);

            replicant.replicantFSM.SetState<MoveToPoint>();
        }
    }

    private void UpdateTargetObjectPosition()
    {
        if (_targeObject == null)
            return;

        var currentTargerPosition = _targeObject.WorldPosition;

        if (currentTargerPosition == _targetObjectPosition)
            return;

        _targetObjectPosition = _targeObject.WorldPosition;

        MoveAroundBuilding();
    }

    private void MoveAroundBuilding()
    {
        int numUnits = Replicants.Count;
        float radius = 50.0f; // Радиус окружения
        float angleStep = 360.0f / numUnits; // Угол между юнитами

        for (int i = 0; i < numUnits; i++)
        {
            float angle = angleStep * i * ((float)Math.PI / 180); // Перевод в радианы
            float x = _targeObject.WorldPosition.x + radius * (float)Math.Cos(angle);
            float y = _targeObject.WorldPosition.y + radius * (float)Math.Sin(angle);

            Vector3 targetPosition = new Vector3(x, y, _targeObject.WorldPosition.z);

            Replicants[i].SetTargetPoint(targetPosition);
            Replicants[i].replicantFSM.SetState<AttackBuilding>();
        }
    }

    private void ActiveHightlights()
    {
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

    public int GetCountReplicants()
    {
        return Replicants.Count;
    }

    private void Subribe()
    {
        Player.Instance.OnSpecified += PlayerSpecifie;

        Log.Info($"Subscrube");
    }

    private void Unsubscribe()
    {
        if (!Player.Instance.IsValid()) return;
        if (Player.Instance.OnSpecified is null) return;

        Player.Instance.OnSpecified -= PlayerSpecifie;

        Log.Info($"Unsubscribe");
    }
}
