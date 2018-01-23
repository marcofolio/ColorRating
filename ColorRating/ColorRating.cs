using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ColorRating
{
    // Fun rating system based on "Colourful rating system with CSS3"
    // built with Xamarin.Forms using custom animations.
    // https://marcofolio.net/jquery-rating-system/

    public class App : Application
    {
        Color _emptyColor = Color.FromRgb(51, 51, 51);

        Label _rateLabelText;
        List<Frame> _rateSelections = new List<Frame>();
        Dictionary<int, Rating> _ratings = new Dictionary<int, Rating>();
        int _selectedRating = -1;
        Button _rateLabelButton;

        public App()
        {
            SetRatings();
            var rateGrid = DrawRateGrid();
            var rateLabel = DrawRateLabel();

            var layout = new Grid();
            layout.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            layout.RowDefinitions.Add(new RowDefinition { Height = PlatformSize() });
            layout.Children.Add(rateGrid, 0, 0);
            layout.Children.Add(rateLabel, 0, 1);

            var content = new ContentPage
            {
                BackgroundColor = Color.FromRgb(85, 85, 85),
                Title = "Color Rating",
                Content = layout
            };

            _rateLabelButton.Clicked += (s, e) =>
            {
                content.DisplayAlert("Thanks!", $"You rated this {_selectedRating}/5", "OK");
            };

            MainPage = new NavigationPage(content);
        }

        private void SetRatings()
        {
            _ratings.Add(1, new Rating() { Color = Color.FromHex("bd2c33"), Text = "This is just a piece of crap..." });
            _ratings.Add(2, new Rating() { Color = Color.FromHex("e49420"), Text = "Nothing too new or interesting" });
            _ratings.Add(3, new Rating() { Color = Color.FromHex("ecdb00"), Text = "Not bad, I like it" });
            _ratings.Add(4, new Rating() { Color = Color.FromHex("3bad54"), Text = "I would like to see more of this" });
            _ratings.Add(5, new Rating() { Color = Color.FromHex("1b7db9"), Text = "This is the best thing I've seen!" });
        }

        private Grid DrawRateGrid()
        {
            var grid = new Grid();
            grid.WidthRequest = PlatformRateSize();
            grid.HorizontalOptions = LayoutOptions.Center;
            grid.VerticalOptions = LayoutOptions.CenterAndExpand;

            foreach (var rating in _ratings)
            {
                var rateSelection = DrawRateSelection(grid, rating.Key);
                var rateTapGestureRecognizer = CreateRateTapGestureRecognizer();
                rateSelection.GestureRecognizers.Add(rateTapGestureRecognizer);

                _rateSelections.Add(rateSelection);
                grid.Children.Add(rateSelection, 0, 5 - rating.Key); // Count backwards for stacking from the bottom
            }

            return grid;
        }

        // Draw a single Rate Selection button
        private Frame DrawRateSelection(Grid grid, int ratingId)
        {
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(PlatformRateSize()) });
            var frame = new Frame
            {
                HeightRequest = PlatformRateSize(),
                WidthRequest = PlatformRateSize(),
                CornerRadius = PlatformRateSize() / 2,
                OutlineColor = Color.FromRgb(136, 136, 136), // Android doesn't display this ( https://github.com/xamarin/Xamarin.Forms/issues/1347 )
                BackgroundColor = _emptyColor,
                AutomationId = $"{ratingId}" // We'll be using the AutomationId to store 
            };
            return frame;
        }

        private TapGestureRecognizer CreateRateTapGestureRecognizer()
        {
            var tgr = new TapGestureRecognizer();
            tgr.Tapped += async (s, e) =>
            {
                var tappedFrame = (Frame)s;
                _selectedRating = int.Parse(tappedFrame.AutomationId); // Retrieve the ratingId

                // Act when the Rating is tapped
                _rateLabelButton.IsEnabled = true;
                _rateLabelText.Text = _ratings[_selectedRating].Text;
                await ColorFramesAsync(_ratings[_selectedRating].Color);
            };

            return tgr;
        }

        private Frame DrawRateLabel()
        {
            var rateLabelFrame = new Frame();
            var rateLabelGrid = new Grid();
            rateLabelGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            rateLabelGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = PlatformSize() });

            _rateLabelText = new Label() { Text = "Please give a rating", HorizontalOptions = LayoutOptions.StartAndExpand, VerticalOptions = LayoutOptions.Center };
            _rateLabelButton = new Button() { Text = "Submit", HorizontalOptions = LayoutOptions.End, IsEnabled = false };
            rateLabelGrid.Children.Add(_rateLabelText, 0, 0);
            rateLabelGrid.Children.Add(_rateLabelButton, 1, 0);

            rateLabelFrame.Content = rateLabelGrid;
            return rateLabelFrame;
        }

        private async Task ColorFramesAsync(Color color)
        {
            var colorAnimations = new List<Task<bool>>();

            for (int i = 0; i < _rateSelections.Count; i++)
            {
                if (i < _selectedRating)
                    colorAnimations.Add(_rateSelections[i].ColorTo(color));
                else
                    colorAnimations.Add(_rateSelections[i].ColorTo(_emptyColor));
            }

            await Task.WhenAll(colorAnimations);
        }

        private int PlatformRateSize()
        {
            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    return 80;
                case Device.Android:
                    return 60;
                default:
                    throw new PlatformNotSupportedException();
            }
        } 

        private int PlatformSize()
        {
            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    return 60;
                case Device.Android:
                    return 80;
                default:
                    throw new PlatformNotSupportedException();
            }
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
