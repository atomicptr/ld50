using System;
using System.Collections.Generic;
using Godot;
using LD50.Autoload;
using LD50.Common;
using LD50.Constants;
using LD50.Scenes.Game;
using LD50.UserInterface;
using LD50.UserInterface.ShopMenu;

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


        [Export]
        public int SeedAmount {
            get => seedAmount;
            set {
                seedAmount = Mathf.Clamp(value, 0, int.MaxValue);

                if (!IsInsideTree()) {
                    ToSignal(this, "ready").OnCompleted(() => EventBus.Emit(nameof(EventBus.SeedAmountChanged), seedAmount));
                    return;
                }

                EventBus.Emit(nameof(EventBus.SeedAmountChanged), seedAmount);
            }
        }

        private int money;
        private int wateringCanAmount;
        private int seedAmount;

        public int WateringCanMaximum = 5;

        [GetNode("/root/Game/GameManager")] private GameManager gameManager;
        [GetNode("MoveCooldown")] private Timer moveCooldown;
        [GetNode("AnimationPlayer")] private AnimationPlayer animationPlayer;
        [GetNode("ActionPrompt")] private ActionPrompt actionPrompt;
        [GetNode("FloatingTextManager")] private FloatingTextManager floatingTextManager;
        [GetNode("InteractSound")] private AudioStreamPlayer2D interactSound;
        [GetNode("JumpSound")] private AudioStreamPlayer2D jumpSound;
        [GetNode("MoneySound")] private AudioStreamPlayer2D moneySound;

        private Vector2 playerGridPosition = Vector2.Zero;

        private List<PlayerUpgrade> playerUpgrades = new List<PlayerUpgrade>();

        private const string ANIMATION_MOVE = "Move";
        private const string ANIMATION_INTERACT = "Interact";

        public override void _Ready() {
            GetNodeAttribute.Load(this);

            EventBus.ConnectEvent(nameof(EventBus.ShopItemPurchased), this, nameof(onItemPurchased));
        }

        public override void _Process(float delta) {
            EventBus.InitializeEvent(nameof(EventBus.MoneyValueChanged), money);
            EventBus.InitializeEvent(nameof(EventBus.WateringCanAmountChanged), wateringCanAmount);
            EventBus.InitializeEvent(nameof(EventBus.SeedAmountChanged), seedAmount);

            playerGridPosition = gameManager.WorldToMap(Position);
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

            if (!gameManager.IsVacant(targetPos)) {
                return;
            }

            Position = gameManager.MapToWorld(playerGridPosition + direction);

            jumpSound.Play();

            gameManager.NextTurn();

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
            var currentTile = gameManager.CellAt(playerGridPosition);

            if (!currentTile.HasValue) {
                return false;
            }

            var targetTile = currentTile.Value;

            if (targetTile.IsUntouchedFarmPlot()) {
                actionPrompt.ShowPrompt(Icon.ToolHoe);
                return true;
            }

            if (
                targetTile.IsPlowedFarmPlot() &&
                !gameManager.IsFarmPlotWatered(playerGridPosition) &&
                WateringCanAmount > 0
            ) {
                actionPrompt.ShowPrompt(Icon.ToolWateringCan);
                return true;
            }

            if (
                targetTile.IsFarmPlot() &&
                gameManager.IsFarmPlotWatered(playerGridPosition) &&
                !gameManager.HasPlant(playerGridPosition) &&
                SeedAmount > 0
            ) {
                actionPrompt.ShowPrompt(Icon.ToolSeeds);
                return true;
            }

            if (targetTile.IsFarmPlot() && gameManager.HasPlant(playerGridPosition)) {
                var plant = gameManager.PlantAt(playerGridPosition);

                if (plant.IsFullyGrown()) {
                    actionPrompt.ShowPrompt(Icon.Money);
                    return true;
                }
            }

            if (targetTile == TileMapTiles.InteractPlate) {
                var tileAbove = gameManager.CellAt(playerGridPosition + Vector2.Up);
                if (tileAbove.HasValue) {
                    // TODO: refactor this
                    if (WateringCanAmount< WateringCanMaximum && tileAbove.Value == TileMapTiles.WaterTankBottom) {
                        actionPrompt.ShowPrompt(Icon.ToolWater);
                        return true;
                    }

                    // yikes... this only supports shops for now i guess?
                    if (tileAbove.Value == TileMapTiles.HouseFrontDoor) {
                        actionPrompt.ShowPrompt(Icon.SpeechBubble);
                        return true;
                    }
                }
            }

            return false;
        }

        private void interact() {
            var currentTile = gameManager.CellAt(playerGridPosition);

            if (!currentTile.HasValue) {
                return;
            }

            var targetTile = currentTile.Value;

            if (targetTile.IsUntouchedFarmPlot()) {
                gameManager.PlowFarmPlot(playerGridPosition);
                playAnimation(ANIMATION_INTERACT);
                return;
            }

            if (targetTile.IsPlowedFarmPlot() && !gameManager.IsFarmPlotWatered(playerGridPosition) && WateringCanAmount > 0) {
                gameManager.WaterFarmPlot(playerGridPosition);
                WateringCanAmount--;
                floatingTextManager.Spawn(Icon.ToolWateringCan, -1);
                playAnimation(ANIMATION_INTERACT);
                return;
            }

            if (
                targetTile.IsFarmPlot() &&
                gameManager.IsFarmPlotWatered(playerGridPosition) &&
                !gameManager.HasPlant(playerGridPosition) &&
                SeedAmount > 0
            ) {
                // TODO: offer multiple types of seeds
                gameManager.PlaceSeed(playerGridPosition);
                SeedAmount--;
                floatingTextManager.Spawn(Icon.ToolSeeds, -1);
                gameManager.NextTurn();
                playAnimation(ANIMATION_INTERACT);
            }

            if (targetTile == TileMapTiles.InteractPlate) {
                var tileAbove = gameManager.CellAt(playerGridPosition + Vector2.Up);
                if (tileAbove.HasValue) {
                    // TODO: refactor this
                    if (tileAbove.Value == TileMapTiles.WaterTankBottom) {
                        floatingTextManager.Spawn(Icon.ToolWateringCan, WateringCanMaximum - WateringCanAmount);
                        WateringCanAmount = WateringCanMaximum;
                        gameManager.NextTurn();
                        playAnimation(ANIMATION_INTERACT);
                    }

                    // guess this works only for shops...
                    if (tileAbove.Value == TileMapTiles.HouseFrontDoor) {
                        populateShopMenu();
                        EventBus.Emit(nameof(EventBus.OpenShopMenu));
                        gameManager.NextTurn();
                        playAnimation(ANIMATION_INTERACT);
                    }
                }
            }

            if (gameManager.HasPlant(playerGridPosition)) {
                var plant = gameManager.PlantAt(playerGridPosition);

                if (plant.IsFullyGrown()) {
                    // TODO: maybe only put it inventory and have to turn it in somewhere?
                    Money += plant.ProduceValue;
                    moneySound.Play();
                    floatingTextManager.Spawn(Icon.Money, plant.ProduceValue);
                    gameManager.RemovePlant(playerGridPosition);
                }
            }

            if (gameManager.HasPlant(playerGridPosition) && !gameManager.IsFarmPlotWatered(playerGridPosition) && WateringCanAmount > 0) {
                gameManager.WaterFarmPlot(playerGridPosition);
                WateringCanAmount--;
                floatingTextManager.Spawn(Icon.ToolWateringCan, -1);
                playAnimation(ANIMATION_INTERACT);
            }
        }

        private void playAnimation(string animationName, bool playSound = true) {
            if (animationPlayer.IsPlaying()) {
                return;
            }

            if (animationName == ANIMATION_INTERACT && playSound) {
                interactSound.Play();
            }

            animationPlayer.CurrentAnimation = animationName;
            animationPlayer.Play();
        }

        private void populateShopMenu() {
            var shopItems = new Dictionary<MenuItemEntryIdentifier, MenuItemEntry>() {
                {
                    MenuItemEntryIdentifier.BuySeedAmount5,
                    new MenuItemEntry(MenuItemEntryIdentifier.BuySeedAmount5, Icon.ToolSeeds, "Buy 05x Seeds", 500)
                },
                {
                    MenuItemEntryIdentifier.BuySeedAmount10,
                    new MenuItemEntry(MenuItemEntryIdentifier.BuySeedAmount10, Icon.ToolSeeds, "Buy 10x Seeds", 900)
                },
            };

            if (gameManager.HasUnplowedFarmPlots()) {
                shopItems[MenuItemEntryIdentifier.PlowEntireField] = new MenuItemEntry(
                    MenuItemEntryIdentifier.PlowEntireField,
                    Icon.ToolHoe,
                    "Plow entire field",
                    8000
                );
            }

            if (gameManager.HasUnwateredFarmPlot()) {
                shopItems[MenuItemEntryIdentifier.WaterEntireField] = new MenuItemEntry(
                    MenuItemEntryIdentifier.WaterEntireField,
                    Icon.ToolWateringCan,
                    "Water entire field",
                    5000
                );
            }

            if (!playerUpgrades.Contains(PlayerUpgrade.WateringCanSizeIncreaseTo10)) {
                shopItems[MenuItemEntryIdentifier.UpgradeWateringCanSize1] = new MenuItemEntry(
                    MenuItemEntryIdentifier.UpgradeWateringCanSize1,
                    Icon.ToolWateringCan,
                    "Upgrade capacity to 10",
                    1000
                );
            }

            if (
                playerUpgrades.Contains(PlayerUpgrade.WateringCanSizeIncreaseTo10) &&
                !playerUpgrades.Contains(PlayerUpgrade.WateringCanSizeIncreaseTo20)
            ) {
                shopItems[MenuItemEntryIdentifier.UpgradeWateringCanSize2] = new MenuItemEntry(
                    MenuItemEntryIdentifier.UpgradeWateringCanSize2,
                    Icon.ToolWateringCan,
                    "Upgrade capacity to 20",
                    2000
                );
            }

            if (
                playerUpgrades.Contains(PlayerUpgrade.WateringCanSizeIncreaseTo10) &&
                playerUpgrades.Contains(PlayerUpgrade.WateringCanSizeIncreaseTo20) &&
                !playerUpgrades.Contains(PlayerUpgrade.WateringCanSizeIncreaseTo30)
            ) {
                shopItems[MenuItemEntryIdentifier.UpgradeWateringCanSize3] = new MenuItemEntry(
                    MenuItemEntryIdentifier.UpgradeWateringCanSize3,
                    Icon.ToolWateringCan,
                    "Upgrade capacity to 30",
                    3000
                );
            }

            if (!playerUpgrades.Contains(PlayerUpgrade.FarmPlotUpgrade1)) {
                shopItems[MenuItemEntryIdentifier.FarmPlotUpgrade1] = new MenuItemEntry(
                    MenuItemEntryIdentifier.FarmPlotUpgrade1,
                    Icon.ToolHoe,
                    "Increase farm plot size",
                    3000
                );
            }

            if (
                playerUpgrades.Contains(PlayerUpgrade.FarmPlotUpgrade1) &&
                !playerUpgrades.Contains(PlayerUpgrade.FarmPlotUpgrade2)
            ) {
                shopItems[MenuItemEntryIdentifier.FarmPlotUpgrade2] = new MenuItemEntry(
                    MenuItemEntryIdentifier.FarmPlotUpgrade2,
                    Icon.ToolHoe,
                    "Increase farm plot size again",
                    5000
                );
            }

            if (
                playerUpgrades.Contains(PlayerUpgrade.FarmPlotUpgrade1) &&
                playerUpgrades.Contains(PlayerUpgrade.FarmPlotUpgrade2) &&
                !playerUpgrades.Contains(PlayerUpgrade.FarmPlotUpgrade3)
            ) {
                shopItems[MenuItemEntryIdentifier.FarmPlotUpgrade3] = new MenuItemEntry(
                    MenuItemEntryIdentifier.FarmPlotUpgrade3,
                    Icon.ToolHoe,
                    "Increase farm plot size again!",
                    10000
                );
            }

            GlobalState.Instance.ShopItems = shopItems;
        }

        private void onItemPurchased(MenuItemEntryIdentifier identifier) {
            var item = GlobalState.Instance.ShopItems[identifier];

            if (item.Cost > Money) {
                return; // TODO: somehow show user this failed...
            }

            Money -= item.Cost;

            switch (identifier) {
                case MenuItemEntryIdentifier.BuySeedAmount5:
                    SeedAmount += 5;
                    break;
                case MenuItemEntryIdentifier.BuySeedAmount10:
                    SeedAmount += 10;
                    break;
                case MenuItemEntryIdentifier.UpgradeWateringCanSize1:
                    addUpgrade(PlayerUpgrade.WateringCanSizeIncreaseTo10);
                    WateringCanMaximum = 10;
                    WateringCanAmount = WateringCanMaximum;
                    break;
                case MenuItemEntryIdentifier.UpgradeWateringCanSize2:
                    addUpgrade(PlayerUpgrade.WateringCanSizeIncreaseTo20);
                    WateringCanMaximum  = 20;
                    WateringCanAmount = WateringCanMaximum;
                    break;
                case MenuItemEntryIdentifier.UpgradeWateringCanSize3:
                    addUpgrade(PlayerUpgrade.WateringCanSizeIncreaseTo30);
                    WateringCanMaximum = 30;
                    WateringCanAmount = WateringCanMaximum;
                    break;
                case MenuItemEntryIdentifier.FarmPlotUpgrade1:
                    addUpgrade(PlayerUpgrade.FarmPlotUpgrade1);
                    break;
                case MenuItemEntryIdentifier.FarmPlotUpgrade2:
                    addUpgrade(PlayerUpgrade.FarmPlotUpgrade2);
                    break;
                case MenuItemEntryIdentifier.FarmPlotUpgrade3:
                    addUpgrade(PlayerUpgrade.FarmPlotUpgrade3);
                    break;
                case MenuItemEntryIdentifier.PlowEntireField:
                    gameManager.PlowAllFarmPlots();
                    break;
                case MenuItemEntryIdentifier.WaterEntireField:
                    gameManager.WaterAllFarmPlots();
                    break;
            }

            populateShopMenu();
            EventBus.Emit(nameof(EventBus.OpenShopMenu));
        }

        private void addUpgrade(PlayerUpgrade upgrade) {
            playerUpgrades.Add(upgrade);
            EventBus.Emit(nameof(EventBus.UpgradeObtained), upgrade);
        }
    }
}
