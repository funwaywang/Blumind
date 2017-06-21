using System.Windows.Forms;
using Blumind.Core;
using System.Collections.Generic;
using System;

namespace Blumind.Controls.MapViews
{
    partial class MindMapView
    {
        private ShortcutKeysTable ShortcutKeys;

        private void InitializeKeyBoard()
        {
            ShortcutKeys = new ShortcutKeysTable();
            ShortcutKeys.Register(KeyMap.ExpandAll, delegate() { ExpandAll(); });
            ShortcutKeys.Register(KeyMap.CollapseAll, delegate() { CollapseAll(); });

            ShortcutKeys.Register(KeyMap.ZoomIn, delegate() { ZoomIn(); });
            ShortcutKeys.Register(KeyMap.ZoomOut, delegate() { ZoomOut(); });
            ShortcutKeys.Register(KeyMap.ZoomActualSize, delegate() { Zoom = 1.0f; });

            ShortcutKeys.Register(KeyMap.Undo, delegate() { if (CanUndo) { Undo(); } });
            ShortcutKeys.Register(KeyMap.Redo, delegate() { if (CanRedo) { Redo(); } });

            ShortcutKeys.Register(KeyMap.Copy, delegate() { if (CanCopy) { Copy(); } });
            ShortcutKeys.Register(KeyMap.Cut, delegate() { if (CanCut) { Cut(); } });
            ShortcutKeys.Register(KeyMap.Paste, delegate() { if (CanPaste) { Paste(); } });

            ShortcutKeys.Register(KeyMap.MoveUp, delegate() { CustomSort(-1); });
            ShortcutKeys.Register(KeyMap.MoveDown, delegate() { CustomSort(1); });

            ShortcutKeys.Register(KeyMap.SelectAll, delegate() { SelectAll(); });

            ShortcutKeys.Register(KeyMap.Expand, delegate() { ExpandSelect(true); });
            ShortcutKeys.Register(KeyMap.Collapse, delegate() { CollapseSelect(true); });
            ShortcutKeys.Register(KeyMap.ToggleFolding, delegate() { ToggleFolding(); });

            ShortcutKeys.Register(KeyMap.AddSubTopic, delegate() { if (!ReadOnly) { AddSubTopic(); } });
            ShortcutKeys.Register(KeyMap.AddSubTopic2, delegate() { if (!ReadOnly) { AddSubTopic(); } });
            ShortcutKeys.Register(KeyMap.AddTopic, delegate() { if (!ReadOnly) { AddTopic(); } });
            ShortcutKeys.Register(KeyMap.AddTopicFront, delegate() { if (!ReadOnly) { AddTopicFront(); } });
            ShortcutKeys.Register(KeyMap.Edit, delegate() { if (!ReadOnly) { EditObject(); } });
            ShortcutKeys.Register(KeyMap.Edit2, delegate() { if (!ReadOnly) { EditObject(); } });
            ShortcutKeys.Register(KeyMap.Delete, delegate() { if (!ReadOnly) { DeleteObject(); } });

            ShortcutKeys.Register(KeyMap.Left, delegate() { SelectNextTopic(MoveVector.Left); });
            ShortcutKeys.Register(KeyMap.Up, delegate() { SelectNextTopic(MoveVector.Up); });
            ShortcutKeys.Register(KeyMap.Right, delegate() { SelectNextTopic(MoveVector.Right); });
            ShortcutKeys.Register(KeyMap.Down, delegate() { SelectNextTopic(MoveVector.Down); });
            ShortcutKeys.Register(KeyMap.Cancel, delegate() { if (MouseState == ChartMouseState.Drag) { CancelDrag(); } });

        }

        protected override void OnChartKeyDown(KeyEventArgs e)
        {
            base.OnChartKeyDown(e);

            //System.Diagnostics.Debug.WriteLine(string.Format("KeyCode:{0} Control:{1} Alt:{2} Shift:{3}", e.KeyCode, e.Control, e.Alt, e.Shift));
            //Keys keys = e.KeyCode;
            //if (e.Control)
            //    keys |= Keys.Control;
            //if (e.Shift)
            //    keys |= Keys.Shift;
            //if (e.Alt)
            //    keys |= Keys.Alt;

            //
            if (ShortcutKeys.Haldle(e.KeyData))
            {
                e.SuppressKeyPress = true;
                return;
            }

            if (e.Control && DragBox.Visible)
            {
                DragBox.Invalidate();
            }

            //{
            //    switch (e.KeyCode)
            //    {
            //        case Keys.Tab:
            //        case Keys.Insert:
            //            if (!ReadOnly)
            //            {
            //                AddSubTopic();
            //                e.SuppressKeyPress = true;
            //            }
            //            break;
            //        case Keys.Enter:
            //            if (!ReadOnly)
            //            {
            //                AddTopic();
            //                e.SuppressKeyPress = true;
            //            }
            //            break;
            //        case Keys.Space:
            //        case Keys.F2:
            //            if (!ReadOnly)
            //            {
            //                EditObject();
            //                e.SuppressKeyPress = true;
            //            }
            //            break;
            //        case Keys.Delete:
            //            DeleteObject();
            //            e.SuppressKeyPress = true;
            //            break;
            //        case Keys.Left:
            //            SelectNextTopic(MoveVector.Left);
            //            e.SuppressKeyPress = true;
            //            break;
            //        case Keys.Up:
            //            SelectNextTopic(MoveVector.Up);
            //            e.SuppressKeyPress = true;
            //            break;
            //        case Keys.Right:
            //            SelectNextTopic(MoveVector.Right);
            //            e.SuppressKeyPress = true;
            //            break;
            //        case Keys.Down:
            //            SelectNextTopic(MoveVector.Down);
            //            e.SuppressKeyPress = true;
            //            break;
            //        case Keys.Escape:
            //            if (MouseState == ChartMouseState.Drag)
            //            {
            //                CancelDrag();
            //                e.SuppressKeyPress = true;
            //            }
            //            break;
            //    }
            //}
        }

        protected override void OnChartKeyPress(KeyPressEventArgs e)
        {
            base.OnChartKeyPress(e);

            if (e.KeyChar > 0x1F && !ReadOnly)
            {
                if (!EditMode)
                {
                    EditObject();
                }

                if (EditControl != null)
                {
                    EditControl.Send_WM_CHAR(e.KeyChar);
                }

                e.Handled = true;
            }
        }
    }
}
