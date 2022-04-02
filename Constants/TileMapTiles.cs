using System;
using Godot;

namespace LD50.Constants {
    public enum TileMapTiles {
        InvisibleBarrier = 0,
        Barrier = 1,
        Ground = 2,
        GroundWithGrass = 3,
        Block = 4,
        FarmPlotNorthWest = 5,
        FarmPlotNorthMid = 6,
        FarmPlotNorthEast = 7,
        FarmPlotMidWest = 8,
        FarmPlotMidMid = 9,
        FarmPlotMidEast = 10,
        FarmPlotSouthWest = 11,
        FarmPlotSouthMid = 12,
        FarmPlotSouthEast = 13,
        PlowedFarmPlotNorthWest = 14,
        PlowedFarmPlotNorthMid = 15,
        PlowedFarmPlotNorthEast = 16,
        PlowedFarmPlotMidWest = 17,
        PlowedFarmPlotMidMid = 18,
        PlowedFarmPlotMidEast = 19,
        PlowedFarmPlotSouthWest = 20,
        PlowedFarmPlotSouthMid = 21,
        PlowedFarmPlotSouthEast = 22,
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

        public static bool IsFarmPlot(this TileMapTiles tile) {
            var index = (int) tile;
            return index >= (int) TileMapTiles.FarmPlotNorthWest && index <= (int) TileMapTiles.PlowedFarmPlotSouthEast;
        }

        public static bool IsUntouchedFarmPlot(this TileMapTiles tile) {
            var index = (int) tile;
            return index >= (int) TileMapTiles.FarmPlotNorthWest && index <= (int) TileMapTiles.FarmPlotSouthEast;
        }

        public static bool IsPlowedFarmPlot(this TileMapTiles tile) {
            var index = (int) tile;
            return index >= (int) TileMapTiles.PlowedFarmPlotNorthWest && index <= (int) TileMapTiles.PlowedFarmPlotSouthEast;
        }

        public static TileMapTiles ToPlowedFarmPlot(this TileMapTiles tile) {
            switch (tile) {
                case TileMapTiles.FarmPlotNorthWest:
                    return TileMapTiles.PlowedFarmPlotNorthWest;
                    break;
                case TileMapTiles.FarmPlotNorthMid:
                    return TileMapTiles.PlowedFarmPlotNorthMid;
                    break;
                case TileMapTiles.FarmPlotNorthEast:
                    return TileMapTiles.PlowedFarmPlotNorthEast;
                    break;
                case TileMapTiles.FarmPlotMidWest:
                    return TileMapTiles.PlowedFarmPlotMidWest;
                    break;
                case TileMapTiles.FarmPlotMidMid:
                    return TileMapTiles.PlowedFarmPlotMidMid;
                    break;
                case TileMapTiles.FarmPlotMidEast:
                    return TileMapTiles.PlowedFarmPlotMidEast;
                    break;
                case TileMapTiles.FarmPlotSouthWest:
                    return TileMapTiles.PlowedFarmPlotSouthWest;
                    break;
                case TileMapTiles.FarmPlotSouthMid:
                    return TileMapTiles.PlowedFarmPlotSouthMid;
                    break;
                case TileMapTiles.FarmPlotSouthEast:
                    return TileMapTiles.PlowedFarmPlotSouthEast;
                default:
                    // invalid state, don't do anything
                    return tile;
            }
        }
    }
}
