using Godot;

namespace LD50.Constants {
    public enum TileMapTiles {
        InvisibleBarrier = 0,
        Barrier = 1,
        Ground = 2,
        GroundWithGrass = 3,
        Block = 4,
    }

    public static class TileMapTilesExtension {
        public static bool IsBarrier(this TileMapTiles tile) {
            switch (tile) {
                case TileMapTiles.InvisibleBarrier:
                case TileMapTiles.Barrier:
                case TileMapTiles.Block:
                    return true;
                default:
                    return false;
            }
        }
    }
}
