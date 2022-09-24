using Godot;
using Isoland.Items;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Isoland.Globals;



public partial class Flags : Node
{
    [Signal]
    public delegate void ChangeEventHandler(string flag);
    private readonly HashSet<string> _flags = new();
    public void Add(string flag)
    {
        if (_flags.Add(flag))
        {
            EmitSignal(SignalName.Change, flag);
        }
    }
    public bool Has(string flag) => _flags.Contains(flag);
}
public partial class Invertory : Node
{
    [Signal]
    public delegate void ChangeEventHandler();
    private readonly List<Item> _item = new();
    public Item ActiveItem { get; set; }
    private int _index = -1;
    public int Count => _item.Count;
    public Item Current => _index == -1 ? null : _item[_index];

    public void AddItem(Item item)
    {
        if (!_item.Any(p => p == item))
        {
            _item.Add(item);
            _index = Count - 1;
            EmitSignal(SignalName.Change);
        }
    }

    public void RemoveItem(Item item)
    {
        if (_item.Remove(item))
        {
            //移除之后自动指向前一位
            _index--;
            //如果是移除的第一个，并且存在其它的，则重新指向0
            if (_item.Any())
            {
                _index = 0;
            }
            EmitChangeSignal();
        }
    }

    private void EmitChangeSignal() => EmitSignal(SignalName.Change);
    public void Next()
    {
        if (_index == -1)
        {
            return;
        }
        _index++;
        if (_index == _item.Count)
        {
            _index = 0;
        }
        EmitChangeSignal();
    }
    public void Prev()
    {
        if (_index == -1)
        {
            return;
        }
        _index--;
        if (_index == -1)
        {
            _index = _item.Count - 1;
        }
        EmitChangeSignal();
    }
}
public partial class Game : Node
{
    private Flags _flags;
    private static Game Singleton;
    
    private Invertory _invertory;
    public static Flags Flag => Singleton._flags;
    public static Invertory Invertory => Singleton._invertory;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Singleton = this;
        _flags = new();
        _invertory = new();
    }
}
