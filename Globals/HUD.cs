using Godot;
using Isoland.Scenes;
using System;
namespace Isoland.Globals;
public partial class HUD : CanvasLayer
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        SceneChanger.GameEnteredConnect(() => Show());
        SceneChanger.GameExitedConnect(() => Hide());
        Visible = GetTree().CurrentScene is Scene;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
    private void OnMenuPressed()
    {
        Game.BackToTitle();
    }
}
