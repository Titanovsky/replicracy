using System;

public class Idle : ReplicantState
{
    public Idle(Replicant replicant) : base(replicant) 
    {
        _rnd = new();
    }

    private RealTimeUntil _idleSoundTimer = 1.5f;
    private Random _rnd;

    public override void Update()
    {
        CheckDistanceToPlayer();

        PlayRandomIdleSound();
    }

    private void CheckDistanceToPlayer()
    {
        var playerPosition = Player.Instance.GameObject.WorldPosition;
        var distance = Replicant.WorldPosition.Distance(playerPosition);

        if (distance > Replicant.MaxDistanceToPlayer)
        {
            Replicant.replicantFSM.SetState<ReturnToPlayer>();
        }
    }

    private void PlayRandomIdleSound()
    {
        if (!_idleSoundTimer) return;

        var randomIndex = _rnd.Next(Replicant.RandomIdleSounds.Count);

        var soundEvent = Replicant.RandomIdleSounds[randomIndex];

        if (soundEvent != null)
            Sound.Play(soundEvent, Replicant.WorldPosition);
        else
            Replicant.RandomIdleSounds.RemoveAt(randomIndex);

        ResetSoundTimer();
    }

    private void ResetSoundTimer() => _idleSoundTimer = 1.5f;
}