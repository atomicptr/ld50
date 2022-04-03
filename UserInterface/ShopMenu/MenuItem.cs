using Godot;
using LD50.Common;
using LD50.Constants;

namespace LD50.UserInterface.ShopMenu {
    public class MenuItem : HBoxContainer {
        [ExportEnumAsEnumProperty(typeof(MenuItemEntryIdentifier))]
        public MenuItemEntryIdentifier Identifier;

        [Export] public bool IsSelected = false;

        [ExportEnumAsEnumProperty(typeof(Icon))]
        public Icon Icon;

        [Export] public string Name;
        [Export] public int Cost;

        [GetNode("Selector")] private NinePatchRect selectorControl;
        [GetNode("Icon")] private NinePatchRect iconControl;
        [GetNode("Label")] private Label nameControl;
        [GetNode("Cost/Label")] private Label costControl;

        public override void _Ready() {
            GetNodeAttribute.Load(this);

            updateEntry();
        }

        public override void _Process(float delta) {
            updateEntry();
        }

        private void updateEntry() {
            selectorControl.Modulate = new Color(Colors.White, IsSelected ? 1.0f : 0.0f);
            iconControl.RegionRect = Icon.ToRect();
            nameControl.Text = Name;
            costControl.Text = $"-{Cost}";
        }
    }
}
