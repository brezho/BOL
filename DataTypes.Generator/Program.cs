using System;
using System.Helpers;
using System.IO;

namespace DataTypes.Generator
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0) args = new[] { @"C:\Workspaces\BusinessOrientedLanguage\BOL\DataTypes.Definitions\Archive.ini" };


            var dataTypeDefinition = new IniReader(args[0]);
            var name = dataTypeDefinition.GetValue("DataType", "Name");
            var isEquatable = bool.Parse(dataTypeDefinition.GetValue("DataType", "IsEquatable", bool.FalseString));

            Console.WriteLine("DataType: " + name);

            var properties = dataTypeDefinition.GetKeys("Properties");
            Console.WriteLine("Properties: " + string.Join("; ", properties));

            var builder = new CodeGenStringBuilder();
            builder.AppendLine("using System;");
            builder
                .AppendLine("namespace DataTypes")
                    .Enclose(namespaceBody =>
                    {
                        namespaceBody.Append("public class " + name);

                        if (isEquatable) namespaceBody.InLine(" : ");

                        if (isEquatable)
                        {
                            namespaceBody.InLine("IEquatable<" + name + ">");
                        }


                        namespaceBody.AppendLine().Enclose(classBody =>
                        {
                            var parameters = string.Empty;
                            bool secondOrMore = false;
                            foreach (var property in properties)
                            {
                                if (secondOrMore) {
                                    parameters += ", ";
                                }
                                parameters += dataTypeDefinition.GetValue("Properties", property) + " " + property;
                                secondOrMore = true;
                            }

                            classBody.AppendLine("public " + name + "(" + parameters + ")")
                            .Enclose(constructorBody =>
                            {
                                foreach (var property in properties)
                                {
                                    constructorBody.AppendLine("_" + property + " = " + property + ";");
                                }
                            });

                            foreach (var property in properties)
                            {
                                classBody.AppendLine(dataTypeDefinition.GetValue("Properties", property) + " _" + property + ";");
                            }

                            if (isEquatable)
                            {
                                classBody.AppendLine("public bool Equals(" + name + " other)")
                                .Enclose(methodBody =>
                                {
                                    methodBody.AppendLine("if (other == null) return false;");

                                    foreach (var property in properties)
                                    {
                                        methodBody.AppendLine("if (other._" + property + " != _" + property + ") return false;");
                                    }

                                    methodBody.AppendLine("return true;");
                                });

                                classBody.AppendLine("public static bool operator ==(" + name + " x, " + name + " y)")
                                .Enclose(methodBody =>
                                {
                                    methodBody.AppendLine("if ((object)x == null) return ((object)y == null);");
                                    methodBody.AppendLine("return x.Equals(y);");
                                });

                                classBody.AppendLine("public static bool operator !=(" + name + " x, " + name + " y)")
                                .Enclose(methodBody =>
                                {
                                    methodBody.AppendLine("return !(x == y);");
                                });

                                classBody.AppendLine("public override int GetHashCode()")
                                .Enclose(methodBody =>
                                {
                                    methodBody.AppendLine("int hashCode = 17;");

                                    foreach (var property in properties)
                                    {

                                        methodBody.AppendLine("if (_" + property + " != default(" + dataTypeDefinition.GetValue("Properties", property) + ")) hashCode = hashCode * 59 + _" + property + ".GetHashCode();");
                                        //  methodBody.AppendLine("if (_" + property + " != null) hashCode = hashCode * 59 + _" + property + ".GetHashCode();");
                                    }
                                    methodBody.AppendLine("return hashCode;");
                                });

                                classBody.AppendLine("public override bool Equals(object other)")
                                .Enclose(methodBody =>
                                {
                                    methodBody.AppendLine("if (other == null) return false;");
                                    methodBody.AppendLine("if (other.GetType() != this.GetType()) return false;");
                                    methodBody.AppendLine("return this.Equals((" + name + ")other);");
                                });

                            }

                        })
                        ;
                    });

            Console.WriteLine(builder.ToString());


            // generate output file
            var fileName = Path.Combine(Path.GetTempPath(), name + ".cs");
            File.WriteAllText(fileName, builder.ToString());

            Console.WriteLine(fileName);
        }
    }
}
