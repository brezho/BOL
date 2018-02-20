using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace X.Packaging
{
    [Serializable]
    public class ConfigPackageDefinition
    {
        public ConfigPackageDefinition()
        {
            Relations = new List<ConfigPackageRelation>();
        }
        public string Name { get; set; }
        public string Version { get; set; }
        public List<ConfigPackageRelation> Relations { get; set; }
    }

    public class ConfigPackageRelation
    {
        public string Name { get; set; }
        public ConfigPackagePartType SourcePartType { get; set; }
        public ConfigPackagePartType TargetType { get; set; }
    }
    public class ConfigPackagePartType
    {
        public string Name { get; set; }
    }
}
