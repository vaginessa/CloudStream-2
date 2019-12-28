using Android.Content;
using Android.Widget;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using static CloudStreamForms.CloudStreamCore;

[assembly: ExportRenderer(typeof(TableView), typeof(CustomTableViewRenderer))]
public class CustomTableViewRenderer : TableViewRenderer
{
    private Context context;
    public CustomTableViewRenderer(Context context) : base(context)
    {
        this.context = context;
    }

    protected override void OnElementChanged(ElementChangedEventArgs<TableView> e)
    {
        base.OnElementChanged(e);
        if (Control == null)
            return;
        var listView = Control as global::Android.Widget.ListView;
        listView.DividerHeight = 3;
        
        listView.Divider.SetAlpha(0);
      //  listView.Focusable = false;

        listView.VerticalScrollBarEnabled = CloudStreamForms.Settings.HasScrollBar;

    }
}
