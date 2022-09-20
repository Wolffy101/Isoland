using ClassName.Attributes;
using Godot;
using Isoland.Globals;

namespace Isoland.Objects;
[ClassName]
public partial class Teleporter : Interactable
{

    [Export(PropertyHint.File, "*.tscn")]
    private string _targetPath;
    protected override void InteractInput()
    {
        base.InteractInput();
        SceneChanger.Current.ChangeSceneToFile(_targetPath);
    }
}
