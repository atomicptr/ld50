using Godot;
using LD50.Constants;

namespace LD50.Autoload {
    public enum ControllerType {
        XboxController,
        PS4Controller,
    }

    public class InputDeviceHandler : Node {
        private static InputDeviceHandler instance;

        public static InputDeviceHandler Instance {
            get => instance;
        }

        public static Icon InteractIcon {
            get => instance.interactIcon;
        }

        private Icon interactIcon;
        private bool showKeyboardPrompt = true;
        private ControllerType controllerType = ControllerType.XboxController;

        public override void _Ready() {
            instance = this;
            PauseMode = PauseModeEnum.Process;
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
                interactIcon = Icon.ButtonPromptKeyboard;
                return;
            }

            switch (controllerType) {
                case ControllerType.XboxController:
                    interactIcon = Icon.ButtonPromptXboxInteract;
                    break;
                case ControllerType.PS4Controller:
                    interactIcon = Icon.ButtonPromptPSInteract;
                    break;
            }
        }

        private static ControllerType determineControllerTypeByName(string name) {
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
    }
}
