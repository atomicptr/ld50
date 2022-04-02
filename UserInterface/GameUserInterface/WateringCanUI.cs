using System;
using Godot;
using LD50.Autoload;
using LD50.Common;

namespace LD50.UserInterface.GameUserInterface {
    public class WateringCanUI : NinePatchRect {
        [GetNode("Label")] private Label label;

        public override void _Ready() {
            GetNodeAttribute.Load(this);

            EventBus.ConnectEvent(nameof(EventBus.WateringCanAmountChanged), this, nameof(onWateringCanAmountChanged));
        }

        private void onWateringCanAmountChanged(int amount) {
            label.Text = amount.ToString();
        }
    }
}
