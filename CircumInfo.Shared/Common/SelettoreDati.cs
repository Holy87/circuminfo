using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Microsoft.Advertising.Mobile.UI;

namespace CircumInfo.Common
{
    public class SelettoreDati : DataTemplateSelector
    {
        public DataTemplate NormalTemplate { get; set; }
        public DataTemplate AdTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (item is Common.Partenza)
                return NormalTemplate;
            if (item is AdControl)
                return AdTemplate;
            return SelectTemplateCore(item, container);
        }
    }
}
