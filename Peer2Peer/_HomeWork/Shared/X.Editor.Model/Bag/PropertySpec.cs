using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using X.Configuration.Model.Bag;

namespace X.Configuration.Model.Bag
{
    /// <summary>
    /// Represents a single property in a PropertyBag
    /// </summary>
    public class PropertySpec
    {
        /// <summary>
        /// Gets or sets a collection of additional Attributes for this property.  This can
        /// be used to specify attributes beyond those supported intrinsically by the
        /// PropertySpec class, such as ReadOnly and Browsable.
        /// </summary>
        public Attribute[] Attributes { get; set; }

        /// <summary>
        /// Gets or sets the category name of this property.
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// Gets or sets the type converter type for this property.
        /// </summary>
        public Type ConverterType { get; set; }

        /// <summary>
        /// Allowed values if an enum.
        /// </summary>
        public string[] EnumValues
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the default value of this property.
        /// </summary>
        public object DefaultValue { get; set; }

        /// <summary>
        /// Gets or sets the help text description of this property.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the fully qualified name of the editor type for
        /// this property.
        /// </summary>
        public string EditorTypeName { get; set; }

        /// <summary>
        /// Gets or sets the name of this property.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the type of this property.
        /// </summary>
        public Type DataType { get; set; }

        #region ctors

        /// <summary>
        /// Initializes a new instance of the PropertySpec class.
        /// </summary>
        /// <param name="name">The name of the property displayed in the property grid.</param>
        /// <param name="type">A Type that represents the type of the property.</param>
        public PropertySpec(string name, Type type) : this(name, type, null, null, null) { }

        /// <summary>
        /// Initializes a new instance of the PropertySpec class.
        /// </summary>
        /// <param name="name">The name of the property displayed in the property grid.</param>
        /// <param name="type">A Type that represents the type of the property.</param>
        /// <param name="category">The category under which the property is displayed in the
        /// property grid.</param>
        public PropertySpec(string name, Type type, string category) : this(name, type, category, null, null) { }

        /// <summary>
        /// Initializes a new instance of the PropertySpec class.
        /// </summary>
        /// <param name="name">The name of the property displayed in the property grid.</param>
        /// <param name="type">A Type that represents the type of the property.</param>
        /// <param name="category">The category under which the property is displayed in the
        /// property grid.</param>
        /// <param name="description">A string that is displayed in the help area of the
        /// property grid.</param>
        public PropertySpec(string name, Type type, string category, string description)
            :
            this(name, type, category, description, null) { }

        /// <summary>
        /// Initializes a new instance of the PropertySpec class.
        /// </summary>
        /// <param name="name">The name of the property displayed in the property grid.</param>
        /// <param name="category">The category under which the property is displayed in the
        /// property grid.</param>
        /// <param name="description">A string that is displayed in the help area of the
        /// property grid.</param>
        /// <param name="enumValues">Allowed enum values.</param>
        /// <param name="defaultValue">The default value of the property, or null if there is
        /// no default value.</param>
        public PropertySpec(string name, string category, string description, string[] enumValues, string defaultValue)
            : this(name, typeof(string), category, description, defaultValue, (string)null, typeof(PropertyEnumStringConverter))
        {
            this.EnumValues = enumValues;
        }

        /// <summary>
        /// Initializes a new instance of the PropertySpec class.
        /// </summary>
        /// <param name="name">The name of the property displayed in the property grid.</param>
        /// <param name="type">A Type that represents the type of the property.</param>
        /// <param name="category">The category under which the property is displayed in the
        /// property grid.</param>
        /// <param name="description">A string that is displayed in the help area of the
        /// property grid.</param>
        /// <param name="defaultValue">The default value of the property, or null if there is
        /// no default value.</param>
        public PropertySpec(string name, Type type, string category, string description, object defaultValue)
        {
            this.Name = name;
            this.DataType = type;
            this.Category = category;
            this.Description = description;
            this.DefaultValue = defaultValue;
            this.Attributes = null;
        }

        /// <summary>
        /// Initializes a new instance of the PropertySpec class.
        /// </summary>
        /// <param name="name">The name of the property displayed in the property grid.</param>
        /// <param name="type">A Type that represents the type of the property.</param>
        /// <param name="category">The category under which the property is displayed in the
        /// property grid.</param>
        /// <param name="description">A string that is displayed in the help area of the
        /// property grid.</param>
        /// <param name="defaultValue">The default value of the property, or null if there is
        /// no default value.</param>
        /// <param name="editor">The fully qualified name of the type of the editor for this
        /// property.  This type must derive from UITypeEditor.</param>
        /// <param name="typeConverter">The Type that represents the type of the type
        /// converter for this property.  This type must derive from TypeConverter.</param>
        public PropertySpec(string name, Type type, string category, string description, object defaultValue, string editorTypeName, Type typeConverter)
            : this(name, type, category, description, defaultValue)
        {
            this.EditorTypeName = editorTypeName;
            this.ConverterType = typeConverter;
        }

        /// <summary>
        /// Initializes a new instance of the PropertySpec class.
        /// </summary>
        /// <param name="name">The name of the property displayed in the property grid.</param>
        /// <param name="type">A Type that represents the type of the property.</param>
        /// <param name="category">The category under which the property is displayed in the
        /// property grid.</param>
        /// <param name="description">A string that is displayed in the help area of the
        /// property grid.</param>
        /// <param name="defaultValue">The default value of the property, or null if there is
        /// no default value.</param>
        /// <param name="editor">The Type that represents the type of the editor for this
        /// property.  This type must derive from UITypeEditor.</param>
        /// <param name="typeConverter">The Type that represents the type of the type
        /// converter for this property.  This type must derive from TypeConverter.</param>
        public PropertySpec(string name, Type type, string category, string description, object defaultValue,
            Type editor, Type typeConverter)
            :
            this(name, type, category, description, defaultValue, editor.AssemblyQualifiedName, typeConverter) { }

        #endregion
    }



}
