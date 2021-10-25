using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Holoone.Api.Models
{
    public class BaseModel : INotifyPropertyChanged, IDataErrorInfo, ICloneable
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler == null) return;
            handler(this, new PropertyChangedEventArgs(propertyName));
        }

        // Implementation of IDataErrorInfo. Required to fulfill the interface contract; otherwise not used by the class.
        public string Error
        {
            get;
        }

        protected bool _hasErrors;

        /// <summary>
        /// Returns true if one or more properties of the model contain invalid information.
        /// This property can be used with an ICommand's CanExecute to disable a button until the model is valid.
        /// </summary>
        [XmlIgnore]
        public bool HasErrors
        {
            get { return _hasErrors; }
            set
            {
                if (value != _hasErrors)
                {
                    _hasErrors = value;
                };
            }
        }

        [XmlIgnore]
        protected Dictionary<string, string> Errors { get; } = new Dictionary<string, string>();

        private Dictionary<string, PropertyInfo> _propertyInfos;

        /// <summary>
        /// Contains information for all public instance properties having at least one validation attribute.
        /// Filled on creation when it is first accessed so the CollectErrors method does not have to read the properties each time it is called.
        /// </summary>
        private Dictionary<string, PropertyInfo> PropertyInfos
        {
            get
            {
                // If the collection doesn't yet exist, create it. This will happen on the first call to CollectErrors for the first property of the model.
                if (_propertyInfos == null)
                {
                    // Use reflection to get all public instance properties with at least one validation attribute
                    _propertyInfos = this.GetType()
                       .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                       .Where(prop => prop.IsDefined(typeof(RequiredAttribute), true) || prop.IsDefined(typeof(MaxLengthAttribute), true) || prop.IsDefined(typeof(RegularExpressionAttribute), true))
                       .ToDictionary(prop => prop.Name, prop => prop);
                }
                return _propertyInfos;
            }
        }

        private Dictionary<string, string> _propertyFilterValues;

        // Implementation of IDataErrorInfo. If there are any validation errors, it returns the matching error message.
        // Called once for each property bound in xaml that has ValidatesOnDataErrors set to true.
        public string this[string propertyName]
        {
            get
            {
                CollectErrors(propertyName);
                return Errors.ContainsKey(propertyName) ? Errors[propertyName] : string.Empty;
            }
        }

        /// <summary>
        /// Finds all validation errors of the properties and adds an entry to the Errors collection for each error.
        /// Uses attributes to determine if a validation error has occured.
        public virtual void CollectErrors(string propertyName)
        {

            var isProcessed = Errors.Count(x => x.Key == propertyName) > 0;

            if (isProcessed)
                return;

            var isDirty = this.GetType().GetProperty("IsDirty")?.GetValue(this, null);
            if (isDirty != null && bool.Parse(isDirty.ToString()) == false)
                return;

            // remove any outdated errors for this property
            Errors.Remove(propertyName);
            
            PropertyInfo prop;

            if (PropertyInfos.TryGetValue(propertyName, out prop))
            {
                var currentValue = prop.GetValue(this);
                // Try to assign the attributes. if the variables are not null, an attribute of the specified type exists for this property
                var requiredAttr = prop.GetCustomAttribute<RequiredAttribute>();
                var maxLenAttr = prop.GetCustomAttribute<MaxLengthAttribute>();
                var regexAttr = prop.GetCustomAttribute<RegularExpressionAttribute>();

                // check if the property has a specific validation attribute and act accordingly
                if (requiredAttr != null)
                {
                    if (currentValue == null || string.IsNullOrEmpty(currentValue?.ToString() ?? string.Empty))
                    {
                        // retrieve the localized error message
                        string errorMessage = $"{propertyName} is required";
                        // add the error to the Errors dictionary. errorMessage will be displayed in the tooltip
                        Errors.Add(prop.Name, errorMessage);
                        RaisePropertyChanged(propertyName);
                    }
                }
                if (maxLenAttr != null)
                {
                    if ((currentValue?.ToString() ?? string.Empty).Length > maxLenAttr.Length)
                    {
                        string errorMessage = $"Exceeded max length for {propertyName}";
                        Errors.Add(prop.Name, errorMessage);
                    }
                }
                if (regexAttr != null)
                {
                    if (currentValue == null || currentValue.ToString() == "" || !Regex.IsMatch(currentValue.ToString(), regexAttr.Pattern))
                    {
                        string errorMessage = $"Invalid value for {propertyName}";
                        Errors.Add(prop.Name, errorMessage);
                    }
                }
            }

            HasErrors = Errors.Count > 0;
        }

        /// <summary>
        /// Returns true if each property of the item has the same value as the matching property of the other item.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool IsEqualTo(BaseModel other)
        {
            if (other == null)
            {
                return false;
            }
            PropertyInfo[] infos =
    GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var item in infos)
            {
                string name = item.Name;
                // Don't compare these properties as they are not part of the model data.
                if (new string[] { "Error", "Item", "HasErrors", "Assigned", "AssignTargetId", "Password", "Module", "OriginallyAssigned", "PropertyFilterValues", "ItemStateName", "UniqueProperties" }.Contains(name))
                {
                    continue;
                }
                object value = item.GetValue(this);
                object otherValue = item.GetValue(other);

                if (!Equals(value, otherValue))
                {
                    // If the property is of type ModelBase (eg. the Right property of Group), start recursive comparison.
                    if (item.PropertyType.IsSubclassOf(typeof(BaseModel)))
                    {
                        return ((BaseModel)value).IsEqualTo((BaseModel)otherValue);
                    }
                    return false;
                }
            }
            return true;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        public BaseModel ValidateObject<T>(T obj) where T : INotifyPropertyChanged
        {
            if (obj == null)
                throw new ArgumentNullException("object to validate cannot be null");

            Errors.Clear(); //clear all errors

            foreach (var item in GetProperties(obj))
            {
                CollectErrors(item.Name);
                RaisePropertyChanged();
                // Errors.Add(item.Name, string.Join(";", ValidateProperty(obj, item).ToArray())); //Set or remove error
            }
            return this;
        }

        private IEnumerable<PropertyInfo> GetProperties(object obj)
        {
            return obj.GetType().GetProperties().Where(p => p.GetCustomAttributes(typeof(ValidationAttribute), true).Length > 0).Select(p => p);
        }

        private IEnumerable<string> ValidateProperty<T>(T obj, PropertyInfo propInfo)
        {
            if (obj == null || propInfo == null)
                throw new ArgumentNullException("object to validate cannot be null");

            var results = new List<ValidationResult>();

            if (!Validator.TryValidateProperty(propInfo.GetValue(obj), new ValidationContext(obj, null, null) { MemberName = propInfo.Name }, results))
                return results.Select(s => s.ErrorMessage);
            return Enumerable.Empty<string>();
        }

    }
}