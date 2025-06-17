using Sandbox;
using Sandbox.Citizen;
using System;

public sealed class UnitPlayerController : Component
{
    [Property] public Player Player { get; set; }
    [Property] private List<NavMeshAgent> Units { get; set; } = new();

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

    private void MoveToPlayerSpecify(Vector3 targetPoint)
    {
        int count = Units.Count;
        if (count == 0) return;

        var extraSpacing = 0.1f; // Дополнительное расстояние между юнитами

        // Собираем радиусы агентов (предполагается, что они уже прописаны в NavMeshAgent.radius)
        List<float> radii = Units.Select(a => a.Radius).ToList();
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
            Vector3 dest = targetPoint + offset;

            // Трассируем вниз, чтобы скорректировать высоту
            {
                var start = dest + Vector3.Up * 100f;
                var end = dest + Vector3.Down * 100f;
                var tr = Scene.Trace.Ray(start, end)
                                 .IgnoreGameObject(Units[i].GameObject)  // не зацепить сам юнит
                                 .Run();

                if (tr.Hit)
                    dest = tr.HitPosition;
            }

            // Посылаем юнита в рассчитанную точку
            Units[i].MoveTo(dest);

            var testAnimate = Units[i].GameObject.Components.Get<CitizenAnimationHelper>();
            testAnimate.DuckLevel = 0f;
            testAnimate.IsNoclipping = false;
            testAnimate.IsGrounded = !false;
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
        Player.OnSpecified += MoveToPlayerSpecify;
    }

    private void Unsubscribe()
    {
        Player.OnSpecified -= MoveToPlayerSpecify;
    }
}
