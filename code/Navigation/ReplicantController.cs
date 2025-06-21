using System;
using System.Threading.Tasks;

public sealed class ReplicantController : Component
{
    [Property] public Player Player { get; set; }
    [Property] private List<Replicant> Replicants { get; set; } = new();
    
    private Vector3 _targetPoint;
    private GameObject _targeObject;

    private Vector3 _targetObjectPosition;

    protected override void OnStart()
    {
        Player = Player.Instance;

        Subribe();
    }

    protected override void OnUpdate()
    {
        UpdateTargetObjectPosition();
    }

    protected override void OnDestroy()
    {
        Unsubscribe();

        Player = null;
    }

    private void PlayerSpecifie(SceneTraceResult traceResult)
    {
        Player.Instance.Hint.Call("Hello my furer");

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

        foreach (var unit in Replicants)
        {
            unit.SetTargetObject(_targeObject);
        }

        MoveAroundTarget();
    }

    private void MoveToBuilding()
    {
        ActiveHightlights();

        foreach (var unit in Replicants)
        {
            unit.SetTargetObject(_targeObject);
        }

        MoveAroundTarget();
    }

    private void MoveToPoint()
    {
        foreach (var unit in Replicants)
        {
            unit.SetTargetPoint(_targetPoint);
        }

        MoveAroundPoint();
    }

    private void UpdateTargetObjectPosition()
    {
        if (_targeObject == null)
            return;

        var currentTargerPosition = _targeObject.WorldPosition;

        if (currentTargerPosition == _targetObjectPosition)
            return;

        _targetObjectPosition = _targeObject.WorldPosition;

        MoveAroundTarget();
    }

    private void MoveAroundTarget()
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

            Replicants[i].MoveToPoint(targetPosition);
        }
    }

    private void MoveAroundPoint()
    {
        int count = Replicants.Count;
        if (count == 0) return;

        var extraSpacing = 0.2f; // Дополнительное расстояние между юнитами

        // Собираем радиусы агентов (предполагается, что они уже прописаны в NavMeshAgent.radius)
        List<float> radii = Replicants.Select(a => a.GetRadius()).ToList();
        float maxDiameter = radii.Max() * 2f;

        // Для одного юнита просто идём в точку
        float formationRadius = 0f;
        if (count > 1)
        {
            // Минимальный радиус окружности, чтобы между соседями был хотя бы maxDiameter + extraSpacing
            float minChord = maxDiameter + extraSpacing;
            float angleStep = (float)Math.PI * 2f / count;
            formationRadius = minChord / (2f * (float)Math.Sin(angleStep / 2f));
        }

        for (int i = 0; i < count; i++)
        {
            // Рассчитываем позицию на окружности
            float angle = i * MathF.PI * 2f / count;
            Vector3 offset = new Vector3(MathF.Cos(angle), 0, MathF.Sin(angle)) * formationRadius;
            Vector3 dest = _targetPoint + offset;

            // Трассируем вниз, чтобы скорректировать высоту
            {
                var start = dest + Vector3.Up * 100f;
                var end = dest + Vector3.Down * 100f;
                var tr = Scene.Trace.Ray(start, end)
                                 .IgnoreGameObject(Replicants[i].GameObject)  // не зацепить сам юнит
                                 .Run();

                if (tr.Hit)
                    dest = tr.HitPosition;
            }

            // Посылаем юнита в рассчитанную точку
            Replicants[i].MoveToPoint(dest);
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
    }

    public void RemoveReplicant(Replicant agent)
    {
        if (agent is null) return;
        if (!Replicants.Contains(agent)) return;

        Replicants.Remove(agent);
    }

    private void Subribe()
    {
        Player.OnSpecified += PlayerSpecifie;
    }

    private void Unsubscribe()
    {
        if (Player != null)
        Player.OnSpecified -= PlayerSpecifie;
    }
}
