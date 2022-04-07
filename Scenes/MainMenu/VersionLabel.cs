using Godot;
using LD50.Constants;

namespace LD50.Scenes.MainMenu {
    public class VersionLabel : Label {
        public override void _Ready() {
            Text = Meta.VERSION;
        }
    }
}
