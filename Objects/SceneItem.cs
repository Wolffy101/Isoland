using Godot;
using Isoland.Globals;
using Isoland.Items;

namespace Isoland.Objects;

[Tool]
public partial class SceneItem : Interactable
{
    private string Flags => $"picked:{Item.ResourcePath.GetFile()}";
    private Item _item;

    [Export]
    public Item Item
    {
        get => _item;
        set
        {
            _item = value;
            Texture = _item.SceneTexture;
            NotifyPropertyListChanged();
        }
    }
    public override void _Ready()
    {
        if (Engine.IsEditorHint()) return;
        if (Game.Flags.Has(Flags))
        {
            QueueFree();
        }
    }
    protected async override void InteractInput()
    {
        base.InteractInput();
        Game.Flags.Add(Flags);
        Game.Invertory.AddItem(_item);

        var tween = CreateTween();
        tween.SetEase(Tween.EaseType.In).SetTrans(Tween.TransitionType.Back)
             .TweenProperty(this, (string)PropertyName.Scale, Vector2.Zero, 0.15);
        await ToSignal(tween, Tween.SignalName.Finished);
        QueueFree();
    }
}
