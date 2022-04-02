using Godot;
using LD50.Autoload;
using LD50.Common;
using LD50.Scenes.Game;

namespace LD50.UserInterface.GameUserInterface {
    public class PaymentThresholdUI : NinePatchRect {
        [GetNode("Label")] private Label label;

        private int turn = 0;
        private int paymentThreshold = 0;

        public override void _Ready() {
            GetNodeAttribute.Load(this);

            EventBus.ConnectEvent(nameof(EventBus.TurnChanged), this, nameof(onTurnChanged));
            EventBus.ConnectEvent(nameof(EventBus.NewPaymentThreshold), this, nameof(onNewPaymentThresholdReceived));
        }

        private void onTurnChanged(int newTurn) {
            turn = newTurn;
            updateLabel();
        }

        private void onNewPaymentThresholdReceived(int newThreshold) {
            paymentThreshold = newThreshold;
            updateLabel();
        }

        private void updateLabel() {
            var dueInTurns = paymentThreshold - turn;
            label.Text = $"Next payment is due in {dueInTurns} turns.";
        }
    }
}
