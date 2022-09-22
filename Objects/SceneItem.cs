using Godot;
using Isoland.Items;

namespace Isoland.Objects;

[Tool]
public partial class SceneItem : Interactable
{
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

    protected async override void InteractInput()
    {
        base.InteractInput();
        var tween = CreateTween();
        tween.SetEase(Tween.EaseType.In).SetTrans(Tween.TransitionType.Back)
             .TweenProperty(this, (string)PropertyName.Scale, Vector2.Zero, 0.15f);
        await ToSignal(tween, Tween.SignalName.Finished);
        QueueFree();
    }
}
