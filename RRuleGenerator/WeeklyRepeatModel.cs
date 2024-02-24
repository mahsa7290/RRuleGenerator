using GalaSoft.MvvmLight;
using Ical.Net.DataTypes;

namespace RRuleGenerator
{
    [Flags]
    public enum WeeklyWeekDays
    {
        None = 0,
        Monday = 1,
        Tuesday = 2,
        Wednesday = 4,
        Thursday = 8,
        Friday = 16,
        Saturday = 32,
        Sunday = 64,
        Day = Monday | Tuesday | Wednesday | Thursday | Friday | Saturday | Sunday,
        Weekday = Monday | Tuesday | Wednesday | Thursday | Friday,
        WeekendDay = Sunday | Saturday
    }

    public class WeeklyRepeatModel : ViewModelBase
    {
        public IEnumerable<WeeklyWeekDays> WeekDays { get; } =
        [
            WeeklyWeekDays.Monday,
            WeeklyWeekDays.Tuesday,
            WeeklyWeekDays.Wednesday,
            WeeklyWeekDays.Thursday,
            WeeklyWeekDays.Friday,
            WeeklyWeekDays.Saturday,
            WeeklyWeekDays.Sunday,
            WeeklyWeekDays.Day,
            WeeklyWeekDays.Weekday,
            WeeklyWeekDays.WeekendDay
        ];

        private WeeklyWeekDays _selectedWeekDays;
        public WeeklyWeekDays SelectedWeekDays
        {
            get => _selectedWeekDays;
            set => Set(ref _selectedWeekDays, value);
        }

        private bool _isMonday;
        public bool IsMonday
        {
            get => _isMonday;
            set
            {
                Set(ref _isMonday, value);
                SelectedWeekDays = value ? SelectedWeekDays | WeeklyWeekDays.Monday : SelectedWeekDays & ~WeeklyWeekDays.Monday;
            }
        }

        private bool _isTuesday;
        public bool IsTuesday
        {
            get => _isTuesday;
            set
            {
                Set(ref _isTuesday, value);
                SelectedWeekDays = value ? SelectedWeekDays | WeeklyWeekDays.Tuesday : SelectedWeekDays & ~WeeklyWeekDays.Tuesday;
            }
        }

        // Repeat the pattern for the remaining days...

        private bool _isWednesday;
        public bool IsWednesday
        {
            get => _isWednesday;
            set
            {
                Set(ref _isWednesday, value);
                SelectedWeekDays = value ? SelectedWeekDays | WeeklyWeekDays.Wednesday : SelectedWeekDays & ~WeeklyWeekDays.Wednesday;
            }
        }

        private bool _isThursday;
        public bool IsThursday
        {
            get => _isThursday;
            set
            {
                Set(ref _isThursday, value);
                SelectedWeekDays = value ? SelectedWeekDays | WeeklyWeekDays.Thursday : SelectedWeekDays & ~WeeklyWeekDays.Thursday;
            }
        }

        private bool _isFriday;
        public bool IsFriday
        {
            get => _isFriday;
            set
            {
                Set(ref _isFriday, value);
                SelectedWeekDays = value ? SelectedWeekDays | WeeklyWeekDays.Friday : SelectedWeekDays & ~WeeklyWeekDays.Friday;
            }
        }

        private bool _isSaturday;
        public bool IsSaturday
        {
            get => _isSaturday;
            set
            {
                Set(ref _isSaturday, value);
                SelectedWeekDays = value ? SelectedWeekDays | WeeklyWeekDays.Saturday : SelectedWeekDays & ~WeeklyWeekDays.Saturday;
            }
        }

        private bool _isSunday;
        public bool IsSunday
        {
            get => _isSunday;
            set
            {
                Set(ref _isSunday, value);
                SelectedWeekDays = value ? SelectedWeekDays | WeeklyWeekDays.Sunday : SelectedWeekDays & ~WeeklyWeekDays.Sunday;
            }
        }
    }
}