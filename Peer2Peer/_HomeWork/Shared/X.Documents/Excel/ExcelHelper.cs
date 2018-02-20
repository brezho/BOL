using DocumentFormat.OpenXml.CustomProperties;
using DocumentFormat.OpenXml.Extensions;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.VariantTypes;
using X.Documents;
using ExtremeML.Packaging;
using ExtremeML.Spreadsheet.Address;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.IO;
using System.Linq;

namespace X.Documents.Excel
{
    public static class ExcelHelper
    {
        //public static List<T> ToList<T>(DataTable dt) where T : new()
        //{
        //    List<T> result = new List<T>();

        //    List<Tuple<string, string>> cols = new List<Tuple<string, string>>();
        //    foreach (DataColumn dc in dt.Columns) cols.Add(System.Tuple.Create(dc.ColumnName, dc.ColumnName.Trim()
        //         .Replace(" ", "")
        //         .Replace("&", "")
        //         .Replace("%", "")
        //         .Replace("/", "")
        //         ));

        //    foreach(var x in dt)
        //    {
        //        var item = new ExpandoObject();
        //        var d = item as IDictionary<string, object>; //work with the Expando as a Dictionary
        //        cols.ForEach(col => d.Add(col.Item2, x[col.Item1] != null ? x[col.Item1].ToString().Trim() : string.Empty));
        //        result.Add(FastObjectAccessors.DynamicTo<T>(item));
        //    }
        //    return result;
        //}

        public static ExcelData Read(string documentPath)
        {
            ExcelData result = null;
            var fi = new FileInfo(documentPath);
            if (fi.Exists)
            {
                result = new ExcelData(fi.Name);
                using (MemoryStream stream = SpreadsheetReader.Copy(documentPath))
                using (SpreadsheetDocument doc = SpreadsheetDocument.Open(stream, true))
                {
                    result.Fields.AddRange(doc.GetFields());
                    result.DataSet = doc.GetTables();
                    result.Properties.AddRange(doc.GetProperties());
                }
            }
            return result;
        }

        public static ExcelData Read(string documentName, byte[] data)
        {
            ExcelData result = null;
            result = new ExcelData(documentName);
            using (MemoryStream stream = new MemoryStream(data))
            using (SpreadsheetDocument doc = SpreadsheetDocument.Open(stream, true))
            {
                result.Fields.AddRange(doc.GetFields());
                result.DataSet = doc.GetTables();
                result.Properties.AddRange(doc.GetProperties());
            }
            return result;
        }


        private class ReferenceDetails
        {
            public string SheetName;
            public string CellReference;
        }

        private static ReferenceDetails GetReferenceDetails(string reference)
        {
            var splitted = reference.Split("!".ToCharArray());
            return new ReferenceDetails { SheetName = splitted[0], CellReference = splitted[1] };
        }

        private static Dictionary<string, object> GetFields(this SpreadsheetDocument spreadsheet)
        {
            var result = new Dictionary<string, object>();
            SharedStringTablePart shareStringPart = spreadsheet.WorkbookPart.SharedStringTablePart;

            var definedNames = spreadsheet.WorkbookPart.Workbook.Descendants<DefinedNames>();
            if (definedNames.Count() > 0)
            {
                var names = definedNames.SelectMany(u => u.Elements<DefinedName>());
                if (names.IsNotNull())
                {

                    var fields = names.Select(u => new { Name = u.Name.Value, Reference = GetReferenceDetails(u.InnerText) });
                    var items = fields.Select(u => new { Name = u.Name, Cell = WorksheetReader.GetCell(SpreadsheetReader.ColumnFromReference(u.Reference.CellReference), SpreadsheetReader.RowFromReference(u.Reference.CellReference), SpreadsheetReader.GetWorksheetPartByName(spreadsheet, u.Reference.SheetName)) });

                    items.ForEach(u => result.Add(u.Name, GetValue(u.Cell, shareStringPart)));
                }
            }
            return result;
        }

        private static object GetValue(DocumentFormat.OpenXml.Spreadsheet.Cell cell, SharedStringTablePart stringTablePart)
        {

            if (cell == null || cell.ChildElements.Count == 0 || cell.CellValue == null) return null;

            //cell.StyleIndex
            //get cell value
            string value = cell.CellValue.InnerText;

            //Look up real value from shared string table 
            if (cell.DataType != null)
            {
                if (cell.DataType == CellValues.SharedString) return stringTablePart.SharedStringTable.ChildElements[Int32.Parse(value)].InnerText;
                if (cell.DataType == CellValues.Date) return DateTime.FromOADate((double)value.ToDecimal());
                if (cell.DataType == CellValues.Boolean) return value.ToBoolean();
            }

