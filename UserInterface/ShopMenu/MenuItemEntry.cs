using LD50.Constants;

namespace LD50.UserInterface.ShopMenu {
    public enum MenuItemEntryIdentifier {
        Bu5SeedAmount5,
        Bu5SeedAmount10,
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
