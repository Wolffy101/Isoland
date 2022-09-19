using System;
using Godot;
using Isoland.Constants;

namespace Isoland.Objects;

public partial class Interactable : Area2D
{
    public override void _InputEvent(Viewport viewport, InputEvent @event, long shapeIdx)
    {
        if (!@event.IsActionPressed(InputEventContants.Interact))
        {
            return;
        }
        GD.Print("tett");
        System.Console.WriteLine("Console.Write");
        Interact();

    }
    protected virtual void Interact()
    {
        EmitSignal(SignalContants.Interact);
    }
}
