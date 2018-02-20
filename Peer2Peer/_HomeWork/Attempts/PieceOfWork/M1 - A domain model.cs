#pragma warning disable 0169, 0414
using CodeGen.M2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PieceOfWork
{
    public class M2Config
    {
        public static NodeObject GetModel()
        {
            var name = new NodePropertySpec { Name = "Name", DefaultValue = (string) null };

            var rootNode = new NodeObject();
            rootNode.Properties.Add(name);
            rootNode.Properties.Add(new NodePropertySpec { Name = "Entities" });

            rootNode[name] = "Application";
           
            var allEn = NodeObject.AllNodes;

            return rootNode;
        }
    }

}

#pragma warning restore 0169, 0414
