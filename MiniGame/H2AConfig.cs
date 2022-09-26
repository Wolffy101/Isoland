using Godot;
using Godot.Collections;
using System;
using System.Linq;

namespace Isoland.MiniGame;

[Tool]
[RegisteredType]
public partial class H2AConfig : Resource
{
    public enum Slot : int
    {
        Null = 0,

        Time = 1,

        Sun = 2,

        Fish = 3,

        Hill = 4,

        Cross = 5,

        Choice = 6,

    }
    private int _flags;
    private Array<Slot> _placements;
    private Dictionary _collections;

    public H2AConfig()
    {
        var slotValues = Enum.GetValues<Slot>();

        _placements = new Array<Slot>(slotValues);
        for (int i = 0; i < slotValues.Length; i++)
        {
            _placements[i] = Slot.Null;
        }

        _collections = new Dictionary();
        foreach (var item in slotValues)
        {
            _collections[(int)item] = new Array<int>();
        }
    }


    public override Array<Dictionary> _GetPropertyList()
    {
        var properties = new Array<Dictionary>
        {
            new Dictionary
            {
                ["name"] = nameof(_placements),
                ["type"] = (int)Variant.Type.Array,
                ["usage"] = (int)PropertyUsageFlags.Storage,
            },
            new Dictionary
            {
                ["name"] =nameof(_collections),
                ["type"] = (int)Variant.Type.Dictionary,
                ["usage"] = (int)PropertyUsageFlags.Storage,
            },
        };
        var values = Enum.GetValues<Slot>();
        var hintStr = string.Join(',', values: values.Select(p => p.ToString()));
        foreach (var item in values.Skip(1))
        {
            properties.Add(new Dictionary
            {
                ["name"] = $"{nameof(_placements)}/{item}",
                ["type"] = (int)Variant.Type.Int,
                ["usage"] = (int)PropertyUsageFlags.Editor,
                ["hint"] = (int)PropertyHint.Enum,
                ["hint_string"] = hintStr,
            });
        }

        foreach (var item in values.SkipLast(1))
        {
            var collectionHintStr = string.Join(',', values.Select(p => p <= item ? string.Empty : p.ToString()));
            properties.Add(new Dictionary
            {
                ["name"] = $"{nameof(_collections)}/{item}",
                ["type"] = (int)Variant.Type.Int,
                ["usage"] = (int)PropertyUsageFlags.Editor,
                ["hint"] = (int)PropertyHint.Flags,
                ["hint_string"] = collectionHintStr,
            });

        }
        return properties;
    }
    public override Variant _Get(StringName property)
    {
        string propertyName = property;
        if (propertyName.StartsWith(nameof(_placements)))
        {
            var index = propertyName.TrimStart($"{nameof(_placements)}/".ToArray());
            if (Enum.TryParse<Slot>(index, out var slot))
            {
                return (int)_placements[index: (int)slot];
            }
        }
        else if (propertyName.StartsWith(nameof(_collections)))
        {
            var name = propertyName.TrimStart($"{nameof(_collections)}/".ToArray());
            if (Enum.TryParse<Slot>(name, out var slot))
            {
                var index = (int)slot;
                var value = 0;
                foreach (var dst in (Array<int>)_collections[index])
                {
                    value |= 1 << dst;
                }
                return value;
            }
        }
        return base._Get(property);
    }
    /// <summary>
    /// NULL   000 000 
    /// TIME   000 010
    /// SUN    000 100
    /// FISH   001 000 
    /// CROSS  010 000
    /// CHOICE 100 000
    /// </summary>
    /// <param name="property"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public override bool _Set(StringName property, Variant value)
    {
        string propertyName = property;
        if (propertyName.StartsWith(nameof(_placements)))
        {
            var name = propertyName.TrimStart($"{nameof(_placements)}/".ToArray());
            if (Enum.TryParse<Slot>(name, out var slot))
            {
                _placements[(int)slot] = (Slot)(int)value;
                EmitChanged();
                return true;
            }
        }
        else if (propertyName.StartsWith(nameof(_collections)))
        {
            var name = propertyName.TrimStart($"{nameof(_collections)}/".ToArray());
            if (Enum.TryParse<Slot>(name, out var slot))
            {
                var index = (int)slot;
                for (var dst = index + 1; dst < _placements.Count; dst++)
                {
                    /*
                        例子1:
                            dst 4(Hill))-> 1<<4 = 0 001 000
                    connect value:0 001 001 = 17
                            0 001 000 & 000 100 1 = 0 010 000 = 16 > 0  
                    disconn value: 0 000 001 = 1
                            0 001 000 & 0 000 001 = 0001 000 = 0 
                        例子2:    
                            dst 4(Hill)-> 1<<4 = 0 001 000
                    connct  value:0 001 111 = 23 
                            0 001 000 & 0 001 111 = 0 001 000 = 16
                    disconn value: 000 111 = 13
                            0 001 000 & 0 000 111 = 0 000 000 = 0
                    */
                    SetCollection(src: index, dst, ((int)value & (1 << dst)) != 0);
                }
                EmitChanged();
                return true;
            }
        }
        return base._Set(property, value);
    }
    private void SetCollection(int src, int dst, bool connected)
    {
        var srcArray = (Array<int>)_collections[src];
        var dstArray = (Array<int>)_collections[dst];
        var srcIdx = srcArray.IndexOf(dst);
        var dstIdx = dstArray.IndexOf(src);
        if (connected)
        {
            if (srcIdx == -1)
            {
                srcArray.Add(dst);
            }
            if (dstIdx == -1)
            {
                dstArray.Add(src);
            }
        }
        else
        {
            if (srcIdx != -1)
            {
                // gdscrpit 没有找到视频中的remove 方法，4.0应该也换成 remove_at方法

                //C# Remove 是移除某个元素，可以换成 srcArray.Rmove(dst)
                srcArray.RemoveAt(srcIdx);
            }

            if (dstIdx != -1)
            {
                dstArray.RemoveAt(dstIdx);
            }
        }
    }
}
