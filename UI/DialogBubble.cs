using Godot;
using Isoland.Constants;
using System;
using System.Collections.Generic;
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

    public void Show(List<string> dialogs)
    {
        if (_dialogs == null || _dialogs.Current == null)
        {
            _dialogs = dialogs.GetEnumerator();
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
    public void BubbleGuiInput(InputEvent input)
    {
        if (input.IsActionPressed(InputEventContants.Interact))
        {
            MoveNext();
        }
    }
}
