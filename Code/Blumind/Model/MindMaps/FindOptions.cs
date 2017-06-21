using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Blumind.Configuration;

namespace Blumind.Model.MindMaps
{
    class FindOptions
    {
        bool _CaseSensitive;
        bool _WholeWordOnly;
        bool _RegularExpression;
        bool _WithHiddenItems;
        //private FindDirection _Direction = FindDirection.Forward;
        public static readonly FindOptions Default = new FindOptions();

        public event EventHandler CaseSensitiveChanged;
        public event EventHandler WholeWordOnlyChanged;
        public event EventHandler RegularExpressionChanged;
        public event EventHandler WithHiddenItemsChanged;
        //public event EventHandler DirectionChanged;

        private FindOptions()
        {
            CaseSensitive = Options.Current.GetBool("Find_CaseSensitive", false);
            WholeWordOnly = Options.Current.GetBool("Find_WholeWordOnly", false);
            RegularExpression = Options.Current.GetBool("Find_RegularExpression", false);
            WithHiddenItems = Options.Current.GetBool("Find_WithHiddenItems", true);
            //Direction = (FindDirection)Options.Default.Customizations.GetInteger("Find_Direction");
        }

        [DefaultValue(false)]
        public bool CaseSensitive
        {
            get { return _CaseSensitive; }
            set
            {
                if (_CaseSensitive != value)
                {
                    _CaseSensitive = value;
                    OnCaseSensitiveChanged();
                }
            }
        }

        [DefaultValue(false)]
        public bool WholeWordOnly
        {
            get { return _WholeWordOnly; }
            set 
            {
                if (_WholeWordOnly != value)
                {
                    _WholeWordOnly = value;
                    OnWholeWordOnlyChanged();
                }
            }
        }

        [DefaultValue(false)]
        public bool RegularExpression
        {
            get { return _RegularExpression; }
            set 
            {
                if (_RegularExpression != value)
                {
                    _RegularExpression = value;
                    OnRegularExpressionChanged();
                }
            }
        }

        [DefaultValue(false)]
        public bool WithHiddenItems
        {
            get { return _WithHiddenItems; }
            set
            {
                if (_WithHiddenItems != value)
                {
                    _WithHiddenItems = value;
                    OnWithHiddenItemsChanged();
                }
            }
        }

        //[DefaultValue(FindDirection.Forward)]
        //public FindDirection Direction
        //{
        //    get { return _Direction; }
        //    set 
        //    {
        //        if (_Direction != value)
        //        {
        //            _Direction = value;
        //            OnDirectionChanged();
        //        }
        //    }
        //}

        void OnCaseSensitiveChanged()
        {
            Options.Current.SetValue("Find_CaseSensitive", CaseSensitive);

            if (CaseSensitiveChanged != null)
            {
                CaseSensitiveChanged(this, EventArgs.Empty);
            }
        }

        void OnWholeWordOnlyChanged()
        {
            Options.Current.SetValue("Find_WholeWordOnly", WholeWordOnly);

            if (WholeWordOnlyChanged != null)
            {
                WholeWordOnlyChanged(this, EventArgs.Empty);
            }
        }

        void OnRegularExpressionChanged()
        {
            Options.Current.SetValue("Find_RegularExpression", RegularExpression);

            if (RegularExpressionChanged != null)
            {
                RegularExpressionChanged(this, EventArgs.Empty);
            }
        }

        void OnWithHiddenItemsChanged()
        {
            Options.Current.SetValue("Find_WithHiddenItems", WithHiddenItems);

            if (WithHiddenItemsChanged != null)
            {
                WithHiddenItemsChanged(this, EventArgs.Empty);
            }
        }

        //private void OnDirectionChanged()
        //{
        //    Options.Default.Customizations.Set("Find_Direction", ((int)Direction).ToString());

        //    if (DirectionChanged != null)
        //    {
        //        DirectionChanged(this, EventArgs.Empty);
        //    }
        //}
    }
}
