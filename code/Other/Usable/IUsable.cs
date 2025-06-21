public interface IUsable
{
    public string UsableText { get; set; }
    public bool IsUsed { get; set; }
    public void Use();
    public void EnableHightlight();
    public void DisableHightlight();
    public string GetUsableText();
}