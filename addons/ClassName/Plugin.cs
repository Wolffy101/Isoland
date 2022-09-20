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
            var error = Connect("resource_saved", (Delegate)((Resource p) =>
            {
                BuildTypes();
            }));
            AddToolMenuItem("Reload C# Resources", new Callable(this, nameof(BuildTypes)));
        }

        public override void _ExitTree()
        {
            RemoveTypes();
            RemoveToolMenuItem("Reload C# Resources");
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
                Script script = ResourceLoader.Load<Script>(GetScriptPath(typeAttr.ScriptPath));
                var imagePath = icon?.ImagePath ?? "icon.png";
                var texture = ResourceLoader.Load<Texture2D>(imagePath);

                var type = $"{t.Name} ({t.Name}.cs)";
                var @base = GetBaseName(t.BaseType);
                AddCustomType(t.Name, @base, script, texture);
                _customTypes.Add(t.Name);
            }
        }

        /// <summary>
        /// 获取脚本路径 
        /// </summary>
        /// <param name="scriptPath"></param>
        /// <returns></returns>
        private static string GetScriptPath(string scriptPath)
        {
            if (string.IsNullOrWhiteSpace(scriptPath)) return scriptPath;
            if (scriptPath.StartsWith("res://")) return scriptPath;
            var baseDir = System.IO.Directory.GetCurrentDirectory();
            return "res:/" + scriptPath.Replace(baseDir, "").Replace("\\", "/");
        }
        /// <summary>
        /// 获取父类类型
        /// 因为直接是父类无法展示，所有只能获取Godot 内部的定义的类，才能加载出来
        ///  目前还没有好的解决方法
        /// </summary>
        /// <param name="baseType"></param>
        private static string GetBaseName(Type baseType)
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
