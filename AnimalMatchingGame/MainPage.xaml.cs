using System.Diagnostics;
using Microsoft.Maui.Media;
using Microsoft.Maui.Storage;

namespace AnimalMatchingGame
{
    public partial class MainPage : ContentPage
    {
        private const int PairCount = 8;
        private const string BestTimePreferenceKey = "BestCompletionTimeTicks";
        private static readonly TimeSpan RoundDuration = TimeSpan.FromSeconds(60);
        private static readonly TimeSpan PreviewDuration = TimeSpan.FromSeconds(3);
        private static readonly Dictionary<string, string> AnimalCalls = new()
        {
            ["🐶"] = "Woof woof!",
            ["🐱"] = "Meow!",
            ["🐭"] = "Squeak!",
            ["🐹"] = "Squeak squeak!",
            ["🐰"] = "Squeak!",
            ["🦊"] = "Yip!",
            ["🐻"] = "Grr!",
            ["🐼"] = "Grr!",
            ["🐸"] = "Ribbit!",
            ["🐮"] = "Moo!",
            ["🐷"] = "Oink!",
            ["🐵"] = "Ooh ooh ah ah!",
            ["🐔"] = "Cluck cluck!",
            ["🐧"] = "Honk!",
            ["🦁"] = "Roar!",
            ["🐑"] = "Baa!",
            ["🐴"] = "Neigh!",
            ["🦆"] = "Quack!",
            ["🦉"] = "Hoot!",
            ["🐘"] = "Trumpet!"
        };

        private readonly Stopwatch _stopwatch = new();
        private readonly List<string> _previousRoundAnimals = [];
        private Button? _firstClicked;
        private bool _isResolvingPair;
        private bool _isGameActive;
        private int _matchesFound;
        private long _bestTimeTicks;

        public MainPage()
        {
            InitializeComponent();
            _bestTimeTicks = Preferences.Default.Get(BestTimePreferenceKey, 0L);
            UpdateBestTimeLabel();
            TimeRemainingLabel.Text = $"Time remaining: {FormatTime(RoundDuration)}";
        }

        private async void PlayAgainButton_Clicked(object sender, EventArgs e)
        {
            var availableAnimals = AnimalCalls.Keys.ToList();
            Shuffle(availableAnimals);
            var roundAnimals = availableAnimals.Take(PairCount).ToList();

            if (_previousRoundAnimals.Count > 0 &&
                roundAnimals.OrderBy(animal => animal, StringComparer.Ordinal)
                    .SequenceEqual(_previousRoundAnimals.OrderBy(animal => animal, StringComparer.Ordinal)))
            {
                roundAnimals[0] = availableAnimals[PairCount];
            }

            Shuffle(roundAnimals);
            _previousRoundAnimals.Clear();
            _previousRoundAnimals.AddRange(roundAnimals);

            var cards = roundAnimals.SelectMany(animal => new[] { animal, animal }).ToList();
            Shuffle(cards);

            var buttons = AnimalButtons.Children.OfType<Button>().ToList();
            for (var index = 0; index < buttons.Count; index++)
            {
                buttons[index].CommandParameter = cards[index];
                buttons[index].Text = cards[index];
                buttons[index].BackgroundColor = Colors.LightBlue;
                buttons[index].IsEnabled = false;
            }

            _matchesFound = 0;
            _firstClicked = null;
            _isResolvingPair = false;
            _isGameActive = false;

            GameStatusLabel.Text = "Memorize the animals!";
            PlayAgainButton.Text = "Play again?";
            PlayAgainButton.IsVisible = false;
            AnimalButtons.IsVisible = true;
            UpdateRemainingTimeLabel();

            await Task.Delay(PreviewDuration);
            if (!IsLoaded)
            {
                return;
            }

            foreach (var button in buttons)
            {
                button.Text = string.Empty;
                button.IsEnabled = true;
            }

            GameStatusLabel.Text = string.Empty;
            _isGameActive = true;
            _stopwatch.Restart();
            Dispatcher.StartTimer(TimeSpan.FromMilliseconds(100), TimerTick);
        }

