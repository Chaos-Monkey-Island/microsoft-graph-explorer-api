﻿using CodeSnippetsReflection.LanguageGenerators;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Csdl;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Xml;
using Xunit;

namespace CodeSnippetsReflection.Test
{
    public class CommonGeneratorShould
    {
        private const string ServiceRootUrl = "https://graph.microsoft.com/v1.0";
        private readonly IEdmModel _edmModel = CsdlReader.Parse(XmlReader.Create(ServiceRootUrl + "/$metadata"));

        #region Test GenerateQuerySection
        [Fact]
        public void GenerateQuerySection_ShouldReturnEmptyStringIfQueryListIsEmpty()
        {
            //Arrange
            LanguageExpressions expressions = new JavascriptExpressions();
            //no query present
            var requestPayload = new HttpRequestMessage(HttpMethod.Get, "https://graph.microsoft.com/v1.0/me/drive/root");
            var snippetModel = new SnippetModel(requestPayload, ServiceRootUrl, _edmModel);

            //Act
            var result = CommonGenerator.GenerateQuerySection(snippetModel, expressions);

            //Assert string is empty
            Assert.Equal("", result);
        }

        [Fact]
        public void GenerateQuerySection_ShouldReturnAppropiateJavascriptSelectExpression()
        {
            //Arrange
            LanguageExpressions expressions = new JavascriptExpressions();
            //no query present
            var requestPayload = new HttpRequestMessage(HttpMethod.Get, "https://graph.microsoft.com/v1.0/users/{id}?$select=displayName,givenName,postalCode");
            var snippetModel = new SnippetModel(requestPayload, ServiceRootUrl, _edmModel);

            //Act
            var result = CommonGenerator.GenerateQuerySection(snippetModel, expressions);

            //Assert string is empty
            Assert.Equal("\n\t.select('displayName,givenName,postalCode')", result);
        }

        [Fact]
        public void GenerateQuerySection_ShouldReturnAppropiateJavascriptFilterExpression()
        {
            //Arrange
            LanguageExpressions expressions = new JavascriptExpressions();
            //no query present
            var requestPayload = new HttpRequestMessage(HttpMethod.Get, "https://graph.microsoft.com/v1.0/users?$filter=startswith(givenName, 'J')");
            var snippetModel = new SnippetModel(requestPayload, ServiceRootUrl, _edmModel);

            //Act
            var result = CommonGenerator.GenerateQuerySection(snippetModel, expressions);

            //Assert string is empty
            Assert.Equal("\n\t.filter('startswith(givenName, 'J')')", result);
        }

        [Fact]
        public void GenerateQuerySection_ShouldReturnAppropiateJavascriptSearchExpression()
        {
            //Arrange
            LanguageExpressions expressions = new JavascriptExpressions();
            //no query present
            var requestPayload = new HttpRequestMessage(HttpMethod.Get, "https://graph.microsoft.com/v1.0/me/people/?$search=\"Irene McGowen\"");
            var snippetModel = new SnippetModel(requestPayload, ServiceRootUrl, _edmModel);

            //Act
            var result = CommonGenerator.GenerateQuerySection(snippetModel, expressions);

            //Assert string is empty
            Assert.Equal("\n\t.search('Irene McGowen')", result);
        }

        [Fact]
        public void GenerateQuerySection_ShouldReturnAppropiateJavascriptSkipExpression()
        {
            //Arrange
            LanguageExpressions expressions = new JavascriptExpressions();
            //no query present
            var requestPayload = new HttpRequestMessage(HttpMethod.Get, "https://graph.microsoft.com/v1.0/me/events?$skip=20");
            var snippetModel = new SnippetModel(requestPayload, ServiceRootUrl, _edmModel);

            //Act
            var result = CommonGenerator.GenerateQuerySection(snippetModel, expressions);

            //Assert string is empty
            Assert.Equal("\n\t.skip(20)", result);
        }

        [Fact]
        public void GenerateQuerySection_ShouldReturnAppropiateJavascriptTopExpression()
        {
            //Arrange
            LanguageExpressions expressions = new JavascriptExpressions();
            //no query present
            var requestPayload = new HttpRequestMessage(HttpMethod.Get, "https://graph.microsoft.com/v1.0/me/events?$top=5");
            var snippetModel = new SnippetModel(requestPayload, ServiceRootUrl, _edmModel);

            //Act
            var result = CommonGenerator.GenerateQuerySection(snippetModel, expressions);

            //Assert string is empty
            Assert.Equal("\n\t.top(5)", result);
        }

        [Fact]
        public void GenerateQuerySection_ShouldReturnAppropiateJavascriptRequestHeaderExpression()
        {
            //Arrange
            LanguageExpressions expressions = new JavascriptExpressions();
            //no query present
            var requestPayload = new HttpRequestMessage(HttpMethod.Get, "https://graph.microsoft.com/v1.0/users/{id}");
            requestPayload.Headers.Add("Prefer", "kenya-timezone");

            var snippetModel = new SnippetModel(requestPayload, ServiceRootUrl, _edmModel);

            //Act
            var result = CommonGenerator.GenerateQuerySection(snippetModel, expressions);

            //Assert string is empty
            Assert.Equal("\n\t.header('Prefer','kenya-timezone')", result);
        }

