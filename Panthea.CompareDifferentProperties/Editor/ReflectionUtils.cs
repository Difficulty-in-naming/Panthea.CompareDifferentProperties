using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Panthea.CompareDifferentProperties.Editor
{
    public static class ReflectionUtils
    {
        public static Type GetUnderlyingType(this MemberInfo member)
        {
            switch (member.MemberType)
            {
                case MemberTypes.Event:
                    return ((EventInfo)member).EventHandlerType;
                case MemberTypes.Field:
                    return ((FieldInfo)member).FieldType;
                case MemberTypes.Method:
                    return ((MethodInfo)member).ReturnType;
                case MemberTypes.Property:
                    return ((PropertyInfo)member).PropertyType;
                case MemberTypes.TypeInfo:
                case MemberTypes.NestedType:
                    return ((Type)member);
                default:
                    throw new ArgumentException
                    (
                        "Input MemberInfo must be if type EventInfo, FieldInfo, MethodInfo, or PropertyInfo"
                    );
            }
        }
        public static string GetUnderlyingTypeString(this MemberInfo member)
        {
            return member.GetUnderlyingType().TrueTypeString();
        }

        public static string TrueTypeString(this Type type)
        {
            var str = type.ToString().Replace("+", ".");
            var index = str.IndexOf('`');
            if (index > 0)
            {
                str = str.Replace("`1[", "<");
                str = str.Replace("`2[", "<");
                str = str.Replace("`3[", "<");
                str = str.Replace("`4[", "<");
                str = str.Replace("`5[", "<");
                str = str.TrimEnd(']') + ">";
            }

            return str;
        }
        
        public static List<MemberInfo> GetMembers(this MemberInfo member)
        {
            var type = member.GetUnderlyingType();
            return type.GetFields().Cast<MemberInfo>().Concat(type.GetProperties(BindingFlags.Instance | BindingFlags.Public)).ToList();
        }
        
        public static List<MemberInfo> GetAllVar(this Type type)
        {
            return type.GetFields().Cast<MemberInfo>().Concat(type.GetProperties(BindingFlags.Instance | BindingFlags.Public)).ToList();
        }

        public static bool CanSet(this MemberInfo member)
        {
            switch (member.MemberType)
            {
                case MemberTypes.Field:
                    var field = ((FieldInfo)member);
                    return !field.IsInitOnly;
                case MemberTypes.Property:
                    var property = ((PropertyInfo)member);
                    return property.GetSetMethod()?.IsPublic ?? false;
                default:
                    throw new ArgumentException
                    (
                        "Input MemberInfo must be if type EventInfo, FieldInfo, MethodInfo, or PropertyInfo"
                    );
            }
        }
    }
}