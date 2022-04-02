using Godot;
using LD50.Common;
using LD50.Constants;

namespace LD50.Scenes.Game {
    public class Grid : Node2D {
        [GetNode("TileMap")] private TileMap tileMap;
        [GetNode("YSort")] private YSort ySort;

        private int turn = 1;

        [Signal]
        public delegate void TurnChanged(int turn);

        public override void _Ready() {
            GetNodeAttribute.Load(this);
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

        public void NextTurn() {
            turn++;
            GD.Print("Turn: ", turn);
            EmitSignal(nameof(TurnChanged), turn);
        }
    }
}
