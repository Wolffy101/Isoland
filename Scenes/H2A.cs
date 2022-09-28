using Godot;
using Isoland.MiniGame;
using System;
namespace Isoland.Scenes;
public partial class H2A : Scene
{
    private H2ABoard _h2ABoard;
    private Sprite2D _gear;
    private void OnReady()
    {
        _h2ABoard = GetNode<H2ABoard>("Board");
        _gear = GetNode<Sprite2D>("Reset/Gear");
    }
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        base._Ready();
        OnReady();
    }

    private void OnResetInteract()
    {
        var tween = CreateTween();
        tween.SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Sine);

        tween.TweenProperty(_gear, (string)Sprite2D.PropertyName.Rotation, 360, 0.2f).AsRelative();
        tween.TweenCallback(new Callable(_h2ABoard.Reset));
    }
}
