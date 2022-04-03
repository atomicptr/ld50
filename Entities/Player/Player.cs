using Godot;
using LD50.Autoload;
using LD50.Common;
using LD50.Constants;
using LD50.Scenes.Game;

namespace LD50.Entities {
    public class Player : Node2D {
        [Export]
        public int Money {
            get => money;
            set {
                money = Mathf.Clamp(value, 0, int.MaxValue);

                if (!IsInsideTree()) {
                    ToSignal(this, "ready").OnCompleted(() => EventBus.Emit(nameof(EventBus.MoneyValueChanged), money));
                    return;
                }

                EventBus.Emit(nameof(EventBus.MoneyValueChanged), money);
            }
        }

        private int money;

        [Export]
        public int WateringCanAmount {
            get => wateringCanAmount;
            set {
                wateringCanAmount = Mathf.Clamp(value, 0, WateringCanMaximum);

                if (!IsInsideTree()) {
                    ToSignal(this, "ready").OnCompleted(() => EventBus.Emit(nameof(EventBus.WateringCanAmountChanged), wateringCanAmount));
                    return;
                }

                EventBus.Emit(nameof(EventBus.WateringCanAmountChanged), wateringCanAmount);
            }
        }

        private int wateringCanAmount;

        public int WateringCanMaximum = 5;

        [GetNode("/root/Game/Grid")] private Grid grid;
        [GetNode("MoveCooldown")] private Timer moveCooldown;
        [GetNode("AnimationPlayer")] private AnimationPlayer animationPlayer;
        [GetNode("ActionPrompt")] private ActionPrompt actionPrompt;

        private Vector2 playerGridPosition = Vector2.Zero;

        private const string ANIMATION_MOVE = "Move";
        private const string ANIMATION_INTERACT = "Interact";

        public override void _Ready() {
            GetNodeAttribute.Load(this);
        }

        public override void _Process(float delta) {
            playerGridPosition = grid.WorldToMap(Position);
            var direction = determineDirection();

            actionPrompt.Hide();

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

            playAnimation(ANIMATION_MOVE);
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
            return !moveCooldown.IsStopped();
        }

        private bool canInteract() {
            var currentTile = grid.CellAt(playerGridPosition);

            if (!currentTile.HasValue) {
                return false;
            }

            var targetTile = currentTile.Value;

            if (targetTile.IsUntouchedFarmPlot()) {
                actionPrompt.ShowPrompt(ActionPromptEvent.Hoe);
                return true;
            }

            if (
                targetTile.IsPlowedFarmPlot() &&
                !grid.IsFarmPlotWatered(playerGridPosition) &&
                WateringCanAmount > 0
            ) {
                actionPrompt.ShowPrompt(ActionPromptEvent.WateringCan);
                return true;
            }

            if (targetTile.IsFarmPlot() && grid.IsFarmPlotWatered(playerGridPosition) && !grid.HasPlant(playerGridPosition)) {
                actionPrompt.ShowPrompt(ActionPromptEvent.Seed);
                return true;
            }

            if (targetTile.IsFarmPlot() && grid.HasPlant(playerGridPosition)) {
                var plant = grid.PlantAt(playerGridPosition);

                if (plant.IsFullyGrown()) {
                    actionPrompt.ShowPrompt(ActionPromptEvent.Sell);
                    return true;
                }
            }

            if (targetTile == TileMapTiles.InteractPlate) {
                var tileAbove = grid.CellAt(playerGridPosition + Vector2.Up);
                if (tileAbove.HasValue) {
                    // TODO: refactor this
                    if (WateringCanAmount< WateringCanMaximum && tileAbove.Value == TileMapTiles.WaterTankBottom) {
                        actionPrompt.ShowPrompt(ActionPromptEvent.Water);
                        return true;
                    }
                }
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
                playAnimation(ANIMATION_INTERACT);
                return;
            }

            if (targetTile.IsPlowedFarmPlot() && !grid.IsFarmPlotWatered(playerGridPosition) && WateringCanAmount > 0) {
                grid.WaterFarmPlot(playerGridPosition);
                WateringCanAmount--;
                playAnimation(ANIMATION_INTERACT);
                return;
            }

            if (targetTile.IsFarmPlot() && grid.IsFarmPlotWatered(playerGridPosition) && !grid.HasPlant(playerGridPosition)) {
                // TODO: offer multiple types of seeds
                grid.PlaceSeed(playerGridPosition);
                grid.NextTurn();
                playAnimation(ANIMATION_INTERACT);
            }

            if (targetTile == TileMapTiles.InteractPlate) {
                var tileAbove = grid.CellAt(playerGridPosition + Vector2.Up);
                if (tileAbove.HasValue) {
                    // TODO: refactor this
                    if (tileAbove.Value == TileMapTiles.WaterTankBottom) {
                        WateringCanAmount = WateringCanMaximum;
                        grid.NextTurn();
                        playAnimation(ANIMATION_INTERACT);
                    }
                }
            }

            if (grid.HasPlant(playerGridPosition)) {
                var plant = grid.PlantAt(playerGridPosition);

                if (plant.IsFullyGrown()) {
                    // TODO: maybe only put it inventory and have to turn it in somewhere?
                    Money += plant.ProduceValue;
                    grid.RemovePlant(playerGridPosition);
                }
            }

            if (grid.HasPlant(playerGridPosition) && !grid.IsFarmPlotWatered(playerGridPosition) && WateringCanAmount > 0) {
                grid.WaterFarmPlot(playerGridPosition);
                WateringCanAmount--;
                playAnimation(ANIMATION_INTERACT);
            }
        }

        private void playAnimation(string animationName) {
            if (animationPlayer.IsPlaying()) {
                return;
            }
            animationPlayer.CurrentAnimation = animationName;
            animationPlayer.Play();
        }
    }
}
