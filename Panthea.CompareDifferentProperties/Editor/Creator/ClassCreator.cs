using System;
using System.Collections.Generic;
using System.Reflection;

namespace Panthea.CompareDifferentProperties.Editor
{
    internal class ClassCreator
    {
        public static void Check(InternalStringBuilder sb, MemberInfo a)
        {
            sb.AppendLine($"private static bool IsEqual({a.GetUnderlyingType().TrueTypeString()} a, {a.GetUnderlyingType().TrueTypeString()} b)");
            sb.AppendLine("{");
            sb.Indeed++;
            foreach (var node in a.GetMembers())
            {
                var memberType = node.GetUnderlyingType();
                if (memberType.IsValueType || memberType == typeof (string))
                {
                    ValueTypeCreator.Check(sb, node);
                }
                else if (memberType.Implement(typeof (ISet<>)) != null)
                {
                    HashSetCreator.Check(sb, node);
                }
                else if (memberType.Implement(typeof (IDictionary<,>)) != null)
                {
                    DictionaryCreator.Check(sb, node);
                }
                else if (memberType.Implement(typeof (IList<>)) != null)
                {
                    ListCreator.Check(sb, node);
                }
            }
            sb.AppendLine("return true;");
            sb.Indeed--;
            sb.AppendLine("}");
        }

        public static void Access(InternalStringBuilder sb, Type source,Type target)
        {
            List<MemberInfo> sourceMember = source.GetAllVar();
            List<MemberInfo> targetMember = target.GetAllVar();

            foreach (var p in sourceMember)
            {
                if (p.GetCustomAttributes(typeof(IgnoreCompare), true).Length > 0)
                    continue;
                var m = targetMember.Find(t1 => t1.Name == p.Name);
                if (m != null)
                {
                    if (m.GetCustomAttributes(typeof(IgnoreCompare), true).Length > 0)
                        continue;
                    var propertyType = p.GetUnderlyingType();
                    if ((propertyType.IsValueType || propertyType == typeof (string)))
                    {
                        if (m.CanSet())
                        {
                            ValueTypeCreator.Access(sb, m);
                        }
                    }
                    else if (propertyType.Implement(typeof (ISet<>)) != null)
                    {
                        HashSetCreator.Access(sb, p, m.GetUnderlyingType());
                    }
                    else if (propertyType.Implement(typeof (IDictionary<,>)) != null)
                    {
                        DictionaryCreator.Access(sb, p, m.GetUnderlyingType());
                    }
                    else if (propertyType.Implement(typeof (IList<>)) != null)
                    {
                        ListCreator.Access(sb, p, m.GetUnderlyingType());
                    }
                    else
                    {
                        GenerateCode.MethodName[p.GetUnderlyingTypeString()] = ()=>CreateClassCompare(sb, p.GetUnderlyingType(), m.GetUnderlyingType());
                        sb.AppendLine($"var __{p.Name}Tuple = GetDiff_{p.GetUnderlyingTypeString().Replace(".", "_")}(a.{p.Name},b.{p.Name});");
                        sb.AppendLine($"if(__{p.Name}Tuple.IsChanged)");
                        sb.AppendLine("{");
                        sb.Indeed++;
                        sb.AppendLine($"result.{m.Name} = __{p.Name}Tuple.Result;");
                        sb.AppendLine("isChanged = true;");
                        sb.AppendLine($"changed.AddRange(__{p.Name}Tuple.Changed);");
                        sb.Indeed--;
                        sb.AppendLine("}");
                    }
                }
            }
        }

        private static void CreateClassCompare(InternalStringBuilder sb, Type a,Type b)
        {
            if (a.GetCustomAttributes(typeof(IgnoreCompare), true).Length > 0)
                return;
            if (b.GetCustomAttributes(typeof(IgnoreCompare), true).Length > 0)
                return;
            sb.AppendLine(
                $"private static ({a.TrueTypeString()} Result,bool IsChanged,List<string> Changed) GetDiff_{a.TrueTypeString().Replace(".", "_")}({a.TrueTypeString()} a, {b.TrueTypeString()} b)");
            sb.AppendLine("{");
            sb.Indeed++;
            sb.AppendLine($"var result = new {a.TrueTypeString()}();");
            sb.AppendLine($"var changed = new List<string>(32);");
            sb.AppendLine($"var isChanged = false;");
            Access(sb, a, b);
            sb.AppendLine("return (result,isChanged,changed);");
            sb.Indeed--;
            sb.AppendLine("}");
        }
    }
}