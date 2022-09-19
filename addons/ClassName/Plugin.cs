#if TOOLS
using Godot;
using System.Reflection;
using ClassName.Attributes;
using System.Collections.Generic;
using System;
using System.IO;

namespace ClassName
{
    [Tool]
    public partial class Plugin : EditorPlugin
    {
        private List<string> _customTypes;

        public override void _EnterTree()
        {
            _customTypes = new List<string>();
            BuildTypes();
            Connect("resource_saved", new Callable(this, "OnResourceSaved"));
            AddToolMenuItem("Reload C# Resources", new Callable(this, nameof(BuildTypes)));
        }

        public override void _ExitTree()
        {
            RemoveTypes();
            RemoveToolMenuItem("Reload C# Resources");
        }

        public void OnResourceSaved(Resource resource)
        {
            BuildTypes();
        }

        private void RemoveTypes()
        {
            if (_customTypes == null) return;
            foreach (var t in _customTypes)
                RemoveCustomType(t);
        }

        public void BuildTypes(object ud) => BuildTypes();
        public void BuildTypes()
        {
            RemoveTypes(); // Prevent duplicates of the types.
            AddCustomType("Test", "Area2D", null, null);
            _customTypes = new List<string>();
            var assembly = Assembly.GetExecutingAssembly();
            foreach (var t in assembly.GetTypes())
            {
                ClassNameAttribute typeAttr = t.GetCustomAttribute<ClassNameAttribute>();
                if (typeAttr == null) continue;
                if (!t.IsSubclassOf(typeof(Godot.Resource)) && !t.IsSubclassOf(typeof(Godot.Node)))
                {
                    GD.PrintErr($"[{t}]: ClassNameAttribute only works with Resources or Nodes.");
                    continue;
                }

                IconAttribute icon = t.GetCustomAttribute<IconAttribute>();

                Script script = ResourceLoader.Load<Script>(typeAttr.ScriptPath);
                var imagePath = icon?.ImagePath ?? "icon.png";
                var texture = ResourceLoader.Load<Texture2D>(imagePath);

                var type = $"{t.Name} ({t.Name}.cs)";
                var @base = GetBaseName(t.BaseType);
                AddCustomType(type, @base, script, texture);
                _customTypes.Add(type);
            }
        }

        private string GetBaseName(Type baseType)
        {
            if (baseType.BaseType == null)
            {
                return baseType.Name;
            }
            if ("GodotSharp.dll".Equals(Path.GetFileName(baseType.Assembly.Location), StringComparison.OrdinalIgnoreCase))
            {
                return baseType.Name;
            }
            return GetBaseName(baseType.BaseType);
        }
    }
}
#endif
