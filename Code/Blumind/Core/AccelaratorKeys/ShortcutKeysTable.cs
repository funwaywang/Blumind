using System;
using System.Collections.Generic;
using System.Text;

namespace Blumind.Core
{
    class ShortcutKeysTable
    {
        private List<ShortcutKeyAction> Table;

        public ShortcutKeysTable()
        {
            Table = new List<ShortcutKeyAction>();
        }

        public void Register(ShortcutKey key, ShortcutKeyEventHandler handler)
        {
            Table.Add(new ShortcutKeyAction(key, handler));
        }

        public bool Haldle(System.Windows.Forms.Keys keys)
        {
            foreach (ShortcutKeyAction action in Table)
            {
                if (action.Key.Test(keys) && action.Handler != null)
                {
                    action.Handler.Invoke();
                    return true;
                }
            }

            return false;
        }
    }
}
