using Godot;
using LD50.Autoload;
using LD50.Common;
using LD50.Constants;

namespace LD50.Scenes.GameOver {
    public class ButtonPrompt : NinePatchRect {
        [GetNode("AnimationPlayer")] private AnimationPlayer animationPlayer;

        public override void _Ready() {
            GetNodeAttribute.Load(this);

            animationPlayer.Autoplay = "Pulsate";
            animationPlayer.CurrentAnimation = "Pulsate";
            animationPlayer.Play();
        }

        public override void _Process(float delta) {
            RegionRect = InputDeviceHandler.InteractIcon.ToRect();

            if (Input.IsActionJustPressed(InputActions.INTERACT)) {
                GetTree().ChangeScene("res://Scenes/Game/Game.tscn");
            }
        }
    }
}
