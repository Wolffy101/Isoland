using Godot;
using Isoland.Items;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Isoland.Globals;



public partial class Flags : Node
{
    [Signal]
    public delegate void ChangeEventHandler();
    private readonly HashSet<string> _flags = new();
    public void Add(string flag)
    {
        if (_flags.Add(flag))
        {
            EmitChangeSignal();
        }
    }
    private void EmitChangeSignal() => EmitSignal(SignalName.Change);
    public bool Has(string flag) => _flags.Contains(flag);

    public IEnumerable<string> SaveData
    {
        get => _flags;
        set
        {
            _flags.Clear();
            foreach (var item in value)
            {
                _flags.Add(item);
            }
            EmitChangeSignal();
        }
    }
    public void Reset() => _flags.Clear();
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
            EmitChangeSignal();
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

    public (IEnumerable<string> item, int index) SaveData
    {

        get => (_item.Select(p => p.ResourcePath), _index);
        set
        {
            _index = value.index;
            _item.Clear();
            foreach (var item in value.item)
            {
                _item.Add(GD.Load<Item>(item));
            }
            EmitChangeSignal();
        }
    }
    public void Reset()
    {
        _index = -1;
        _item.Clear();
        EmitChangeSignal();
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

    public class Data
    {
        public IEnumerable<string> Items { get; set; }
        public int ItemIndex { get; set; }

        public IEnumerable<string> Flags { get; set; }

        public string CurrentScene { get; set; }
    }
    private const string SavePath = "user://data.sav";
    private Flags _flags;
    private static Game Singleton;

    private Invertory _invertory;
    public static Flags Flags => Singleton._flags;
    public static Invertory Invertory => Singleton._invertory;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Singleton = this;
        _flags = new();
        _invertory = new();
    }
    public static void SaveGame()
    {
        using var file = new File();
        if (file.Open(SavePath, File.ModeFlags.Write) != Error.Ok)
        {
            return;
        }
        var (items, index) = Invertory.SaveData;
        var data = new Data
        {
            Items = items,
            ItemIndex = index,
            Flags = Flags.SaveData,
            CurrentScene = Singleton.GetTree().CurrentScene.SceneFilePath,
        };
        var json = System.Text.Json.JsonSerializer.Serialize(data);
        file.StoreString(json);
    }
    public static void LoadGame()
    {
        using var file = new File();
        if (file.Open(SavePath, File.ModeFlags.Read) != Error.Ok)
        {
            return;
        }
        var json = file.GetAsText();
        var data = System.Text.Json.JsonSerializer.Deserialize<Data>(json);
        Invertory.SaveData = (data.Items, data.ItemIndex);
        Flags.SaveData = data.Flags;
        SceneChanger.ChangeSceneToFile(data.CurrentScene);
    }
    public static void NewGame()
    {
        Invertory.Reset();
        Flags.Reset();
        SceneChanger.ChangeSceneToFile("res://Scenes/H1.tscn");
    }
    public static bool HasSaveFile => File.FileExists(SavePath);
    internal static void BackToTitle()
    {
        SaveGame();
        SceneChanger.ChangeSceneToFile("res://UI/TitleScreen.tscn");
    }
}
