using System.Reflection;

namespace Panthea.CompareDifferentProperties.Editor
{
    internal static class ValueTypeCreator
    {
        public static void Access(InternalStringBuilder sb, MemberInfo a)
        {
            sb.AppendLine($"if(a.{a.Name} != b.{a.Name})");
            sb.AppendLine("{");
            sb.Indeed++;
            sb.AppendLine($"result.{a.Name} = a.{a.Name};");
            sb.AppendLine("isChanged = true;");
            sb.AppendLine($"changed.Add(\"{a.Name}\");");
            sb.Indeed--;
            sb.AppendLine("}");

        }

        public static void Check(InternalStringBuilder sb, MemberInfo a)
        {
            sb.AppendLine($"if(a.{a.Name} != b.{a.Name})");
            sb.Indeed++;
            sb.AppendLine($"return false;");
            sb.Indeed--;
        }
    }

}