            return value;
        }

        private static DataSet GetTables(this SpreadsheetDocument spreadsheet)
        {
            DataSet result = new System.Data.DataSet();
            spreadsheet.Tables().ForEach(t =>
            {
                DataTable dt = new DataTable(t.TableName);
                foreach (var col in t.TableColumns()) dt.Columns.Add(new DataColumn(col.Name));

                int i = 0;
                int toBypass = t.HeaderRowCount.HasValue ? t.HeaderRowCount.Value : 0;
                foreach (var row in t.TableRows())
                {
                    if (i >= toBypass)
                    {
                        DataRow dr = dt.NewRow();
                        foreach (var col in t.TableColumns()) dr[col.Name] = row[col.Name].Value;
                        dt.Rows.Add(dr);
                    }
                    i++;
                }
                result.Tables.Add(dt);
            });

            result.AcceptChanges();
            return result;
        }

        private static Dictionary<string, string> GetProperties(this SpreadsheetDocument spreadsheet)
        {
            var result = new Dictionary<string, string>();
            if (spreadsheet.CustomFilePropertiesPart.IsNotNull())
            {
                var properties = spreadsheet.CustomFilePropertiesPart.Properties.OfType<CustomDocumentProperty>();
                if (properties.IsNotNull())
                {
                    properties.ForEach(u => result.Add(u.Name.Value, u.VTLPWSTR.InnerText));
                }
            }
            return result;
        }

        public static void Transform(string sourcePath, string targetPath, ExcelData documentData, ITranslationProvider translationProvider)
        {
            File.Copy(sourcePath, targetPath, true);

            using (var package = SpreadsheetDocumentWrapper.Open(targetPath))
            {
                var othernames = BuildDefinedNamesTable((WorkbookPart)package.WorkbookPart.GetWrappedContentObject());

                // SET FIELDS
                IEnumerable<string> keys = documentData.Fields.Where(u => u.Value.IsNotNull()).Select(u => u.Key);
                foreach (var key in keys.ToList())
                {
                    if (translationProvider.IsNotNull())
                    {
                        var stringVal = documentData.Fields[key] as string;
                        if (stringVal.IsFilled()) documentData.Fields[key] = translationProvider.TranslateStream(stringVal);
                    }

                    var namedDefinition = othernames.Where(u => u.Key == key).FirstOrDefault();
                    if (namedDefinition.IsNotNull())
                    {
                        var sheet = package.WorkbookPart.WorksheetParts.Where(u => u.Name == namedDefinition.SheetName).FirstOrDefault();
                        sheet.Worksheet.SetCellValue(new GridReference(namedDefinition.StartRowIndex, namedDefinition.StartColumnIndex), documentData.Fields[key]);
                    }
                }

                //// SET TABLES  
                //if (translationProvider.IsNotNull())
                //{
                //    documentData.DataSet = translationProvider.GetTranslation(documentData.DataSet);
                //}

                foreach (DataTable dt in documentData.DataSet.Tables)
                {
                    var table = package.WorkbookPart.GetTablePart(dt.TableName);

                    if (table.IsNotNull()) table.Table.Fill(dt);
                }

                // SET PROPERTIES
                foreach (var key in documentData.Properties.Where(u => u.Value.IsNotNull()).Select(u => u.Key).ToList())
                {
                    var spreadsheet = (SpreadsheetDocument)package.WorkbookPart.Spreadsheet.GetWrappedContentObject();
                    var property = spreadsheet.CustomFilePropertiesPart.Properties.OfType<CustomDocumentProperty>().Where(u => u.Name == key).FirstOrDefault();
                    if (property.IsNotNull()) property.VTLPWSTR = new VTLPWSTR(documentData.Properties[key]);
                    else
                    {
                        // create custom prop if not existing
                        CustomDocumentProperty newProp = new CustomDocumentProperty();
                        newProp.FormatId = "{D5CDD505-2E9C-101B-9397-08002B2CF9AE}";
                        newProp.PropertyId = spreadsheet.CustomFilePropertiesPart.Properties.Count() + 2;
                        newProp.Name = key;
                        newProp.VTLPWSTR = new VTLPWSTR(documentData.Properties[key]);
                        spreadsheet.CustomFilePropertiesPart.Properties.Append(newProp);

                    }
                }
            }

            // SET TRANSLATION
            if (translationProvider.IsNotNull())
            {
                using (var package = SpreadsheetDocumentWrapper.Open(targetPath))
                {
                    SharedStringTablePart shareStringPart = ((WorkbookPart)package.WorkbookPart.GetWrappedContentObject()).SharedStringTablePart;
                    if (shareStringPart != null)
                    {
                        package.WorkbookPart.WorksheetParts.ForEach(sheetPart =>
                        {
                            if (sheetPart.IsNotNull() && sheetPart.Worksheet.IsNotNull() && sheetPart.Worksheet.SheetData.IsNotNull() && sheetPart.Worksheet.SheetData.Rows.IsNotNull() && sheetPart.Worksheet.SheetData.Rows.Count > 0)
                            {
                                sheetPart.Worksheet.SheetData.Rows.Where(r => r.IsNotNull() && r.Cells.IsNotNull() && r.Cells.Count > 0).ForEach(row =>
                                {
                                    row.Cells.ForEach(cell =>
                                    {
                                        var wrappedCell = (Cell)cell.GetWrappedContentObject();
                                        if (wrappedCell != null)
                                        {
                                            var value = GetValue(wrappedCell, shareStringPart);
                                            if (value != null && value is string)
                                            {
                                                var stringVal = value as string;
                                                var translatedValue = translationProvider.TranslateStream(stringVal);
                                                if (translatedValue != stringVal) cell.SetValue(translatedValue);
                                            }
                                        }
                                    });
                                });
                            }
                        });
                    }
                }
            }

        }


