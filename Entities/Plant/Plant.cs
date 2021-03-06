using System;
using Godot;
using LD50.Autoload;
using LD50.Common;

namespace LD50.Entities.Plant {
    public enum PlantStage {
        SeedsPlanted,
        Growing,
        FullyGrown,
    }

    public class Plant : Node2D {
        [Export] public int ProduceValue = 500;
        [Export] private int growTurns = 50;
        [Export] private readonly int growTurnsOnBetterSoil = 38;

        private PlantStage plantStage = PlantStage.SeedsPlanted;
        private int age = 0;

        [GetNode("Sprite")] private Sprite sprite;

        private static readonly Rect2 seedStageRect = new Rect2(16, 0, 16, 22);
        private static readonly Rect2 growingStageRect = new Rect2(32, 0, 16, 22);
        private static readonly Rect2 fullyGrownStageRect = new Rect2(48, 0, 16, 22);

        public override void _Ready() {
            GetNodeAttribute.Load(this);
        }

        public void ContinueGrowing() {
            if (GlobalState.Instance.SoilUpgradePurchased) {
                growTurns = growTurnsOnBetterSoil;
            }

            age++;

            // if is still seeds and half the grow time has passed, advance to next stage
            if (plantStage == PlantStage.SeedsPlanted && age >= growTurns * 0.5) {
                plantStage = PlantStage.Growing;
            }

            if (plantStage == PlantStage.Growing && age >= growTurns) {
                plantStage = PlantStage.FullyGrown;
            }

            updateSprite();
        }

        private void updateSprite() {
            switch (plantStage) {
                case PlantStage.SeedsPlanted:
                    sprite.RegionRect = seedStageRect;
                    break;
                case PlantStage.Growing:
                    sprite.RegionRect = growingStageRect;
                    break;
                case PlantStage.FullyGrown:
                    sprite.RegionRect = fullyGrownStageRect;
                    break;
            }
        }

        public bool IsFullyGrown() {
            return plantStage == PlantStage.FullyGrown;
        }
    }
}
