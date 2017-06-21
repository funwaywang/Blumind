using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Blumind.Controls;
using Blumind.Core;
using Blumind.Globalization;

namespace Blumind.Dialogs
{
    partial class TimerDialog : BaseDialog
    {
        public enum TimerStatus
        {
            Standby,
            Running,
            Paused,
        }

        TimerStatus _Status;
        TimeSegment _TimerValue;
        TimeSegment _NowTimerValue;

        public TimerDialog()
        {
            InitializeComponent();

            ShowIcon = true;
            ShowInTaskbar = true;
            Icon = Properties.Resources.hourglass_ico;

            Font font = new Font(SystemFonts.MenuFont.FontFamily, SystemFonts.MenuFont.Size + 12, FontStyle.Bold);
            LabHourValue.Font = LabMinuteValue.Font = LabSecondValue.Font = font;
            label6.Font = label7.Font = font;
            label6.ForeColor = label7.ForeColor = SystemColors.ControlDark;

            //
            OnStatusChanged();
            OnTimerValueChanged();
            OnNowTimerValueChanged();

            //
            AfterInitialize();
        }

        public TimerDialog(int hour, int minute, int second)
            : this()
        {
            TimerValue = new TimeSegment(hour, minute, second);
            Status = TimerStatus.Paused;
        }

