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

    public enum ControllerType {
        XboxController,
        PS4Controller,
    }

    public class ActionPrompt : Node2D {
        [GetNode("IconPrompt")] private Sprite iconPrompt;
        [GetNode("ButtonPrompt")] private Sprite buttonPrompt;
        [GetNode("AnimationPlayer")] private AnimationPlayer animationPlayer;

        private bool showKeyboardPrompt = true;
        private ControllerType controllerType = ControllerType.XboxController;

        private readonly Rect2 xboxGamePadButtonPromptRect = new Rect2(0, 0, 16, 16);
        private readonly Rect2 keyboardButtonPromptRect = new Rect2(16, 0, 16, 16);
        private readonly Rect2 psGamePadButtonPromptRect = new Rect2(32, 0, 16, 16);

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
                controllerType = determineControllerTypeByName(
                    Input.GetJoyName((int) Input.GetConnectedJoypads()[@event.Device])
                );
            }

            if (showKeyboardPrompt) {
                buttonPrompt.RegionRect = keyboardButtonPromptRect;
                return;
            }

            switch (controllerType) {
                case ControllerType.XboxController:
                    buttonPrompt.RegionRect = xboxGamePadButtonPromptRect;
                    break;
                case ControllerType.PS4Controller:
                    buttonPrompt.RegionRect = psGamePadButtonPromptRect;
                    break;
            }
        }

        private ControllerType determineControllerTypeByName(string name) {
            switch (name.ToLower()) {
                case "xbox one controller":
                case "xinput gamepad":
                    return ControllerType.XboxController;
                case "ps4 controller":
                    return ControllerType.PS4Controller;
                // TODO: add more controllers...
            }

            // If everything fails, just fall back to xbox controller
            return ControllerType.XboxController;
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
