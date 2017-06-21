using System;
using Blumind.Globalization;

namespace Blumind.Core
{
    struct TimeSegment
    {
        int _Hours;
        int _Minutes;
        int _Seconds;

        public TimeSegment(int hours, int minutes, int seconds)
        {
            _Hours = 0;
            _Minutes = 0;
            _Seconds = 0;

            Hours = hours;
            Minutes = minutes;
            Seconds = seconds;
        }

        public int Hours
        {
            get { return _Hours; }
            set
            {
                value = Math.Max(0, value);
                _Hours = value; 
            }
        }

        public int Minutes
        {
            get { return _Minutes; }
            set 
            {
                value = Math.Max(0, Math.Min(59, value));
                _Minutes = value; 
            }
        }

        public int Seconds
        {
            get { return _Seconds; }
            set
            {
                value = Math.Max(0, Math.Min(59, value));
                _Seconds = value; 
            }
        }

        public int Ticks
        {
            get { return Hours * 3600 + Minutes * 60 + Seconds; }
        }

        public override string ToString()
        {
            return string.Format("{0}:{1}:{2}", Hours, Minutes, Seconds);
        }

        public override int GetHashCode()
        {
            return Ticks;
        }

        public override bool Equals(object obj)
        {
            if (obj is TimeSegment)
                return this == (TimeSegment)obj;

            return base.Equals(obj);
        }

        public static bool operator !=(TimeSegment ts1, TimeSegment ts2)
        {
            return ts1.Ticks != ts2.Ticks;
        }

        public static bool operator ==(TimeSegment ts1, TimeSegment ts2)
        {
            return ts1.Ticks == ts2.Ticks;
        }
    }

    class TimeSegmentInfo
    {
        public TimeSegmentInfo()
        {
        }

        public TimeSegmentInfo(string name, TimeSegment timeSegment, string description)
        {
            Name = name;
            TimeSegment = timeSegment;
            Description = description;
        }

        public string Name { get; set; }

        public string Description { get; set; }

        public TimeSegment TimeSegment { get; set; }

        public static TimeSegmentInfo[] DefaultTimeSegments
        {
            get
            {
                return new TimeSegmentInfo[]{
                    new TimeSegmentInfo(Lang.Format("{0} Minutes", 5), new TimeSegment(0,5,0), null),
                    new TimeSegmentInfo(Lang.Format("{0} Minutes", 10), new TimeSegment(0,10,0), null),
                    new TimeSegmentInfo(Lang.Format("{0} Minutes", 15), new TimeSegment(0,15,0), null),
                    new TimeSegmentInfo(Lang.Format("{0} Minutes", 20), new TimeSegment(0,20,0), null),
                    new TimeSegmentInfo(Lang.Format("{0} Minutes", 30), new TimeSegment(0,30,0), null),
                    new TimeSegmentInfo(Lang.Format("{0} Minutes", 60), new TimeSegment(1,0,0), null),
                };
            }
        }
    }
}
