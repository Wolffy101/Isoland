using Godot;
using Isoland.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using static Godot.Tween;

namespace Isoland.UI;
public partial class DialogBubble : Control
{
    private IEnumerator<string> _dialogs;
    private Label _content;
    public override void _Ready()
    {
        _content = GetNode<Label>("Content");
        Hide();
    }

    public void ShowDialog(params string[] dialogs)
    {
        if (_dialogs is null || _dialogs.Current is null)
        {
            _dialogs = dialogs.ToList().GetEnumerator();
        }
        Show();
        MoveNext();
    }
    private void MoveNext()
    {
        if (_dialogs.MoveNext())
        {
            _content.Text = _dialogs.Current;
            CreateTween().
            SetEase(EaseType.Out).SetTrans(TransitionType.Sine)
            .TweenProperty(this, (string)PropertyName.Scale, Vector2.One, 0.2f)
            .From(Vector2.Zero);
        }
        else
        {
            Hide();
        }
    }
    private void BubbleGuiInput(InputEvent input)
    {
        if (input.IsActionPressed(InputEventContants.Interact))
        {
            MoveNext();
        }
    }
}
