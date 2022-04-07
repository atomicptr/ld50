using Godot;
using System.Collections.Generic;
using LD50.UserInterface.ShopMenu;

namespace LD50.Autoload {
    // HACK: fast way to have shared state between player and shop menu, cuz player decides what is sold
    public class GlobalState : Node {
        private static GlobalState instance;

        public static GlobalState Instance {
            get => instance;
        }

        public Dictionary<MenuItemEntryIdentifier, MenuItemEntry> ShopItems;
        public bool SoilUpgradePurchased = false;

        public override void _Ready() {
            instance = this;
            PauseMode = PauseModeEnum.Process;
        }
    }
}
