using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Ical.Net;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using Ical.Net.Serialization;
using Ical.Net.Serialization.DataTypes;
using NodaTime.Text;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using WPFLocalizeExtension.Engine;

namespace RRuleGenerator
{
    public enum LanguageEnum
    {
        English,
        German,
        French,
        Italian
    }

    public class MainViewModel : ObservableObject
    {
        public IEnumerable<LanguageEnum> Languages { get; } = Enum.GetValues<LanguageEnum>();

        private LanguageEnum _selectedLanguage = LanguageEnum.English;
        public LanguageEnum SelectedLanguage
        {
            get => _selectedLanguage;
            set
            {
                _selectedLanguage = value;
                var cultureName = _selectedLanguage switch
                {
                    LanguageEnum.German => "de-DE",
                    LanguageEnum.French => "fr-FR",
                    LanguageEnum.Italian => "it-IT",
                    _ => "en-US"
                };
                var culture = new CultureInfo(cultureName);
                culture.DateTimeFormat.ShortDatePattern = "yyyy-MM-dd";
                Thread.CurrentThread.CurrentCulture = culture;
                Thread.CurrentThread.CurrentUICulture = culture;
                LocalizeDictionary.Instance.Culture = culture;
            }
        }

        public MainViewModel()
        {
            var culture = new CultureInfo("en-US");
            culture.DateTimeFormat.ShortDatePattern = "yyyy-MM-dd";
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
            LocalizeDictionary.Instance.Culture = culture;
            this.PropertyChanged += (sender, args) => GetRRule(args.PropertyName);
            this.WeeklyRepeatModel.PropertyChanged += (sender, args) => GetRRule(args.PropertyName);
            this.MonthlyRepeatModel.PropertyChanged += (sender, args) => GetRRule(args.PropertyName);
            this.YearlyRepeatModel.PropertyChanged += (sender, args) => GetRRule(args.PropertyName);
        }


        public ICommand CopyCommand => new RelayCommand(CopyCommandExecuted);

        private void CopyCommandExecuted()
        {
            CopyContent = "Copied";
        }

        private HashSet<string> _filter = new HashSet<string>()
        {
            nameof(StartDate),
            nameof(SelectedFrequency),
            nameof(SelectedEndType),
            nameof(AfterExecutionNumber),
            nameof(SelectedType),
            nameof(MonthlyRepeatModel.SelectedDay),
            nameof(MonthlyRepeatModel.SelectedFrequencyOccurrence),
            nameof(YearlyRepeatModel.SelectedDay),
            nameof(YearlyRepeatModel.SelectedMonth),
            nameof(Interval),
            nameof(Until),
            nameof(WeeklyRepeatModel.SelectedWeekDays)
        };

        private void GetRRule(string propertyName)
        {
            if (!_filter.Contains(propertyName))
            {
                return;
            }


            if (_isIsDeserlization) { return; }
            CopyContent = "Copy";
            var rrule = new RecurrencePattern(SelectedFrequency, SelectedFrequency == FrequencyType.Yearly ? 1 : Interval);
            if (SelectedEndType == EndTypeEnum.After)
            {
                rrule.Count = AfterExecutionNumber;
            }
            else if (SelectedEndType == EndTypeEnum.OnDate)
            {
                rrule.Until = Until;
            }

            if (SelectedFrequency == FrequencyType.Weekly)
            {
                SetWeekDays(rrule);
            }
            else if (SelectedFrequency == FrequencyType.Monthly)
            {
                if (SelectedType)
                {
                    rrule.ByMonthDay.Add(MonthlyRepeatModel.SelectedDay);
                }
                else
                {
                    rrule.BySetPosition.Add((int)MonthlyRepeatModel.SelectedFrequencyOccurrence);
                    SetWeekDays(rrule);
                }
            }
            else if (SelectedFrequency == FrequencyType.Yearly)
            {
                if (SelectedType)
                {
                    rrule.ByMonth.Add(YearlyRepeatModel.SelectedMonth + 1);
                    rrule.ByMonthDay.Add(YearlyRepeatModel.SelectedDay);
                }
                else
                {
                    rrule.BySetPosition.Add((int)MonthlyRepeatModel.SelectedFrequencyOccurrence);
                    rrule.ByMonth.Add(YearlyRepeatModel.SelectedMonth + 1);
                    SetWeekDays(rrule);
                }
            }

            var serializer = new RecurrencePatternSerializer();
            _rRule = $"DTSTART:{StartDate:yyyyMMddTHHmmss}\r\nRRULE:" + serializer.SerializeToString(rrule);
            RaisePropertyChanged(()=> RRule);
        }

        private void SetWeekDays(RecurrencePattern rrule)
        {
            List<WeekDay> weekDays = new List<WeekDay>();
            foreach (var item in WeeklyRepeatModel.WeekDays.Take(7))
            {
                if (WeeklyRepeatModel.SelectedWeekDays.HasFlag(item))
                {
                    weekDays.Add(new WeekDay() { DayOfWeek = (DayOfWeek)Enum.Parse(typeof(DayOfWeek), item.ToString()) });
                }
            }

            if (weekDays.Count != 7)
            {
                rrule.ByDay = weekDays;
            }
        }

