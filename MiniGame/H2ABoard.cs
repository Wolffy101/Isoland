using System;
using System.Diagnostics;
using System.Linq;
using Godot;
using Godot.Collections;
using Isoland.Globals;

namespace Isoland.MiniGame;
[Tool]
public partial class H2ABoard : Node2D
{
    private readonly static Texture2D SlotTexture = ResourceLoader.Load<Texture2D>("res://assets/H2A/CIRCLE.png");
    private readonly static Texture2D LineTexture = ResourceLoader.Load<Texture2D>("res://assets/H2A/CIRCLELINE.png");
    private float _radius = 300;
    [Export]
    public float Radius
    {
        get => _radius;
        set
        {
            _radius = value;
            //在godot 4 中 update 改名为此
            // https://github.com/godotengine/godot/pull/64377
            QueueRedraw();
        }
    }
    private readonly Dictionary<int, H2AStone> _stoneMap = new();
    private H2AConfig _config;
    [Export]
    public H2AConfig Config
    {
        get { return _config; }
        set
        {
            if (_config is not null && !_config.IsConnected(nameof(UpdateBoard), new Callable(UpdateBoard)))
                _config.Changed -= UpdateBoard;

            _config = value;

            if (_config is not null && !_config.IsConnected(nameof(UpdateBoard), new Callable(UpdateBoard)))
                _config.Changed += UpdateBoard;
            UpdateBoard();
        }
    }

    public void Reset()
    {
        foreach (var stone in _stoneMap.Values)
        {
            MoveStone(stone, (int)Config.Placements[stone.TargetSlot]);
        }
    }

    private void UpdateBoard()
    {
        foreach (var node in GetChildren())
        {
            if (node.Owner is null)
            {
                node.QueueFree();
            }
        }
        if (_config is null)
        {
            return;
        }

        //改了原视频的遍历方式。这种只需要遍历已经来连接的线就可以了
        foreach (var item in Config.Collections)
        {
            var src = (int)item.Key;
            foreach (var dst in (Array<int>)item.Value)
            {
                var line = new Line2D()
                {
                    Width = LineTexture.GetSize().y,
                    Texture = LineTexture,
                    TextureMode = Line2D.LineTextureMode.Tile,
                    DefaultColor = Colors.White,
                    ShowBehindParent = true,
                };
                line.AddPoint(SlotPosition(src));
                line.AddPoint(SlotPosition(dst));
                AddChild(line);
            }
        }
        foreach (var slot in Enum.GetValues<H2AConfig.Slot>().Skip(1).Cast<int>())
        {
            var stone = new H2AStone
            {
                TargetSlot = slot,
                CurrentSlot = (int)Config.Placements[slot],
            };
            stone.Position = SlotPosition(stone.CurrentSlot);
            stone.Interact += () => RequestMove(stone);
            _stoneMap[slot] = stone;
            AddChild(stone);
        }
    }

    private void RequestMove(H2AStone stone)
    {
        var values = Enum.GetValues<H2AConfig.Slot>().Cast<int>().ToList();
        foreach (var item in _stoneMap.Values)
        {
            values.Remove(item.CurrentSlot);
        }
        Debug.Assert(values.Count == 1);
        var slot = values.First();
        var array = (Array<int>)Config.Collections[stone.CurrentSlot];
        if (array.Contains(slot))
        {
            MoveStone(stone, slot);
        }
    }

    private void MoveStone(H2AStone stone, int slot)
    {
        stone.CurrentSlot = slot;
        var tween = CreateTween();

        tween.SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Sine);
        tween.TweenProperty(stone, (string)H2AStone.PropertyName.Position, SlotPosition(slot), 0.2f);
        tween.TweenInterval(1f);
        tween.TweenCallback(new Callable(Check));
    }
    private void Check()
    {
        foreach (var item in _stoneMap.Values)
        {
            if (item.TargetSlot != item.CurrentSlot)
            {
                return;
            }
        }
        Game.Flags.Add("h2a_unlocked");
        SceneChanger.ChangeSceneToFile("res://Scenes/H2.tscn");
    }

    public override void _Draw()
    {
        base._Draw();
        foreach (var slot in System.Enum.GetValues<H2AConfig.Slot>())
        {
            DrawTexture(SlotTexture, SlotPosition((int)slot) - SlotTexture.GetSize() / 2.0f);
        }
    }
    private Vector2 SlotPosition(int slot)
    {
        return Vector2.Down.Rotated(Mathf.Tau / System.Enum.GetValues<H2AConfig.Slot>().Length * slot) * Radius;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}
