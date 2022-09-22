using Godot;
using System;
using System.Collections.Generic;
namespace Isoland.Globals;
public partial class Game : Node
{


    [Signal]
    public delegate void ChangeEventHandler(string flag);

    public partial class Flags
    {
        private readonly HashSet<string> _flags = new();
        public void Add(string flag)
        {
            if (_flags.Add(flag))
            {
                Singleton.EmitSignal(SignalName.Change, flag);
            }
        }
        public bool Has(string flag) => _flags.Contains(flag);
    }
    public Flags Flag { get; private set; }
    public static Game Singleton { get; private set; }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Singleton = this;
        Flag = new();
    }
}
