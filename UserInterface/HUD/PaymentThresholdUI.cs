using Godot;
using LD50.Autoload;
using LD50.Common;
using LD50.Scenes.Game;

namespace LD50.UserInterface.GameUserInterface {
    public class PaymentThresholdUI : NinePatchRect {
        [GetNode("Label")] private Label label;

        private int deadline = GameManager.PAYMENT_THRESHOLD;

        public override void _Ready() {
            GetNodeAttribute.Load(this);
            EventBus.ConnectEvent(nameof(EventBus.TurnChanged), this, nameof(onTurnChanged));
            EventBus.ConnectEvent(nameof(EventBus.NextPaymentThresholdAnnounced), this, nameof(onNewPaymentThresholdAnnounced));
        }

        private void onTurnChanged(int turn) {
            var dueInTurns = deadline - turn;
            label.Text = $"Next payment is due in {dueInTurns} turns.";
        }

        private void onNewPaymentThresholdAnnounced(int newDeadline) {
            deadline = newDeadline;
        }
    }
}
