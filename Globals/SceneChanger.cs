using Godot;
using Isoland.Scenes;
using System;
namespace Isoland.Globals;
/// <summary>
/// Autoload ,并且启动全局变量 
/// </summary>
public partial class SceneChanger : CanvasLayer
{
    [Signal]
    public delegate void GameEnteredEventHandler();

    [Signal]
    public delegate void GameExitedEventHandler();
    /// <summary>
    /// 当前类的实列 
    /// </summary>
    private static SceneChanger Singleton;

    private ColorRect _colorRect;
    public static ColorRect ColorRect => Singleton._colorRect;

    public static void GameEnteredConnect(GameEnteredEventHandler gameEntered) => Singleton.GameEntered += gameEntered;
    public static void GameExitedConnect(GameExitedEventHandler gameExited) => Singleton.GameExited += gameExited;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Singleton = this;
        _colorRect = GetNode<ColorRect>("ColorRect");
    }


    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
    public static void ChangeSceneToFile(string path)
    {
        var tween = Singleton.CreateTween();
        tween.TweenCallback(new Callable(ColorRect.Show));
        tween.TweenProperty(ColorRect, "color:a", 1.0f, 0.2f);
        tween.TweenCallback(new Callable(() => Singleton.ChangeScene(path)));
        tween.TweenProperty(ColorRect, "color:a", 0f, 0.3f);
        tween.TweenCallback(new Callable(ColorRect.Hide));
    }
    private void ChangeScene(string path)
    {
        var oldScene = GetTree().CurrentScene;
        var newScene = GD.Load<PackedScene>(path).Instantiate<Node>();
        var root = GetTree().Root;
        root.RemoveChild(oldScene);
        root.AddChild(newScene);
        GetTree().CurrentScene = newScene;

        var wasInGame = oldScene is Scene;
        var isInGame = newScene is Scene;
        if (wasInGame != isInGame)
        {
            if (isInGame)
            {
                EmitSignal(SignalName.GameEntered);
            }
            else
            {
                EmitSignal(SignalName.GameExited);
            }
        }
        oldScene.QueueFree();
    }
}
