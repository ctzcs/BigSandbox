using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace AutoTranfer
{
    public class SerializeData
    {
        public string nameSpace = "";
        public string structName = "";
        public List<string> params2Serialize = new List<string>();
    }
    /// <summary>
    /// 语法树结点,收集数据
    /// </summary>
    class AutoTransferSyntaxReceiver:ISyntaxReceiver
    {
        public List<SerializeData> serializeDataList = new List<SerializeData>();
        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (syntaxNode is StructDeclarationSyntax sds && sds.AttributeLists.FirstOrDefault(node=>node.Attributes.ToString().Contains("AutoTransfer"))!= null)
            {
                SerializeData serializeData = new SerializeData();
                var namespaceNode = syntaxNode.FirstAncestorOrSelf<NamespaceDeclarationSyntax>();
                if (namespaceNode != null)
                {
                    serializeData.nameSpace = namespaceNode.Name.ToString();
                }

                serializeData.structName = sds.Identifier.ToString();

                foreach (var member in sds.Members)
                {
                    foreach (var attribute in member.AttributeLists)
                    {
                        var targetAttribute =
                            attribute.Attributes.FirstOrDefault(node => node.Name.ToString() == "AutoTransferField");
                        if (targetAttribute != null)
                        {
                            foreach (var variable in ((FieldDeclarationSyntax)member).Declaration.Variables)
                            {
                                serializeData.params2Serialize.Add($"({variable.Identifier.ToString()})");
                            }
                        }
                    }
                }
                
                serializeDataList.Add(serializeData);

            }
        }
    }
    
    [Generator]
    public class AutoCodeGen:ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(()=>new AutoTransferSyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            if (context.SyntaxReceiver is AutoTransferSyntaxReceiver autoTransferSyntaxReceiver)
            {
                foreach (var data in autoTransferSyntaxReceiver.serializeDataList)
                {
                    string srcCode = "";
                    srcCode += $"public partial struct {data.structName}{{\n";
                    srcCode += "public void DoTransfer(ITransferAction action){\n";
                    foreach (var param in data.params2Serialize)
                    {
                        srcCode += $"action.Transfer{param};\n";
                        srcCode += "}}";
                        //写到这个文件中，参与编译，在dll生成的过程中插了一腿
                        context.AddSource($"{data.structName}.gen.cs",SourceText.From(srcCode,Encoding.UTF8));
                    }
                    
                }
            }
        }
    }
}