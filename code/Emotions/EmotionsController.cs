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
            Emotions.Idle => "texture/emotions/idle.png",
            Emotions.Angry => "texture/emotions/angry.png",
            Emotions.Scared1 => "texture/emotions/scared_1.png",
            Emotions.Scared2 => "texture/emotions/scared_2.png",
            Emotions.Confused => "texture/emotions/confused.png",
            Emotions.Vampire => "texture/emotions/vampire.png",
            Emotions.Demon => "texture/emotions/demon.png",
            _ => "texture/emotions/idle.png"
        };
    }
}