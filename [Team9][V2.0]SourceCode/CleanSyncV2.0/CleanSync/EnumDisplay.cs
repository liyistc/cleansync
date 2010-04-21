/***********************************************************************
 * 
 * *******************CleanSync Version 2.0 EnumDisplay*****************
 * 
 * Written By : Li Yi
 * Team 0110
 * 
 * 15/04/2010
 * 
 * ************************All Rights Reserved**************************
 * 
 * *********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.ComponentModel;

namespace CleanSync
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class EnumDisplayNameAttribute : Attribute
    {
        public EnumDisplayNameAttribute(string displayName)
        {
            DisplayName = displayName;
        }
        public string DisplayName { get; set; }
    }

    /// <summary>
    /// Convert enum to its descriptions
    /// </summary>
    public class EnumTypeConverter : EnumConverter
    {
        public EnumTypeConverter(Type enumType) : base(enumType) { }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value != null)
            {
                var enumType = value.GetType();
                if (enumType.IsEnum)
                    return GetDisplayName(value);
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        private string GetDisplayName(object enumValue)
        {
            var displayNameAttribute = EnumType.GetField(enumValue.ToString()).GetCustomAttributes(typeof(EnumDisplayNameAttribute), false).FirstOrDefault() as EnumDisplayNameAttribute;
            if (displayNameAttribute != null)
                return displayNameAttribute.DisplayName;

            return Enum.GetName(EnumType, enumValue);
        }
    }

    [TypeConverter(typeof(EnumTypeConverter))]
    public enum AutoConflictOption
    {
        [EnumDisplayName("Handle Manually")]
        Off,
        [EnumDisplayName("Keep Local Copy")]
        KeepPCItems,
        [EnumDisplayName("Keep Remote Copy")]
        KeepUSBItems,
    }

    [TypeConverter(typeof(EnumTypeConverter))]
    public enum AutoSyncOption
    {
        [EnumDisplayName("OFF")]
        Off,
        [EnumDisplayName("ON")]
        On
    }
}