using System.Linq;
using Godot;
using System.Collections.Generic;
using System.Security.Policy;
using LD50.Autoload;
using LD50.Common;
using LD50.Constants;
using LD50.Entities;

namespace LD50.UserInterface.ShopMenu {
    public class ShopMenuController : Control {
        private bool isOpen;

        private static readonly PackedScene menuItemScene =
            GD.Load<PackedScene>("res://UserInterface/ShopMenu/MenuItem.tscn");

        [GetNode("Container/CenterContainer/VBoxContainer/MenuItems")]
        private VBoxContainer menuItems;

        [GetNode("InputCooldown")] private Timer inputCooldown;

        private MenuItem selectedMenuItem = null;

        public override void _Ready() {
            GetNodeAttribute.Load(this);

            EventBus.ConnectEvent(nameof(EventBus.OpenShopMenu), this, nameof(OpenMenu));
        }

        public override void _Process(float delta) {
            if (!isOpen) {
                return;
            }

            if (! inputCooldown.IsStopped()) {
                return;
            }

            if (Input.IsActionJustPressed(InputActions.UI_UP)) {
                selectPreviousMenuItem();
            } else if (Input.IsActionJustPressed(InputActions.UI_DOWN)) {
                selectNextMenuItem();
            } else if (Input.IsActionJustPressed(InputActions.UI_ACCEPT)) {
                // TODO: maybe play an animation?
                EventBus.Emit(nameof(EventBus.ShopItemPurchased), selectedMenuItem.Identifier);
            } else if (Input.IsActionJustPressed(InputActions.UI_CANCEL)) {
                CloseMenu();
            }

            foreach (var item in menuItems.GetChildren().OfType<MenuItem>()) {
                var selector = item.GetNodeOrNull<NinePatchRect>("Selector");

                if (selector != null) {
                    selector.RegionRect = InputDeviceHandler.InteractIcon.ToRect();
                }
            }
        }

        public void OpenMenu() {
            destroyMenuItems();
            createMenuItems(GlobalState.Instance.ShopItems);

            isOpen = true;

            GetTree().Paused = isOpen;
            Visible = isOpen;

            inputCooldown.Start();
        }

        public void CloseMenu() {
            destroyMenuItems();
            isOpen = false;

            GetTree().Paused = isOpen;
            Visible = isOpen;
        }

        private void createMenuItems(Dictionary<MenuItemEntryIdentifier, MenuItemEntry> menuItemEntries) {
            bool isFirst = true;
            foreach (var entry in menuItemEntries.Values) {
                var menuItem = menuItemScene.Instance<MenuItem>();
                menuItem.Identifier = entry.Identifier;
                menuItem.Icon = entry.Icon;
                menuItem.Name = entry.Name;
                menuItem.Cost = entry.Cost;

                if (isFirst) {
                    menuItem.IsSelected = true;
                    selectedMenuItem = menuItem;
                    isFirst = false;
                }

                menuItems.AddChild(menuItem);
            }
        }

        private void destroyMenuItems() {
            foreach (var child in menuItems.GetChildren().OfType<Node>()) {
                child.QueueFree();
            }
        }

        private void selectNextMenuItem() {
            bool foundCurrent = false;
            bool foundAnElement = false;

            foreach (var menuItem in menuItems.GetChildren().OfType<MenuItem>()) {
                // if we already found current the next one is the next node
                if (foundCurrent) {
                    selectedMenuItem.IsSelected = false;
                    menuItem.IsSelected = true;
                    selectedMenuItem = menuItem;
                    foundAnElement = true;
                    break;
                }

                if (menuItem.Identifier == selectedMenuItem.Identifier) {
                    foundCurrent = true;
                }
            }

            // this either means something is horribly broken or that we've reached the end making the first child
            // the new selected one...
            if (!foundAnElement) {
                selectedMenuItem.IsSelected = false;
                selectedMenuItem = menuItems.GetChildren().OfType<MenuItem>().First();
                selectedMenuItem.IsSelected = true;
            }
        }

        private void selectPreviousMenuItem() {
            MenuItem previous = null;

            foreach (var menuItem in menuItems.GetChildren().OfType<MenuItem>()) {
                if (menuItem.Identifier == selectedMenuItem.Identifier) {
                    break;
                }

                previous = menuItem;
            }

            // if this is still null, this matched on the first element, meaning the previous one is the last one
            if (previous == null) {
                previous = menuItems.GetChild<MenuItem>(menuItems.GetChildCount() - 1);
            }

            selectedMenuItem.IsSelected = false;
            previous.IsSelected = true;

            selectedMenuItem = previous;
        }
    }
}
