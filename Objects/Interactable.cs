using System;
using ClassName.Attributes;
using Godot;
using Isoland.Constants;

namespace Isoland.Objects;
[ClassName]
public partial class Interactable : Area2D
{
    // public struct SignalName
    // {
    //     public string const
    // }
    [Signal]
    public delegate void InteractEventHandler();
    public override void _InputEvent(Viewport viewport, InputEvent @event, long shapeIdx)
    {
        if (!@event.IsActionPressed(InputEventContants.Interact))
        {
            return;
        }
        GD.Print("tett");
        System.Console.WriteLine("Console.Write");
        InteractInput();

    }
    protected virtual void InteractInput()
    {
        EmitSignal(SignalName.Interact);
    }
}
