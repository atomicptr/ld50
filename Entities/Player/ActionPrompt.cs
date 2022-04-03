using System;
using Godot;
using LD50.Common;
using LD50.Constants;

namespace LD50.Entities {
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
                controllerType = DetermineControllerTypeByName(
                    Input.GetJoyName((int) Input.GetConnectedJoypads()[@event.Device])
                );
            }

            if (showKeyboardPrompt) {
                buttonPrompt.RegionRect = Icon.ButtonPromptKeyboard.ToRect();
                return;
            }

            switch (controllerType) {
                case ControllerType.XboxController:
                    buttonPrompt.RegionRect = Icon.ButtonPromptXboxInteract.ToRect();
                    break;
                case ControllerType.PS4Controller:
                    buttonPrompt.RegionRect = Icon.ButtonPromptPSInteract.ToRect();
                    break;
            }
        }

        public static ControllerType DetermineControllerTypeByName(string name) {
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

        public void ShowPrompt(Icon icon) {
            iconPrompt.RegionRect = icon.ToRect();
            Show();
        }
    }
}
