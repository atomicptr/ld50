using Godot;
using LD50.Constants;

namespace LD50.UserInterface {
    public class FloatingTextManager : Node2D {
        [Export] private Vector2 travelPath = new Vector2(0, -20);
        [Export] private float duration = 1.0f;

        private static readonly PackedScene floatingTextScene =
            GD.Load<PackedScene>("res://UserInterface/FloatingText/FloatingText.tscn");

        public void Spawn(Icon icon, int value) {
            var floatingText = floatingTextScene.Instance<FloatingText>();
            AddChild(floatingText);
            floatingText.Render(icon, value, travelPath, duration);
        }
    }
}
