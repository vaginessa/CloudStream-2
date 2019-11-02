using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content.Res;
using Android.Views;
using Android.Widget;
using CloudStreamForms.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using static Java.Util.ResourceBundle;
using static CloudStreamForms.Main;

[assembly: ExportRenderer(typeof(SwitchCell), typeof(CustomSwitchCellRenderer))]
public class CustomSwitchCellRenderer : SwitchCellRenderer
{
    protected override void OnCellPropertyChanged(object sender, PropertyChangedEventArgs args)
    {
        base.OnCellPropertyChanged(sender, args);
        /*
        ((Xamarin.Forms.SwitchCell)sender)
        CloudStreamForms.Main.print(">-->" + sender);
        CloudStreamForms.Main.print(">-->" + ((LinearLayout)sender).GetChildAt(0));
        CloudStreamForms.Main.print(">-->" + ((LinearLayout)sender).GetChildAt(1));
        CloudStreamForms.Main.print(">-->" + ((LinearLayout)sender).GetChildAt(2));
        Android.Widget.Switch child0 = (Android.Widget.Switch)((LinearLayout)sender).GetChildAt(1);
        SetColorOfToggle(child0);*/

    }

    protected override Android.Views.View GetCellCore(Cell item, Android.Views.View convertView, Android.Views.ViewGroup parent, Android.Content.Context context)
    {

        var cell = base.GetCellCore(item, convertView, parent, context);
      //  print("-->>>" + cell);
        // var child1 = ((LinearLayout)cell).GetChildAt(1);
        try {
            Android.Widget.Switch child0 = (Android.Widget.Switch)((LinearLayout)cell).GetChildAt(2);
            child0.LayoutChange += (o, e) => {
               // print("AAAAAAAAAAAAAAAAAAAAA-->>");
                SetColorOfToggle(o);

            };
            child0.Click += (o, e) => {
               // CloudStreamForms.Main.print("__> DDAAAAAAAAAAAAA");
                // CloudStreamForms.Main.print("----> " + .Checked);
                SetColorOfToggle(o);
            };

        }
        catch (Exception) {

        }

        //SetColorOfToggle(child0);
        //var child2 = (Android.Widget.Switch)((LinearLayout)child0).GetChildAt(0);
        //  child0.ThumbDrawable.SetColorFilter((child0.Checked ? Android.Graphics.Color.Blue : Android.Graphics.Color.Red), PorterDuff.Mode.Multiply);
        //  var label = (TextView)((LinearLayout)child1).GetChildAt(0);
        // label.SetTextColor(new Android.Graphics.Color(ResourcesCompat.GetColor(context.Resources, Android.Resource.Color.HoloGreenDark, null)));

        return cell;
    }

    void SetColorOfToggle(object o)
    {
        try {
            var _c = ((Android.Widget.Switch)o);
            _c.ThumbDrawable.SetColorFilter((_c.Checked ? Android.Graphics.Color.ParseColor("#1976D2") : Android.Graphics.Color.White), PorterDuff.Mode.Multiply);
        }
        catch (Exception) {
        }

    }
}