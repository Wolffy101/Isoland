using Godot;
using Isoland.Globals;
using Isoland.Constants;

namespace Isoland.UI;
public partial class Invertory : VBoxContainer
{
    private Label _label;
    private TextureButton _prev;
    private Sprite2D _prop;
    private Sprite2D _hand;
    private TextureButton _next;

    private Globals.Invertory _invertory;
    private Timer _timer;

    private Tween _handOuter;
    private Tween _labelOuter;
    private void OnReady()
    {
        _label = GetNode<Label>("Label");
        _prev = GetNode<TextureButton>("ItemBar/Prev");

        _prop = GetNode<Sprite2D>("ItemBar/Use/Prop");
        _hand = GetNode<Sprite2D>("ItemBar/Use/Hand");

        _next = GetNode<TextureButton>("ItemBar/Next");
        _timer = GetNode<Timer>("Label/Timer");
        _invertory = Game.Invertory;
    }
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        OnReady();

        // //测试代码
        // _invertory.AddItem(GD.Load<Item>("Items/Key.tres"));
        // _invertory.AddItem(GD.Load<Item>("Items/Mail.tres"));
        //默认隐藏
        _hand.Hide();
        _label.Hide();

        _invertory.Change += () => UpdateUi();
        UpdateUi(true);
    }
    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed(InputEventContants.Interact) && _invertory.ActiveItem is not null)
        {
            _invertory.SetDeferred(Isoland.Globals.Invertory.PropertyName.ActiveItem, default);
            // _hand.Hide();
            _handOuter = CreateTween();
            _handOuter.SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Sine).SetParallel();
            _handOuter.TweenProperty(_hand, (string)Sprite2D.PropertyName.Scale, Vector2.One * 3, 0.15);
            _handOuter.TweenProperty(_hand, $"{Sprite2D.PropertyName.Modulate}:{nameof(Color.A)}", 0.0, 0.15);
            _handOuter.Chain().TweenCallback(Callable.From(_hand.Hide));
        }
    }
    private void UpdateUi(bool isInit = false)
    {
        var count = _invertory.Count;
        _prev.Disabled = count < 2;
        _next.Disabled = count < 2;
        Visible = count > 0;
        var item = _invertory.Current;
        if (item is null) return;

        _label.Text = item.Description;
        _prop.Texture = item.PropTexture;

        if (isInit) return;

        var tween = CreateTween();
        tween.SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Back);
        tween.TweenProperty(_prop, (string)Sprite2D.PropertyName.Scale, Vector2.One, 0.15).From(Vector2.Zero);

        ShowLabel();
    }

    private void ShowLabel()
    {
        if (_labelOuter is not null && _labelOuter.IsValid())
        {
            _labelOuter.Kill();
        }
        _label.Show();
        var tween = CreateTween();
        tween.SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Sine);
        tween.TweenProperty(_label, $"{Sprite2D.PropertyName.Modulate}:{nameof(Color.A)}", 1.0, 0.2);
        tween.TweenCallback(Callable.From(() => _timer.Start()));
    }

    #region Connect Signal 
    private void OnPrevPressed() => _invertory.Prev();
    private void OnNextPressed() => _invertory.Next();

    private void OnUsePressed()
    {
        _invertory.ActiveItem = _invertory.Current;
        if (_handOuter is not null && _handOuter.IsValid())
        {
            _handOuter.Kill();
        }
        _hand.Show();
        var tween = CreateTween();
        tween.SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Back).SetParallel();
        tween.TweenProperty(_hand, (string)Sprite2D.PropertyName.Scale, Vector2.One, 0.15).From(Vector2.Zero);
        tween.TweenProperty(_hand, $"{Sprite2D.PropertyName.Modulate}:{nameof(Color.A)}", 1.0, 0.15);
        ShowLabel();
    }
    private void OnTimerTimeout()
    {
        _labelOuter = CreateTween();
        _labelOuter.SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Sine);
        _labelOuter.TweenProperty(_label, $"{Sprite2D.PropertyName.Modulate}:{nameof(Color.A)}", 0.0, 0.2);
        _labelOuter.TweenCallback(Callable.From(_label.Hide));
    }
    #endregion
}
