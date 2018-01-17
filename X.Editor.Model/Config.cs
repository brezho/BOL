using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace X.Editor.Model
{
    public static class Config
    {
        public static IHierarchyProvider GetProvider(params string[] args)
        {
            FileInfo fileToLoad = null;

            Trace.WriteLine("Looking for a HierarchyProvider");

            Trace.WriteLine("-- In args");
            if (args != null && args.Length > 0)
            {
                var loc = args[0];
                if (File.Exists(loc))
                {
                    fileToLoad = new FileInfo(loc);
                }
            }
            if (fileToLoad == null)
            {
                Trace.WriteLine("-- In App.config");
                var cfg = System.Configuration.ConfigurationManager.AppSettings["ModelPath"];
                if (cfg != null)
                {
                    var cur = Environment.CurrentDirectory;
                    if (File.Exists(cfg))
                    {
                        fileToLoad = new FileInfo(cfg);
                    }
                }
            }

            // TODO : re-enable TypeCatalog loading one file at a time

            if (fileToLoad != null)
            {
                Trace.WriteLine("Loading from " + fileToLoad.FullName);
                
                TypeCatalog.Instance.Add(Assembly.LoadFrom(fileToLoad.FullName));
                var ctlgSearch = TypeCatalog.Instance.FirstOrDefault(x => x.IsConcrete() && x.Match(typeof(IHierarchyProvider)));

                if (ctlgSearch != null)
                {
                    Trace.WriteLine("Identified IHierarchyProvider =>  " + ctlgSearch.FullName);
                    try
                    {
                        var obj = ctlgSearch.Hype().GetOne();
                        return (IHierarchyProvider)obj;
                    }
                    catch (Exception x)
                    {
                        Trace.WriteLine(x.Format());
                        throw new ApplicationException(fileToLoad.FullName + " does not have a valid IHierarchyProvider implementation");
                    }
                }
            }
            throw new ApplicationException("Unable to locate any Hierarchy Provider" );
        }
    }
}
