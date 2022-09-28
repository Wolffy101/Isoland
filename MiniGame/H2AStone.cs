using Godot;
using Isoland.Objects;
using System;
namespace Isoland.MiniGame;

[Tool]
[RegisteredType]
public partial class H2AStone : Interactable
{
    private int _currentSlot;
    [Export]
    public int CurrentSlot
    {
        get { return _currentSlot; }
        set
        {
            _currentSlot = value;
            UpdateTextTure();
        }
    }

    private int _targetSlot;
    public int TargetSlot
    {
        get { return _targetSlot; }
        set
        {
            _targetSlot = value;
            UpdateTextTure();
        }
    }
    private void UpdateTextTure()
    {
        var index = TargetSlot;
        if (TargetSlot != CurrentSlot)
        {
            index += 6;
        }
        Texture = ResourceLoader.Load<Texture2D>($"res://assets/H2A/SS_{index:D2}.png");
    }
}
