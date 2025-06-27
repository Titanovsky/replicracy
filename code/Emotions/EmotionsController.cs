using Sandbox;

public sealed class EmotionsController: Component
{
    [Property] private Decal EmotionDecal { get; set; }
    [Property] private Emotions CurrentEmotion { get; set; }

    public enum Emotions
    {
        Idle,
        Angry,
        Scared1,
        Scared2,
        Confused,
        Vampire,
        Demon
    }

    protected override void OnStart()
    {
        UpdatedEmotion();
    }

    public void SetEmotion(Emotions emotion)
    {
        CurrentEmotion = emotion;

        UpdatedEmotion();
    }

    private void UpdatedEmotion() => 
        EmotionDecal.ColorTexture = EmotionDecal.ColorTexture = Texture.Load(GetEmotionDecal(CurrentEmotion));

    private string GetEmotionDecal(Emotions emotion)
    {
        return emotion switch
        {
            Emotions.Idle => "texture/emotions/idle.vtex",
            Emotions.Angry => "texture/emotions/angry.vtex",
            Emotions.Scared1 => "texture/emotions/scared_1.vtex",
            Emotions.Scared2 => "texture/emotions/scared_2.vtex",
            Emotions.Confused => "texture/emotions/confused.vtex",
            Emotions.Vampire => "texture/emotions/vampire.vtex",
            Emotions.Demon => "texture/emotions/demon.vtex",
            _ => "texture/emotions/idle.vtex"
        };
    }
}