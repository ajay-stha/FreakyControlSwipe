using Microsoft.Maui.Layouts;
using System.Windows.Input;

namespace FreakyControlsSample.Component;

public class FreakySwipeButton : AbsoluteLayout, IDisposable
{
    private readonly PanGestureRecognizer panGesture;
    private readonly View gestureListener;
    private const double _fadeEffect = 0.5;
    private const uint _animLength = 50;

    public event EventHandler SlideCompleted;

    public static readonly BindableProperty SlideCompleteCommandProperty = BindableProperty.Create(
           nameof(SlideCompleteCommand),
           typeof(ICommand),
           typeof(FreakySwipeButton),
           defaultValue: default(ICommand));

    public ICommand SlideCompleteCommand
    {
        get => (ICommand)GetValue(SlideCompleteCommandProperty);
        set => SetValue(SlideCompleteCommandProperty, value);
    }

    public static readonly BindableProperty ThumbProperty = BindableProperty.Create(
            nameof(Thumb),
            typeof(View),
            typeof(FreakySwipeButton),
            defaultValue: default(View));

    public View Thumb
    {
        get => (View)GetValue(ThumbProperty);
        set => SetValue(ThumbProperty, value);
    }

    public static readonly BindableProperty TrackBarProperty = BindableProperty.Create(
            nameof(TrackBar),
            typeof(View),
            typeof(FreakySwipeButton),
            defaultValue: default(View));

    public View TrackBar
    {
        get => (View)GetValue(TrackBarProperty);
        set => SetValue(TrackBarProperty, value);
    }

    public static readonly BindableProperty FillBarProperty = BindableProperty.Create(
            nameof(FillBar),
            typeof(View),
            typeof(FreakySwipeButton),
            defaultValue: default(View));

    public View FillBar
    {
        get => (View)GetValue(FillBarProperty);
        set => SetValue(FillBarProperty, value);
    }

    public static readonly BindableProperty TrackLabelProperty = BindableProperty.Create(
            nameof(TrackLabel),
            typeof(View),
            typeof(FreakySwipeButton),
            defaultValue: default(View));

    public View TrackLabel
    {
        get => (View)GetValue(TrackLabelProperty);
        set => SetValue(TrackLabelProperty, value);
    }

    public FreakySwipeButton()
    {
        panGesture = new();
        panGesture.PanUpdated += OnPanGestureUpdated;
        SizeChanged += OnSizeChanged;

        gestureListener = new ContentView
        {
            BackgroundColor = Colors.White,
            Opacity = 0.05
        };
        var label  = new Label
        {
            Text = "Slide to act",
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            FontSize = 10,
            TextColor = Colors.Black
        };
        Thumb = new Frame()
        {
            BackgroundColor = Colors.WhiteSmoke,
            Padding = 0,
            CornerRadius = 10,
            HasShadow = false,
            Content = label
        };
        gestureListener.GestureRecognizers.Add(panGesture);
    }

    private async void OnPanGestureUpdated(object sender, PanUpdatedEventArgs e)
    {
        if (Thumb == null || TrackBar == null || FillBar == null || TrackLabel == null)
            return;

        switch (e.StatusType)
        {
            case GestureStatus.Started:
                await TrackBar.FadeTo(_fadeEffect, _animLength);
                break;

            case GestureStatus.Running:
                // Translate and ensure we don't pan beyond the wrapped user interface element bounds.
                var x = Math.Max(0, e.TotalX);
                if (x > (Width - Thumb.Width))
                    x = (Width - Thumb.Width);

                //Uncomment this if you want only forward dragging.
                //if (e.TotalX < Thumb.TranslationX)
                //    return;
                Thumb.TranslationX = x;
                this.SetLayoutBounds(FillBar, new Rect(0, 0, x + (Thumb.Width / 2), this.Height));
                break;

            case GestureStatus.Completed:
                var posX = Thumb.TranslationX;
                this.SetLayoutBounds(FillBar, new Rect(0, 0, 0, this.Height));

                // Reset translation applied during the pan
                await Task.WhenAll(
                    TrackBar.FadeTo(1, _animLength),
                    Thumb.TranslateTo(0, 0, _animLength * 2, Easing.CubicIn)
                );

                if (posX >= (Width - Thumb.Width - 10/* keep some margin for error*/))
                {
                    if(SlideCompleteCommand != null)
                    {
                        if (SlideCompleteCommand.CanExecute(null))
                            SlideCompleteCommand.Execute(null);
                    }
                    SlideCompleted?.Invoke(this, EventArgs.Empty);
                }
                break;
        }
    }

    private void OnSizeChanged(object sender, EventArgs e)
    {
        if (Width == 0 || Height == 0)
            return;
        if (Thumb == null || TrackBar == null || FillBar == null || TrackLabel == null)
            return;

        Children.Clear();

        this.SetLayoutFlags(TrackBar, AbsoluteLayoutFlags.SizeProportional);
        this.SetLayoutBounds(TrackBar, new Rect(0, 0, 1, 1));
        Children.Add(TrackBar);
        
        this.SetLayoutFlags(FillBar, AbsoluteLayoutFlags.None);
        this.SetLayoutBounds(FillBar, new Rect(0, 0, 0, this.Height));
        Children.Add(FillBar);

        this.SetLayoutFlags(TrackLabel, AbsoluteLayoutFlags.SizeProportional);
        this.SetLayoutBounds(TrackLabel, new Rect(0, 0, 1, 1));
        Children.Add(TrackLabel);

        this.SetLayoutFlags(Thumb, AbsoluteLayoutFlags.None);
        this.SetLayoutBounds(Thumb, new Rect(0, 0, Width / 5, Height));
        Children.Add(Thumb);

        this.SetLayoutFlags(gestureListener, AbsoluteLayoutFlags.SizeProportional);
        this.SetLayoutBounds(gestureListener, new Rect(0, 0, 1, 1));
        Children.Add(gestureListener);

        //Dispatcher.Dispatch(() =>
        //{
        //    Thumb.Measure(double.PositiveInfinity, double.PositiveInfinity);
        //    this.SetLayoutBounds(Thumb, new Rect(0, 0, Thumb.DesiredSize.Width, Height));
        //});
        //Device.BeginInvokeOnMainThread(() =>
        //{
        //    Thumb.Measure(double.PositiveInfinity, double.PositiveInfinity);
        //    this.SetLayoutBounds(Thumb, new Rect(0, 0, Thumb.DesiredSize.Width, Height));
        //});
    }

