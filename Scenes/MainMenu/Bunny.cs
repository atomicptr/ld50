using Godot;
using LD50.Common;

namespace LD50.Scenes.MainMenu {
    public class Bunny : NinePatchRect {
        [GetNode("AnimationPlayer")] private AnimationPlayer animationPlayer;

        public override void _Ready() {
            GetNodeAttribute.Load(this);

            animationPlayer.CurrentAnimation = "Spin2Win";
            animationPlayer.Play();
        }
    }
}
