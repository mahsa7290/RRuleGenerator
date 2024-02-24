using GalaSoft.MvvmLight;
using Ical.Net;

namespace RRuleGenerator
{
    public class MonthlyRepeatModel :  ViewModelBase
    {
        public IList<FrequencyOccurrence> FrequencyOccurrences { get; } =
            [FrequencyOccurrence.First,
            FrequencyOccurrence.Second,
            FrequencyOccurrence.Third,
            FrequencyOccurrence.Fourth,
            FrequencyOccurrence.Last];

        private FrequencyOccurrence _selectedFrequencyOccurrence;
        public FrequencyOccurrence SelectedFrequencyOccurrence
        {
            get => _selectedFrequencyOccurrence;
            set => Set(ref _selectedFrequencyOccurrence, value);
        }

        private List<int> _daysOfMonth = Enumerable.Range(1, 31).ToList();
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