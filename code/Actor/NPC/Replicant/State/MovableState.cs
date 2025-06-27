public class MovableState : ReplicantState
{
    public MovableState(Replicant replicant) : base(replicant) { }

    protected void UpdatedRotation(Vector3 toRotatePosition)
    {
        // Рассчитываем направление
        Vector3 direction = (toRotatePosition - Replicant.WorldPosition).Normal;

        // Целевой поворот
        Rotation rotate = Rotation.LookAt(new Vector3(direction.x, direction.y, 0));

        Replicant.WorldRotation = Rotation.Lerp(Replicant.WorldRotation, rotate, Replicant.RotationSpeed * Time.Delta);
    }
}