// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="Markdown.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Markdown.Xaml
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Windows;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Markup;
    using System.Windows.Shapes;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1724:TypeNamesShouldNotMatchNamespaces", Justification = "Wouldn't make sense to do so")]
    public class Markdown : DependencyObject
    {
        // Using a DependencyProperty as the backing store for CodeStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CodeStyleProperty =
            DependencyProperty.Register("CodeStyle", typeof(Style), typeof(Markdown), new PropertyMetadata(null));

        // Using a DependencyProperty as the backing store for DocumentStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DocumentStyleProperty =
            DependencyProperty.Register("DocumentStyle", typeof(Style), typeof(Markdown), new PropertyMetadata(null));

        // Using a DependencyProperty as the backing store for Heading1Style.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty Heading1StyleProperty =
            DependencyProperty.Register("Heading1Style", typeof(Style), typeof(Markdown), new PropertyMetadata(null));

        // Using a DependencyProperty as the backing store for Heading2Style.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty Heading2StyleProperty =
            DependencyProperty.Register("Heading2Style", typeof(Style), typeof(Markdown), new PropertyMetadata(null));

        // Using a DependencyProperty as the backing store for Heading3Style.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty Heading3StyleProperty =
            DependencyProperty.Register("Heading3Style", typeof(Style), typeof(Markdown), new PropertyMetadata(null));

        // Using a DependencyProperty as the backing store for Heading4Style.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty Heading4StyleProperty =
            DependencyProperty.Register("Heading4Style", typeof(Style), typeof(Markdown), new PropertyMetadata(null));

        private const string AnchorLineStringFormat = @"
                (                           # wrap whole match in $1
                    \[
                        ({0})               # link text = $2
                    \]
                    \(                      # literal paren
                        [ ]*
                        ({1})               # href = $3
                        [ ]*
                        (                   # $4
                        (['""])           # quote char = $5
                        (.*?)               # title = $6
                        \5                  # matching quote
                        [ ]*                # ignore any spaces between closing quote and )
                        )?                  # title is optional
                    \)
                )";

        private const string MarkerOl = @"\d+[.]";

        private const string MarkerUl = @"[*+-]";

        /// <summary>
        /// maximum nested depth of [] and () supported by the transform; implementation detail
        /// </summary>
        private const int NestDepth = 6;

        /// <summary>
        /// Tabs are automatically converted to spaces as part of the transform  
        /// this constant determines how "wide" those tabs become in spaces  
        /// </summary>
        private const int TabWidth = 4;
        private const string WholeLineStringFormat = @"
            (                               # $1 = whole list
              (                             # $2
                [ ]{{0,{1}}}
                ({0})                       # $3 = first list item marker
                [ ]+
              )
              (?s:.+?)
              (                             # $4
                  \z
                |
                  \n{{2,}}
                  (?=\S)
                  (?!                       # Negative lookahead for another list item marker
                    [ ]*
                    {0}[ ]+
                  )
              )
            )";

        private static readonly Regex AnchorInline = new Regex(
            string.Format(
                CultureInfo.CurrentCulture,
                AnchorLineStringFormat,
                GetNestedBracketsPattern(),
                GetNestedParensPattern()),
                RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);

        private static readonly Regex Bold = new Regex(
            @"(\*\*|__) (?=\S) (.+?[*_]*) (?<=\S) \1",
            RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline | RegexOptions.Compiled);

        private static readonly Regex CodeSpan = new Regex(
                    @"
                    (?<!\\)   # Character before opening ` can't be a backslash
                    (`+)      # $1 = Opening run of `
                    (.+?)     # $2 = The code block
                    (?<!`)
                    \1
                    (?!`)",
                          RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline | RegexOptions.Compiled);

        private static readonly Regex Eoln = new Regex("\\s+");

        private static readonly Regex HeaderAtx = new Regex(
                @"
                ^(\#{1,6})  # $1 = string of #'s
                [ ]*
                (.+?)       # $2 = Header text
                [ ]*
                \#*         # optional closing #'s (not counted)
                \n+",
            RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);

        private static readonly Regex HeaderSetext = new Regex(
                @"
                ^(.+?)
                [ ]*
                \n
                (=+|-+)     # $1 = string of ='s or -'s
                [ ]*
                \n+",
            RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);

        private static readonly Regex HorizontalRules = new Regex(
            @"
            ^[ ]{0,3}         # Leading space
                ([-*_])       # $1: First marker
                (?>           # Repeated marker group
                    [ ]{0,2}  # Zero, one, or two spaces.
                    \1        # Marker character
                ){2,}         # Group repeated at least twice
                [ ]*          # Trailing spaces
                $             # End of line.
            ",
             RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);

        private static readonly Regex Italic = new Regex(
            @"(\*|_) (?=\S) (.+?) (?<=\S) \1",
            RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline | RegexOptions.Compiled);

        private static readonly Regex ListNested = new Regex(
            @"^" + WholeList,
            RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);

        private static readonly Regex ListTopLevel = new Regex(
            @"(?:(?<=\n\n)|\A\n?)" + WholeList,
            RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);

        private static readonly Regex NewlinesLeadingTrailing = new Regex(@"^\n+|\n+\z", RegexOptions.Compiled);

        private static readonly Regex NewlinesMultiple = new Regex(@"\n{2,}", RegexOptions.Compiled);

        private static readonly Regex OutDent = new Regex(@"^[ ]{1," + TabWidth + @"}", RegexOptions.Multiline | RegexOptions.Compiled);

        private static readonly Regex StrictBold =
            new Regex(
                @"([\W_]|^) (\*\*|__) (?=\S) ([^\r]*?\S[\*_]*) \2 ([\W_]|$)",
                RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline | RegexOptions.Compiled);

        private static readonly Regex StrictItalic = new Regex(
            @"([\W_]|^) (\*|_) (?=\S) ([^\r\*_]*?\S) \2 ([\W_]|$)",
            RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline | RegexOptions.Compiled);

        private static readonly string WholeList = string.Format(
              CultureInfo.CurrentCulture,
              WholeLineStringFormat,
              string.Format(CultureInfo.CurrentCulture, "(?:{0}|{1})", MarkerUl, MarkerOl),
              TabWidth - 1);

        private static string _nestedBracketsPattern;

        private static string _nestedParensPattern;

        private int _listLevel;

        public Markdown()
        {
            this.HyperlinkCommand = NavigationCommands.GoToPage;
        }

        public Style CodeStyle
        {
            get { return (Style)GetValue(CodeStyleProperty); }
            set { this.SetValue(CodeStyleProperty, value); }
        }

        public Style DocumentStyle
        {
            get { return (Style)GetValue(DocumentStyleProperty); }
            set { this.SetValue(DocumentStyleProperty, value); }
        }

        public Style Heading1Style
        {
            get { return (Style)GetValue(Heading1StyleProperty); }
            set { this.SetValue(Heading1StyleProperty, value); }
        }

        public Style Heading2Style
        {
            get { return (Style)GetValue(Heading2StyleProperty); }
            set { this.SetValue(Heading2StyleProperty, value); }
        }

        public Style Heading3Style
        {
            get { return (Style)GetValue(Heading3StyleProperty); }
            set { this.SetValue(Heading3StyleProperty, value); }
        }

        public Style Heading4Style
        {
            get { return (Style)GetValue(Heading4StyleProperty); }
            set { this.SetValue(Heading4StyleProperty, value); }
        }

        public ICommand HyperlinkCommand { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether, when true, bold and italic require non-word characters on either side  
        /// WARNING: this is a significant deviation from the markdown spec
        /// </summary>
        public bool StrictBoldItalic { get; set; }

        public Block CreateHeader(int level, IEnumerable<Inline> content)
        {
            if (content == null)
            {
                throw new ArgumentNullException("content");
            }

            var block = Create<Paragraph, Inline>(content);

            switch (level)
            {
                case 1:
                    if (this.Heading1Style != null)
                    {
                        block.Style = this.Heading1Style;
                    }

                    break;

                case 2:
                    if (this.Heading2Style != null)
                    {
                        block.Style = this.Heading2Style;
                    }

                    break;

                case 3:
                    if (this.Heading3Style != null)
                    {
                        block.Style = this.Heading3Style;
                    }

                    break;

                case 4:
                    if (this.Heading4Style != null)
                    {
                        block.Style = this.Heading4Style;
                    }

                    break;
            }

            return block;
        }

        public IEnumerable<Inline> DoText(string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }

            var t = Eoln.Replace(text, " ");
            yield return new Run(t);
        }

        public FlowDocument Transform(string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }

            text = Normalize(text);
            var document = Create<FlowDocument, Block>(this.RunBlockGamut(text));

            document.PagePadding = new Thickness(0);
            if (this.DocumentStyle != null)
            {
                document.Style = this.DocumentStyle;
            }

            return document;
        }

        private static TResult Create<TResult, TContent>(IEnumerable<TContent> content)
                            where TResult : IAddChild, new()
        {
            var result = new TResult();
            foreach (var c in content)
            {
                result.AddChild(c);
            }

            return result;
        }

        /// <summary>
        /// Reusable pattern to match balanced [brackets]. See Friedl's 
        /// "Mastering Regular Expressions", Second Edition, pages 328-331.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/> current Nested Brackets Pattern.
        /// </returns>
        private static string GetNestedBracketsPattern()
        {
            // in other words [this] and [this[also]] and [this[also[too]]]
            // up to _nestDepth
            if (_nestedBracketsPattern == null)
            {
                _nestedBracketsPattern = RepeatString(
                    @"
                    (?>              # Atomic matching
                       [^\[\]]+      # Anything other than brackets
                     |
                       \[
                           ",
                    NestDepth) + RepeatString(
                    @" \]
                    )*",
                       NestDepth);
            }

            return _nestedBracketsPattern;
        }

        /// <summary>
        /// Reusable pattern to match balanced (parenthesis). See Friedl's 
        /// "Mastering Regular Expressions", Second Edition, pages 328-331.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/> current Nested Parenthesis Pattern.
        /// </returns>
        private static string GetNestedParensPattern()
        {
            // in other words (this) and (this(also)) and (this(also(too)))
            // up to _nestDepth
            if (_nestedParensPattern == null)
            {
                _nestedParensPattern = RepeatString(
                    @"
                    (?>              # Atomic matching
                       [^()\s]+      # Anything other than parenthesis or whitespace
                     |
                       \(
                           ",
                            NestDepth) + RepeatString(
                    @" \)
                    )*",
                        NestDepth);
            }

            return _nestedParensPattern;
        }

        /// <summary>
        /// convert all tabs to _tabWidth spaces; 
        /// standardizes line endings from DOS (CR LF) or Mac (CR) to UNIX (LF); 
        /// makes sure text ends with a couple of newlines; 
        /// removes any blank lines (only spaces) in the text
        /// </summary>
        /// <param name="text">
        /// The text that is to be normalized.
        /// </param>
        /// <returns>
        /// The <see cref="string"/> which has been normalized.
        /// </returns>
        private static string Normalize(string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }

            var output = new StringBuilder(text.Length);
            var line = new StringBuilder();
            bool valid = false;

            for (int i = 0; i < text.Length; i++)
            {
                switch (text[i])
                {
                    case '\n':
                        if (valid)
                        {
                            output.Append(line);
                        }

                        output.Append('\n');
                        line.Length = 0;
                        valid = false;
                        break;
                    case '\r':
                        if ((i < text.Length - 1) && (text[i + 1] != '\n'))
                        {
                            if (valid)
                            {
                                output.Append(line);
                            }

                            output.Append('\n');
                            line.Length = 0;
                            valid = false;
                        }

                        break;
                    case '\t':
                        int width = (TabWidth - line.Length) % TabWidth;

                        for (int k = 0; k < width; k++)
                        {
                            line.Append(' ');
                        }

                        break;
                    case '\x1A':
                        break;
                    default:
                        if (!valid && text[i] != ' ')
                        {
                            valid = true;
                        }

                        line.Append(text[i]);
                        break;
                }
            }

            if (valid)
            {
                output.Append(line);
            }

            output.Append('\n');

            // add two newlines to the end before return
            return output.Append("\n\n").ToString();
        }

        /// <summary>
        /// Remove one level of line-leading spaces
        /// </summary>
        /// <param name="block">
        /// The block of text which should be modified.
        /// </param>
        /// <returns>
        /// The <see cref="string"/> containing the modified text.
        /// </returns>
        private static string Outdent(string block)
        {
            return OutDent.Replace(block, string.Empty);
        }

        /// <summary>
        /// This is to emulate what's available in PHP
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="count">
        /// The count.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string RepeatString(string text, int count)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }

            var sb = new StringBuilder(text.Length * count);
            for (int i = 0; i < count; i++)
            {
                sb.Append(text);
            }

            return sb.ToString();
        }

        private Inline AnchorInlineEvaluator(Match match)
        {
            if (match == null)
            {
                throw new ArgumentNullException("match");
            }

            string linkText = match.Groups[2].Value;
            string url = match.Groups[3].Value;

            var result = Create<Hyperlink, Inline>(this.RunSpanGamut(linkText));
            result.Command = this.HyperlinkCommand;
            result.CommandParameter = url;
            return result;
        }

        private Block AtxHeaderEvaluator(Match match)
        {
            if (match == null)
            {
                throw new ArgumentNullException("match");
            }

            string header = match.Groups[2].Value;
            int level = match.Groups[1].Value.Length;
            return this.CreateHeader(level, this.RunSpanGamut(header));
        }

        private Inline BoldEvaluator(Match match, int contentGroup)
        {
            if (match == null)
            {
                throw new ArgumentNullException("match");
            }

            var content = match.Groups[contentGroup].Value;
            return Markdown.Create<Bold, Inline>(this.RunSpanGamut(content));
        }

        private Inline CodeSpanEvaluator(Match match)
        {
            if (match == null)
            {
                throw new ArgumentNullException("match");
            }

            string span = match.Groups[2].Value;
            span = Regex.Replace(span, @"^[ ]*", string.Empty); // leading whitespace
            span = Regex.Replace(span, @"[ ]*$", string.Empty); // trailing whitespace

            var result = new Run(span);
            if (this.CodeStyle != null)
            {
                result.Style = this.CodeStyle;
            }

            return result;
        }

        /// <summary>
        /// Turn Markdown link shortcuts into hyper links
        /// </summary>
        /// <param name="text">
        /// The text which is to be processed.
        /// </param>
        /// <param name="defaultHandler">
        /// The default Handler.
        /// </param>
        /// <remarks>
        /// [link text](URL "title") 
        /// </remarks>
        /// <returns>
        /// The <see cref="IEnumerable"/> containing all the processed anchors.
        /// </returns>
        private IEnumerable<Inline> DoAnchors(string text, Func<string, IEnumerable<Inline>> defaultHandler)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }

            // Next, inline-style links: [link text](url "optional title") or [link text](url "optional title")
            return this.Evaluate(text, AnchorInline, this.AnchorInlineEvaluator, defaultHandler);
        }

        /// <summary>
        /// Turn Markdown `code spans` into HTML code tags
        /// </summary>
        /// <param name="text">
        /// The text which is to be processed.
        /// </param>
        /// <param name="defaultHandler">
        /// The default Handler.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/> containing all the processed code spans.
        /// </returns>
        private IEnumerable<Inline> DoCodeSpans(string text, Func<string, IEnumerable<Inline>> defaultHandler)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }

            ////    * You can use multiple backticks as the delimiters if you want to
            ////        include literal backticks in the code span. So, this input:
            ////
            ////        Just type ``foo `bar` baz`` at the prompt.
            ////
            ////        Will translate to:
            ////
            ////          <p>Just type <code>foo `bar` baz</code> at the prompt.</p>
            ////
            ////        There's no arbitrary limit to the number of backticks you
            ////        can use as delimters. If you need three consecutive backticks
            ////        in your code, use four for delimiters, etc.
            ////
            ////    * You can use spaces to get literal backticks at the edges:
            ////
            ////          ... type `` `bar` `` ...
            ////
            ////        Turns to:
            ////
            ////          ... type <code>`bar`</code> ...         
            ////

            return this.Evaluate(text, CodeSpan, this.CodeSpanEvaluator, defaultHandler);
        }

        /// <summary>
        /// Turn Markdown headers into HTML header tags
        /// </summary>
        /// <param name="text">
        /// The text which is to be processed.
        /// </param>
        /// <param name="defaultHandler">
        /// The default Handler.
        /// </param>
        /// <remarks>
        /// <para>
        /// Header 1  
        /// ========  
        /// </para>
        /// <para>
        /// Header 2  
        /// --------  
        /// </para>
        /// # Header 1  
        /// ## Header 2  
        /// ## Header 2 with closing hashes ##  
        /// ...  
        /// ###### Header 6  
        /// </remarks>
        /// <returns>
        /// The <see cref="IEnumerable"/> containing all the processed headers.
        /// </returns>
        private IEnumerable<Block> DoHeaders(string text, Func<string, IEnumerable<Block>> defaultHandler)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }

            return this.Evaluate(
                text,
                HeaderSetext,
                this.SetextHeaderEvaluator,
                s => this.Evaluate(s, HeaderAtx, this.AtxHeaderEvaluator, defaultHandler));
        }

        /// <summary>
        /// Turn Markdown horizontal rules into HTML hr tags
        /// </summary>
        /// <param name="text">
        /// The text which is to be processed.
        /// </param>
        /// <param name="defaultHandler">
        /// The default Handler.
        /// </param>
        /// <remarks>
        /// <para>
        /// ***  
        /// * * *  
        /// ---
        /// - - -
        /// </para>
        /// </remarks>
        /// <returns>
        /// The <see cref="IEnumerable"/> containing all the processed horizontal rules.
        /// </returns>
        private IEnumerable<Block> DoHorizontalRules(string text, Func<string, IEnumerable<Block>> defaultHandler)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }

            return this.Evaluate(text, HorizontalRules, this.RuleEvaluator, defaultHandler);
        }

        /// <summary>
        /// Turn Markdown *italics* and **bold** into HTML strong and em tags
        /// </summary>
        /// <param name="text">
        /// The text which is to be processed.
        /// </param>
        /// <param name="defaultHandler">
        /// The default Handler.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/> containing the processed tags.
        /// </returns>
        private IEnumerable<Inline> DoItalicsAndBold(string text, Func<string, IEnumerable<Inline>> defaultHandler)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }

            // <strong> must go first, then <em>
            if (this.StrictBoldItalic)
            {
                return this.Evaluate(
                    text,
                    StrictBold,
                    m => this.BoldEvaluator(m, 3),
                    s1 => this.Evaluate(s1, StrictItalic, m => this.ItalicEvaluator(m, 3), defaultHandler));
            }
            else
            {
                return this.Evaluate(
                    text,
                    Bold,
                    m => this.BoldEvaluator(m, 2),
                    s1 => this.Evaluate(s1, Italic, m => this.ItalicEvaluator(m, 2), defaultHandler));
            }
        }

        /// <summary>
        /// Turn Markdown lists into HTML ul and ol and li tags
        /// </summary>
        /// <param name="text">
        /// The text which is to be processed
        /// </param>
        /// <param name="defaultHandler">
        /// The default Handler.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/> containing the processed lists.
        /// </returns>
        private IEnumerable<Block> DoLists(string text, Func<string, IEnumerable<Block>> defaultHandler)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }

            // We use a different prefix before nested lists than top-level lists.
            // See extended comment in _ProcessListItems().
            if (this._listLevel > 0)
            {
                return this.Evaluate(text, ListNested, this.ListEvaluator, defaultHandler);
            }
            else
            {
                return this.Evaluate(text, ListTopLevel, this.ListEvaluator, defaultHandler);
            }
        }

        private IEnumerable<T> Evaluate<T>(string text, Regex expression, Func<Match, T> build, Func<string, IEnumerable<T>> rest)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }

            var matches = expression.Matches(text);
            var index = 0;
            foreach (Match m in matches)
            {
                if (m.Index > index)
                {
                    var prefix = text.Substring(index, m.Index - index);
                    foreach (var t in rest(prefix))
                    {
                        yield return t;
                    }
                }

                yield return build(m);

                index = m.Index + m.Length;
            }

            if (index < text.Length)
            {
                var suffix = text.Substring(index, text.Length - index);
                foreach (var t in rest(suffix))
                {
                    yield return t;
                }
            }
        }

        /// <summary>
        /// splits on two or more newlines, to form "paragraphs";    
        /// </summary>
        /// <param name="text">
        /// The text which is to be processed.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/> containing the processed paragraphs.
        /// </returns>
        private IEnumerable<Block> FormParagraphs(string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }

            // split on two or more newlines
            var grafs = NewlinesMultiple.Split(NewlinesLeadingTrailing.Replace(text, string.Empty));

            return grafs.Select(g => Markdown.Create<Paragraph, Inline>(this.RunSpanGamut(g)));
        }

        private Inline ItalicEvaluator(Match match, int contentGroup)
        {
            if (match == null)
            {
                throw new ArgumentNullException("match");
            }

            var content = match.Groups[contentGroup].Value;
            return Markdown.Create<Italic, Inline>(this.RunSpanGamut(content));
        }

        private Block ListEvaluator(Match match)
        {
            if (match == null)
            {
                throw new ArgumentNullException("match");
            }

            string list = match.Groups[1].Value;
            string listType = Regex.IsMatch(match.Groups[3].Value, MarkerUl) ? "ul" : "ol";

            // Turn double returns into triple returns, so that we can make a
            // paragraph for the last item in a list, if necessary:
            list = Regex.Replace(list, @"\n{2,}", "\n\n\n");

            var resultList = Create<List, ListItem>(this.ProcessListItems(list, listType == "ul" ? MarkerUl : MarkerOl));

            resultList.MarkerStyle = listType == "ul" ? TextMarkerStyle.Disc : TextMarkerStyle.Decimal;

            return resultList;
        }

        private ListItem ListItemEvaluator(Match match)
        {
            if (match == null)
            {
                throw new ArgumentNullException("match");
            }

            string item = match.Groups[4].Value;
            string leadingLine = match.Groups[1].Value;

            // we could correct any bad indentation here..
            if (!string.IsNullOrEmpty(leadingLine) || Regex.IsMatch(item, @"\n{2,}"))
            {
                return Markdown.Create<ListItem, Block>(this.RunBlockGamut(item));
            }
            else
            {
                // recursion for sub-lists
                return Markdown.Create<ListItem, Block>(this.RunBlockGamut(item));
            }
        }

        /// <summary>
        /// Process the contents of a single ordered or unordered list, splitting it
        /// into individual list items.
        /// </summary>
        /// <param name="list">
        /// The list which needs to be processed.
        /// </param>
        /// <param name="marker">
        /// The marker that should be used.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/> containing the changes.
        /// </returns>
        private IEnumerable<ListItem> ProcessListItems(string list, string marker)
        {
            //// The listLevel global keeps track of when we're inside a list.
            //// Each time we enter a list, we increment it; when we leave a list,
            //// we decrement. If it's zero, we're not in a list anymore.

            //// We do this because when we're not inside a list, we want to treat
            //// something like this:

            ////    I recommend upgrading to version
            ////    8. Oops, now this line is treated
            ////    as a sub-list.

            //// As a single paragraph, despite the fact that the second line starts
            //// with a digit-period-space sequence.

            //// Whereas when we're inside a list (or sub-list), that line will be
            //// treated as the start of a sub-list. What a kludge, huh? This is
            //// an aspect of Markdown's syntax that's hard to parse perfectly
            //// without resorting to mind-reading. Perhaps the solution is to
            //// change the syntax rules such that sub-lists must start with a
            //// starting cardinal number; e.g. "1." or "a.".

            this._listLevel++;
            try
            {
                // Trim trailing blank lines:
                list = Regex.Replace(list, @"\n{2,}\z", "\n");

                string pattern = string.Format(
                                              @"(\n)?                      # leading line = $1
                                            (^[ ]*)                    # leading whitespace = $2
                                            ({0}) [ ]+                 # list marker = $3
                                            ((?s:.+?)                  # list item text = $4
                                            (\n{{1,2}}))      
                                            (?= \n* (\z | \2 ({0}) [ ]+))",
                                              marker);

                var regex = new Regex(pattern, RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline);
                var matches = regex.Matches(list);
                foreach (Match m in matches)
                {
                    yield return this.ListItemEvaluator(m);
                }
            }
            finally
            {
                this._listLevel--;
            }
        }

        private Block RuleEvaluator(Match match)
        {
            if (match == null)
            {
                throw new ArgumentNullException("match");
            }

            var line = new Line { X2 = 1, StrokeThickness = 1.0 };
            var container = new BlockUIContainer(line);
            return container;
        }

        /// <summary>
        /// Perform transformations that form block-level tags like paragraphs, headers, and list items.
        /// </summary>
        /// <param name="text">
        /// The text on which the Gamut should be run.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/> containing the changes.
        /// </returns>
        private IEnumerable<Block> RunBlockGamut(string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }

            return this.DoHeaders(text, s1 => this.DoHorizontalRules(s1, s2 => this.DoLists(s2, this.FormParagraphs)));

            // text = DoCodeBlocks(text);
            // text = DoBlockQuotes(text);

            //// We already ran HashHTMLBlocks() before, in Markdown(), but that
            //// was to escape raw HTML in the original Markdown source. This time,
            //// we're escaping the markup we've just created, so that we don't wrap
            //// <p> tags around block-level tags.
            // text = HashHTMLBlocks(text);

            // text = FormParagraphs(text);

            // return text;
        }

        /// <summary>
        /// Perform transformations that occur *within* block-level tags like paragraphs, headers, and list items.
        /// </summary>
        /// <param name="text">
        /// The text on which the Gamut should be run.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/> containing the changes..
        /// </returns>
        private IEnumerable<Inline> RunSpanGamut(string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }

            return this.DoCodeSpans(text, s0 => this.DoAnchors(s0, s1 => this.DoItalicsAndBold(s1, this.DoText)));

            // text = EscapeSpecialCharsWithinTagAttributes(text);
            // text = EscapeBackslashes(text);

            //// Images must come first, because ![foo][f] looks like an anchor.
            // text = DoImages(text);
            // text = DoAnchors(text);

            //// Must come after DoAnchors(), because you can use < and >
            //// delimiters in inline links like [this](<url>).
            // text = DoAutoLinks(text);

            // text = EncodeAmpsAndAngles(text);
            // text = DoItalicsAndBold(text);
            // text = DoHardBreaks(text);

            // return text;
        }

        private Block SetextHeaderEvaluator(Match match)
        {
            if (match == null)
            {
                throw new ArgumentNullException("match");
            }

            string header = match.Groups[1].Value;
            int level = match.Groups[2].Value.StartsWith("=", StringComparison.CurrentCulture) ? 1 : 2;

            // TODO: Style the paragraph based on the header level
            return this.CreateHeader(level, this.RunSpanGamut(header.Trim()));
        }
    }
}