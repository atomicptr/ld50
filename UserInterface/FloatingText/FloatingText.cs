using Godot;
using LD50.Common;
using LD50.Constants;

namespace LD50.UserInterface {
    public class FloatingText : Control {
        [GetNode("Icon")] private NinePatchRect icon;
        [GetNode("Label")] private Label label;
        [GetNode("Tween")] private Tween tween;

        public override void _Ready() {
            GetNodeAttribute.Load(this);
        }

        public void Render(Icon renderIcon, int value, Vector2 travelPath, float duration) {
            icon.RegionRect = renderIcon.ToRect();

            var sign = value >= 0 ? "+" : "";

            var text = $"{sign}{value}";
            label.Text = text;

            RectPivotOffset = RectSize * 0.5f;

            tween.InterpolateProperty(
                this,
                "rect_position",
                RectPosition,
                RectPosition + travelPath,
                duration
            );

            tween.Start();
            ToSignal(tween, "tween_all_completed").OnCompleted(QueueFree);
        }
    }
}