        private bool TimerTick()
        {
            if (!IsLoaded || !_isGameActive)
            {
                return false;
            }

            var remaining = RoundDuration - _stopwatch.Elapsed;
            if (remaining <= TimeSpan.Zero)
            {
                FinishGame(timedOut: true);
                return false;
            }

            TimeRemainingLabel.Text = $"Time remaining: {FormatTime(remaining)}";
            return true;
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            if (!_isGameActive || _isResolvingPair || sender is not Button clickedButton ||
                !clickedButton.IsEnabled)
            {
                return;
            }

            if (clickedButton.CommandParameter is not string animal)
            {
                return;
            }

            if (clickedButton == _firstClicked)
            {
                return;
            }

            clickedButton.Text = animal;
            clickedButton.BackgroundColor = Colors.Red;

            if (_firstClicked is null)
            {
                _firstClicked = clickedButton;
                _isResolvingPair = true;
                await SpeakAnimalCallAsync(animal);
                if (_isGameActive)
                {
                    _isResolvingPair = false;
                }
                return;
            }

            var firstButton = _firstClicked;
            if (firstButton is null)
            {
                return;
            }

            _isResolvingPair = true;
            var isMatch = firstButton.CommandParameter is string firstAnimal && firstAnimal == animal;

            if (isMatch)
            {
                _matchesFound++;
                firstButton.BackgroundColor = Colors.LightGreen;
                clickedButton.BackgroundColor = Colors.LightGreen;
                firstButton.IsEnabled = false;
                clickedButton.IsEnabled = false;
                _firstClicked = null;

                if (_matchesFound == PairCount)
                {
                    FinishGame(timedOut: false);
                }
            }

            await SpeakAnimalCallAsync(animal);

            if (!_isGameActive)
            {
                return;
            }

            if (isMatch)
            {
                _isResolvingPair = false;
                return;
            }

            await Task.Delay(TimeSpan.FromMilliseconds(700));
            if (!_isGameActive)
            {
                return;
            }

            firstButton.Text = string.Empty;
            clickedButton.Text = string.Empty;
            firstButton.BackgroundColor = Colors.LightBlue;
            clickedButton.BackgroundColor = Colors.LightBlue;
            _firstClicked = null;
            _isResolvingPair = false;
        }

        private async Task SpeakAnimalCallAsync(string animal)
        {
            if (AnimalCalls.TryGetValue(animal, out var call))
            {
                await TextToSpeech.Default.SpeakAsync(call);
            }
        }

        private void FinishGame(bool timedOut)
        {
            if (!_isGameActive)
            {
                return;
            }

            _isGameActive = false;
            _stopwatch.Stop();
            _isResolvingPair = false;
            _firstClicked = null;
            UpdateRemainingTimeLabel();

            if (timedOut)
            {
                TimeRemainingLabel.Text = "Time remaining: 00:00";
                GameStatusLabel.Text = "Time's up! Try again.";
            }
            else
            {
                if (_bestTimeTicks == 0 || _stopwatch.Elapsed.Ticks < _bestTimeTicks)
                {
                    _bestTimeTicks = _stopwatch.Elapsed.Ticks;
                    Preferences.Default.Set(BestTimePreferenceKey, _bestTimeTicks);
                    GameStatusLabel.Text = "You matched every animal! New best time!";
                    UpdateBestTimeLabel();
                }
                else
                {
                    GameStatusLabel.Text = "You matched every animal! Try to beat your best time.";
                }
            }

            AnimalButtons.IsVisible = false;
            PlayAgainButton.Text = "Play again?";
            PlayAgainButton.IsVisible = true;
        }

        private void UpdateRemainingTimeLabel()
        {
            var remaining = RoundDuration - _stopwatch.Elapsed;
            TimeRemainingLabel.Text = $"Time remaining: {FormatTime(remaining > TimeSpan.Zero ? remaining : TimeSpan.Zero)}";
        }

        private void UpdateBestTimeLabel()
        {
            BestTimeLabel.Text = _bestTimeTicks > 0
                ? $"Best time: {FormatPreciseTime(TimeSpan.FromTicks(_bestTimeTicks))}"
                : "Best time: --";
        }

        private static string FormatTime(TimeSpan time)
        {
            var wholeSeconds = (int)Math.Ceiling(time.TotalSeconds);
            return $"{wholeSeconds / 60:00}:{wholeSeconds % 60:00}";
        }

        private static string FormatPreciseTime(TimeSpan time)
        {
            return $"{(int)time.TotalMinutes:00}:{time.Seconds:00}.{time.Milliseconds / 100}";
        }

        private static void Shuffle<T>(IList<T> items)
        {
            for (var index = items.Count - 1; index > 0; index--)
            {
                var swapIndex = Random.Shared.Next(index + 1);
                (items[index], items[swapIndex]) = (items[swapIndex], items[index]);
            }
        }
    }
}
