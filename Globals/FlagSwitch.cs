using Godot;
using System;
namespace Isoland.Globals;

[RegisteredType]
public partial class FlagSwitch : Node2D
{
    [Export]
    public string Flag { get; set; }

    //Flag 不存在默认展示的节点
    private Node2D _defaultNode;

    //flag 存在时展示的节点
    private Node2D _swtichNode;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // var nodeCount = GetChildCount();
        // if (nodeCount > 0)
        // {
        //     _defaultNode = GetChild<Node2D>(0);
        // }
        // if (nodeCount > 1)
        // {
        //     _swtichNode = GetChild<Node2D>(1);
        // }

        //使用此方法，内部会进行上面的判断
        _defaultNode = GetChildOrNull<Node2D>(0);
        _swtichNode = GetChildOrNull<Node2D>(1);

        Game.Flags.Change += UpdateNode;
        UpdateNode();
    }
    public override void _ExitTree()
    {
        Game.Flags.Change -= UpdateNode;
    }


    private void UpdateNode()
    {
        var exsits = Game.Flags.Has(Flag);
        if (_defaultNode is not null)
        {
            _defaultNode.Visible = !exsits;
        }
        if (_swtichNode is not null)
        {
            _swtichNode.Visible = exsits;
        }

    }
    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}
