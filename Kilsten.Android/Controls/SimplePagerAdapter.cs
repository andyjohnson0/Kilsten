using Android.Views;

using AndroidX.ViewPager.Widget;


namespace Kilsten.Android.Controls;

/// <summary>
/// <see cref="PagerAdapter"/> that inflates each page from a layout resource, and raises <see cref="PageCreated"/>
/// so that pages can be initialised.
/// </summary>
public class SimplePagerAdapter : PagerAdapter
{
    /// <summary>
    /// Initialises a new <see cref="SimplePagerAdapter"/>.
    /// </summary>
    /// <param name="layoutResIds">Layout resource ID for each page, in order</param>
    public SimplePagerAdapter(int[] layoutResIds)
    {
        this.layoutResIds = layoutResIds;
    }

    private readonly int[] layoutResIds;


    /// <summary>
    /// Whether the pager may reuse pages it has already created. Set to false to have pages recreated whenever the
    /// adapter is refreshed. Default is true.
    /// </summary>
    public bool UsePageCaching { get; set; } = true;


    /// <summary>
    /// Number of pages.
    /// </summary>
    public override int Count => layoutResIds.Length;


    /// <summary>
    /// Event arguments for <see cref="PageCreated"/>.
    /// </summary>
    public class PageCreatedEventArgs : EventArgs
    {
        /// <summary>
        /// Position of the page
        /// </summary>
        public int Position { get; init; }

        /// <summary>
        /// The created page
        /// </summary>
        public View? PageView { get; init; }

        /// <summary>
        /// Layout resource ID of the created page
        /// </summary>
        public int PageLayoutResId { get; init; }
    }


    /// <summary>
    /// Raised when a page is created. The page at a given position may be created more than once.
    /// </summary>
    public event EventHandler<PageCreatedEventArgs>? PageCreated;


    /// <summary>
    /// Removes all <see cref="PageCreated"/> handlers.
    /// </summary>
    public void ResetPageCreated()
    {
        this.PageCreated = null;
    }



    #region Implementation

    /// <inheritdoc/>
    public override Java.Lang.Object InstantiateItem(ViewGroup collection, int position)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(position);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(position, layoutResIds.Length);

        // Inflate the view for the current page.
        var context = collection.Context;
        View? pageView = LayoutInflater.From(context)!.Inflate(layoutResIds[position], null);

        // Fire page created event
        var ea = new PageCreatedEventArgs()
        {
            Position = position,
            PageView = pageView,
            PageLayoutResId = this.layoutResIds[position]
        };
        this.PageCreated?.Invoke(this, ea);

        // Add the view to the collection and return it.
        collection.AddView(pageView);

        return pageView!;
    }


    /// <inheritdoc/>
    public override bool IsViewFromObject(View arg0, Java.Lang.Object arg1) => arg0 == arg1;

    /// <inheritdoc/>
    public override void DestroyItem(ViewGroup container, int position, Java.Lang.Object obj)
    {
        (container as ViewPager)!.RemoveView(obj as View);
    }


    /// <inheritdoc/>
    public override int GetItemPosition(Java.Lang.Object _)
    {
        // If caching disabled then return PositionNone rather than PositionUnchanged to override the ViewPager's caching.
        return UsePageCaching ? PagerAdapter.PositionUnchanged : PagerAdapter.PositionNone;
    }

    #endregion Implementation
}
