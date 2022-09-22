using Godot;
namespace Isoland.Items;

[RegisteredType]
[Tool]
public partial class Item : Resource
{
    [Export(PropertyHint.MultilineText)]
    public string Description { get; set; } = string.Empty;

    private Texture2D _propTexture;
    [Export]
    public Texture2D PropTexture { get; set; }

    private Texture2D _sceneTexture;
    [Export]
    public Texture2D SceneTexture { get; set; }
}
