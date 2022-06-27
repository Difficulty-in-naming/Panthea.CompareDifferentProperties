using System;

namespace Panthea.CompareDifferentProperties.Editor
{
    internal static class ImplementUtils
    {
        internal static Type Implement(this Type type, Type targetType)
        {
            foreach (var node in type.GetInterfaces())
            {
                if (node.IsGenericType)
                {
                    if (node.GetGenericTypeDefinition() == targetType)
                    {
                        return node;
                    }
                }
            }

            return null;
        }
    }
}