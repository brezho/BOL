
namespace System.Web.UI
{
    public static class AttributeCollectionExtensions
    {

        /// <summary>
        /// Add or update the specified attribute.
        /// </summary>
        /// <param Name="collection">The collection.</param>
        /// <param Name="key">The key.</param>
        /// <param Name="value">The value.</param>
        public static AttributeCollection SetAttribute(this AttributeCollection collection, string key, string value)
        {
            key.CanNotBeNullOrEmpty();
            if (collection[key] != null)
                collection[key] = value;
            else
                collection.Add(key, value);

            return collection;
        }
    }
}
