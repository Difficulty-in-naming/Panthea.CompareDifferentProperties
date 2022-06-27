using System;
using System.Collections.Generic;
using System.Reflection;

namespace Panthea.CompareDifferentProperties.Editor
{
    internal class HashSetCreator
    {
        public static void Access(InternalStringBuilder sb, MemberInfo a,Type targetType)
        {
            var genericType = a.GetUnderlyingType().Implement(typeof (ISet<>)).GenericTypeArguments[0];
            sb.AppendLine($"foreach(var node in a.{a.Name})");
            sb.AppendLine("{");
            sb.Indeed++;
            if (genericType.IsValueType || genericType == typeof (string))
            {
                sb.AppendLine($"if(!b.{a.Name}.Contains(node))");
                sb.AppendLine("{");
                sb.Indeed++;
                if (a.CanSet())
                {
                    sb.AppendLine($"if(result.{a.Name} == null)");
                    sb.AppendLine("{");
                    sb.Indeed++;
                    sb.AppendLine($"result.{a.Name} = new {targetType.TrueTypeString()}();");
                    sb.Indeed--;
                    sb.AppendLine("}");
                }
                sb.AppendLine($"result.{a.Name}.Add(node);");
                sb.AppendLine("isChanged = true;");
                sb.AppendLine($"changed.Add(\"{a.Name}\");");
                sb.Indeed--;
                sb.AppendLine("}");
            }
            else
            {
                GenerateCode.MethodName[genericType.FullName] = () => ClassCreator.Check(sb, genericType);
                sb.AppendLine("bool pass = false;");
                sb.AppendLine($"foreach(var node in b.{a.Name})");
                sb.AppendLine("{");
                sb.Indeed++;
                sb.AppendLine("if(!IsEqual(a, b))");
                sb.AppendLine("{");
                sb.Indeed++;
                sb.AppendLine("pass = true;");
                sb.AppendLine("break;");
                sb.Indeed--;
                sb.AppendLine("}");
                sb.Indeed--;
                sb.AppendLine("}");
                sb.AppendLine($"if(pass)");
                sb.AppendLine("{");
                sb.Indeed++;
                if (a.CanSet())
                {
                    sb.AppendLine($"if(result.{a.Name} == null)");
                    sb.AppendLine("{");
                    sb.Indeed++;
                    sb.AppendLine($"result.{a.Name} = new {targetType.TrueTypeString()}();");
                    sb.Indeed--;
                    sb.AppendLine("}");
                }
                sb.AppendLine($"result.{a.Name}.Add(node);");
                sb.AppendLine("isChanged = true;");
                sb.AppendLine($"changed.Add(\"{a.Name}\");");
                sb.Indeed--;
                sb.AppendLine("}");
            }
            sb.Indeed--;
            sb.AppendLine("}");
        }

        public static void Check(InternalStringBuilder sb, MemberInfo a)
        {
            var genericType = a.GetUnderlyingType().Implement(typeof (ISet<>)).GenericTypeArguments[0];
            if (genericType.IsValueType || genericType == typeof (string))
            {
                sb.AppendLine($"foreach(var node in a.{a.Name})");
                sb.AppendLine("{");
                sb.Indeed++;
                sb.AppendLine($"if(!b.{a.Name}.Contains(node))");
                sb.AppendLine("{");
                sb.Indeed++;
                sb.AppendLine($"return false;");
                sb.Indeed--;
                sb.AppendLine("}");
                sb.Indeed--;
                sb.AppendLine("}");
            }
            else
            {
                GenerateCode.MethodName[genericType.TrueTypeString()] = () => ClassCreator.Check(sb, genericType);
                sb.AppendLine($"foreach(var node in a.{a.Name})");
                sb.AppendLine("{");
                sb.Indeed++;
                sb.AppendLine("if(!IsEqual(a, b))");
                sb.AppendLine("{");
                sb.Indeed++;
                sb.AppendLine($"return false;");
                sb.Indeed--;
                sb.AppendLine("}");
                sb.Indeed--;
                sb.AppendLine("}");
            }
        }
    }
}