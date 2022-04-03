using Godot;
using LD50.Autoload;
using LD50.Common;
using LD50.Constants;

namespace LD50.Entities {
    public class ActionPrompt : Node2D {
        [GetNode("IconPrompt")] private Sprite iconPrompt;
        [GetNode("ButtonPrompt")] private Sprite buttonPrompt;
        [GetNode("AnimationPlayer")] private AnimationPlayer animationPlayer;

        public override void _Ready() {
            GetNodeAttribute.Load(this);

            Hide();

            animationPlayer.CurrentAnimation = "Running";
            animationPlayer.Play();
        }

        public override void _Process(float delta) {
            buttonPrompt.RegionRect = InputDeviceHandler.InteractIcon.ToRect();
        }

        public void ShowPrompt(Icon icon) {
            iconPrompt.RegionRect = icon.ToRect();
            Show();
        }
    }
}
