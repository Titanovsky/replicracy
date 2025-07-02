using System;

public sealed class PickupManager : Component
{
    public static PickupManager Instance { get; private set; }

    [Property] public List<PickupBase> Pickups { get; set; } = new();

    private List<Vector3> _cachePos = new();

    public void Add(PickupBase pickup)
    {
        if (Pickups.Contains(pickup)) return;

        _cachePos.Add(pickup.WorldPosition);
        Pickups.Add(pickup);

        Log.Info($"Added {pickup.GameObject}");
    }

    private void Prepare()
    {
        foreach (var pickup in Pickups)
        {
            if (!pickup.IsValid()) continue;

            _cachePos.Add(pickup.WorldPosition);
        }
    }

    private void MovingAndRotating()
    {
        for (int i = 0; i < Pickups.Count; i++)
        {
            var pickup = Pickups[i];
            if (!pickup.IsValid()) continue;

            pickup.WorldRotation *= new Angles(.6f, 1f * pickup.SpeedRotate, .1f);
            pickup.WorldPosition = _cachePos[i] + new Vector3(1f, 1f, MathF.Sin(Time.Now * pickup.Frequency) * pickup.Amplitude);
        }
    }

    private void CreateSingleton()
    {
        if (Instance is null)
            Instance = this;
    }

    private void DestroySingleton()
    {
        if (Instance is not null)
            Instance = null;
    }

    protected override void OnStart()
    {
        Prepare();
    }

    protected override void OnFixedUpdate()
    {
        MovingAndRotating();
    }

    protected override void OnAwake()
    {
        CreateSingleton();
    }

    protected override void OnDestroy()
    {
        DestroySingleton(); // fix: pick up dna
    }
}