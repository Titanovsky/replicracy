public abstract class EnemyBase : Component, Component.ITriggerListener, Component.IDamageable
{
    [Property, Category("Stats")] public virtual float Health { get; set; } = 0f;

    public virtual void Die()
    {

    }

    public virtual void OnDamage(in DamageInfo damage)
    {
    }

    public virtual void OnTriggerEnter(Collider other)
    {
    }

    public virtual bool IsFriend(GameObject target)
    {
        return false;
    }  
}