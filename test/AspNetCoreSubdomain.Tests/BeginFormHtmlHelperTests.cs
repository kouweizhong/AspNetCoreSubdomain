using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestHelpers;
using Xunit;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
using System.IO;
using System.Buffers;
using System.Globalization;
using Microsoft.Extensions.WebEncoders.Testing;

namespace AspNetCoreSubdomain.Tests
{
    public class BeginFormHtmlHelperTests
    {
        [Theory]
        [MemberData(nameof(MemberDataFactories.AreaInSubdomainTestData.Generate), MemberType = typeof(MemberDataFactories.AreaInSubdomainTestData))]
        public void CanCreateAreaSubdomainBeginFormHtmlHelper(
            string host,
            string appRoot,
            string subdomain,
            string controller,
            string action,
            string expectedUrl)
        {
            // Arrange
            string expectedStartTag = $"<form action=\"HtmlEncode[[{expectedUrl}]]\" method=\"HtmlEncode[[post]]\">";
            var htmlHelper = ConfigurationFactories.HtmlHelperFactory.Get(routeBuilder =>
            {
                routeBuilder.MapSubdomainRoute(
                    new[] { "localhost" },
                    "default",
                    "{area}",
                    "{controller=Home}/{action=Index}");
            }, host, appRoot, controller, action, subdomain, expectedUrl);
            // Act
            var form = htmlHelper.BeginForm(
                                            actionName: action,
                                            controllerName: controller,
                                            routeValues: new { area = subdomain },
                                            method: FormMethod.Post,
                                            antiforgery: false,
                                            htmlAttributes: null);
            
            var writer = Assert.IsAssignableFrom<StringWriter>(htmlHelper.ViewContext.Writer);
            var builder = writer.GetStringBuilder();

            // Assert
            Assert.Equal(expectedStartTag, builder.ToString());
        }
        
        [Theory]
        [MemberData(nameof(MemberDataFactories.ControllerInSubdomainTestData.Generate), MemberType = typeof(MemberDataFactories.ControllerInSubdomainTestData))]
        public void CanCreateControllerSubdomainBeginFormHtmlHelper(
            string host,
            string appRoot,
            string subdomain,
            string action,
            string expectedUrl)
        {
            // Arrange
            string expectedStartTag = $"<form action=\"HtmlEncode[[{expectedUrl}]]\" method=\"HtmlEncode[[post]]\">";
            var htmlHelper = ConfigurationFactories.HtmlHelperFactory.Get(routeBuilder =>
            {
                routeBuilder.MapSubdomainRoute(
                    new[] { "localhost" },
                    "default",
                    "{controller}",
                    "{action=Index}");
            }, host, appRoot, subdomain, action, null, expectedUrl);

            // Act
            var form = htmlHelper.BeginForm(
                                            actionName: action,
                                            controllerName: subdomain);
            
            var writer = Assert.IsAssignableFrom<StringWriter>(htmlHelper.ViewContext.Writer);
            var builder = writer.GetStringBuilder();

            // Assert
            Assert.Equal(expectedStartTag, builder.ToString());
        }

        [Theory]
        [MemberData(nameof(MemberDataFactories.ConstantSubdomainTestData.Generate), MemberType = typeof(MemberDataFactories.ConstantSubdomainTestData))]
        public void CanCreateConstantActionLinkHtmlHelper(
            string host,
            string appRoot,
            string controller,
            string action,
            string expectedUrl)
        {
            // Arrange
            string expectedStartTag = $"<form action=\"HtmlEncode[[{expectedUrl}]]\" method=\"HtmlEncode[[post]]\">";
            var htmlHelper = ConfigurationFactories.HtmlHelperFactory.Get(routeBuilder =>
            {
                routeBuilder.MapSubdomainRoute(
                    new[] { "localhost" },
                    "default",
                    "constantsubdomain",
                    "{controller=Home}/{action=Index}");
            }, host, appRoot, controller, action, null, expectedUrl);

            // Act
            var form = htmlHelper.BeginForm(
                                            actionName: action,
                                            controllerName: controller);
            
            var writer = Assert.IsAssignableFrom<StringWriter>(htmlHelper.ViewContext.Writer);
            var builder = writer.GetStringBuilder();

            // Assert
            Assert.Equal(expectedStartTag, builder.ToString());
        }
    }
}
