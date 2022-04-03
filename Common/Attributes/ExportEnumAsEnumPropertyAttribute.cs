using System;
using System.Linq;
using Godot;

namespace LD50.Common {
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class ExportEnumAsEnumPropertyAttribute : ExportAttribute {
        public ExportEnumAsEnumPropertyAttribute(Type enumType) : base(
            PropertyHint.Flags,
            getFlagsHint(enumType)
        ) {
        }

        private static string getFlagsHint(Type enumType) {
            if (!enumType.IsEnum) {
                return "Type is not an enum";
            }

            var values = Enum.GetValues(enumType)
                .OfType<Enum>()
                .Where(val => (int) (object) val != 0);
            return string.Join(",", values);
        }
    }
}
