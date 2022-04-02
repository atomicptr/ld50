using System;
using Godot;
using LD50.Common;
using LD50.Constants;
using LD50.Scenes.Game;

namespace LD50.Entities {
    public class Player : Node2D {
        [Signal] public delegate void MoneyValueChanged(int money);
        [Signal] public delegate void WateringCanAmountChanged(int amount);

        [Export]
        public int Money {
            get => money;
            set {
                money = Mathf.Clamp(value, 0, int.MaxValue);
                EmitSignal(nameof(MoneyValueChanged), money);
            }
        }
        private int money;

        [Export]
        public int WateringCanAmount {
            get => WateringCanAmount;
            set {
                wateringCanAmount = Mathf.Clamp(value, 0, WateringCanMaximum);
                EmitSignal(nameof(WateringCanAmountChanged), wateringCanAmount);
            }
        }
        private int wateringCanAmount;

        [Export] public readonly int WateringCanMaximum = 5;

        [GetNode("/root/Game/Grid")] private Grid grid;
        [GetNode("MoveCooldown")] private Timer moveCooldown;

        private Vector2 playerGridPosition = Vector2.Zero;

        public override void _Ready() {
            GetNodeAttribute.Load(this);
        }

        public override void _Process(float delta) {
            playerGridPosition = grid.WorldToMap(Position);
            var direction = determineDirection();

            // TODO: add an actual button prompt...
            Modulate = canInteract() ? Colors.Green : Colors.White;

            if (canInteract() && Input.IsActionJustPressed(InputActions.INTERACT)) {
                interact();
            }

            if (direction == Vector2.Zero) {
                return;
            }

            if (isCurrentlyMoving()) {
                return;
            }

            var targetPos = playerGridPosition + direction;

            if (!grid.IsVacant(targetPos)) {
                return;
            }

            // TODO: add animation
            Position = grid.MapToWorld(playerGridPosition + direction);

            grid.NextTurn();
            moveCooldown.Start();
        }

        private Vector2 determineDirection() {
            if (Input.IsActionPressed(InputActions.MOVE_LEFT)) {
                return Vector2.Left;
            } else if (Input.IsActionPressed(InputActions.MOVE_RIGHT)) {
                return Vector2.Right;
            } else if (Input.IsActionPressed(InputActions.MOVE_UP)) {
                return Vector2.Up;
            } else if (Input.IsActionPressed(InputActions.MOVE_DOWN)) {
                return Vector2.Down;
            }

            return Vector2.Zero;
        }

        private bool isCurrentlyMoving() {
            return moveCooldown.TimeLeft > 0.00001f;
        }

        private bool canInteract() {
            var currentTile = grid.CellAt(playerGridPosition);

            if (!currentTile.HasValue) {
                return false;
            }

            if (currentTile.Value.IsUntouchedFarmPlot()) {
                return true;
            }

            if (currentTile.Value.IsPlowedFarmPlot() && !grid.IsFarmPlotWatered(playerGridPosition)) {
                return true;
            }

            return false;
        }

        private void interact() {
            var currentTile = grid.CellAt(playerGridPosition);

            if (!currentTile.HasValue) {
                return;
            }

            var targetTile = currentTile.Value;

            if (targetTile.IsUntouchedFarmPlot()) {
                grid.PlowFarmPlot(playerGridPosition);
                return;
            }

            if (targetTile.IsPlowedFarmPlot() && !grid.IsFarmPlotWatered(playerGridPosition)) {
                grid.WaterFarmPlot(playerGridPosition);
            }

            // TODO: if plot is watered (and has no plant), offer seed selection
            // TODO: if plot has plant and is fully grown, grab produce and reset field to base state
            // TODO: if plot has plant and is not watered (anymore) water again
        }
    }
}
