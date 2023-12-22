using System.CodeDom.Compiler;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;

namespace XsdClassesNormalizer
{
    // for generate classes run command
    // xsd.exe CategoriesCust.xsd CommonAggregateTypesCust.xsd CommonLeafTypesCust.xsd CUESADCommonAggregateTypesCust.xsd CUESADCommonLeafTypesCust.xsd ESADout_CU.xsd RUDeclCommonAggregateTypesCust.xsd RUSCommonAggregateTypes.xsd RUSCommonLeafTypes.xsd /c /o:"C:\Users\v.zmeev\Documents\xsd\GTD_CS"

    internal class Program
    {
        static void Main(string[] args)
        {
            string namespaceModels = "DocCloudSDK.Models";
            string dirOut = @"C:\Users\v.zmeev\source\repos\api\DocCloudSuite\DocCloudSDK\Models";

            var assmbly = typeof(Program).Assembly;
            var types = assmbly.GetTypes().Where(x => x.GetCustomAttribute<GeneratedCodeAttribute>() != null).ToArray();

            foreach (var t in types)
            {
                var xmlTypeAttribute = t.GetCustomAttribute<XmlTypeAttribute>();
                var namespaceParts = xmlTypeAttribute.Namespace.Split(':').ToList();
                namespaceParts.RemoveAt(0);
                namespaceParts.RemoveAt(0);
                if (namespaceParts[0] == "Information")
                    namespaceParts.RemoveAt(0);
                namespaceParts.RemoveAt(namespaceParts.Count - 1);
                //if (namespaceParts[namespaceParts.Count - 1] == "ESADout_CU")
                //    namespaceParts.RemoveAt(namespaceParts.Count - 1);
                string dir = Path.Combine(dirOut, string.Join("\\", namespaceParts));

                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                string fileName = Path.Combine(dir, t.Name) + ".cs";

                var sb = new StringBuilder();
                sb.AppendLine($"namespace {namespaceModels}.{string.Join(".", namespaceParts)}");
                sb.AppendLine("{");
                sb.Append($"\tpublic class {t.Name}");
                if (t.BaseType != typeof(object))
                    sb.AppendLine($" : {t.BaseType.Name}");
                else
                    sb.AppendLine();
                sb.AppendLine("\t{");

                foreach (var p in t.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
                {
                    string normalTypeName;
                    switch (p.PropertyType.Name)
                    {
                        case "String": normalTypeName = "string"; break;
                        case "String[]": normalTypeName = "string[]"; break;
                        case "Boolean": normalTypeName = "bool"; break;
                        case "Boolean[]": normalTypeName = "bool[]"; break;
                        case "Int32": normalTypeName = "int"; break;
                        case "Int32[]": normalTypeName = "int[]"; break;
                        case "Decimal": normalTypeName = "decimal"; break;
                        case "Decimal[]": normalTypeName = "decimal[]"; break;
                        case "Double": normalTypeName = "double"; break;
                        case "Double[]": normalTypeName = "double[]"; break;
                        case "Object": normalTypeName = "object"; break;
                        case "Object[]": normalTypeName = "object[]"; break;
                        default: normalTypeName = p.PropertyType.Name; break;
                    }

                    sb.AppendLine($"\t\tpublic {normalTypeName} {p.Name} {{ get; set; }}");
                }

                sb.AppendLine("\t}");
                sb.AppendLine("}");

                File.WriteAllText(fileName, sb.ToString(), Encoding.UTF8);
            }
        }


    }
}
