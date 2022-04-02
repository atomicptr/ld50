using Godot;

namespace LD50.Constants {
    public enum TileMapTiles {
        InvisibleBarrier = 0,
        Barrier = 1,
        Ground = 2,
        GroundWithGrass = 3,
        Block = 4,
        FarmplotNorthWest = 5,
        FarmplotNorthMid = 6,
        FarmplotNorthEast = 7,
        FarmplotMidWest = 8,
        FarmplotMidMid = 9,
        FarmplotMidEast = 10,
        FarmplotSouthWest = 11,
        FarmplotSouthMid = 12,
        FarmplotSouthEast = 13,
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

        public static bool IsFarmland(this TileMapTiles tile) {
            var index = (int) tile;
            return index >= (int) TileMapTiles.FarmplotNorthWest && index <= (int) TileMapTiles.FarmplotSouthEast;
        }
    }
}
