using System.Collections.Generic;
using Godot;
using LD50.Common;
using LD50.Constants;

namespace LD50.UserInterface {
    public class FloatingTextManager : Node2D {
        [Export] private Vector2 travelPath = new Vector2(0, -20);
        [Export] private float duration = 1.0f;
        [Export] private float cooldownDuration = 0.2f;

        private Timer cooldownTimer;

        private readonly struct FloatingTextRequest {
            public readonly Icon Icon;
            public readonly int Value;

            public FloatingTextRequest(Icon icon, int value) {
                Icon = icon;
                Value = value;
            }
        }

        private Queue<FloatingTextRequest> queue = new Queue<FloatingTextRequest>();

        private static readonly PackedScene floatingTextScene =
            GD.Load<PackedScene>("res://UserInterface/FloatingText/FloatingText.tscn");

        public override void _Ready() {
            cooldownTimer = new Timer();
            AddChild(cooldownTimer);

            cooldownTimer.OneShot = true;
            cooldownTimer.Autostart = false;
            cooldownTimer.WaitTime = cooldownDuration;
        }

        public void Spawn(Icon icon, int value) {
            queue.Enqueue(new FloatingTextRequest(icon, value));

            if (!cooldownTimer.IsStopped()) {
                return;
            }

            // fire out instantly
            spawnNextFloatingText();
            cooldownTimer.Start();
        }

        public override void _Process(float delta) {
            GD.Print(cooldownTimer.TimeLeft, "-- Q: ", queue.Count);

            if (cooldownTimer != null && cooldownTimer.IsStopped() && queue.Count > 0) {
                spawnNextFloatingText();
                cooldownTimer.Start();
            }
        }

        private void spawnNextFloatingText() {
            if (queue.Count == 0) {
                return;
            }

            var floatingTextRequest = queue.Dequeue();
            renderNewFloatingText(floatingTextRequest.Icon, floatingTextRequest.Value);
        }

        private void renderNewFloatingText(Icon icon, int value) {
            var floatingText = floatingTextScene.Instance<FloatingText>();
            AddChild(floatingText);
            floatingText.Render(icon, value, travelPath, duration);
        }
    }
}