    #region IDisposable

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~FreakySwipeButton()
    {
        Dispose(false);
    }

    protected virtual void Dispose(bool disposing)
    {
        panGesture.PanUpdated -= OnPanGestureUpdated;
        SizeChanged -= OnSizeChanged;
        gestureListener.GestureRecognizers.Clear();
    }

    #endregion IDisposable
}

public class SlideToActView : AbsoluteLayout
{
    public static readonly BindableProperty ThumbProperty =
        BindableProperty.Create(
            "Thumb", typeof(View), typeof(SlideToActView),
            defaultValue: default(View));

    public View Thumb
    {
        get { return (View)GetValue(ThumbProperty); }
        set { SetValue(ThumbProperty, value); }
    }

    public static readonly BindableProperty TrackBarProperty =
        BindableProperty.Create(
            "TrackBar", typeof(View), typeof(SlideToActView),
            defaultValue: default(View));

    public View TrackBar
    {
        get { return (View)GetValue(TrackBarProperty); }
        set { SetValue(TrackBarProperty, value); }
    }

    public static readonly BindableProperty FillBarProperty =
        BindableProperty.Create(
            "FillBar", typeof(View), typeof(SlideToActView),
            defaultValue: default(View));

    public View FillBar
    {
        get { return (View)GetValue(FillBarProperty); }
        set { SetValue(FillBarProperty, value); }
    }

    private PanGestureRecognizer _panGesture = new PanGestureRecognizer();
    private View _gestureListener;
    public SlideToActView()
    {
        _panGesture.PanUpdated += OnPanGestureUpdated;
        SizeChanged += OnSizeChanged;

        _gestureListener = new ContentView { BackgroundColor = Colors.White, Opacity = 0.05 };
        _gestureListener.GestureRecognizers.Add(_panGesture);
    }

    public event EventHandler SlideCompleted;

    private const double _fadeEffect = 0.5;
    private const uint _animLength = 50;
    async void OnPanGestureUpdated(object sender, PanUpdatedEventArgs e)
    {
        if (Thumb == null || TrackBar == null || FillBar == null)
            return;

        switch (e.StatusType)
        {
            case GestureStatus.Started:
                await TrackBar.FadeTo(_fadeEffect, _animLength);
                break;

            case GestureStatus.Running:
                // Translate and ensure we don't pan beyond the wrapped user interface element bounds.
                var x = Math.Max(0, e.TotalX);
                if (x > (Width - Thumb.Width))
                    x = (Width - Thumb.Width);

                //Uncomment this if you want only forward dragging.
                //if (e.TotalX < Thumb.TranslationX)
                //    return;
                Thumb.TranslationX = x;
                this.SetLayoutBounds(FillBar, new Rect(0, 0, x + Thumb.Width / 2, this.Height));
                break;

            case GestureStatus.Completed:
                var posX = Thumb.TranslationX;
                this.SetLayoutBounds(FillBar, new Rect(0, 0, 0, this.Height));

                // Reset translation applied during the pan
                await Task.WhenAll(new Task[]{
                    TrackBar.FadeTo(1, _animLength),
                    Thumb.TranslateTo(0, 0, _animLength * 2, Easing.CubicIn),
                });

                if (posX >= (Width - Thumb.Width - 10/* keep some margin for error*/))
                    SlideCompleted?.Invoke(this, EventArgs.Empty);
                break;
        }
    }

    void OnSizeChanged(object sender, EventArgs e)
    {
        if (Width == 0 || Height == 0)
            return;
        if (Thumb == null || TrackBar == null || FillBar == null)
            return;


        Children.Clear();

        this.SetLayoutFlags(TrackBar, AbsoluteLayoutFlags.SizeProportional);
        this.SetLayoutBounds(TrackBar, new Rect(0, 0, 1, 1));
        Children.Add(TrackBar);

        this.SetLayoutFlags(FillBar, AbsoluteLayoutFlags.None);
        this.SetLayoutBounds(FillBar, new Rect(0, 0, 0, this.Height));
        Children.Add(FillBar);

        this.SetLayoutFlags(Thumb, AbsoluteLayoutFlags.None);
        this.SetLayoutBounds(Thumb, new Rect(0, 0, this.Width / 5, this.Height));
        Children.Add(Thumb);

        this.SetLayoutFlags(_gestureListener, AbsoluteLayoutFlags.SizeProportional);
        this.SetLayoutBounds(_gestureListener, new Rect(0, 0, 1, 1));
        Children.Add(_gestureListener);

        Dispatcher.Dispatch(() =>
        {
            Thumb.Measure(double.PositiveInfinity, double.PositiveInfinity);
            this.SetLayoutBounds(Thumb, new Rect(0, 0, Thumb.DesiredSize.Width, Height));
        });

    }
}
