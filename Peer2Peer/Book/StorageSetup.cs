using System;
using System.Collections.Generic;
using System.IO;
using System.Storage;
using System.Storage.Providers;
using System.Storage.Records;
using System.Text;

namespace Book
{
    class Storage
    {
        public static DataSource Setup(Config config)
        {
            IPersistenceProvider provider = null;
            if (config._storageMode != null)
            {
                switch (config._storageMode.ToLowerInvariant())
                {
                    case "archive":
                        File.Delete(config._storageConfig);
                        provider = new ZipSystem(new FileInfo(config._storageConfig));
                        break;
                    case "disk":
                        provider = new FileSystem(new DirectoryInfo(config._storageConfig));
                        break;
                }
            }

            if (provider == null) provider = new InMemory();

            var pipe = new DataStore(provider);
            //pipe.StackMiddleware(new Cache());
            //pipe.StackMiddleware(new Synthetizer());
            //pipe.StackMiddleware(new TransactionManager());

            return new DataSource(pipe);
        }
    }
}
