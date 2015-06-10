﻿#region Copyright
// 
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2014
// by DotNetNuke Corporation
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
// to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.

#endregion
#region Usings
using System;
using System.IO;

using DotNetNuke.Entities.Controllers;

using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;


#endregion
namespace DotNetNuke.Services.Search.Internals
{
    /// <summary>
    /// This is responsible for the filters chain that analyzes search documents/queries
    /// </summary>
    internal class SearchQueryAnalyzer : Analyzer
    {
        private readonly bool _useStemmingFilter;

        public SearchQueryAnalyzer(bool useStemmingFilter)
        {
            _useStemmingFilter = useStemmingFilter;
        }

        public override TokenStream TokenStream(string fieldName, TextReader reader)
        {
            var wordLengthMinMax = SearchHelper.Instance.GetSearchMinMaxLength();

            //Note: the order of filtering is important for both operation and performane, so we try to make it work faster
            // Also, note that filters are applied from the innermost outwards.
            var filter =
                    new ASCIIFoldingFilter( // accents filter
                        new LowerCaseFilter(
                            new LengthFilter(
                                new StandardFilter(
                                    new StandardTokenizer(Constants.LuceneVersion, reader)
                                )
                            , wordLengthMinMax.Item1, wordLengthMinMax.Item2)
                        )
                    );

            if (!_useStemmingFilter)
                return filter;

            return new PorterStemFilter(filter);
        }
    }
}