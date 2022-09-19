using System;

namespace Godot;

public static class TweenExtentions
{
    public static CallbackTweener TweenCallback(this Tween tween, Action action)
    {
        return tween.TweenCallback(action);
    }
}