using Godot;
using System;
namespace Isoland.Scenes;
public partial class Scene : Sprite2D
{
    [Export(PropertyHint.File, "*.mp3")]
    private string musicOverride;
    public override void _Ready()
    {
        var tween = CreateTween();
        tween.SetEase(Tween.EaseType.Out)
             .SetTrans(Tween.TransitionType.Sine)
             .TweenProperty(this, (string)PropertyName.Scale, Vector2.One, 0.3)
             .From(Vector2.One * 1.05f);

    }
    public bool MusicOverrideIsEmpty => string.IsNullOrWhiteSpace(MusicOverride);

    public string MusicOverride { get => musicOverride; }
}
