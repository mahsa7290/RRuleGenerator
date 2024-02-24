using GalaSoft.MvvmLight;
using Ical.Net;
using NodaTime;
using System.Globalization;

namespace RRuleGenerator
{
    public class YearlyRepeatModel :  ViewModelBase
    {
        public IEnumerable<string> MonthNames { get; } = DateTimeFormatInfo.CurrentInfo.MonthNames.Where(x => !string.IsNullOrWhiteSpace(x));

        private int _selectedMonth;
        public int SelectedMonth
        {
            get => _selectedMonth;
            set
            {
                Set(ref _selectedMonth, value);
                DaysOfMonth = Enumerable.Range(1, DateTime.DaysInMonth(2024, value + 1)).ToList();
            }
        }

        private List<int> _daysOfMonth = Enumerable.Range(1, DateTime.DaysInMonth(2024, 1)).ToList();
        public List<int> DaysOfMonth
        {
            get => _daysOfMonth;
            set => Set(ref _daysOfMonth, value);
        }

        private int _selectedDay = 1;
        public int SelectedDay
        {
            get => _selectedDay;
            set => Set(ref _selectedDay, value);
        }
    }
}