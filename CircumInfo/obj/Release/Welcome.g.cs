﻿

#pragma checksum "C:\Users\Francesco\OneDrive\Lavori in corso\CircumInfo\CircumInfo\CircumInfo\Welcome.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "4170DB4CD8029525BC49C4E7282EBD2F"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CircumInfo
{
    partial class Welcome : global::Windows.UI.Xaml.Controls.Page, global::Windows.UI.Xaml.Markup.IComponentConnector
    {
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
 
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 1:
                #line 121 "..\..\Welcome.xaml"
                ((global::Windows.UI.Xaml.Controls.ToggleSwitch)(target)).Toggled += this.Posizione_Toggled;
                 #line default
                 #line hidden
                break;
            case 2:
                #line 122 "..\..\Welcome.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.Button_Click;
                 #line default
                 #line hidden
                break;
            }
            this._contentLoaded = true;
        }
    }
}


