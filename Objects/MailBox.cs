using Godot;
using Isoland.Globals;
using System;

namespace Isoland.Objects;
public partial class MailBox : FlagSwitch
{

    private void OnInteractableInteract()
    {
        var item = Game.Invertory.ActiveItem;
        if (item is null || item != GD.Load("Items/Key.tres"))
        {
            return;
        }
        Game.Flags.Add(Flag);
        Game.Invertory.RemoveItem(item);
    }
}
