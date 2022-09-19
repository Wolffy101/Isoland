using Godot;
using System;
namespace Isoland.Globals;
/// <summary>
/// Autoload ,并且启动全局变量 
/// </summary>
public partial class SceneChanger : CanvasLayer
{
    //Singlton 单类模式
    private static SceneChanger _current;


    private static ColorRect _colorRect;
    /// <summary>
    /// 当前类的实列 
    /// </summary>
    public static SceneChanger Current => _current;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _colorRect = GetNode<ColorRect>("ColorRect");
        _current = this;
    }


    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
    public void ChangeSceneToFile(string path)
    {
        var tween = CreateTween();
        tween.TweenCallback(_colorRect.Show);
        tween.TweenProperty(_colorRect, "color:a", 1.0f, 0.2f);
        tween.TweenCallback(() => GetTree().ChangeSceneToFile(path));
        tween.TweenProperty(_colorRect, "color:a", 0f, 0.3f);
        tween.TweenCallback(_colorRect.Hide);
    }
}
