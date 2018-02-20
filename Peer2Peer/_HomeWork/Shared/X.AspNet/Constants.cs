using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace X.AspNet
{
    public static class Constants
    {


        public static class Directories
        {
            const string GProd = "GProd";
            static Directories()
            {
                if (!Directory.Exists(RootDirectory)) Directory.CreateDirectory(RootDirectory);
                if (!Directory.Exists(TraceDirectory)) Directory.CreateDirectory(TraceDirectory);
                if (!Directory.Exists(TempDirectory)) Directory.CreateDirectory(TempDirectory);
                if (!Directory.Exists(DataDirectory)) Directory.CreateDirectory(DataDirectory);
            }

            public static readonly string RootDirectory = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.CommonApplicationData), GProd, X.AspNet.Utils.EnvironmentHelper.ApplicationName);
            public static readonly string TraceDirectory = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.CommonApplicationData), GProd, X.AspNet.Utils.EnvironmentHelper.ApplicationName, "Trace");
            public static readonly string TempDirectory = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.CommonApplicationData), GProd, X.AspNet.Utils.EnvironmentHelper.ApplicationName, "Temp");
            public static readonly string DataDirectory = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.CommonApplicationData), GProd, X.AspNet.Utils.EnvironmentHelper.ApplicationName, "Data");
        }



        public static class UrlParameter
        {
            public const string Master = "Master";
            public const string Print = "Print";
            public static class MasterValues
            {
                public const string Default = "Default";
                public const string Popup = "Popup";
            }
        }
        public static class RegularExpressions
        {
            public const string EmailAddress =
                @"^[a-zA-Z0-9][\w'\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]$";

            public const string Url =
                @"^(ftp|http|https):\/\/(\w+:{0,1}\w*@)?(\S+)(:[0-9]+)?(\/|\/([\w#!:.?+=&%@!\-\/]))?$";

            public const string Decimal = @"^[+\-]?(([\d]+(,[0-9]{3})*(\.\d*)?)|\.\d+)([+-]?[0-9]+)?";

            public const string ForbiddenFileCharacters = @"[:<>?*""/\\]";
        }

    }
}
