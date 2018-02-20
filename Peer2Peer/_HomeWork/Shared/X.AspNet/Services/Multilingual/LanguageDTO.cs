using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace X.AspNet.Services.Multilingual
{
  //  [Core.DataAccess.Databases.Attributes.DBObjectName("Fwk_Language")]
    public partial class LanguageDTO
    {
    //    [Core.DataAccess.Attributes.Key]
        public string Code { get; set; }
        public bool IsDefault { get; set; }
        public bool IsEnabled { get; set; }
        public string Description { get; set; }
    }
}
