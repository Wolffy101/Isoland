using Godot;
using System;
namespace Isoland.Scenes;
public partial class Scene : Sprite2D
{
    public override void _Ready()
    {
        var tween = CreateTween();
        tween.SetEase(Tween.EaseType.Out)
             .SetTrans(Tween.TransitionType.Sine)
             .TweenProperty(this, (string)PropertyName.Scale, Vector2.One, 0.3)
             .From(Vector2.One * 1.05f);

    }
}
