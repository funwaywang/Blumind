using System;
using System.Collections;
using System.Text.RegularExpressions;
using Blumind.Core.Documents;
using Blumind.Model;

namespace Blumind.Model.MindMaps
{
    class MindMapFinder
    {
        string LastFindWhat;
        Regex LastRegex;
        FindOptions _Options;
        ArrayList AccessTable;
        int _FoundCount;

        public MindMapFinder()
        {
            Options = FindOptions.Default;
            AccessTable = new ArrayList();
        }

        public FindOptions Options
        {
            get { return _Options; }
            set { _Options = value; }
        }

        public int FoundCount
        {
            get { return _FoundCount; }
            private set { _FoundCount = value; }
        }

        public void Reset()
        {
            FoundCount = 0;
            AccessTable.Clear();
        }

        public Topic FindNext(Topic start, string findWhat)
        {
            if (Options.RegularExpression)
            {
                ReadyRegex(findWhat);
            }

            LastFindWhat = findWhat;

            if(!AccessTable.Contains(start) && start.Selected)
                AccessTable.Add(start);

            Topic topic = _FindNext(start, findWhat);
            //if (topic == null && !start.IsRoot)
            //{
            //    Topic root = start.GetRoot();
            //    topic = _FindNext(root, findWhat);
            //}
            if (topic != null)
                FoundCount++;
            return topic;
        }

        public string Replace(string text, string findWhat, string replaceWith)
        {
            if (Options.RegularExpression)
            {
                ReadyRegex(findWhat);
                return LastRegex.Replace(text, replaceWith);
            }
            else
            {
                StringComparison sc = Options.CaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
                int index = text.IndexOf(findWhat, sc);
                if (index > -1)
                {
                    return text.Substring(0, index) + replaceWith + text.Substring(index + findWhat.Length);
                }
                else
                {
                    return text;
                }
            }
        }

        private void ReadyRegex(string findWhat)
        {
            RegexOptions ro = RegexOptions.Compiled;
            if (!Options.CaseSensitive)
            {
                ro |= RegexOptions.IgnoreCase;
            }

            if (LastRegex == null || LastRegex.Options != ro || LastFindWhat != findWhat)
            {
                LastRegex = new Regex(findWhat, ro);
            }
        }

        /*private Topic _FindNext(Topic node, string findWhat, Topic callFrom)
        {
            if (node != callFrom && TestMatch(node.Text, findWhat))
            {
                return node;
            }

            if (node.Children.Count > 0 && (!node.Folded || Options.WithHiddenItems))
            {
                if (Options.Direction == FindDirection.Forward)
                {
                    for (int i = 0; i < node.Children.Count; i++)
                    {
                        if (callFrom == null || !node.Children.Contains(callFrom) || i > callFrom.Index)
                        {
                            Topic r = _FindNext(node.Children[i], findWhat, null);
                            if (r != null)
                            {
                                return r;
                            }
                        }
                    }
                }
                else
                {
                    for (int i = node.Children.Count - 1; i >= 0; i--)
                    {
                        if (callFrom == null || !node.Children.Contains(callFrom) || i < callFrom.Index)
                        {
                            Topic r = _FindNext(node.Children[i], findWhat, null);
                            if (r != null)
                            {
                                return r;
                            }
                        }
                    }
                }
            }

            if (node.Parent != null && callFrom != null)
            {
                return _FindNext(node.Parent, findWhat, node);
            }
            else
            {
                return null;
            }
        }*/

        private Topic _FindNext(Topic node, string findWhat)
        {
            if (!AccessTable.Contains(node))
            {
                AccessTable.Add(node);
                if (TestMatch(node.Text, findWhat))
                    return node;
            }

            // 1. find children
            if (node.Children.Count > 0 && (!node.Folded || Options.WithHiddenItems))
            {
                //Topic child = GetNextUnaccessChild(node, Options.Direction == FindDirection.Forward ? 0 : node.Children.Count - 1, true);
                Topic child = GetNextUnaccessChild(node, 0, true);
                if (child != null)
                    return _FindNext(child, findWhat);
            }

            if (node.ParentTopic != null)
            {
                // find brother
                Topic brother = GetNextUnaccessChild(node.ParentTopic, node.Index, false);
                if (brother != null)
                {
                    Topic brother_found = _FindNext(brother, findWhat);
                    if (brother_found != null)
                        return brother_found;
                }
                
                // find parent
                if (!AccessTable.Contains(node.ParentTopic))
                    return _FindNext(node.ParentTopic, findWhat);
                else
                {
                    Topic ancestor = GetUnaccessAncestor(node);
                    if (ancestor != null)
                        return _FindNext(ancestor, findWhat);
                }
            }

            return null;
        }

        private Topic GetUnaccessAncestor(Topic node)
        {
            Topic parent = node.ParentTopic;
            while (parent != null && AccessTable.Contains(parent))
            {
                //Topic child = GetNextUnaccessChild(parent, (Options.Direction == FindDirection.Forward ? 0 : parent.Children.Count - 1), true);
                Topic child = GetNextUnaccessChild(parent, 0, true);
                if (child != null)
                    return child;

                parent = parent.ParentTopic;
            }

            return parent;
        }

        private Topic GetNextUnaccessChild(Topic parent, int startIndex, bool withStart)
        {
            if(parent.Children.IsEmpty || (parent.Folded && !Options.WithHiddenItems))
                return null;
            if (startIndex < 0 || startIndex >= parent.Children.Count) 
                return null;

            //if (Options.Direction == FindDirection.Forward)
            //{
                int i = startIndex;
                if (!withStart)
                    i++;

                while(true)
                {
                    if(i >= parent.Children.Count)
                        i= 0;
                    if (i == startIndex)
                    {
                        if (withStart)
                            withStart = false;
                        else
                            return null;
                    }
                    if(!AccessTable.Contains(parent.Children[i]))
                        return parent.Children[i];
                    i++;
                }
            //}
            //else
            //{
            //    int i = startIndex;
            //    if (!withStart)
            //        i--;

            //    while (true)
            //    {
            //        if (i < 0)
            //            i = parent.Children.Count - 1;
            //        if (i == startIndex)
            //        {
            //            if (withStart)
            //                withStart = false;
            //            else
            //                return null;
            //        }
            //        if (!AccessTable.Contains(parent.Children[i]))
            //            return parent.Children[i];
            //        i--;
            //    }
            //}
        }

        private bool TestMatch(string text, string findWhat)
        {
            if (Options.RegularExpression)
            {
                return LastRegex.IsMatch(text);
            }
            else
            {
                if (Options.WholeWordOnly)
                {
                    StringComparer sc = Options.CaseSensitive ? StringComparer.Ordinal : StringComparer.OrdinalIgnoreCase;
                    return sc.Equals(text, findWhat);
                }
                else
                {
                    StringComparison sc = Options.CaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
                    return text.IndexOf(findWhat, sc) > -1;
                }
            }
        }
    }
}
