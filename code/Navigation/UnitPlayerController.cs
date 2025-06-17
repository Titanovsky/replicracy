using Sandbox.Citizen;
using System;
using static Sandbox.PhysicsContact;

public sealed class UnitPlayerController : Component
{
    [Property] public Player Player { get; set; }
    [Property] private List<NavMeshAgent> Units { get; set; } = new();

    private Vector3 _targetPoint;
    private GameObject _targeObject;

    protected override void OnStart()
    {
        Player = Player.Instance;

        Subribe();
    }

    protected override void OnDestroy()
    {
        Unsubscribe();

        Player = null;
    }

    private void PlayerSpecifie(SceneTraceResult traceResult)
    {
        DisableHightlights();

        _targetPoint = traceResult.HitPosition;
        _targeObject = traceResult.GameObject;

        if (_targeObject.Tags.Has("enemy"))
        {
            MoveToEnemy();

            return;
        }

        if (_targeObject.Tags.Has("building"))
        {
            MoveToBuilding();

            return;
        }

        MoveToPoint();
    }

    private void MoveToEnemy()
    {
        ActiveHightlights();

        MoveAroundTarget();
    }

    private void MoveToBuilding()
    {
        ActiveHightlights();

        MoveAroundTarget();
    }

    private void MoveToPoint()
    {
        MoveAroundPoint();
    }

    private void MoveAroundTarget()
    {
        int numUnits = Units.Count;
        float radius = 5.0f; // ������ ���������
        float angleStep = 360.0f / numUnits; // ���� ����� �������

        for (int i = 0; i < numUnits; i++)
        {
            float angle = angleStep * i * ((float)Math.PI / 180); // ������� � �������
            float x = _targeObject.WorldPosition.x + radius * (float)Math.Cos(angle);
            float y = _targeObject.WorldPosition.y + radius * (float)Math.Sin(angle);

            Vector3 targetPosition = new Vector3(x, y, _targeObject.WorldPosition.z);

            Units[i].MoveTo(targetPosition);
        }
    }

    private void MoveAroundPoint()
    {
        int count = Units.Count;
        if (count == 0) return;

        var extraSpacing = 0.2f; // �������������� ���������� ����� �������

        // �������� ������� ������� (��������������, ��� ��� ��� ��������� � NavMeshAgent.radius)
        List<float> radii = Units.Select(a => a.Radius).ToList();
        float maxDiameter = radii.Max() * 2f;

        // ��� ������ ����� ������ ��� � �����
        float formationRadius = 0f;
        if (count > 1)
        {
            // ����������� ������ ����������, ����� ����� �������� ��� ���� �� maxDiameter + extraSpacing
            float minChord = maxDiameter + extraSpacing;
            float angleStep = (float)Math.PI * 2f / count;
            formationRadius = minChord / (2f * (float)Math.Sin(angleStep / 2f));
        }

        for (int i = 0; i < count; i++)
        {
            // ������������ ������� �� ����������
            float angle = i * MathF.PI * 2f / count;
            Vector3 offset = new Vector3(MathF.Cos(angle), 0, MathF.Sin(angle)) * formationRadius;
            Vector3 dest = _targetPoint + offset;

            // ���������� ����, ����� ��������������� ������
            {
                var start = dest + Vector3.Up * 100f;
                var end = dest + Vector3.Down * 100f;
                var tr = Scene.Trace.Ray(start, end)
                                 .IgnoreGameObject(Units[i].GameObject)  // �� �������� ��� ����
                                 .Run();

                if (tr.Hit)
                    dest = tr.HitPosition;
            }

            // �������� ����� � ������������ �����
            Units[i].MoveTo(dest);
        }
    }

    private void ActiveHightlights()
    {
        var targerHightlight = _targeObject.GetComponent<HighlightOutline>();

        if (targerHightlight != null)
            targerHightlight.Color = Color.Red.WithAlphaMultiplied(1f);
    }

    private void DisableHightlights()
    {
        if (_targeObject != null && _targeObject.IsValid())
        {
            var targerHightlight = _targeObject.GetComponent<HighlightOutline>();

            if (targerHightlight != null)
                targerHightlight.Color = targerHightlight.Color.WithAlphaMultiplied(0f);
        }
    }

    public void AddUnit(NavMeshAgent agent)
    {
        if (agent is null) return;
        if (Units.Contains(agent)) return;

        Units.Add(agent);
    }

    public void RemoveUnit(NavMeshAgent agent)
    {
        if (agent is null) return;
        if (!Units.Contains(agent)) return;

        Units.Remove(agent);
    }

    private void Subribe()
    {
        Player.OnSpecified += PlayerSpecifie;
    }

    private void Unsubscribe()
    {
        Player.OnSpecified -= PlayerSpecifie;
    }
}
