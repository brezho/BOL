using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace X.Repository.Analysis
{
    public class SelectCommand<T>
    {
        public List<string> Projection { get; set; }
        public Expression<Func<T, bool>> Where { get; set; }
    }

    public class InsertCommand<T>
    {
        public InsertCommand() { Values = new Dictionary<string, object>(); }
        public Dictionary<string, object> Values { get; set; }
    }

    public class SaveCommand<T>
    {
        public SaveCommand()
        {
            ColumnValues = new Dictionary<string, object>();
            KeyValues = new Dictionary<string, object>();
        }
        public Dictionary<string, object> ColumnValues { get; set; }
        public Dictionary<string, object> KeyValues { get; set; }
    }

    public class InsertManyCommand<T>
    {
        public InsertManyCommand() { InsertComands = new List<InsertCommand<T>>(); }
        public List<InsertCommand<T>> InsertComands { get; set; }
    }

    public class UpdateCommand<T>
    {
        public UpdateCommand() { Set = new Dictionary<string, object>(); }
        public Dictionary<string, object> Set { get; set; }
        public Expression<Func<T, bool>> Where { get; set; }
    }

    public class DeleteAllCommand<T>
    {
    }

    public class DeleteCommand<T>
    {
        public Expression<Func<T, bool>> Where { get; set; }
    }
}
