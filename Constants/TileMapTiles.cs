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
        WateredFarmPlotNorthWest = 23,
        WateredFarmPlotNorthMid = 24,
        WateredFarmPlotNorthEast = 25,
        WateredFarmPlotMidWest = 26,
        WateredFarmPlotMidMid = 27,
        WateredFarmPlotMidEast = 28,
        WateredFarmPlotSouthWest = 29,
        WateredFarmPlotSouthMid = 30,
        WateredFarmPlotSouthEast = 31,
        WateredPlowedFarmPlotNorthWest = 32,
        WateredPlowedFarmPlotNorthMid = 33,
        WateredPlowedFarmPlotNorthEast = 34,
        WateredPlowedFarmPlotMidWest = 35,
        WateredPlowedFarmPlotMidMid = 36,
        WateredPlowedFarmPlotMidEast = 37,
        WateredPlowedFarmPlotSouthWest = 38,
        WateredPlowedFarmPlotSouthMid = 39,
        WateredPlowedFarmPlotSouthEast = 40,
        WaterTankTop = 41,
        WaterTankBottom = 42,
        InteractPlate = 43,
        HouseRoofTopLeft = 44,
        HouseRoofTopMid = 45,
        HouseRoofTopRight = 46,
        HouseRoofMidLeft = 47,
        HouseRoofMidMid = 48,
        HouseRooMidRight = 49,
        HouseRoofBottomLeft = 50,
        HouseRoofBottomMid = 51,
        HouseRoofBottomRight = 52,
        HouseFrontTopLeft = 53,
        HouseFrontTopMid = 54,
        HouseFrontTopRight = 55,
        HouseFrontBottomLeft = 56,
        HouseFrontBottomMid = 57,
        HouseFrontBottomRight = 58,
        HouseFrontShopSign = 59,
        HouseFrontDoor = 60,
    }

    public static class TileMapTilesExtension {
        public static bool IsBarrier(this TileMapTiles tile) {
            switch (tile) {
                case TileMapTiles.InvisibleBarrier:
                case TileMapTiles.Barrier:
                case TileMapTiles.Block:
                case TileMapTiles.WaterTankTop:
                case TileMapTiles.WaterTankBottom:
                case TileMapTiles.HouseRoofTopLeft:
                case TileMapTiles.HouseRoofTopMid:
                case TileMapTiles.HouseRoofTopRight:
                case TileMapTiles.HouseRoofMidLeft:
                case TileMapTiles.HouseRoofMidMid:
                case TileMapTiles.HouseRooMidRight:
                case TileMapTiles.HouseRoofBottomLeft:
                case TileMapTiles.HouseRoofBottomMid:
                case TileMapTiles.HouseRoofBottomRight:
                case TileMapTiles.HouseFrontTopLeft:
                case TileMapTiles.HouseFrontTopMid:
                case TileMapTiles.HouseFrontTopRight:
                case TileMapTiles.HouseFrontBottomLeft:
                case TileMapTiles.HouseFrontBottomMid:
                case TileMapTiles.HouseFrontBottomRight:
                case TileMapTiles.HouseFrontShopSign:
                case TileMapTiles.HouseFrontDoor:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsFarmPlot(this TileMapTiles tile) {
            var index = (int) tile;
            return index >= (int) TileMapTiles.FarmPlotNorthWest &&
                   index <= (int) TileMapTiles.WateredPlowedFarmPlotSouthEast;
        }

        public static bool IsUntouchedFarmPlot(this TileMapTiles tile) {
            var index = (int) tile;
            return (index >= (int) TileMapTiles.FarmPlotNorthWest && index <= (int) TileMapTiles.FarmPlotSouthEast) ||
                   (index >= (int) TileMapTiles.WateredFarmPlotNorthWest && index <= (int) TileMapTiles.WateredFarmPlotSouthEast);
        }

        public static bool IsPlowedFarmPlot(this TileMapTiles tile) {
            var index = (int) tile;
            return index >= (int) TileMapTiles.PlowedFarmPlotNorthWest &&
                   index <= (int) TileMapTiles.PlowedFarmPlotSouthEast;
        }

        public static TileMapTiles ToPlowedFarmPlot(this TileMapTiles tile) {
            switch (tile) {
                case TileMapTiles.FarmPlotNorthWest:
                    return TileMapTiles.PlowedFarmPlotNorthWest;
                case TileMapTiles.FarmPlotNorthMid:
                    return TileMapTiles.PlowedFarmPlotNorthMid;
                case TileMapTiles.FarmPlotNorthEast:
                    return TileMapTiles.PlowedFarmPlotNorthEast;
                case TileMapTiles.FarmPlotMidWest:
                    return TileMapTiles.PlowedFarmPlotMidWest;
                case TileMapTiles.FarmPlotMidMid:
                    return TileMapTiles.PlowedFarmPlotMidMid;
                case TileMapTiles.FarmPlotMidEast:
                    return TileMapTiles.PlowedFarmPlotMidEast;
                case TileMapTiles.FarmPlotSouthWest:
                    return TileMapTiles.PlowedFarmPlotSouthWest;
                case TileMapTiles.FarmPlotSouthMid:
                    return TileMapTiles.PlowedFarmPlotSouthMid;
                case TileMapTiles.FarmPlotSouthEast:
                    return TileMapTiles.PlowedFarmPlotSouthEast;
                case TileMapTiles.WateredFarmPlotNorthWest:
                    return TileMapTiles.WateredPlowedFarmPlotNorthWest;
                case TileMapTiles.WateredFarmPlotNorthMid:
                    return TileMapTiles.WateredPlowedFarmPlotNorthMid;
                case TileMapTiles.WateredFarmPlotNorthEast:
                    return TileMapTiles.WateredPlowedFarmPlotNorthEast;
                case TileMapTiles.WateredFarmPlotMidWest:
                    return TileMapTiles.WateredPlowedFarmPlotMidWest;
                case TileMapTiles.WateredFarmPlotMidMid:
                    return TileMapTiles.WateredPlowedFarmPlotMidMid;
                case TileMapTiles.WateredFarmPlotMidEast:
                    return TileMapTiles.WateredPlowedFarmPlotMidEast;
                case TileMapTiles.WateredFarmPlotSouthWest:
                    return TileMapTiles.WateredPlowedFarmPlotSouthWest;
                case TileMapTiles.WateredFarmPlotSouthMid:
                    return TileMapTiles.WateredPlowedFarmPlotSouthMid;
                case TileMapTiles.WateredFarmPlotSouthEast:
                    return TileMapTiles.WateredPlowedFarmPlotSouthEast;
                default:
                    // invalid state, don't do anything
                    return tile;
            }
        }

        public static TileMapTiles ToWateredFarmPlot(this TileMapTiles tile) {
            switch (tile) {
                case TileMapTiles.FarmPlotNorthWest:
                    return TileMapTiles.WateredFarmPlotNorthWest;
                case TileMapTiles.FarmPlotNorthMid:
                    return TileMapTiles.WateredFarmPlotNorthMid;
                case TileMapTiles.FarmPlotNorthEast:
                    return TileMapTiles.WateredFarmPlotNorthEast;
                case TileMapTiles.FarmPlotMidWest:
                    return TileMapTiles.WateredFarmPlotMidWest;
                case TileMapTiles.FarmPlotMidMid:
                    return TileMapTiles.WateredFarmPlotMidMid;
                case TileMapTiles.FarmPlotMidEast:
                    return TileMapTiles.WateredFarmPlotMidEast;
                case TileMapTiles.FarmPlotSouthWest:
                    return TileMapTiles.WateredFarmPlotSouthWest;
                case TileMapTiles.FarmPlotSouthMid:
                    return TileMapTiles.WateredFarmPlotSouthMid;
                case TileMapTiles.FarmPlotSouthEast:
                    return TileMapTiles.WateredFarmPlotSouthEast;
                case TileMapTiles.PlowedFarmPlotNorthWest:
                    return TileMapTiles.WateredPlowedFarmPlotNorthWest;
                case TileMapTiles.PlowedFarmPlotNorthMid:
                    return TileMapTiles.WateredPlowedFarmPlotNorthMid;
                case TileMapTiles.PlowedFarmPlotNorthEast:
                    return TileMapTiles.WateredPlowedFarmPlotNorthEast;
                case TileMapTiles.PlowedFarmPlotMidWest:
                    return TileMapTiles.WateredPlowedFarmPlotMidWest;
                case TileMapTiles.PlowedFarmPlotMidMid:
                    return TileMapTiles.WateredPlowedFarmPlotMidMid;
                case TileMapTiles.PlowedFarmPlotMidEast:
                    return TileMapTiles.WateredPlowedFarmPlotMidEast;
                case TileMapTiles.PlowedFarmPlotSouthWest:
                    return TileMapTiles.WateredPlowedFarmPlotSouthWest;
                case TileMapTiles.PlowedFarmPlotSouthMid:
                    return TileMapTiles.WateredPlowedFarmPlotSouthMid;
                case TileMapTiles.PlowedFarmPlotSouthEast:
                    return TileMapTiles.WateredPlowedFarmPlotSouthEast;
                default:
                    // invalid state, don't do anything
                    return tile;
            }
        }

        public static TileMapTiles ToDryFarmPlot(this TileMapTiles tile) {
            switch (tile) {
                case TileMapTiles.WateredFarmPlotNorthWest:
                    return TileMapTiles.FarmPlotNorthWest;
                case TileMapTiles.WateredFarmPlotNorthMid:
                    return TileMapTiles.FarmPlotNorthMid;
                case TileMapTiles.WateredFarmPlotNorthEast:
                    return TileMapTiles.FarmPlotNorthEast;
                case TileMapTiles.WateredFarmPlotMidWest:
                    return TileMapTiles.FarmPlotMidWest;
                case TileMapTiles.WateredFarmPlotMidMid:
                    return TileMapTiles.FarmPlotMidMid;
                case TileMapTiles.WateredFarmPlotMidEast:
                    return TileMapTiles.FarmPlotMidEast;
                case TileMapTiles.WateredFarmPlotSouthWest:
                    return TileMapTiles.FarmPlotSouthWest;
                case TileMapTiles.WateredFarmPlotSouthMid:
                    return TileMapTiles.FarmPlotSouthMid;
                case TileMapTiles.WateredFarmPlotSouthEast:
                    return TileMapTiles.FarmPlotSouthEast;
                case TileMapTiles.WateredPlowedFarmPlotNorthWest:
                    return TileMapTiles.PlowedFarmPlotNorthWest;
                case TileMapTiles.WateredPlowedFarmPlotNorthMid:
                    return TileMapTiles.PlowedFarmPlotNorthMid;
                case TileMapTiles.WateredPlowedFarmPlotNorthEast:
                    return TileMapTiles.PlowedFarmPlotNorthEast;
                case TileMapTiles.WateredPlowedFarmPlotMidWest:
                    return TileMapTiles.PlowedFarmPlotMidWest;
                case TileMapTiles.WateredPlowedFarmPlotMidMid:
                    return TileMapTiles.PlowedFarmPlotMidMid;
                case TileMapTiles.WateredPlowedFarmPlotMidEast:
                    return TileMapTiles.PlowedFarmPlotMidEast;
                case TileMapTiles.WateredPlowedFarmPlotSouthWest:
                    return TileMapTiles.PlowedFarmPlotSouthWest;
                case TileMapTiles.WateredPlowedFarmPlotSouthMid:
                    return TileMapTiles.PlowedFarmPlotSouthMid;
                case TileMapTiles.WateredPlowedFarmPlotSouthEast:
                    return TileMapTiles.PlowedFarmPlotSouthEast;
                default:
                    // invalid state, don't do anything
                    return tile;
            }
        }
    }
}