        private void SetSelectedWeekDays(RecurrencePattern rrule)
        {
            if (!rrule.ByDay.Any())
            {
                WeeklyRepeatModel.SelectedWeekDays = WeeklyWeekDays.Day;
                return;
            }

            WeeklyRepeatModel.IsSunday = false;
            WeeklyRepeatModel.IsMonday = false;
            WeeklyRepeatModel.IsTuesday = false;
            WeeklyRepeatModel.IsWednesday = false;
            WeeklyRepeatModel.IsThursday = false;
            WeeklyRepeatModel.IsFriday = false;
            WeeklyRepeatModel.IsSaturday = false;

            foreach (var item in rrule.ByDay)
            {
                switch (item.DayOfWeek)
                {
                    case DayOfWeek.Sunday:
                        WeeklyRepeatModel.IsSunday = true; break;
                    case DayOfWeek.Monday:
                        WeeklyRepeatModel.IsMonday = true; break;
                    case DayOfWeek.Tuesday:
                        WeeklyRepeatModel.IsTuesday = true; break;
                    case DayOfWeek.Wednesday:
                        WeeklyRepeatModel.IsWednesday = true; break;
                    case DayOfWeek.Thursday:
                        WeeklyRepeatModel.IsThursday = true; break;
                    case DayOfWeek.Friday:
                        WeeklyRepeatModel.IsFriday = true; break;
                    case DayOfWeek.Saturday:
                        WeeklyRepeatModel.IsSaturday = true; break;
                    default:
                        break;
                }
            }
        }

        public RecurrencePattern Pattern { get; } = new RecurrencePattern();

        public IList<FrequencyType> Frequencies { get; } = Enum.GetValues(typeof(FrequencyType)).Cast<FrequencyType>().Where(x => (int)x >= 3).ToList();

        private FrequencyType _selectedFrequency = FrequencyType.Hourly;
        public FrequencyType SelectedFrequency
        {
            get => _selectedFrequency;
            set => Set(ref _selectedFrequency, value);
        }

        private bool _selectedType;
        public bool SelectedType
        {
            get => _selectedType;
            set => Set(ref _selectedType, value);
        }

        public YearlyRepeatModel YearlyRepeatModel { get; set; } = new();

        public MonthlyRepeatModel MonthlyRepeatModel { get; set; } = new();

        public WeeklyRepeatModel WeeklyRepeatModel { get; set; } = new();

        private int _interval = 1;
        public int Interval
        {
            get => _interval;
            set => Set(ref _interval, value);
        }

        private DateTime _startDate = DateTime.Now;
        public DateTime StartDate
        {
            get => _startDate;
            set => Set(ref _startDate, value);
        }

        public IList<EndTypeEnum> EndTypes { get; } = Enum.GetValues(typeof(EndTypeEnum)).Cast<EndTypeEnum>().ToList();

        private EndTypeEnum _selectedEndType;
        public EndTypeEnum SelectedEndType
        {
            get => _selectedEndType;
            set => Set(ref _selectedEndType, value);
        }

        private DateTime _until = DateTime.Now;
        public DateTime Until
        {
            get => _until;
            set => Set(ref _until, value);
        }

        private int _afterExecutionNumber;
        public int AfterExecutionNumber
        {
            get => _afterExecutionNumber;
            set => Set(ref _afterExecutionNumber, value);
        }

        private string _rRule;
        public string RRule
        {
            get => _rRule;
            set
            {
                Set(ref _rRule, value);
                DeserilizeRRule();
            }
        }


        bool _isIsDeserlization = false;
        private void DeserilizeRRule()
        {
            _isIsDeserlization = true;
            try
            {
                var serializer = new RecurrencePatternSerializer();

                var rruleArr = RRule.Split("\r\n");

                DateTime.TryParseExact(rruleArr[0].Replace("DTSTART:", ""), "yyyyMMddTHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var startDate);
                StartDate = startDate;
                using (var reader = new StringReader(rruleArr[1].Replace("RRULE:", "")))
                {
                    var rrule = (RecurrencePattern)serializer.Deserialize(reader);
                    SetModel(rrule);
                }
            }
            catch
            {
                throw new ArgumentException("Invalid RRule");
            }
            finally
            {
                _isIsDeserlization = false;
            }
        }


        private void SetModel(RecurrencePattern rrule)
        {
            SelectedFrequency = rrule.Frequency;

            if (rrule.Count > 0)
            {
                SelectedEndType = EndTypeEnum.After;
                AfterExecutionNumber = rrule.Count;
            }
            else if (rrule.Until != DateTime.MinValue)
            {
                SelectedEndType = EndTypeEnum.OnDate;
                Until = rrule.Until;
            }
            else
            {
                SelectedEndType = EndTypeEnum.Never;
            }

            Interval = rrule.Interval;

            if (SelectedFrequency == FrequencyType.Weekly)
            {
                // Extract and set the weekdays
                SetSelectedWeekDays(rrule);
            }
            else if (SelectedFrequency == FrequencyType.Monthly)
            {
                // Set monthly repeat model properties based on the selection
                if (rrule.BySetPosition.Count == 0)
                {
                    SelectedType = true;
                    MonthlyRepeatModel.SelectedDay = rrule.ByMonthDay[0];
                }
                else
                {
                    SelectedType = false;
                    MonthlyRepeatModel.SelectedFrequencyOccurrence = (FrequencyOccurrence)rrule.BySetPosition[0];
                    SetSelectedWeekDays(rrule);
                }
            }
            else if (SelectedFrequency == FrequencyType.Yearly)
            {
                if (rrule.BySetPosition.Count == 0)
                {
                    SelectedType = true;
                    YearlyRepeatModel.SelectedMonth = rrule.ByMonth[0] - 1;
                    YearlyRepeatModel.SelectedDay = rrule.ByMonthDay[0];
                }
                else
                {
                    SelectedType = false;
                    MonthlyRepeatModel.SelectedFrequencyOccurrence = (FrequencyOccurrence)rrule.BySetPosition[0];
                    YearlyRepeatModel.SelectedMonth = rrule.ByMonth[0] - 1;
                    SetSelectedWeekDays(rrule);
                }
            }
        }

        private string _copyContent = "Copy";
        public string CopyContent
        {
            get => _copyContent;
            set => Set(ref _copyContent, value);
        }
    }
}
