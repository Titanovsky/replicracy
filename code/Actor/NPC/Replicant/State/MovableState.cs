public class MovableState : ReplicantState
{
    public MovableState(Replicant replicant) : base(replicant) { }

    protected void UpdatedRotation(Vector3 toRotatePosition)
    {
        // ������������ �����������
        Vector3 direction = (toRotatePosition - Replicant.WorldPosition).Normal;

        // ������� �������
        Rotation rotate = Rotation.LookAt(new Vector3(direction.x, direction.y, 0));

        Replicant.WorldRotation = Rotation.Lerp(Replicant.WorldRotation, rotate, Replicant.RotationSpeed * Time.Delta);
    }
}