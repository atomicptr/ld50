using System.Linq;
using Godot;
using Godot.Collections;
using LD50.Autoload;
using LD50.Common;
using LD50.Constants;
using LD50.Entities.Plant;

namespace LD50.Scenes.Game {
    // TODO: find new name for this class... Game Manager?
    public class Grid : Node2D {
        [GetNode("TileMap")] private TileMap tileMap;
        [GetNode("YSort/Plants")] private YSort plants;

        private static PackedScene plantScene = GD.Load<PackedScene>("res://Entities/Plant/Plant.tscn");

        private Dictionary<Vector2, int> turnPlotWasWatered = new Dictionary<Vector2, int>();
        private Dictionary<Vector2, Plant> plantedPlants = new Dictionary<Vector2, Plant>();

        private int turn = 1;

        private const int TURNS_PLOT_STAYS_WATERED = 300;
        public const int PAYMENT_THRESHOLD = 200;

        public override void _Ready() {
            GetNodeAttribute.Load(this);

            // propagate first turn
            EventBus.Emit(nameof(EventBus.TurnChanged), turn);
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

        public bool HasPlant(Vector2 coords) {
            return plantedPlants.ContainsKey(coords);
        }

        public void PlaceSeed(Vector2 coords) {
            if (HasPlant(coords)) {
                return;
            }

            var plant = plantScene.Instance<Plant>();
            plants.AddChild(plant);
            plant.Position = MapToWorld(coords);
            plantedPlants[coords] = plant;
        }

        public void NextTurn() {
            progressWorld();

            turn++;
            GD.Print("Turn: ", turn);
            EventBus.Emit(nameof(EventBus.TurnChanged), turn);
        }

        private void progressWorld() {
            // grow plants
            foreach (var plant in plants.GetChildren().OfType<Plant>()) {
                var plantGridPos = WorldToMap(plant.Position);

                if (!IsFarmPlotWatered(plantGridPos)) {
                    continue;
                }

                plant.ContinueGrowing();
            }

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
