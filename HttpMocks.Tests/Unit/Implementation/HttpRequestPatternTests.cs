﻿using FluentAssertions;
using HttpMocks.Implementation;
using HttpMocks.Whens.RequestPatterns;
using HttpMocks.Whens.RequestPatterns.ContentPatterns;
using HttpMocks.Whens.RequestPatterns.HeadersPatterns;
using HttpMocks.Whens.RequestPatterns.MethodPatterns;
using HttpMocks.Whens.RequestPatterns.PathPatterns;
using HttpMocks.Whens.RequestPatterns.QueryPatterns;
using NUnit.Framework;

namespace HttpMocks.Tests.Unit.Implementation
{
    public class HttpRequestPatternTests : UnitTest
    {
        [Test]
        [TestCase("GET", "GET", true)]
        [TestCase("GET", "get", true)]
        [TestCase("GET", "gEt", true)]
        [TestCase("GET", "gEtd", false)]
        public void TestIsMatch(string method, string expectedMethod, bool expected)
        {
            const string pathPattern = "/";
            var httpRequestMock = new HttpRequestMock
            {
                Method = MethodPattern.Standart(method),
                Path = PathPattern.Smart(pathPattern),
                Query = QueryPattern.Any(),
                Headers = HeadersPattern.Any(),
                Content = ContentPattern.Any()
            };
            var httpRequestPattern = new HttpRequestPattern(httpRequestMock);
            var httpRequestInfo = new HttpRequestInfo
            {
                Method = expectedMethod,
                Path = pathPattern
            };
            httpRequestPattern.IsMatch(httpRequestInfo).ShouldBeEquivalentTo(expected);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void TestIsMatchWithContentPattern(bool isMatchResult)
        {
            var contnetPatternMock = NewMock<IHttpRequestContentPattern>();
            const string method = "post";
            const string path = "/";
            var contentBytes = new byte[0];
            const string contentType = "application/json";

            contnetPatternMock.Setup(x => x.IsMatch(contentBytes, contentType)).Returns(isMatchResult);

            var httpRequestMock = new HttpRequestMock
            {
                Method = MethodPattern.Standart(method),
                Path = PathPattern.Smart(path),
                Content = contnetPatternMock.Object,
                Query = QueryPattern.Any(),
                Headers = HeadersPattern.Any()
            };
            var httpRequestPattern = new HttpRequestPattern(httpRequestMock);
            var httpRequestInfo = new HttpRequestInfo
            {
                Method = method,
                Path = path,
                ContentBytes = contentBytes,
                ContentType = contentType
            };
            
            httpRequestPattern.IsMatch(httpRequestInfo).ShouldBeEquivalentTo(isMatchResult);
        }
    }
}