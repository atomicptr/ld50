using Godot;
using Godot.Collections;
using LD50.Entities;
using LD50.UserInterface.ShopMenu;

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

        [Signal]
        public delegate void NextPaymentThresholdAnnounced(int nextThreshold);

        [Signal]
        public delegate void OpenShopMenu();

        [Signal]
        public delegate void ShopItemPurchased(MenuItemEntryIdentifier identifier);

        [Signal]
        public delegate void UpgradeObtained(PlayerUpgrade upgrade);

        private static EventBus instance;
        private readonly Dictionary<string, bool> initializedEvents = new Dictionary<string, bool>();

        public static EventBus Instance {
            get => instance;
        }

        public override void _Ready() {
            instance = this;
            PauseMode = PauseModeEnum.Process;
        }

        public static void Emit(string signal, params object[] args) {
            instance.EmitSignal(signal, args);
        }

        public static void ConnectEvent(string signal, Object target, string methodName) {
            instance.Connect(signal, target, methodName);
        }

        // This serves to initialize an initial value at a later-ish state when all consumers are setup
        public static void InitializeEvent(string eventName, params object[] values) {
            if (instance.initializedEvents.ContainsKey(eventName)) {
                return;
            }
            Emit(eventName, values);
            instance.initializedEvents[eventName] = true;
        }
    }
}
