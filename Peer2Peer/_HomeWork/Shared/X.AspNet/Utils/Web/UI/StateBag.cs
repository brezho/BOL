
namespace System.Web.UI
{
    public static class StateBagExtensions
    {
        /// <summary>
        /// Gets a property saved in the ViewState.
        /// </summary>
        /// <typeparam Name="T">Type of the property.</typeparam>
        /// <param Name="viewState">The ViewState instance.</param>
        /// <param Name="propertyName">Name of the property.</param>
        /// <param Name="defaultValue">The default value if the property value is null in
        /// the ViewState.</param>
        /// <returns>The value of the property.</returns>
        public static T Get<T>(this StateBag viewState, string propertyName, T defaultValue)
        {
            if (viewState[propertyName] == null) return defaultValue;
            //{
            //    viewState[propertyName] = defaultValue;
            //}
            return (T)viewState[propertyName];
        }

        public static T GetOrSet<T>(this StateBag viewState, string propertyName, Func<T> assignDefault)
        {
            if (viewState[propertyName] == null)
            {
                viewState[propertyName] = assignDefault();
            }
            return (T)viewState[propertyName];
        }

        /// <summary>
        /// Gets a property saved in the ViewState.
        /// </summary>
        /// <typeparam Name="T">Type of the property.</typeparam>
        /// <param Name="viewState">The ViewState instance.</param>
        /// <param Name="propertyName">Name of the property.</param>
        /// <returns>The value of the property.</returns>
        public static T Get<T>(this StateBag viewState, string propertyName)
        {
            return viewState.Get<T>(propertyName, default(T));
        }

        /// <summary>
        /// Saves a property value in the ViewState.
        /// </summary>
        /// <param Name="viewState">The ViewState instance.</param>
        /// <param Name="propertyName">Name of the property</param>
        /// <param Name="value">Value of the property</param>
        public static void Set(this StateBag viewState, string propertyName, object value)
        {
            viewState[propertyName] = value;
        }
    }
}
