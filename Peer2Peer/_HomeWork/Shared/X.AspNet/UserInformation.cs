using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace X.AspNet
{
    [Serializable]
    public class UserInformation
    {
        /// <summary>
        /// Type of the message
        /// </summary>
        public InformationType Type = InformationType.None;
        /// <summary>
        /// The text of the message
        /// </summary>
        public string Text;
        /// <summary>
        /// The title of the message
        /// </summary>
        public string Title;
        /// <summary>
        /// A list of items displayed in the message (for example a list
        /// of validation messages if several validators are not valid for
        /// the same action)
        /// </summary>
        public List<string> List = new List<string>();

        /// <summary>
        /// Generates an empty instance of the StatusUIMessage class
        /// </summary>
        public static UserInformation Empty
        {
            get
            {
                return new UserInformation();
            }
        }

        public static UserInformation New(string title)
        {
            return UserInformation.New(title, null);
        }

        public static UserInformation New(string title, string message)
        {
            return UserInformation.New(InformationType.Error, title, message);
        }

        public static UserInformation Error(string title)
        {
            return UserInformation.Error(title, null);
        }

        public static UserInformation Error(string title, string message)
        {
            return UserInformation.New(InformationType.Error, title, message);
        }

        public static UserInformation Success(string title)
        {
            return UserInformation.Success(title, null);
        }

        public static UserInformation Success(string title, string message)
        {
            return UserInformation.New(InformationType.Success, title, message);
        }

        public static UserInformation New(InformationType type, string title, string message)
        {
            return new UserInformation { Title = title, Text = message, Type = type };
        }
    }

    /// <summary>
    /// Defines the type of message
    /// </summary>
    public enum InformationType
    {
        /// <summary>
        /// No message
        /// </summary>
        None,
        /// <summary>
        /// Information message
        /// </summary>
        Info,
        /// <summary>
        /// Success message
        /// </summary>
        Success,
        /// <summary>
        /// Error message
        /// </summary>
        Error
    }
}
