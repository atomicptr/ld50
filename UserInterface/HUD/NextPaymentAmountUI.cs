using Godot;
using LD50.Autoload;
using LD50.Common;

namespace LD50.UserInterface.HUD {
    public class NextPaymentAmountUI : NinePatchRect {
        [GetNode("Label")] private Label label;

        public override void _Ready() {
            GetNodeAttribute.Load(this);
            EventBus.ConnectEvent(nameof(EventBus.NextPaymentThresholdAnnounced), this, nameof(onNewPaymentThresholdAnnounced));
        }

        private void onNewPaymentThresholdAnnounced(int newDeadline, int amount) {
            label.Text = $"-{amount:N0}";
        }
    }
}
