using Microsoft.Maui.Controls;
using MauiColor = Microsoft.Maui.Graphics.Color;

#if ANDROID
using Android.Widget;
using AndroidColor = Android.Graphics.Color;
#elif IOS || MACCATALYST
using UIKit;
using Microsoft.Maui;
#endif

namespace TracNghiemLaiXe.Behaviors
{
    /// <summary>
    /// Behavior to tint an image with a specified color.
    /// Works with SVG and PNG icons that have transparency.
    /// </summary>
    public class TintColorBehavior : Behavior<Image>
    {
        public static readonly BindableProperty TintColorProperty =
            BindableProperty.Create(
                nameof(TintColor),
                typeof(MauiColor),
                typeof(TintColorBehavior),
                Colors.Transparent,
                propertyChanged: OnTintColorChanged);

        public MauiColor TintColor
        {
            get => (MauiColor)GetValue(TintColorProperty);
            set => SetValue(TintColorProperty, value);
        }

        private static void OnTintColorChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is TintColorBehavior behavior && behavior._image != null)
            {
                behavior.ApplyTintColor();
            }
        }

        private Image? _image;

        protected override void OnAttachedTo(Image bindable)
        {
            base.OnAttachedTo(bindable);
            _image = bindable;
            ApplyTintColor();
        }

        protected override void OnDetachingFrom(Image bindable)
        {
            base.OnDetachingFrom(bindable);
            _image = null;
        }

        private void ApplyTintColor()
        {
            if (_image == null) return;

#if ANDROID
            // Android implementation using ColorFilter
            _image.HandlerChanged += (s, e) =>
            {
                if (_image.Handler?.PlatformView is ImageView imageView)
                {
                    var color = TintColor.ToInt();
                    imageView.SetColorFilter(new AndroidColor(color));
                }
            };
#elif IOS || MACCATALYST
            // iOS implementation using TintColor
            _image.HandlerChanged += (s, e) =>
            {
                if (_image.Handler?.PlatformView is UIImageView imageView)
                {
                    // Manual color conversion since ColorExtensions is internal
                    var r = (nfloat)TintColor.Red;
                    var g = (nfloat)TintColor.Green;
                    var b = (nfloat)TintColor.Blue;
                    var a = (nfloat)TintColor.Alpha;
                    imageView.TintColor = UIColor.FromRGBA(r, g, b, a);
                }
            };
#elif WINDOWS
            // Windows implementation - Apply tint via opacity mask or similar
            // Note: UWP/WinUI has different tinting approach
#endif
        }
    }
}
