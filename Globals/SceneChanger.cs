using Godot;
using System;
namespace Isoland.Globals;
/// <summary>
/// Autoload ,并且启动全局变量 
/// </summary>
public partial class SceneChanger : CanvasLayer
{
    /// <summary>
    /// 当前类的实列 
    /// </summary>
    private static SceneChanger Singleton;

    private ColorRect _colorRect;
    public static ColorRect ColorRect => Singleton._colorRect;
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
        tween.TweenCallback(ColorRect.Show);
        tween.TweenProperty(ColorRect, "color:a", 1.0f, 0.2f);
        tween.TweenCallback(() => Singleton.GetTree().ChangeSceneToFile(path));
        tween.TweenProperty(ColorRect, "color:a", 0f, 0.3f);
        tween.TweenCallback(ColorRect.Hide);
    }
}
