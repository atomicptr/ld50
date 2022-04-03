using LD50.Constants;

namespace LD50.UserInterface.ShopMenu {
    public enum MenuItemEntryIdentifier {
        BuySeedAmount5,
        BuySeedAmount10,
        UpgradeWateringCanSize1,
        UpgradeWateringCanSize2,
        UpgradeWateringCanSize3,
        FarmPlotUpgrade1,
        FarmPlotUpgrade2,
        FarmPlotUpgrade3,
        PlowEntireField,
        WaterEntireField,
    }

    public readonly struct MenuItemEntry {
        public readonly MenuItemEntryIdentifier Identifier;
        public readonly Icon Icon;
        public readonly string Name;
        public readonly int Cost;

        public MenuItemEntry(
            MenuItemEntryIdentifier identifier,
            Icon icon,
            string name,
            int cost,
            MenuItemEntryIdentifier[] prerequisites = null
        ) {
            Identifier = identifier;
            Icon = icon;
            Name = name;
            Cost = cost;
        }
    }
}
