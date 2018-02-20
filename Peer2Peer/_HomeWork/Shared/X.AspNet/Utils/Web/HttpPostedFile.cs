//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.IO;
//using Core.Features.DocumentsManagement;

//namespace System.Web
//{
//    public static class HttpPostedFileExtension
//    {
//        public static Document ToDocument(this HttpPostedFile item)
//        {
//            var fileName = item.FileName.Split("\\").LastOrDefault();
//            fileName = fileName ?? item.FileName;
//            var result = Document.Create(fileName, item.InputStream.ToByteArray());
//            return result.Save();
//        }
//    }
//}
