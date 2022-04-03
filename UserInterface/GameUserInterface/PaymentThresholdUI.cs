using Godot;
using LD50.Autoload;
using LD50.Common;
using LD50.Scenes.Game;

namespace LD50.UserInterface.GameUserInterface {
    public class PaymentThresholdUI : NinePatchRect {
        [GetNode("Label")] private Label label;

        public override void _Ready() {
            GetNodeAttribute.Load(this);
            EventBus.ConnectEvent(nameof(EventBus.TurnChanged), this, nameof(onTurnChanged));
        }

        private void onTurnChanged(int turn) {
            var dueInTurns = GameManager.PAYMENT_THRESHOLD - turn;
            label.Text = $"Next payment is due in {dueInTurns} turns.";
        }
    }
}
