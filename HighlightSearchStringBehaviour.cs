using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;
using Microsoft.Xaml.Interactivity;

namespace Seboschtion
{
    internal static class MatchCollectionExtensions
    {
        internal static string[] ToArray(this MatchCollection matchCollection)
        {
            var matches = new string[matchCollection.Count];
            for (int i = 0; i < matches.Length; i++)
            {
                matches[i] = matchCollection[i].Value;
            }
            return matches;
        }
    }

    public class HighlightSearchStringBehaviour : DependencyObject, IBehavior
    {
        private enum StartColorizationWith
        {
            First = 0,
            Second = 1
        }

        private string _textBlockContent = string.Empty;
        private TextBlock TextBlock => (TextBlock)AssociatedObject;

        public DependencyProperty HighlightForegroundProperty = DependencyProperty.Register("HighlightForeground",
           typeof(Brush), typeof(HighlightSearchStringBehaviour), null);

        public DependencyProperty SearchStringProperty = DependencyProperty.Register("SearchString", typeof(string),
            typeof(HighlightSearchStringBehaviour), new PropertyMetadata(string.Empty, RegexChangedCallback));

        public DependencyProperty RegexOptionsProperty = DependencyProperty.Register("RegexOptions", typeof(RegexOptions),
            typeof(HighlightSearchStringBehaviour), new PropertyMetadata(RegexOptions.IgnoreCase));

        public DependencyProperty ImproperMatchProperty = DependencyProperty.Register("ImproperMatch", typeof(bool),
            typeof(HighlightSearchStringBehaviour), new PropertyMetadata(true));

        public DependencyObject AssociatedObject { get; private set; }

        public Brush HighlightForeground
        {
            get { return (Brush)GetValue(HighlightForegroundProperty); }
            set { SetValue(HighlightForegroundProperty, value); }
        }

        public string SearchString
        {
            get { return (string)GetValue(SearchStringProperty); }
            set { SetValue(SearchStringProperty, value); }
        }

        /// <summary>
        /// Custom RegexOptions which influence the search result.
        /// Default: RegexOptions.IgnoreCase
        /// </summary>
        public RegexOptions RegexOptions
        {
            get { return (RegexOptions)GetValue(RegexOptionsProperty); }
            set { SetValue(RegexOptionsProperty, value); }
        }

        /// <summary>
        /// If false, the whole SearchString must match the text in the TextBlock. If true, spaces are assumed to be search string delimiters.
        /// Default: true
        /// </summary>
        public bool ImproperMatch
        {
            get { return (bool)GetValue(ImproperMatchProperty); }
            set { SetValue(ImproperMatchProperty, value); }
        }

        public void Attach(DependencyObject associatedObject)
        {
            if (associatedObject is TextBlock)
            {
                AssociatedObject = associatedObject;
            }
            else
            {
                throw new Exception("HighlightSearchStringBehaviour is only attachable to TextBlocks.");
            }
        }

        public void Detach()
        {
            AssociatedObject = null;
        }

        private static void RegexChangedCallback(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var behaviour = (HighlightSearchStringBehaviour) obj;
            behaviour.ColorizeTextElements(e.NewValue.ToString());
        }

        private void ColorizeTextElements(string pattern)
        {
            _textBlockContent = TextBlock.Text;

            pattern = PrepareForImproperMatch(pattern);
            var regex = new Regex(pattern, this.RegexOptions);
            var mismatches = regex.Split(_textBlockContent);
            TextBlock.Inlines.Clear();
            var matches = regex.Matches(_textBlockContent).ToArray();
            var startWithMismatch = mismatches.Length > 0 && _textBlockContent.StartsWith(mismatches.First());
            string[] splits = startWithMismatch ? MergedSplits(mismatches, matches) : MergedSplits(matches, mismatches);

            foreach (var run in ColorizedSplits(splits, startWithMismatch ? StartColorizationWith.Second : StartColorizationWith.First))
            {
                TextBlock.Inlines.Add(run);
            }
        }

        private string[] MergedSplits(string[] elements1, string[] elements2)
        {
            string[] merged = new string[elements1.Length + elements2.Length];
            int pointer1 = 0, pointer2 = 0;
            for (int i = 0; i < merged.Length; i++)
            {
                merged[i] = i%2 == 0 ? elements1[pointer1++] : elements2[pointer2++];
            }
            return merged;
        }

        private IEnumerable<Run> ColorizedSplits(string[] splits, StartColorizationWith start)
        {
            for (int i = 0; i < splits.Length; i++)
            {
                var run = new Run {Text = splits[i]};
                if (i%2 == (int)start)
                {
                    run.Foreground = HighlightForeground;
                }
                yield return run;
            }
        }

        private string PrepareForImproperMatch(string source)
        {
            return ImproperMatch ? source.Replace(' ', '|') : source;
        }
    }
}