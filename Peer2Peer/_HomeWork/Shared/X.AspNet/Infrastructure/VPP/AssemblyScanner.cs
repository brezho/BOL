using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Web.Hosting;
using System.Web.Caching;
using System.Reflection;
using System.Diagnostics;
using System.Web;
using System.Web.Compilation;
using X.AspNet.Infrastructure.VPP;

namespace Host.Infrastructure.VPP
{
    public static class AssemblyScanner
    {
        public static StreamReader GetResourceStream(this Assembly item, string resourceName)
        {
            var list = item.GetManifestResourceNames().Where(resName => resName.ToLower().EndsWith(resourceName.ToLower()));

            if (list.Count() == 0)
            {
                throw new ApplicationException("Unable to find embedded resource : " + resourceName);
            }

            return list.Select(resName => new StreamReader(item.GetManifestResourceStream(resName))).FirstOrDefault();
        }
        public static string GetResourceContent(this Assembly item, string resourceName)
        {
            var sr = item.GetResourceStream(resourceName);
            var data = sr.ReadToEnd();
            sr.Close();
            return data;
        }
        private static List<EmbeddedResource> _AllResources = null;
        static object locker = new object();

        public static List<EmbeddedResource> AllResources
        {
            get
            {
                if (_AllResources == null)
                {
                    lock (locker)
                    {
                        if (_AllResources == null)
                        {
                            _AllResources = new List<EmbeddedResource>();
                            AutoScan();
                        }
                    }
                }
                return _AllResources;
            }
        }

        internal static EmbeddedResource GetResource(string virtualPath)
        {
            virtualPath = virtualPath.ToLowerInvariant();
            return AllResources.FirstOrDefault(x => x.VirtualPath == virtualPath);
        }

        internal static bool Contains(string virtualPath)
        {
            virtualPath = virtualPath.ToLowerInvariant();
            return AllResources.Any(x => x.VirtualPath == virtualPath);
        }

        public static string GetVirtualPath(string Name)
        {
            return AllResources
                .Where(x => string.Equals(x.Name, Name, StringComparison.InvariantCultureIgnoreCase))
                .Select(x => x.VirtualPath)
                .FirstOrDefault();
        }

        public static void ReleaseVirtualDirectory(string virtualDir)
        {
            AllResources.RemoveAll(x => x.VirtualPath.StartsWith(virtualDir));
        }
        public static void LoadAssemblies(string directoryPath)
        {
            var loadedAssemblyNames = AppDomain.CurrentDomain
                                        .GetAssemblies()
                                        .Select(x => x.GetName().Name)
                                        .ToArray();

            if (directoryPath != null)
            {
                var directoryInfo = new DirectoryInfo(directoryPath);
                if (directoryInfo.Exists)
                {
                    var files = directoryInfo
                        .GetFiles("*.*", SearchOption.AllDirectories)
                        .Select(y => new { FullName = y.FullName, Name = Path.GetFileNameWithoutExtension(y.Name) })
                        .Where(x => !loadedAssemblyNames.Contains(x.Name))
                       ;

                    foreach (var x in files)
                    {
                        try
                        {
                            Assembly.LoadFrom(x.FullName);
                        }
                        catch { }
                    }
                }
            }
        }

        public static void AutoScan()
        {
            LoadAssemblies(HostingEnvironment.IsHosted ? HttpRuntime.BinDirectory : Path.GetDirectoryName(typeof(AssemblyScanner).Assembly.Location));


            // LoadAssemblies(Path.GetDirectoryName(typeof(AssemblyScanner).Assembly.Location));

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    var virtualDirName = assembly.GetName().Name;
                    var attrib = assembly.GetCustomAttributes(typeof(VirtualDirectoryNameAttribute), false).FirstOrDefault();
                    if (attrib != null)
                    {
                        virtualDirName = (attrib as VirtualDirectoryNameAttribute).VirtualDirectoryAlias;
                    }
                    Scan(assembly, virtualDirName);
                }
                catch (Exception x)
                {
                    System.Diagnostics.Trace.WriteLine("Error Scanning assembly : " + assembly.FullName);
                    //x.LogToTrace();
                }
            }

