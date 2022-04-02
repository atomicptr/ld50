using Godot;
using Godot.Collections;
using LD50.Autoload;
using LD50.Common;
using LD50.Constants;

namespace LD50.Scenes.Game {
    // TODO: find new name for this class... Game Manager?
    public class Grid : Node2D {
        [GetNode("TileMap")] private TileMap tileMap;

        public int NextPaymentThreshold {
            get => nextPaymentThreshold;
            private set {
                nextPaymentThreshold = value;

                if (!IsInsideTree()) {
                    ToSignal(this, "ready").OnCompleted(() => EventBus.Emit(nameof(EventBus.NewPaymentThreshold), nextPaymentThreshold));
                    return;
                }

                EventBus.Emit(nameof(EventBus.NewPaymentThreshold), nextPaymentThreshold);
            }
        }

        private int nextPaymentThreshold;

        private Dictionary<Vector2, int> turnPlotWasWatered = new Dictionary<Vector2, int>();

        private int turn = 1;

        private const int TURNS_PLOT_STAYS_WATERED = 300;
        public const int FIRST_PAYMENT_THRESHOLD = 200;

        public override void _Ready() {
            GetNodeAttribute.Load(this);

            // set initial payment threshold
            NextPaymentThreshold = FIRST_PAYMENT_THRESHOLD;
        }

        public Vector2 MapToWorld(Vector2 coords) {
            return tileMap.MapToWorld(coords);
        }

        public Vector2 WorldToMap(Vector2 coords) {
            return tileMap.WorldToMap(coords);
        }

        public TileMapTiles? CellAt(Vector2 coords) {
            var index = tileMap.GetCellv(coords);
            if (index == -1) {
                return null;
            }

            return (TileMapTiles) index;
        }

        public bool IsVacant(Vector2 coords) {
            var tile = CellAt(coords);
            if (!tile.HasValue) {
                return false;
            }

            return !tile.Value.IsBarrier();
        }

        public void PlowFarmPlot(Vector2 coords) {
            var tile = CellAt(coords);
            if (!tile.HasValue) {
                return;
            }

            if (!tile.Value.IsUntouchedFarmPlot()) {
                return;
            }

            tileMap.SetCellv(coords, (int) tile.Value.ToPlowedFarmPlot());
            NextTurn();
        }

        public bool IsFarmPlotWatered(Vector2 coords) {
            return turnPlotWasWatered.ContainsKey(coords);
        }

        public void WaterFarmPlot(Vector2 coords) {
            if (IsFarmPlotWatered(coords)) {
                return;
            }

            var tile = CellAt(coords);
            if (!tile.HasValue) {
                return;
            }

            turnPlotWasWatered[coords] = turn;
            tileMap.SetCellv(coords, (int) tile.Value.ToWateredFarmPlot());
            NextTurn();
        }

        public void NextTurn() {
            progressWorld();

            turn++;
            GD.Print("Turn: ", turn);
            EventBus.Emit(nameof(EventBus.TurnChanged), turn);
        }

        private void progressWorld() {
            // process watered plots
            var wateredPlotCoords = turnPlotWasWatered.Keys;
            foreach (var wateredPlotCoord in wateredPlotCoords) {
                if (turn <= turnPlotWasWatered[wateredPlotCoord] + TURNS_PLOT_STAYS_WATERED) {
                    continue;
                }

                var tile = CellAt(wateredPlotCoord);

                if (!tile.HasValue) {
                    continue;
                }

                tileMap.SetCellv(wateredPlotCoord, (int) tile.Value.ToDryFarmPlot());
                turnPlotWasWatered.Remove(wateredPlotCoord);
            }
        }
    }
}
