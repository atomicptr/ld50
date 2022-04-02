using System;
using System.Reflection;
using Godot;
using JetBrains.Annotations;

namespace LD50.Common {
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    [MeansImplicitUse(ImplicitUseKindFlags.Assign)]
    public class GetNodeAttribute : Attribute {
        public readonly NodePath Path;
        public readonly bool ResolveAsField;

        public GetNodeAttribute(string path, bool resolveAsField = false) {
            Path = path;
            ResolveAsField = resolveAsField;
        }

        public static void Load<T>(T instance) where T : Node {
            var fields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (var field in fields) {
                var attributes = field.GetCustomAttributes(true);

                foreach (var attribute in attributes) {
                    if (attribute is GetNodeAttribute getNodeAttrib) {
                        var path = getNodeAttrib.ResolveAsField
                            ? ResolvePathFromField(instance, getNodeAttrib.Path)
                            : getNodeAttrib.Path;

                        if (path == null) {
                            return;
                        }

                        var resource = instance.GetNodeOrNull(path);
                        field.SetValue(instance, resource);
                    }
                }
            }
        }

        private static NodePath ResolvePathFromField<T>(T instance, string fieldName) where T : Node {
            var field = typeof(T).GetField(fieldName);
            var value = field.GetValue(instance);

            if (value is string || value is NodePath) {
                return (NodePath) value;
            }

            return null;
        }
    }
}
