using Godot;

namespace LD50.Autoload {
    public class EventBus : Node {
        [Signal]
        public delegate void TurnChanged(int turn);

        [Signal]
        public delegate void MoneyValueChanged(int money);

        [Signal]
        public delegate void WateringCanAmountChanged(int amount);

        [Signal]
        public delegate void SeedAmountChanged(int seed);

        private static EventBus instance;

        public static EventBus Instance {
            get => instance;
        }

        public override void _Ready() {
            instance = this;
        }

        public static void Emit(string signal, params object[] args) {
            instance.EmitSignal(signal, args);
        }

        public static void ConnectEvent(string signal, Object target, string methodName) {
            instance.Connect(signal, target, methodName);
        }
    }
}
