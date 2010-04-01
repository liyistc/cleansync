using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Globalization;

namespace CleanSync
{
    [TypeConverter(typeof(EnumTypeConverter))]
    public enum AutoConflictOption
    {
        [EnumDisplayName("Handle Manually")]
        Off,
        [EnumDisplayName("Keep Local Copy")]
        KeepPCItems,
        [EnumDisplayName("Keep Remote Copy")]
        KeepUSBItems,
        //[EnumDisplayName("Keep Both Copies")]
        //KeepBoth,
        //[EnumDisplayName("Ignore Both Copies")]
        //IgnoreBoth
    }

    [TypeConverter(typeof(EnumTypeConverter))]
    public enum AutoSyncOption
    {
        [EnumDisplayName("OFF")]
        Off,
        [EnumDisplayName("ON")]
        On
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class EnumDisplayNameAttribute : Attribute
    {
        public EnumDisplayNameAttribute(string displayName)
        {
            DisplayName = displayName;
        }

        public string DisplayName { get; set; }
    }

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


    [Serializable]
    public class JobConfig
    {
        

        public JobConfig()
        {
            this.ConflictConfig = AutoConflictOption.Off;
            this.SyncConfig = AutoSyncOption.Off;
        }

        public JobConfig(AutoConflictOption conflictOpt, AutoSyncOption syncOpt)
        {
            ConflictConfig = conflictOpt;
            SyncConfig = syncOpt;
        }

        public AutoConflictOption ConflictConfig
        {
            get;
            set;
        }

        public AutoSyncOption SyncConfig
        {
            get;
            set;
        }
    }
}
