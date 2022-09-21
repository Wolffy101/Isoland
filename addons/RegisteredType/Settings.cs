using Godot;
using Godot.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace RegisteredType;

public static class Settings
{
    public enum ResourceSearchType
    {
        Recursive = 0,
        Namespace = 1,
    }


    public static string ClassPrefix => GetSettings(nameof(ClassPrefix)).ToString();
    public static ResourceSearchType SearchType => (ResourceSearchType)(int)GetSettings(nameof(SearchType));
    public static IReadOnlyCollection<string> ResourceScriptDirectories => (Array<string>)GetSettings(nameof(ResourceScriptDirectories));

    public static void Init()
    {
        AddSetting(nameof(ClassPrefix), Variant.Type.String, "");
        AddSetting(nameof(SearchType), Variant.Type.Int, (int)ResourceSearchType.Recursive, PropertyHint.Enum, "Recursive,Namespace");
        AddSetting(nameof(ResourceScriptDirectories), Variant.Type.PackedStringArray, new Array<string>(new string[] { "res://" }));
    }

    public static void Remove()
    {
        RemoveSetting(nameof(ClassPrefix));
        RemoveSetting(nameof(SearchType));
        RemoveSetting(nameof(ResourceScriptDirectories));
    }

    private static Variant GetSettings(string title)
    {
        var value = ProjectSettings.GetSetting(SettingPath(title));
        return value;
    }


    private static void RemoveSetting(string title)
    {
        ProjectSettings.Clear(SettingPath(title));
    }

    private static void AddSetting(string title, Variant.Type type, Variant value, PropertyHint hint = PropertyHint.None, string hintString = "")
    {
        title = SettingPath(title);
        if (!ProjectSettings.HasSetting(title))
            ProjectSettings.SetSetting(title, value);
        var info = new Dictionary
        {
            ["name"] = title,
            ["type"] = (int)type,
            ["hint"] = (int)hint,
            ["hint_string"] = hintString,
        };
        ProjectSettings.AddPropertyInfo(info);
    }

    private static string SettingPath(string title) => $"{nameof(RegisteredType)}/{title}";
}