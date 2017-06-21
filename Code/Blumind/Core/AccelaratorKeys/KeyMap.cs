using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace Blumind.Core
{
    class KeyMap
    {
        public static readonly KeyMap Default = new KeyMap();

        public event EventHandler KeyManChanged;

        private KeyMap()
        {
        }

        private void InvokeKeyManChanged()
        {
            if (KeyManChanged != null)
                KeyManChanged(this, EventArgs.Empty);
        }

        public static bool ApplyChanges(Hashtable Changes)
        {
            if (Changes == null)
                return false;

            bool hasChanged = false;
            foreach (ShortcutKeyGroup group in ShortcutKeyGroup.AllGroups)
            {
                foreach (ShortcutKey key in group.Keys)
                {
                    if (Changes.ContainsKey(key.Name))
                    {
                        key.Keys = (Keys)Changes[key.Name];
                        hasChanged = true;
                    }
                }
            }

            if (hasChanged)
            {
                KeyMap.Default.InvokeKeyManChanged();
            }
            return hasChanged;
        }

        public static Hashtable GetChanges()
        {
            Hashtable ht = new Hashtable(StringComparer.OrdinalIgnoreCase);
            foreach (ShortcutKeyGroup group in ShortcutKeyGroup.AllGroups)
            {
                foreach (ShortcutKey key in group.Keys)
                {
                    if (key.Changed)
                        ht[key.Name] = key.Keys;
                }
            }

            return ht;
        }

        public static void ResetAll()
        {
            bool hasChanged = false;
            foreach (ShortcutKeyGroup group in ShortcutKeyGroup.AllGroups)
            {
                foreach (ShortcutKey key in group.Keys)
                {
                    if (key.Changed)
                    {
                        key.Reset();
                        hasChanged = true;
                    }
                }
            }

            if (hasChanged)
            {
                KeyMap.Default.InvokeKeyManChanged();
            }
        }

        #region Valid Keys
        public static readonly Keys[] ValidKeys = new Keys[] { 
            Keys.A, Keys.Add, Keys.B, Keys.C, Keys.D, Keys.D0, Keys.D1, Keys.D2, Keys.D3, Keys.D4, Keys.D5, Keys.D6, Keys.D7, Keys.D8, Keys.D9, Keys.Delete, Keys.Down, 
            Keys.E, Keys.End, Keys.Enter, Keys.Escape, Keys.F, Keys.F1, Keys.F10, Keys.F11, Keys.F12, Keys.F13, Keys.F14, Keys.F15, Keys.F16, Keys.F17, Keys.F18, Keys.F19, Keys.F2, Keys.F20, 
            Keys.F21, Keys.F22, Keys.F23, Keys.F24, Keys.F3, Keys.F4, Keys.F5, Keys.F6, Keys.F7, Keys.F8, Keys.F9, Keys.G, Keys.H, Keys.I, Keys.Insert, Keys.J, 
            Keys.K, Keys.L, Keys.Left, Keys.M, Keys.Multiply, Keys.N, Keys.NumLock, Keys.NumPad0, Keys.NumPad1, Keys.NumPad2, Keys.NumPad3, Keys.NumPad4, Keys.NumPad5, Keys.NumPad6, Keys.NumPad7, Keys.NumPad8, Keys.NumPad9, 
            Keys.O, Keys.OemBackslash, Keys.OemClear, Keys.OemCloseBrackets, Keys.Oemcomma, Keys.OemMinus, Keys.OemOpenBrackets, Keys.OemPeriod, Keys.OemPipe, Keys.Oemplus, Keys.OemQuestion, Keys.OemQuotes, Keys.OemSemicolon, Keys.Oemtilde, Keys.P, Keys.Pause, 
            Keys.Q, Keys.R, Keys.Right, Keys.S, Keys.Space, Keys.Subtract, Keys.T, Keys.Tab, Keys.U, Keys.Up, Keys.V, Keys.W, Keys.X, Keys.Y, Keys.Z
         };

        public static bool IsValidKey(Keys keyCode)
        {
            return Array.IndexOf(ValidKeys, keyCode & Keys.KeyCode) > -1;
        }
        #endregion

        #region ShortcutKeys
        // document
        public static readonly ShortcutKey New = new ShortcutKey("New", Keys.Control | Keys.N, "Create a new Mind Map", Properties.Resources._new);
        public static readonly ShortcutKey Open = new ShortcutKey("Open", Keys.Control | Keys.O, "Open a old file", Properties.Resources.open);
        public static readonly ShortcutKey Save = new ShortcutKey("Save", Keys.Control | Keys.S, Properties.Resources.save);

        // view
        public static readonly ShortcutKey ZoomIn = new ShortcutKey("Zoom In"
                    , Keys.Control | Keys.Add
                    , Properties.Resources.zoom_in);
        public static readonly ShortcutKey ZoomOut = new ShortcutKey("Zoom Out"
                    , Keys.Control | Keys.Subtract
                    , Properties.Resources.zoom_out);
        public static readonly ShortcutKey ZoomActualSize = new ShortcutKey("Actual size"
                    , Keys.Control | Keys.D0);

        public static readonly ShortcutKey FullScreen = new ShortcutKey("Full Screen"
                    , Keys.F11
                    , Properties.Resources.full_screen);

        public static readonly ShortcutKey Collapse = new ShortcutKey("Collapse"
                    , Keys.Shift | Keys.Subtract);

        public static readonly ShortcutKey Expand = new ShortcutKey("Expand"
                    , Keys.Shift | Keys.Add);

        public static readonly ShortcutKey ToggleFolding = new ShortcutKey("Toggle Folding"
                    , Keys.Shift | Keys.Multiply);

        public static readonly ShortcutKey CollapseAll = new ShortcutKey("Collapse All"
                    , Keys.Control | Keys.Shift | Keys.Subtract);

        public static readonly ShortcutKey ExpandAll = new ShortcutKey("Expand All"
                    , Keys.Control | Keys.Shift | Keys.Add);

        public static readonly ShortcutKey Sidebar = new ShortcutKey("Sidebar"
                    , Keys.Control | Keys.B
                    , Properties.Resources.sidebar);

        public static readonly ShortcutKey NextTab = new ShortcutKey("Next Tab"
                    , Keys.Control | Keys.Tab);

        public static readonly ShortcutKey PreviousTab = new ShortcutKey("Previous Tab"
                    , Keys.Control | Keys.Shift | Keys.Tab);

        // edit
        public static readonly ShortcutKey Undo = new ShortcutKey("Undo", Keys.Control | Keys.Z, Properties.Resources.undo);
        public static readonly ShortcutKey Redo = new ShortcutKey("Redo", Keys.Control | Keys.Y, Properties.Resources.redo);
        public static readonly ShortcutKey Copy = new ShortcutKey("Copy", Keys.Control | Keys.C, Properties.Resources.copy);
        public static readonly ShortcutKey Cut = new ShortcutKey("Cut", Keys.Control | Keys.X, Properties.Resources.cut);
        public static readonly ShortcutKey Paste = new ShortcutKey("Paste", Keys.Control | Keys.V, Properties.Resources.paste);
        public static readonly ShortcutKey AddTopic = new ShortcutKey("Add Topic", Keys.Enter, Properties.Resources.add_topic);
        public static readonly ShortcutKey AddTopicFront = new ShortcutKey("Add Topic Front", Keys.Enter | Keys.Shift, Properties.Resources.add_topic_front);
        public static readonly ShortcutKey AddSubTopic = new ShortcutKey("Add Sub Topic", Keys.Tab, Properties.Resources.add_sub_topic);
        public static readonly ShortcutKey AddSubTopic2 = new ShortcutKey("Add Sub Topic", Keys.Insert, Properties.Resources.add_sub_topic);
        public static readonly ShortcutKey Edit = new ShortcutKey("Edit", Keys.Space, Properties.Resources.edit);
        public static readonly ShortcutKey Edit2 = new ShortcutKey("Edit", Keys.F2, Properties.Resources.edit);
        public static readonly ShortcutKey Delete = new ShortcutKey("Delete", Keys.Delete, Properties.Resources.delete);
        public static readonly ShortcutKey Find = new ShortcutKey("Find", Keys.Control | Keys.F, Properties.Resources.find);
        public static readonly ShortcutKey Replace = new ShortcutKey("Replace", Keys.Control | Keys.H, Properties.Resources.replace);
        public static readonly ShortcutKey MoveUp = new ShortcutKey("Move Up", Keys.Control | Keys.Up, Properties.Resources.up);
        public static readonly ShortcutKey MoveDown = new ShortcutKey("Move Down", Keys.Control | Keys.Down, Properties.Resources.down);

        // selection
        public static readonly ShortcutKey SelectAll = new ShortcutKey("Select All", Keys.Control | Keys.A);
        public static readonly ShortcutKey Left = new ShortcutKey("Left", Keys.Left);
        public static readonly ShortcutKey Up = new ShortcutKey("Up", Keys.Up);
        public static readonly ShortcutKey Right = new ShortcutKey("Right", Keys.Right);
        public static readonly ShortcutKey Down = new ShortcutKey("Down", Keys.Down);

        // misc
        public static readonly ShortcutKey Cancel = new ShortcutKey("Cancel", Keys.Escape);
        public static readonly ShortcutKey Help = new ShortcutKey("Help", Keys.F1, Properties.Resources.help);
        #endregion
    }
}
