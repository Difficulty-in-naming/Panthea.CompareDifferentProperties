using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Panthea.CompareDifferentProperties.Editor
{
    internal class DictionaryCreator
    {
        public static void Access(InternalStringBuilder sb, MemberInfo a, Type targetType)
        {
            var iDict = a.GetUnderlyingType().Implement(typeof (IDictionary<,>));
            var keyType = iDict.GenericTypeArguments[0];
            var valueType = iDict.GenericTypeArguments[1];
            sb.AppendLine($"foreach(var node in a.{a.Name})");
            sb.AppendLine("{");
            sb.Indeed++;
            sb.AppendLine($"if(!b.{a.Name}.TryGetValue(node.Key,out var bResult))");
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
            sb.AppendLine($"result.{a.Name}[node.Key] = node.Value;");
            sb.AppendLine("isChanged = true;");
            sb.AppendLine($"changed.Add(\"{a.Name}\");");
            sb.Indeed--;
            sb.AppendLine("}");
            sb.AppendLine("else");
            sb.AppendLine("{");
            sb.Indeed++;
            if (valueType.IsValueType || valueType == typeof (string))
            {
                sb.AppendLine("if(bResult != node.Value)");
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
                sb.AppendLine($"result.{a.Name}[node.Key] = node.Value;");
                sb.AppendLine("isChanged = true;");
                sb.AppendLine($"changed.Add(\"{a.Name}\");");
                sb.Indeed--;
                sb.AppendLine("}");
            }
            else
            {
                GenerateCode.MethodName[valueType.TrueTypeString()] = () => ClassCreator.Check(sb, valueType);
                sb.AppendLine("if(!IsEqual(bResult,node.Value))");
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
                sb.AppendLine($"result.{a.Name}[node.Key] = node.Value;");
                sb.AppendLine("isChanged = true;");
                sb.AppendLine($"changed.Add(\"{a.Name}\");");
                sb.Indeed--;
                sb.AppendLine("}");
            }
            sb.Indeed--;
            sb.AppendLine("}");
            sb.Indeed--;
            sb.AppendLine("}");
        }

        public static void Check(InternalStringBuilder sb, MemberInfo a)
        {
            var iDict = a.GetUnderlyingType().Implement(typeof (IDictionary<,>));
            var keyType = iDict.GenericTypeArguments[0];
            var valueType = iDict.GenericTypeArguments[1];
            sb.AppendLine($"foreach(var node in a.{a.Name})");
            sb.AppendLine("{");
            sb.Indeed++;
            sb.AppendLine($"if(!b.{a.Name}.TryGetValue(node.Key,out var bResult))");
            sb.AppendLine("{");
            sb.Indeed++;
            sb.AppendLine("return false");
            sb.Indeed--;
            sb.AppendLine("}");
            sb.Indeed--;
            sb.AppendLine("}");
            sb.AppendLine("else");
            sb.AppendLine("{");
            if (valueType.IsValueType || valueType == typeof (string))
            {
                sb.AppendLine("if(bResult != node.Value)");
                sb.AppendLine("{");
                sb.Indeed++;
                sb.AppendLine("return false");
                sb.Indeed--;
                sb.AppendLine("}");
            }
            else
            {
                GenerateCode.MethodName[valueType.TrueTypeString()] = () => ClassCreator.Check(sb, valueType);
                sb.AppendLine("if(!IsEqual(bResult != node.Value))");
                sb.AppendLine("{");
                sb.Indeed++;
                sb.AppendLine("return false");
                sb.Indeed--;
                sb.AppendLine("}");
            }
            sb.Indeed--;
            sb.AppendLine("}");
            sb.Indeed--;
            sb.AppendLine("}");
        } 
    }
}