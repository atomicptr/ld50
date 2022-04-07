using Godot;
using LD50.Autoload;
using LD50.Common;

namespace LD50.UserInterface.HUD {
    public class TurnCounterUI : NinePatchRect {
        [GetNode("Label")] private Label label;

        public override void _Ready() {
            GetNodeAttribute.Load(this);

            EventBus.ConnectEvent(nameof(EventBus.TurnChanged), this, nameof(onTurnChanged));
        }

        private void onTurnChanged(int turn) {
            label.Text = $"Turn: {turn}";
        }
    }
}
