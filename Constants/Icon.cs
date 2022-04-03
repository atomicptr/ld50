using System;
using Godot;

namespace LD50.Constants {
    public enum Icon {
        // row 1
        ButtonPromptXboxInteract,
        ButtonPromptKeyboard,
        ButtonPromptPSInteract,
        // row 2
        ToolWateringCan,
        ToolHoe,
        ToolSeeds,
        ToolWater,
        Money,
        Skull,
    }

    public static class UIIconsExtension {
        public static Rect2 ToRect(this Icon icon) {
            switch (icon) {
                case Icon.ButtonPromptXboxInteract:
                    return new Rect2(0, 0, 16, 16);
                case Icon.ButtonPromptKeyboard:
                    return new Rect2(16, 0, 16, 16);
                case Icon.ButtonPromptPSInteract:
                    return new Rect2(32, 0, 16, 16);
                case Icon.ToolWateringCan:
                    return new Rect2(0, 16, 16, 16);
                case Icon.ToolHoe:
                    return new Rect2(16, 16, 16, 16);
                case Icon.ToolSeeds:
                    return new Rect2(32, 16, 16, 16);
                case Icon.ToolWater:
                    return new Rect2(48, 16, 16, 16);
                case Icon.Money:
                    return new Rect2(64, 16, 16, 16);
                case Icon.Skull:
                    return new Rect2(80, 16, 16, 16);
                default:
                    throw new ArgumentOutOfRangeException(nameof(icon), icon, null);
            }
        }
    }
}
