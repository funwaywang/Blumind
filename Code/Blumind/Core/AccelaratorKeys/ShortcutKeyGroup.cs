using System;
using System.Collections.Generic;
using System.Text;

namespace Blumind.Core
{
    class ShortcutKeyGroup
    {
        public string Name { get; set; }
        public ShortcutKey[] Keys { get; set; }

        public ShortcutKeyGroup(string name, ShortcutKey[] keys)
        {
            Name = name;
            Keys = keys;
        }

        public static ShortcutKeyGroup[] AllGroups
        {
            get
            {
                return new ShortcutKeyGroup[] {
                    ShortcutKeyGroup.Document,
                    ShortcutKeyGroup.Edit,
                    ShortcutKeyGroup.View,
                    ShortcutKeyGroup.Selection,
                    ShortcutKeyGroup.Miscellaneous
                };
            }
        }

        public static ShortcutKeyGroup Document
        {
            get
            {
                return new ShortcutKeyGroup("Document", new ShortcutKey[] {
                    KeyMap.New,
                    KeyMap.Open,
                    KeyMap.Save
                });
            }
        }

        public static ShortcutKeyGroup View
        {
            get
            {
                return new ShortcutKeyGroup("View", new ShortcutKey[] {
                    KeyMap.ZoomIn,
                    KeyMap.ZoomOut,
                    KeyMap.ZoomActualSize,
                    KeyMap.FullScreen,
                    KeyMap.Collapse,
                    KeyMap.Expand,
                    KeyMap.ToggleFolding,
                    KeyMap.CollapseAll,
                    KeyMap.ExpandAll,
                    KeyMap.Sidebar,
                    KeyMap.NextTab,
                    KeyMap.PreviousTab
                });
            }
        }

        public static ShortcutKeyGroup Edit
        {
            get
            {
                return new ShortcutKeyGroup("Edit", new ShortcutKey[] {
                    KeyMap.Undo,
                    KeyMap.Redo,
                    KeyMap.Copy,
                    KeyMap.Cut,
                    KeyMap.Paste,
                    KeyMap.AddTopic,
                    KeyMap.AddTopicFront,
                    KeyMap.AddSubTopic,
                    KeyMap.Edit,
                    KeyMap.Delete,
                    KeyMap.Find,
                    KeyMap.Replace,
                    KeyMap.MoveUp,
                    KeyMap.MoveDown
                });
            }
        }

        public static ShortcutKeyGroup Selection
        {
            get
            {
                return new ShortcutKeyGroup("Selection", new ShortcutKey[] {
                    KeyMap.SelectAll,
                    KeyMap.Left,
                    KeyMap.Up,
                    KeyMap.Right,
                    KeyMap.Down
                });
            }
        }

        public static ShortcutKeyGroup Miscellaneous
        {
            get
            {
                return new ShortcutKeyGroup("Miscellaneous", new ShortcutKey[] {
                    KeyMap.Cancel,
                    KeyMap.Help,
                });
            }
        }
    }
}
