using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace X.AspNet.Services.Multilingual
{
//    [Core.DataAccess.Databases.Attributes.DBObjectName("Fwk_Translation")]
    public partial class TranslationDTO
    {
//        [Core.DataAccess.Attributes.Key]
        public string Key { get; set; }
//        [Core.DataAccess.Attributes.Key]
        public string LanguageCode { get; set; }
        public string Value { get; set; }

        public override string ToString()
        {
            return string.Format("{0}: {1} => {2}", Key, LanguageCode, Value);
        }
    }
}
