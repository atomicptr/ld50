using System;
using Godot;
using LD50.Common;

namespace LD50.Entities {
    public enum ActionPromptEvent {
        WateringCan,
        Hoe,
        Seed,
        Water,
    }

    public class ActionPrompt : Node2D {
        [GetNode("IconPrompt")] private Sprite iconPrompt;
        [GetNode("ButtonPrompt")] private Sprite buttonPrompt;
        [GetNode("AnimationPlayer")] private AnimationPlayer animationPlayer;

        private bool showKeyboardPrompt = true;

        private readonly Rect2 keyboardButtonPromptRect = new Rect2(16, 0, 16, 16);
        private readonly Rect2 xboxGamePadButtonPromptRect = new Rect2(0, 0, 16, 16);

        private readonly Rect2 wateringCanPromptRect = new Rect2(0, 16, 16, 16);
        private readonly Rect2 hoePromptRect = new Rect2(16, 16, 16, 16);
        private readonly Rect2 seedPromptRect = new Rect2(32, 16, 16, 16);
        private readonly Rect2 waterPromptRect = new Rect2(48, 16, 16, 16);

        public override void _Ready() {
            GetNodeAttribute.Load(this);

            Hide();

            animationPlayer.CurrentAnimation = "Running";
            animationPlayer.Play();
        }

        public override void _UnhandledInput(InputEvent @event) {
            if (@event is InputEventKey) {
                showKeyboardPrompt = true;
            } else if (@event is InputEventJoypadButton || @event is InputEventJoypadMotion) {
                showKeyboardPrompt = false;
            }

            buttonPrompt.RegionRect = showKeyboardPrompt ? keyboardButtonPromptRect : xboxGamePadButtonPromptRect;
        }

        public void ShowPrompt(ActionPromptEvent @event) {
            switch (@event) {
                case ActionPromptEvent.WateringCan:
                    iconPrompt.RegionRect = wateringCanPromptRect;
                    break;
                case ActionPromptEvent.Hoe:
                    iconPrompt.RegionRect = hoePromptRect;
                    break;
                case ActionPromptEvent.Seed:
                    iconPrompt.RegionRect = seedPromptRect;
                    break;
                case ActionPromptEvent.Water:
                    iconPrompt.RegionRect = waterPromptRect;
                    break;
            }

            Show();
        }
    }
}
