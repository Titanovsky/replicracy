public abstract class EnemyBase : Component, Component.ITriggerListener, Component.IDamageable
{
    public virtual void OnDamage(in DamageInfo damage)
    {
    }

    public virtual void OnTriggerEnter(Collider other)
    {
    }
}