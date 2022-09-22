using Godot;
using Isoland.Constants;

namespace Isoland.Objects;

[Tool]
[RegisteredType]
public partial class Interactable : Area2D
{
    private Texture2D _texture;
    [Export]
    public Texture2D Texture
    {
        get => _texture; set
        {
            _texture = value;
            foreach (var node in GetChildren())
            {
                if (node.Owner is null)
                {
                    node.QueueFree();
                }
            }
            if (_texture == null) return;

            var sprite = new Sprite2D
            {
                Texture = _texture
            };
            AddChild(sprite);

            var rect = new RectangleShape2D
            {
                //视频中使用的是Extents = get_size() / 2 未找到

                Size = value.GetSize(),
            };

            var collider = new CollisionShape2D()
            {
                Shape = rect,
            };
            AddChild(collider);
        }
    }
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