        #endregion

        #region Test GetListAsStringForSnippet
        [Fact]
        public void GetListAsStringForSnippet_ShouldReturnEmptyStringIfFieldListEmpty()
        {
            //Arrange
            List<string> fieldList = new List<string>();

            //Act
            var result = CommonGenerator.GetListAsStringForSnippet(fieldList,",");

            //Assert
            Assert.Equal("",result);
        }

        [Fact]
        public void GetListAsStringForSnippet_ShouldReturnCommaSeparatedStringWithCommaDelimiter()
        {
            //Arrange
            List<string> fieldList = new List<string>
            {
                "Test",
                "Test2",
                "Test3"
            };

            //Act
            var result = CommonGenerator.GetListAsStringForSnippet(fieldList, ",");

            //Assert
            Assert.Equal("Test,Test2,Test3", result);
        }

        [Fact]
        public void GetListAsStringForSnippet_ShouldReturnUndelimitedStringWithEmptyDelimiter()
        {
            //Arrange
            List<string> fieldList = new List<string>
            {
                "Test",
                "Test2",
                "Test3"
            };

            //Act
            var result = CommonGenerator.GetListAsStringForSnippet(fieldList, "");

            //Assert
            Assert.Equal("TestTest2Test3", result);
        }
        #endregion

        #region Test GetClassNameFromIdentifier
        [Fact]
        public void GetClassNameFromIdentifier_ShouldReturnRootIdentifierOnFirstSearch()
        {
            //Arrange
            List<string> path = new List<string>
            {
                "people"//last item so search for classname
            };

            var requestPayload = new HttpRequestMessage(HttpMethod.Get, "https://graph.microsoft.com/v1.0/me/people");
            var snippetModel = new SnippetModel(requestPayload, ServiceRootUrl, _edmModel);

            //Act
            var result = CommonGenerator.GetClassNameFromIdentifier(snippetModel.Segments.Last(), path);

            //Assert
            Assert.Equal("microsoft.graph.person", result);
        }

        [Fact]
        public void GetClassNameFromIdentifier_ShouldReturnParameterTypeForActionOrFunction()
        {
            //Arrange
            List<string> path = new List<string>
            {
                "message"//last item so search for classname
            };

            var requestPayload = new HttpRequestMessage(HttpMethod.Post, "https://graph.microsoft.com/v1.0/me/sendMail");
            var snippetModel = new SnippetModel(requestPayload, ServiceRootUrl, _edmModel);

            //Act
            var result = CommonGenerator.GetClassNameFromIdentifier(snippetModel.Segments.Last(), path);

            //Assert
            Assert.Equal("microsoft.graph.message", result);
        }

        [Fact]
        public void GetClassNameFromIdentifier_ShouldSearchForOneLevelNestedType()
        {
            //Arrange
            List<string> path = new List<string>
            {
                "messages",
                "toRecipients"//under the message entity there is a toReceipient entity
            };

            var requestPayload = new HttpRequestMessage(HttpMethod.Post, "https://graph.microsoft.com/v1.0/me/messages");
            var snippetModel = new SnippetModel(requestPayload, ServiceRootUrl, _edmModel);

            //Act
            var result = CommonGenerator.GetClassNameFromIdentifier(snippetModel.Segments.Last(), path);

            //Assert
            Assert.Equal("microsoft.graph.recipient", result);

        }

        [Fact]
        public void GetClassNameFromIdentifier_ShouldSearchForOneLevelNestedType_2()
        {
            //Arrange
            List<string> path = new List<string>
            {
                "messages",
                "body"//under the message entity there is a toReceipient entity
            };

            var requestPayload = new HttpRequestMessage(HttpMethod.Post, "https://graph.microsoft.com/v1.0/me/messages");
            var snippetModel = new SnippetModel(requestPayload, ServiceRootUrl, _edmModel);

            //Act
            var result = CommonGenerator.GetClassNameFromIdentifier(snippetModel.Segments.Last(), path);

            //Assert
            Assert.Equal("microsoft.graph.itemBody", result);

        }

        [Fact]
        public void GetClassNameFromIdentifier_ShouldSearchForTwoLevelNestedType()
        {
            //Arrange
            List<string> path = new List<string>
            {
                "messages",
                "toRecipients",//under the message entity there is a toReceipient entity
                "emailAddress"//under the toReceipient there is an email address property
            };

            var requestPayload = new HttpRequestMessage(HttpMethod.Post, "https://graph.microsoft.com/v1.0/me/messages");
            var snippetModel = new SnippetModel(requestPayload, ServiceRootUrl, _edmModel);

            //Act
            var result = CommonGenerator.GetClassNameFromIdentifier(snippetModel.Segments.Last(), path);

            //Assert
            Assert.Equal("microsoft.graph.emailAddress", result);
        }

        #endregion
    }
}
