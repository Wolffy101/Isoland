using Godot;
using Isoland.Globals;
using System;
namespace Isoland.UI;
public partial class TitleScreen : TextureRect
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GetNode<Button>("VBoxContainer/Load").Disabled = !Game.HasSaveFile;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	private void OnLoadPressed() => Game.LoadGame();
	private void OnNewPressed() => Game.NewGame();
	private void OnQuitPressed() => GetTree().Quit();
}
