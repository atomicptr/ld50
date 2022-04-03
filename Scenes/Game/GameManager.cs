using System.Linq;
using Godot;
using Godot.Collections;
using JetBrains.Annotations;
using LD50.Autoload;
using LD50.Common;
using LD50.Constants;
using LD50.Entities;
using LD50.Entities.Plant;

namespace LD50.Scenes.Game {
    public class GameManager : Node2D {
        [GetNode("TileMap")] private TileMap tileMap;
        [GetNode("YSort/Plants")] private YSort plants;
        [GetNode("YSort/Player")] private Player player;

        private static readonly PackedScene plantScene = GD.Load<PackedScene>("res://Entities/Plant/Plant.tscn");
        private readonly LoanProcessor loanProcessor = new LoanProcessor();

        private readonly Dictionary<Vector2, int> turnPlotWasWatered = new Dictionary<Vector2, int>();
        private readonly Dictionary<Vector2, Plant> plantedPlants = new Dictionary<Vector2, Plant>();

        private int turn = 0;
        private int currentDeadline = PAYMENT_THRESHOLD;

        private const int TURNS_PLOT_STAYS_WATERED = 300;
        public const int PAYMENT_THRESHOLD = 300;

        public override void _Ready() {
            GetNodeAttribute.Load(this);

            // remove red modulate from barrier
            tileMap.TileSet.TileSetModulate((int) TileMapTiles.Barrier, Colors.White);

            EventBus.ConnectEvent(nameof(EventBus.UpgradeObtained), this, nameof(onUpgradeObtained));
        }

        public override void _Process(float delta) {
            EventBus.InitializeEvent(nameof(EventBus.TurnChanged), turn);
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
            plantedPlants[coords] = plant;

            var position = MapToWorld(coords);;
            position.y -= 1; // HACK: to make YSort work properly with the tiles
            plant.Position = position;
        }

        [CanBeNull]
        public Plant PlantAt(Vector2 coords) {
            if (!HasPlant(coords)) {
                return null;
            }

            return plantedPlants[coords];
        }

        public void RemovePlant(Vector2 coords) {
            if (!HasPlant(coords)) {
                return;
            }

            var plant = plantedPlants[coords];
            plant.QueueFree();
            plantedPlants.Remove(coords);
        }

        public void NextTurn() {
            progressWorld();

            turn++;
            EventBus.Emit(nameof(EventBus.TurnChanged), turn);
        }

        private void progressWorld() {
            // grow plants
            foreach (var plantGridPos in plantedPlants.Keys) {
                var plant = plantedPlants[plantGridPos];

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

            // pay loan
            if (turn >= currentDeadline - 1) {
                var nextPayment = loanProcessor.Next;

                if (!nextPayment.HasValue) {
                    // you won the game! How the hell...
                    GetTree().ChangeScene("res://Scenes/GameOver/YouWin.tscn");
                    return;
                }

                if (player.Money - nextPayment.Value < 0) {
                    // you went into the red -> you lost
                    GetTree().ChangeScene("res://Scenes/GameOver/GameOver.tscn");
                    return;
                }

                // player could pay the next payment, so continue...
                player.Money -= nextPayment.Value;

                currentDeadline += PAYMENT_THRESHOLD;
                EventBus.Emit(nameof(EventBus.NextPaymentThresholdAnnounced), currentDeadline);
                loanProcessor.AdvanceToNextStage();
            }
        }

        private void onUpgradeObtained(PlayerUpgrade upgrade) {
            switch (upgrade) {
                case PlayerUpgrade.FarmPlotUpgrade1:
                    increaseFarmPlot(6);
                    break;
                case PlayerUpgrade.FarmPlotUpgrade2:
                    increaseFarmPlot(9);
                    break;
                case PlayerUpgrade.FarmPlotUpgrade3:
                    increaseFarmPlot(12);
                    break;
            }
        }

        private void increaseFarmPlot(int size) {
            var farmPlotStart = Vector2.Zero;

            // find the start of the farm plot
            foreach (var cellv in tileMap.GetUsedCells().OfType<Vector2>()) {
                var cellId = tileMap.GetCellv(cellv);

                if (cellId == (int) TileMapTiles.FarmPlotNorthWest) {
                    farmPlotStart = cellv;
                    break;
                }
            }

            for (var x = 0; x < size; x++) {
                for (var y = 0; y < size; y++) {
                    var cellv = farmPlotStart + new Vector2(x, y);

                    var currentTile = (TileMapTiles) tileMap.GetCellv(cellv);

                    var isTopRow = y == 0;
                    var isBottomRow = y == size - 1;
                    var isLeftEdgeColumn = x == 0;
                    var isRightEdgeColumn = x == size - 1;

                    var tile = TileMapTiles.FarmPlotMidMid;

                    if (isTopRow && isLeftEdgeColumn) {
                        tile = TileMapTiles.FarmPlotNorthWest;
                    } else if (isTopRow && isRightEdgeColumn) {
                        tile = TileMapTiles.FarmPlotNorthEast;
                    } else if (isTopRow) {
                        tile = TileMapTiles.FarmPlotNorthMid;
                    } else if (isBottomRow && isLeftEdgeColumn) {
                        tile = TileMapTiles.FarmPlotSouthWest;
                    }  else if (isBottomRow && isRightEdgeColumn) {
                        tile = TileMapTiles.FarmPlotSouthEast;
                    } else if (isBottomRow) {
                        tile = TileMapTiles.FarmPlotSouthMid;
                    } else if (isLeftEdgeColumn) {
                        tile = TileMapTiles.FarmPlotMidWest;
                    } else if (isRightEdgeColumn) {
                        tile = TileMapTiles.FarmPlotMidEast;
                    }

                    if (currentTile.IsPlowedFarmPlot()) {
                        tile = tile.ToPlowedFarmPlot();
                    }

                    if (IsFarmPlotWatered(cellv)) {
                        tile = tile.ToWateredFarmPlot();
                    }

                    tileMap.SetCellv(cellv, (int) tile);
                }
            }
        }
    }
}
