
namespace System.Web.UI.WebControls
{
    public static class ParameterCollectionExtensions
    {

        /// <summary>
        /// Add or update the specified parameter.
        /// </summary>
        /// <param Name="collection">The collection.</param>
        /// <param Name="parameter">The parameter.</param>
        public static void SetParameter(this ParameterCollection collection, Parameter parameter)
        {
            collection.CanNotBeNull();
            parameter.CanNotBeNull();

            if (collection[parameter.Name] != null)
            {
                collection[parameter.Name] = parameter;
            }
            else
            {
                collection.Add(parameter);
            }
        }

        /// <summary>
        /// Ar or update a parameter with the specified Name
        /// </summary>
        /// <param Name="collection">The collection.</param>
        /// <param Name="Name">The Name.</param>
        /// <param Name="type">The type.</param>
        /// <param Name="value">The value.</param>
        public static void SetParameter(this ParameterCollection collection, string name, System.Data.DbType type, string value)
        {
            var parameter = new Parameter(name, type, value);
            SetParameter(collection, parameter);
        }
    }
}