        public static void Transform(string sourcePath, string targetPath, ExcelData documentData)
        {
            Transform(sourcePath, targetPath, documentData, null);
        }



        class DefinedNameVal
        {
            public DefinedNameVal(string name, string location)
            {
                //Parse defined name string...
                string key = name;
                string reference = location;

                string sheetName = reference.Split('!')[0];
                sheetName = sheetName.Trim('\'');

                //Assumption: None of my defined names are relative defined names (i.e. A1)
                string range = reference.Split('!')[1];
                string[] rangeArray = range.Split('$');

                string startCol = rangeArray[1];
                string startRow = rangeArray[2].TrimEnd(':');

                string endCol = null;
                string endRow = null;

                if (rangeArray.Length > 3)
                {
                    endCol = rangeArray[3];
                    endRow = rangeArray[4];
                }

                Key = key;
                SheetName = sheetName;
                StartColumn = startCol;
                StartRow = startRow;
                EndColumn = endCol;
                EndRow = endRow;
            }

            internal string Key;
            internal string SheetName;
            internal string StartColumn;
            internal string EndColumn;
            internal string StartRow;
            internal string EndRow;

            internal bool isRange
            {
                get
                {
                    return (EndColumn != null) || (EndRow != null);
                }
            }

            internal int ColumnsCount
            {
                get
                {
                    return DistanceCalculator.Eval(StartColumn, EndColumn) + 1;
                }
            }

            internal int StartColumnIndex
            {
                get
                {
                    return DistanceCalculator.ColumnNumber(StartColumn) - 1;
                }
            }

            internal int EndColumnIndex
            {
                get
                {
                    return DistanceCalculator.ColumnNumber(EndColumn) - 1;
                }
            }
            internal int StartRowIndex
            {
                get
                {
                    return StartRow.ToInt() - 1;
                }
            }

            internal int EndRowIndex
            {
                get
                {
                    return EndRow.ToInt() - 1;
                }
            }
        }

        static class DistanceCalculator
        {
            internal static int Eval(string col1, string col2)
            {
                return Math.Abs(ColumnNumber(col1) - ColumnNumber(col2));
            }

            internal static int ColumnNumber(string col1)
            {
                int result = 0;
                var chars = col1.Trim().ToCharArray().Reverse();
                int loopCounter = 0;
                foreach (var c in chars)
                {
                    var charIndexValue = ((int)c) - 64; //65 is ASCII index of char 'A'
                    result += charIndexValue * ((int)Math.Pow(26, loopCounter));
                    loopCounter++;
                }
                return result;
            }
        }

        static List<DefinedNameVal> BuildDefinedNamesTable(WorkbookPart workbookPart)
        {
            //Build a list
            List<DefinedNameVal> definedNames = new List<DefinedNameVal>();

            if (workbookPart.Workbook.DefinedNames != null)
            {
                foreach (DefinedName name in workbookPart.Workbook.GetFirstChild<DefinedNames>().Where(x => !x.InnerText.Contains("#REF!")))
                {
                    definedNames.Add(new DefinedNameVal(name.Name, name.InnerText));
                }
            }



            return definedNames;
        }
    }
}
