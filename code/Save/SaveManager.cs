public sealed class SaveManager : Component
{
    public SaveData Data { get; set; }

    public void Save()
    {
        Data.Push();

        FileSystem.Data.WriteJson("last_game.json", Data);
    }

    public void Load()
    {
        Data = FileSystem.Data.ReadJson<SaveData>("last_game.json");
        if (Data is null) return;
    }

    public void StartLastGame()
    {
        Data.Push();
    }
}