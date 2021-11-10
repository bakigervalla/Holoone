﻿using Autodesk.Navisworks.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;

namespace HolooneNavis.Helpers.Converters
{
    internal class ModelItemDisplayNameConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            PropertyCategoryCollection oColl = value as PropertyCategoryCollection;

            if (oColl == null)
                return "error!";

            // try to check if there is name with the item
            DataProperty oDP = oColl.FindPropertyByDisplayName("Item", "Name");

            if (oDP != null) //use "name" as node name
                return oDP.Value.ToDisplayString();


            // if no "name", then check "type"
            oDP = oColl.FindPropertyByDisplayName("Item", "Type");
            if (oDP != null) //use "type" as node name
                return oDP.Value.ToDisplayString();

            // can be null? that must be a terrible error!
            return "Error!";
        }

        public object ConvertBack(object value, Type targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
