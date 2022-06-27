using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Panthea.CompareDifferentProperties.Editor
{
    public static class GenerateCode
    {
        public static Dictionary<string,Action> MethodName = new Dictionary<string,Action>();
        [MenuItem(("Tools/生成协议对比代码"))]
        public static void DoIt()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            Dictionary<Type, Type> types = new Dictionary<Type, Type>();
            var targetType = typeof (IPropertiesCompare<>);
            foreach (var ass in assemblies)
            {
                foreach (var t in ass.GetTypes())
                {
                    var generic = t.Implement(targetType);
                    if (generic != null)
                        types[t] = generic.GenericTypeArguments[0];
                }
            }

            InternalStringBuilder sb = new InternalStringBuilder();
            sb.AppendLine("using System.Collections.Generic;\n");
            sb.AppendLine("public static class Compare");
            sb.AppendLine("{");
            sb.Indeed++;
            foreach (var node in types)
            {
                if (node.Value.GetCustomAttributes(typeof(IgnoreCompare), true).Length > 0)
                    continue;
                sb.AppendLine(
                    $"public static ({node.Value.FullName} Result,bool IsChanged,List<string> Changed) GetDiff_{node.Key.TrueTypeString().Replace(".", "_")}({node.Key.TrueTypeString()} a,{node.Key.TrueTypeString()} b)");
                sb.AppendLine("{");
                sb.Indeed++;
                sb.AppendLine($"var result = new {node.Value.FullName}();");
                sb.AppendLine($"var isChanged = false;");
                sb.AppendLine($"var changed = new List<string>(32);");
                ClassCreator.Access(sb, node.Key, node.Value);
                sb.AppendLine("return (result,isChanged,changed);");
                sb.Indeed--;
                sb.AppendLine("}");
            }

            while(MethodName.Count > 0)
            {
                var node = MethodName.First();
                node.Value();
                MethodName.Remove(node.Key);
            }
            sb.Indeed--;
            sb.AppendLine("}");
            System.IO.File.WriteAllText(Application.dataPath + "/Scripts/CodeGenerate/CompareUtils.cs", sb.ToString());
        }
    }
}
