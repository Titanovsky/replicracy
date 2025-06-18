using Sandbox;
using Sandbox.Navigation;
using Sandbox.VR;
using System;

public sealed class PlayerUnit : Component
{
    [Property] float RotationSpeed { get; set; } = 2.5f;
    [Property] float AttackDelay { get; set; } = 1f;
    [Property] int AttackDamage { get; set; } = 5;
    [Property] GameObject eye { get; set; }

    [Property]
    [RequireComponent]
    private NavMeshAgent NavMeshAgent { get; set; }

    private GameObject _targetObject;
    private Vector3 _targerPoint;
    private Vector3 _targerPlayer;

    SceneTraceResult tr;

    private RealTimeUntil _timeUntil;

    private Player Player { get; set; }
    private Vector3 oldPlayerPos;

    protected override void OnAwake()
    {
        _timeUntil = AttackDelay;
    }

    protected override void OnStart()
    {
        Player = Player.Instance;

        ReturnToPlayer();
    }

    protected override void OnUpdate()
    {
        UpdatedRotation();

        UpdateTargetPointBehaviours();
        UpdateTargetObjectBehaviours();
        UpdateNoBehaviours();

        DrawSpecified();
    }

    protected override void OnDestroy()
    {
        Player = null;
    }

    public void MoveToPoint(Vector3 point)
    {
        NavMeshAgent.MoveTo(point);
    }

    public void SetTargetPoint(Vector3 point)
    {
        _targetObject = null;

        _targerPoint = point;

        NavMeshAgent.MoveTo(point);
    }

    public void SetTargetObject(GameObject targerObject)
    {
        _targerPoint = Vector3.Zero;

        _targetObject = targerObject;
    }

    public void AttackEnemy()
    {
        if (!_timeUntil) return;

        Log.Info("AttackEnemy");

        ReseTimer();
    }

    public void AttackBuilding()
    {
        if (!_timeUntil) return;

        var building = _targetObject.Components.Get<Building>();

        building.TakeDamage(AttackDamage);

        if (building.IsDead)
        {
            _targetObject = null;
            ReturnToPlayer();
        }

        ReseTimer();
    }

    private void UpdateTargetPointBehaviours()
    {
        if (_targerPoint == Vector3.Zero) return;

    }

    private void UpdateNoBehaviours()
    {
        if (_targerPoint != Vector3.Zero) return;
        if (_targetObject != null) return;
        if (oldPlayerPos == Player.GameObject.WorldPosition) return;

        ReturnToPlayer();
    }

    private void UpdateTargetObjectBehaviours()
    {
        if (_targetObject == null) return;

        tr = Scene.Trace.Ray(new Ray(eye.WorldPosition, eye.WorldRotation.Forward), 30)
                .IgnoreGameObject(GameObject)
                .Run();

        if (!tr.Hit) return;

        var hitObject = tr.GameObject;

        if (hitObject.Tags.Has("enemy"))
        {
            AttackEnemy();
        }

        if (hitObject.Tags.Has("building"))
        {
            AttackBuilding();
        }
    }

    private void ReturnToPlayer()
    {
        _targerPlayer = (Vector3)Scene.NavMesh.GetRandomPoint(Player.GameObject.WorldPosition, 300);

        oldPlayerPos = Player.GameObject.WorldPosition;

        MoveToPoint(_targerPlayer);
    }

    private void UpdatedRotation()
    {
        Vector3 rotatiobPoint = _targetObject?.WorldPosition == null
            ? _targerPoint
            : _targetObject.WorldPosition;

        if (rotatiobPoint == Vector3.Zero)
            rotatiobPoint = _targerPlayer;

        // Рассчитываем направление
        Vector3 direction = (rotatiobPoint - WorldPosition).Normal;

        // Целевой поворот
        Rotation rotate = Rotation.LookAt(new Vector3(direction.x, direction.y, 0));

        WorldRotation = Rotation.Lerp(WorldRotation, rotate, RotationSpeed * Time.Delta);

    }

    private void DrawSpecified()
    {
        Gizmo.Draw.Color = Color.White.WithAlpha(0.1f);
        Gizmo.Draw.LineThickness = 4;
        Gizmo.Draw.Line(tr.StartPosition, tr.EndPosition);

        Gizmo.Draw.Color = Color.Green;
        Gizmo.Draw.Line(tr.EndPosition, tr.EndPosition + tr.Normal * 1.0f);
    }

    public float GetRadius() => NavMeshAgent.Radius;
    private void ReseTimer() => _timeUntil = AttackDelay;
}
