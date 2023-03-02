using Godot;
using System;
namespace Isoland;
public partial class SoundManager : Node
{
    private static AudioStreamPlayer bgmPlayer;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        bgmPlayer = GetNode<AudioStreamPlayer>("BGMPlayer");
    }

    public static void PlayMusic(string path)
    {
        if (bgmPlayer.Playing && bgmPlayer.Stream.ResourcePath.Equals(path, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }
        bgmPlayer.Stream = GD.Load<AudioStream>(path);
        bgmPlayer.Play();
    }
}