        public TimerDialog(TimeSegment timerValue)
            : this()
        {
            TimerValue = new TimeSegment(timerValue.Hours, timerValue.Minutes, timerValue.Seconds);
            Status = TimerStatus.Paused;
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public TimeSegment TimerValue
        {
            get { return _TimerValue; }
            set 
            {
                if (_TimerValue != value)
                {
                    _TimerValue = value;
                    OnTimerValueChanged();
                }
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public TimeSegment NowTimerValue
        {
            get { return _NowTimerValue; }
            private set 
            {
                if (_NowTimerValue != value)
                {
                    _NowTimerValue = value;
                    OnNowTimerValueChanged();
                }
            }
        }

        [DefaultValue(TimerStatus.Standby)]
        public TimerStatus Status
        {
            get { return _Status; }
            private set 
            {
                if (_Status != value)
                {
                    _Status = value;
                    OnStatusChanged();
                }
            }
        }

        protected override bool ShowButtonArea
        {
            get
            {
                return true;
            }
        }

        protected virtual void OnStatusChanged()
        {
            NudHour.Visible
                = NudMinute.Visible
                = NudSecond.Visible
                = label4.Visible
                = label5.Visible
                = (Status == TimerStatus.Standby);
            LabHourValue.Visible
                = LabMinuteValue.Visible
                = LabSecondValue.Visible
                = label6.Visible
                = label7.Visible
                = progressBar1.Visible
                = (Status != TimerStatus.Standby);

            tableLayoutPanel1.RowStyles[0].Height = 30.0f;
            if (Status == TimerStatus.Standby)
            {
                tableLayoutPanel1.RowStyles[1].Height = 70.0f;
                tableLayoutPanel1.RowStyles[2].Height = 0.0f;
            }
            else
            {
                tableLayoutPanel1.RowStyles[1].Height = 0.0f;
                tableLayoutPanel1.RowStyles[2].Height = 70.0f;
            }

            BtnStartStop.Text = (Status == TimerStatus.Running) ? Lang._("Pause") : Lang._("Start");
            timer1.Enabled = Status == TimerStatus.Running;
            BtnReset.Enabled = Status == TimerStatus.Running || Status == TimerStatus.Paused || (TimerValue.Ticks > 0 && NowTimerValue != TimerValue);
        }

        protected virtual void OnNowTimerValueChanged()
        {
            BtnReset.Enabled = Status == TimerStatus.Running || Status == TimerStatus.Paused || (TimerValue.Ticks > 0 && NowTimerValue != TimerValue);
            LabHourValue.Text = NowTimerValue.Hours.ToString();
            LabMinuteValue.Text = NowTimerValue.Minutes.ToString();
            LabSecondValue.Text = NowTimerValue.Seconds.ToString();

            progressBar1.Value = progressBar1.Maximum - (NowTimerValue.Hours * 3600 + NowTimerValue.Minutes * 60 + NowTimerValue.Seconds);
        }

        protected virtual void OnTimerValueChanged()
        {
            BtnStartStop.Enabled = TimerValue.Ticks > 0;
            progressBar1.Maximum = TimerValue.Hours * 3600 + TimerValue.Minutes * 60 + TimerValue.Seconds;
            NudHour.Value = Math.Max(NudHour.Minimum, Math.Min(NudHour.Maximum, TimerValue.Hours));
            NudMinute.Value = TimerValue.Minutes;
            NudSecond.Value = TimerValue.Seconds;
            NowTimerValue = TimerValue;
        }

        public override void ApplyTheme(UITheme theme)
        {
            base.ApplyTheme(theme);

            this.SetFontNotScale(theme.DefaultFont);
        }

        void BtnStartStop_Click(object sender, EventArgs e)
        {
            if (Status == TimerStatus.Running)
            {
                PauseTimer();
            }
            else
            {
                StartTimer();
            }
        }

        void BtnReset_Click(object sender, EventArgs e)
        {
            if (Status == TimerStatus.Running || NowTimerValue != TimerValue)
            {
                Status = TimerStatus.Paused;
            }
            else if (Status == TimerStatus.Paused)
            {
                Status = TimerStatus.Standby;
            }

            NowTimerValue = TimerValue;
        }

        public void StartTimer()
        {
            if (TimerValue.Ticks == 0)
                return;

            if (Status == TimerStatus.Standby)
            {
                NowTimerValue = TimerValue;
            }
            Status = TimerStatus.Running;
        }

        public void PauseTimer()
        {
            Status = TimerStatus.Paused;
        }

        void OnTimeIsOver()
        {
            Status = TimerStatus.Standby;

            this.ShowMessage("Time is over!", MessageBoxIcon.Information);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);

            if (timer1 != null && timer1.Enabled)
                timer1.Enabled = false;
        }

        void timer1_Tick(object sender, EventArgs e)
        {
            if (Status == TimerStatus.Running)
            {
                int h = NowTimerValue.Hours;
                int m = NowTimerValue.Minutes;
                int s = NowTimerValue.Seconds;
                if(s > 0)
                {
                    s --;
                }
                else if(m > 0)
                {
                    m--;
                    s = 59;
                }
                else if (h > 0)
                {
                    h--;
                    m = 59;
                    s = 59;
                }
                else
                {
                    OnTimeIsOver();
                }

                if (h <= 0 && m <= 0 && s <= 0)
                    OnTimeIsOver();
                else
                    NowTimerValue = new TimeSegment(h, m, s);
            }
            else
            {
                timer1.Enabled = false;
            }
        }

        void Nud_ValueChanged(object sender, EventArgs e)
        {
            if (Status != TimerStatus.Standby)
                return;

            TimerValue = new TimeSegment((int)NudHour.Value, (int)NudMinute.Value, (int)NudSecond.Value);
        }

        void NudHour_KeyDown(object sender, KeyEventArgs e)
        {
            if (Status != TimerStatus.Standby || !(sender is NumericUpDown) || e.KeyCode != Keys.Enter)
                return;

            NumericUpDown nud = (NumericUpDown)sender;
            string text = nud.Text.Trim();
            if (text == string.Empty)
                nud.Value = 0;
            else
                nud.Value = Math.Max(nud.Minimum, Math.Min(nud.Maximum, ST.GetInt(text, (int)nud.Value)));
            e.SuppressKeyPress = true;
        }

        protected override void OnLayout(LayoutEventArgs e)
        {
            base.OnLayout(e);

            LocateButtons(new Button[] { BtnStartStop, BtnReset });
        }

        protected override void OnCurrentLanguageChanged()
        {
            base.OnCurrentLanguageChanged();

            Text = Lang._("Timer");
            LabHours.Text = Lang._("Hours");
            LabMinutes.Text = Lang._("Minutes");
            LabSeconds.Text = Lang._("Seconds");
            BtnReset.Text = Lang._("Reset");
        }
    }
}