            //Trace.WriteLine("");
            //Trace.WriteLine("");
            //Trace.WriteLine("AUTOSCAN result");
            //AllResources.ForEach(x => Trace.WriteLine(x.VirtualPath));
        }

        //private static IEnumerable<string> GetAssemblyFiles()
        //{
        //    // When running under ASP.NET, find assemblies in the bin folder.
        //    // Outside of ASP.NET, use whatever folder We are in
        //    string directory = HostingEnvironment.IsHosted
        //        ? HttpRuntime.BinDirectory
        //        : Path.GetDirectoryName(typeof(AssemblyScanner).Assembly.Location);

        //   // System.Diagnostics.Trace.WriteLine("Scanning IN : " + directory);

        //    return Directory.GetFiles(directory, "*.dll", SearchOption.AllDirectories);
        //}

        public static void Scan(Assembly assembly, string virtualDir)
        {
            if (assembly.IsDynamic) return;
            //Debug.WriteLine("");

            if (!virtualDir.StartsWith("~/"))
            {
                if (!virtualDir.StartsWith("/")) virtualDir = "/" + virtualDir;
                virtualDir = "~" + virtualDir;
            }

            if (!virtualDir.EndsWith("/"))
            {
                virtualDir += "/";
            }

            string AssemblyName = assembly.GetName().Name;

            // System.Diagnostics.Trace.WriteLine("Scanning assembly : " + assembly.GetName().Name + " and associates to : " + virtualDir);
            // Debug.Indent();

            string AssemblyLocation = assembly.Location;

            if (!AllResources.Any(x => x.Assembly == assembly))
            {
                //Debug.WriteLine("Unknown assembly let's load");
                List<string> resourcesList = assembly.GetManifestResourceNames().ToList();

                //Debug.Indent();
                //Debug.WriteLine("Loading " + resourcesList.Count.ToString() + "resources from assembly");

                foreach (string resourceName in resourcesList)
                {
                    if (resourceName.StartsWith(AssemblyName))
                    {
                        //    Trace.Write("Found resource " + resourceName);
                        EmbeddedResource resource = null;
                        var url = virtualDir + resourceName.Remove(0, AssemblyName.Length + 1).Replace('.', '/');

                        var splitted = url.Split('/');
                        var extension = splitted.Last();


                        var virtualPath = string.Empty;
                        for (int i = 0; i < splitted.Length - 1; ++i)
                        {
                            virtualPath += "/" + splitted[i];
                        }

                        virtualPath = virtualPath.Remove(0, 1); // remove 1st slash added in loop
                        virtualPath += "." + extension;

                        virtualPath = virtualPath.ToLowerInvariant();
                        //    Trace.WriteLine(" associated with virtual path " + virtualPath);
                        resource = new EmbeddedResource(resourceName, virtualPath, assembly);

                        AllResources.Add(resource);
                    }
                }
                //Debug.Unindent();
            }
            //else //Debug.WriteLine("Assembly already scanned");

            //    Debug.Unindent();
        }
    }

    [Serializable]
    public class EmbeddedResource
    {
        public Assembly Assembly { get; set; }
        public string Name { get; set; }
        public string VirtualPath { get; set; }

        public EmbeddedResource()
        { }
        public EmbeddedResource(string name, string url, Assembly assembly)
        {
            Name = name;
            VirtualPath = url;
            Assembly = assembly;
        }

        public VirtualFile GetFile()
        {
            return new EmbeddedResourceVirtualFile(VirtualPath, Assembly, Name);
        }

        public CacheDependency GetCacheDependency()
        {
            // Trace.WriteLine("Making dependecy on " + Assembly.Location);
            // return null;
            return new System.Web.Caching.CacheDependency(Assembly.Location);
        }

        public override string ToString()
        {
            return VirtualPath;
        }

        public byte[] GetBytes()
        {
            var reader = new BinaryReader(Assembly.GetManifestResourceStream(Name));
            return reader.ReadBytes((int)reader.BaseStream.Length);
        }
    }

    [Serializable]
    public class EmbeddedResourceVirtualFile : VirtualFile
    {
        public Assembly Assembly { get; set; }
        public new string Name { get; set; }

        public EmbeddedResourceVirtualFile(string virtualPath, Assembly assembly, string name)
            : base(virtualPath)
        {
            Assembly = assembly;
            Name = name;
        }

        public override Stream Open()
        {
            if (IsText)
            {
                string data = Assembly.GetResourceContent(Name);
                //if (data.Contains("~/"))
                //{
                //    Debug.WriteLine("Replacing ~/ with " + VirtualPathUtility.ToAbsolute("~/") + " in " + Name);
                //}
                data = data.Replace("~/", VirtualPathUtility.ToAbsolute("~/"));
                return new MemoryStream(UTF8Encoding.Default.GetBytes(data));
            }
            else
                return Assembly.GetManifestResourceStream(Name);
        }

        private static List<string> _handledExtensions = new string[] { ".js", ".css" }.ToList();
        //, ".ascx", ".aspx", ".master"
        bool IsText
        {
            get
            {
                return _handledExtensions.Any(x => Name.EndsWith(x, StringComparison.InvariantCultureIgnoreCase));
            }
        }
    }
}
