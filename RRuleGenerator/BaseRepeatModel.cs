using GalaSoft.MvvmLight;

namespace RRuleGenerator
{
    public class BaseRepeatModel : ViewModelBase
    {
        private int _interval;
        public int Interval
        {
            get => _interval;
            set => Set(ref _interval, value);
        }
    }
}