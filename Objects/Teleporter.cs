using Godot;
using Isoland.Globals;

namespace Isoland.Objects;
public partial class Teleporter : Interactable
{

    [Export(PropertyHint.File, "*.tscn")]
    private string _targetPath;
    protected override void Interact()
    {
        base.Interact();
        SceneChanger.Current.ChangeSceneToFile(_targetPath);
    }
}
