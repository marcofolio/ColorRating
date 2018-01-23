using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ColorRating
{
    public static class FrameExtensions
    {
        private static string ColorToHandle = "ColorTo";

        public static void CancelColorAnimations(Frame view)
        {
            if (view == null)
                throw new ArgumentNullException(nameof(view));

            view.AbortAnimation(ColorToHandle);
        }

        public static Task<bool> ColorTo(this Frame frame, Color toColor, uint length = 250, Easing easing = null)
        {
            if (frame == null)
                throw new ArgumentNullException(nameof(frame));
            if (easing == null)
                easing = Easing.Linear;

            var tcs = new TaskCompletionSource<bool>();
            var startColor = frame.BackgroundColor;

            new Animation(AnimateColorCallback(frame, startColor, toColor), 0, 1, easing)
                .Commit(frame, ColorToHandle, 16, length, finished: (f, a) => tcs.SetResult(a));

            return tcs.Task;
        }

        private static Action<double> AnimateColorCallback(Frame frame, Color startColor, Color toColor)
        {
            Func<double, Color> computeColor = progress =>
            {
                var r = startColor.R + (toColor.R - startColor.R) * progress;
                var g = startColor.G + (toColor.G - startColor.G) * progress;
                var b = startColor.B + (toColor.B - startColor.B) * progress;
                var a = startColor.A + (toColor.A - startColor.A) * progress;

                return Color.FromRgba(r, g, b, a);
            };

            var weakView = new WeakReference<Frame>(frame);
            Frame frameRef;
            if (!weakView.TryGetTarget(out frameRef))
            {
                throw new ArgumentException("Can't target Frame");
            }

            Action<double> setColor = f =>
            {
                frameRef.BackgroundColor = computeColor(f);
            };
            return setColor;
        }
    }
}
