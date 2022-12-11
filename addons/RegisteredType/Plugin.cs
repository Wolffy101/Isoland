using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Godot;

// Originally written by wmigor
// Edited by Atlinx to recursively search for files.
// wmigor's Public Repo: https://github.com/wmigor/godot-mono-custom-resource-register
namespace RegisteredType;

#if TOOLS
[Tool]
public partial class Plugin : EditorPlugin
{
    // We're not going to hijack the Mono Build button since it actually takes time to build
    // and we can't be sure how long that is. I guess we have to leave refreshing to the user for now.
    // There isn't any automation we can do to fix that.
    // private Button MonoBuildButton => GetNode<Button>("/root/EditorNode/@@580/@@581/@@589/@@590/ToolButton");
    private readonly List<string> customTypes = new();
    private Button refreshButton;

    public override void _EnterTree()
    {
        refreshButton = new Button
        {
            Text = "CCR"
        };

        AddControlToContainer(CustomControlContainer.Toolbar, refreshButton);
        refreshButton.Icon = refreshButton.Icon;
        refreshButton.Connect("pressed", new(this, nameof(OnRefreshPressed)));

        Settings.Init();
        RefreshCustomClasses();
        GD.PushWarning("You may change any setting for MonoCustomResourceRegistry in Project -> ProjectSettings -> General -> MonoCustomResourceRegistry");
    }

    public override void _ExitTree()
    {
        UnregisterCustomClasses();
        RemoveControlFromContainer(CustomControlContainer.Toolbar, refreshButton);
        refreshButton.QueueFree();
        Settings.Remove();
    }

    public void RefreshCustomClasses()
    {
        GD.Print("\nRefreshing Registered Resources...");
        UnregisterCustomClasses();
        RegisterCustomClasses();
    }

    private void RegisterCustomClasses()
    {
        customTypes.Clear();


        foreach (Type type in GetCustomRegisteredTypes())
            if (type.IsSubclassOf(typeof(Resource)))
                AddRegisteredType(type);
            else
                AddRegisteredType(type);
    }
    private static string GetBaseName(Type baseType)
    {
        if (baseType.BaseType == null)
        {
            return baseType.Name;
        }
        if (baseType.Assembly == typeof(Node).Assembly)
        {
            return baseType.Name;
        }
        return GetBaseName(baseType.BaseType);
    }
    private void AddRegisteredType(Type type)
    {
        var attribute = type.GetCustomAttribute<RegisteredTypeAttribute>(true);
        String path = FindClassPath(type);
        if (path == null && FileAccess.FileExists(path))
            return;

        Script script = GD.Load<Script>(path);
        if (script == null)
            return;

        string baseTypeName = GetBaseName(type);

        ImageTexture icon = null;
        string iconPath = attribute.iconPath;
        if (iconPath == "")
        {
            Type baseType = type.BaseType;
            while (baseType != null)
            {
                var baseTypeAttribute = baseType.GetCustomAttribute<RegisteredTypeAttribute>();
                if (baseTypeAttribute != null && baseTypeAttribute.iconPath != "")
                {
                    iconPath = baseTypeAttribute.iconPath;
                    break;
                }
                baseType = baseType.BaseType;
            }
        }

        if (iconPath != "")
        {
            if (FileAccess.FileExists(iconPath))
            {
                Texture2D rawIcon = ResourceLoader.Load<Texture2D>(iconPath);
                if (rawIcon != null)
                {
                    Image image = rawIcon.GetImage();
                    int length = (int)Mathf.Round(16 * GetEditorInterface().GetEditorScale());
                    image.Resize(length, length);
                    icon = new ImageTexture();
                    icon.SetImage(image);
                }
                else
                    GD.PushError($"Could not load the icon for the registered type \"{type.FullName}\" at path \"{path}\".");
            }
            else
                GD.PushError($"The icon path of \"{path}\" for the registered type \"{type.FullName}\" does not exist.");
        }

        AddCustomType($"{Settings.ClassPrefix}{type.Name}", baseTypeName, script, icon);
        customTypes.Add($"{Settings.ClassPrefix}{type.Name}");
        GD.Print($"Registered custom type: {type.Name} -> {path}");
    }

    private static string FindClassPath(Type type)
    {
        return Settings.SearchType switch
        {
            Settings.ResourceSearchType.Recursive => FindClassPathRecursive(type),
            Settings.ResourceSearchType.Namespace => FindClassPathNamespace(type),
            _ => throw new Exception($"ResourceSearchType {Settings.SearchType} not implemented!"),
        };
    }

    private static string FindClassPathNamespace(Type type)
    {
        foreach (string dir in Settings.ResourceScriptDirectories)
        {
            string filePath = $"{dir}/{type.Namespace?.Replace(".", "/") ?? ""}/{type.Name}.cs";

            if (FileAccess.FileExists(filePath))
                return filePath;
        }
        return null;
    }

    private static string FindClassPathRecursive(Type type)
    {
        foreach (string directory in Settings.ResourceScriptDirectories)
        {
            string fileFound = FindClassPathRecursiveHelper(type, directory);
            if (fileFound != null)
                return fileFound;
        }
        return null;
    }

    private static string FindClassPathRecursiveHelper(Type type, string directory)
    {
        if (!DirAccess.DirExistsAbsolute(directory))
        {
            return null;
        }
        var dir = DirAccess.Open(directory);
        dir.ListDirBegin();

        while (true)
        {
            var fileOrDirName = dir.GetNext();

            // Skips hidden files like .
            if (fileOrDirName == "")
                break;
            else if (fileOrDirName.StartsWith("."))
                continue;
            else if (dir.CurrentIsDir())
            {
                string foundFilePath = FindClassPathRecursiveHelper(type, dir.GetCurrentDir() + "/" + fileOrDirName);
                if (foundFilePath != null)
                {
                    dir.ListDirEnd();
                    return foundFilePath;
                }
            }
            else if (fileOrDirName == $"{type.Name}.cs")
                return dir.GetCurrentDir() + "/" + fileOrDirName;
        }
        return null;
    }

    private static IEnumerable<Type> GetCustomRegisteredTypes()
    {
        var assembly = Assembly.GetAssembly(typeof(Plugin));
        return assembly.GetTypes().Where(t => !t.IsAbstract
            && Attribute.IsDefined(t, typeof(RegisteredTypeAttribute))
            && (t.IsSubclassOf(typeof(Node)) || t.IsSubclassOf(typeof(Resource)))
            );
    }

    private void UnregisterCustomClasses()
    {
        foreach (var script in customTypes)
        {
            RemoveCustomType(script);
            GD.Print($"Unregister custom resource: {script}");
        }

        customTypes.Clear();
    }

    private void OnRefreshPressed()
    {
        RefreshCustomClasses();
    }
}
#endif
