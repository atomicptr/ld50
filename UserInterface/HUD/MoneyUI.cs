using System;
using Godot;
using LD50.Autoload;
using LD50.Common;

namespace LD50.UserInterface.GameUserInterface {
    public class MoneyUI : NinePatchRect {
        [GetNode("Label")] private Label label;

        public override void _Ready() {
            GetNodeAttribute.Load(this);

            EventBus.ConnectEvent(nameof(EventBus.MoneyValueChanged), this, nameof(onMoneyValueChanged));
        }

        private void onMoneyValueChanged(int amount) {
            label.Text = amount.ToString("N0");
        }
    }
}
