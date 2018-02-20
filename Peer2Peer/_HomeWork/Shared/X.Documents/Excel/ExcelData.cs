using System;
using System.Collections.Generic;
using System.Data;

namespace X.Documents.Excel
{
    [Serializable]
    public class ExcelData
    {
        public string Name { get; private set; }
        public ExcelData(string documentName)
        {
            Name = documentName;
        }

        public Dictionary<string, string> Properties = new Dictionary<string, string>();
        public Dictionary<string, object> Fields = new Dictionary<string, object>();
        public DataSet DataSet = new DataSet();
    }

    //[AttributeUsage(AttributeTargets.Property)]
    //public class ExcelColumnMappingAttribute : System.Attribute
    //{
    //    public string ColumnName { get; set; }
    //    public ExcelColumnMappingAttribute(string columnName)
    //        : base()
    //    {
    //        ColumnName = columnName;
    //    }
    //}

    //[AttributeUsage(AttributeTargets.Property)]
    //public class ExcelFieldMappingAttribute : System.Attribute
    //{
    //    public string FieldName { get; set; }
    //    public ExcelFieldMappingAttribute(string fieldName)
    //        : base()
    //    {
    //        FieldName = fieldName;
    //    }
    //}

    //[AttributeUsage(AttributeTargets.Property)]
    //public class ExcelPropertyMappingAttribute : System.Attribute
    //{
    //    public string PropertyName { get; set; }
    //    public ExcelPropertyMappingAttribute(string propertyName)
    //        : base()
    //    {
    //        PropertyName = propertyName;
    //    }
    //}

    //[AttributeUsage(AttributeTargets.Class)]
    //public class ExcelTableMappingAttribute : System.Attribute
    //{
    //    public string TableName { get; set; }
    //    public ExcelTableMappingAttribute(string columnName)
    //        : base()
    //    {
    //        TableName = columnName;
    //    }
    //}

    //[AttributeUsage(AttributeTargets.Class)]
    //public class ExcelDocTypeMappingAttribute : System.Attribute
    //{
    //    public string DocTypeName { get; set; }
    //    public ExcelDocTypeMappingAttribute(string docTypeName)
    //        : base()
    //    {
    //        DocTypeName = docTypeName;
    //    }
    //}
}
