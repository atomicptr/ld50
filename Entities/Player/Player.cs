using System.Runtime.Remoting.Messaging;
using Godot;
using LD50.Common;
using LD50.Constants;
using LD50.Scenes.Game;

namespace LD50.Entities {
    public class Player : Node2D {
        [GetNode("/root/Game/Grid")] private Grid grid;
        [GetNode("MoveCooldown")] private Timer moveCooldown;

        public override void _Ready() {
            GetNodeAttribute.Load(this);

            //GetTree().CurrentScene.GetNode<Grid>("Grid");;
        }

        public override void _Process(float delta) {
            var direction = determineDirection();

            if (direction == Vector2.Zero) {
                return;
            }

            if (isCurrentlyMoving()) {
                return;
            }

            var playerGridPos = grid.WorldToMap(Position);
            var targetPos = playerGridPos + direction;

            if (!grid.IsVacant(targetPos)) {
                return;
            }

            // TODO: add animation
            Position = grid.MapToWorld(playerGridPos + direction);

            grid.NextTurn();
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
            return moveCooldown.TimeLeft > 0.00001f;
        }
    }
}
