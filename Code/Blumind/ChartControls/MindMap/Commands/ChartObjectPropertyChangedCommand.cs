using System.Reflection;
using Blumind.Core;
using Blumind.Model;

namespace Blumind.Controls.MapViews
{
    class ChartObjectPropertyChangedCommand : Command
    {
        Chart Chart;
        object ChartObject;
        string PropertyName;
        ChangeTypes Changes;
        object OldValue;
        object NewValue;
        bool HasRollback;

        public ChartObjectPropertyChangedCommand(Chart chart, 
            object chartObject, string propertyName, 
            object oldValue, object newValue,
            ChangeTypes changes)
        {
            Chart = chart;
            ChartObject = chartObject;
            PropertyName = propertyName;
            OldValue = oldValue;
            NewValue = newValue;
            Changes = changes;

            HasRollback = false;
        }

        public override string Name
        {
            get { return "ChartObjectPropertyChanged"; }
        }

        public override bool Execute()
        {
            if (ChartObject == null || string.IsNullOrEmpty(PropertyName))
                return false;

            // This method isn't need execute At first time
            if (HasRollback)
            {
                PropertyInfo pif = ChartObject.GetType().GetProperty(PropertyName);
                if (pif == null)
                    return false;

                if (ChartObject is Blumind.Core.INotifyPropertyChanged)
                {
                    ((Blumind.Core.INotifyPropertyChanged)ChartObject).PropertyChangeSuspending = true;
                }

                pif.SetValue(ChartObject, NewValue, null);

                if (Chart != null && !Chart.IsUpdating)
                {
                    if ((Changes & ChangeTypes.Layout) == ChangeTypes.Layout)
                        Chart.UpdateView(Changes);
                    else if ((Changes & ChangeTypes.Visual) == ChangeTypes.Visual)
                        Chart.InvalidateChart();
                }

                if (ChartObject is Blumind.Core.INotifyPropertyChanged)
                {
                    ((Blumind.Core.INotifyPropertyChanged)ChartObject).PropertyChangeSuspending = false;
                }
            }

            return true;
        }

        public override bool Rollback()
        {
            if (ChartObject == null || string.IsNullOrEmpty(PropertyName))
                return false;

            PropertyInfo pif= ChartObject.GetType().GetProperty(PropertyName);
            if (pif == null)
                return false;

            if (ChartObject is Blumind.Core.INotifyPropertyChanged)
            {
                ((Blumind.Core.INotifyPropertyChanged)ChartObject).PropertyChangeSuspending = true;
            }

            pif.SetValue(ChartObject, OldValue, null);
            HasRollback = true;

            if (Chart != null && !Chart.IsUpdating)
            {
                if ((Changes & ChangeTypes.Layout) == ChangeTypes.Layout)
                    Chart.UpdateView(Changes);
                else if ((Changes & ChangeTypes.Visual) == ChangeTypes.Visual)
                    Chart.InvalidateChart();
            }

            if (ChartObject is Blumind.Core.INotifyPropertyChanged)
            {
                ((Blumind.Core.INotifyPropertyChanged)ChartObject).PropertyChangeSuspending = false;
            }

            return true;
        }
    }
}
