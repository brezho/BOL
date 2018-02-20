using System;
using System.Collections.Generic;
using System.Helpers;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace X.Packaging
{
    public static class Relations
    {
        public const string EmbeddedResource = "X.Packaging.Relations.EmbeddedResource";
        public const string Contains = "X.Packaging.Relations.Contains";
    }

    public sealed class PackageDescriptor
    {
        public string Path { get; set; }
    }

    public static class Helper
    {
        public static void IncludeDirectory(this PackageDescriptor path, DirectoryInfo directory, bool includeSubDirectories = true, string uriRoot = null)
        {
            uriRoot = uriRoot ?? "/";
            uriRoot = uriRoot.PrependInCase("/").AppendInCase("/");
            uriRoot = uriRoot + directory.Name + "/";

            Package package = Package.Open(path.Path, FileMode.Open);
            foreach (var file in directory.GetFiles())
            {
                var uri = new Uri(uriRoot + file.Name, UriKind.RelativeOrAbsolute);

                var p = package.CreatePart(uri, MimeTypeHelper.GetMimeType(file.Name));
                using (var outstream = p.GetStream(FileMode.Create))
                using (var instream = new FileStream(file.FullName, FileMode.Open))
                {
                    instream.CopyTo(outstream);
                }
                package.CreateRelationship(uri, TargetMode.Internal, Relations.EmbeddedResource);

            }
            package.Close();

            if (includeSubDirectories)
            {
                foreach (var subDirectory in directory.GetDirectories())
                {
                    IncludeDirectory(path, subDirectory, true, uriRoot.Append(subDirectory.Name));
                }
            }
        }

        public static PackageDescriptor GeneratePackage(string filePath, string Title)
        {
            Package package = Package.Open(filePath, FileMode.Create);
            package.PackageProperties.Creator = Environment.UserDomainName;
            package.PackageProperties.Created = DateTime.Now;
            package.PackageProperties.Title = Title;
            package.Close();
            return new PackageDescriptor { Path = filePath };
        }
    }
}
