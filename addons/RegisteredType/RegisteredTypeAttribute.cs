using System;

namespace Godot;
[AttributeUsage(System.AttributeTargets.Class)]
public class RegisteredTypeAttribute : System.Attribute
{
    public string iconPath;

    public RegisteredTypeAttribute(string iconPath = "")
    {
        this.iconPath = iconPath;
    }
}