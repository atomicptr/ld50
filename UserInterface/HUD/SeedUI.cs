using Godot;
using LD50.Autoload;
using LD50.Common;

namespace LD50.UserInterface.GameUserInterface {
    public class SeedUI : NinePatchRect {
        [GetNode("Label")] private Label label;

        public override void _Ready() {
            GetNodeAttribute.Load(this);

            EventBus.ConnectEvent(nameof(EventBus.SeedAmountChanged), this, nameof(onSeedAmountChanged));
        }

        private void onSeedAmountChanged(int seeds) {
            label.Text = seeds.ToString();
        }
    }
}
