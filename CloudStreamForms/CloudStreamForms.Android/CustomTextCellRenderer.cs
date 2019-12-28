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
using static CloudStreamForms.CloudStreamCore;

[assembly: ExportRenderer(typeof(TextCell), typeof(CustomTextCellRenderer))]
public class CustomTextCellRenderer : TextCellRenderer
{
    protected override void OnCellPropertyChanged(object sender, PropertyChangedEventArgs args)
    {
        base.OnCellPropertyChanged(sender, args);
    }

    protected override Android.Views.View GetCellCore(Cell item, Android.Views.View convertView, Android.Views.ViewGroup parent, Android.Content.Context context)
    {
        var cell = base.GetCellCore(item, convertView, parent, context);
        var backColor = new Android.Graphics.Color(20, 20, 20);

        var layout = (LinearLayout)((LinearLayout)cell).GetChildAt(1);
        TextView t = (TextView)layout.GetChildAt(0);
        TextView t2 = (TextView)layout.GetChildAt(1);

        List<string> attributes = new List<string>() { "FONTSIZE", "BOLD", "BLACK", "POSX","POSY" };
        bool applyCanges = false;
        string resTxt = t2.Text;
        List<bool> values = new List<bool>();
        List<float> ftts = new List<float>();

        for (int i = 0; i < attributes.Count; i++) {
            values.Add(false);
            ftts.Add(1);
            if (resTxt.Contains(attributes[i] + ":")) {
                string _val = FindHTML(resTxt, attributes[i] + ":", ":");
                float val = 0;
                if (_val != "") {
                    val = float.Parse(_val);
                }
                resTxt = resTxt.Replace(attributes[i] + ":" + _val + ":", "");
                ftts[i] = val;
                values[i] = true;
            }
        }
        t.Typeface = values[1] ? Typeface.DefaultBold : Typeface.Default;
        t.TextSize = values[0] ? ftts[0] : 14;
    
        t.TranslationX = values[3] ? ftts[3] : 0;
        t.TranslationY = values[4] ? ftts[4] : 0;
        backColor = values[2] ? new Android.Graphics.Color(17, 17, 17) : new Android.Graphics.Color(20, 20, 20);
        // t.texts = values[0] ? ftts[0] : 1;

        cell.SetBackgroundColor(backColor);


        /*
        for (int i = 0; i < layout.ChildCount; i++) {
            print("CHILDCOUNT:" + layout.GetChildAt(i).ToString() + "::" + i);
        }*/
        // var f =.ChildCount.GetChildAt(2);
        return cell;
    }


}