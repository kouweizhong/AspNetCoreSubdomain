﻿using Microsoft.AspNetCore.Builder;
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
    public class ActionLinkHtmlHelperTests
    {
        [Theory]
        [MemberData(nameof(MemberDataFactories.AreaInSubdomainTestData.Generate), MemberType = typeof(MemberDataFactories.AreaInSubdomainTestData))]
        public void CanCreateAreaSubdomainActionLinkHtmlHelper(
            string host,
            string appRoot,
            string subdomain,
            string controller,
            string action,
            string expectedUrl)
        {
            // Arrange
            string expectedLink = $"<a href=\"HtmlEncode[[{expectedUrl}]]\">HtmlEncode[[Test]]</a>";
            var htmlHelper = ConfigurationFactories.HtmlHelperFactory.Get(routeBuilder =>
            {
                routeBuilder.MapSubdomainRoute(
                    new[] { "localhost" },
                    "default",
                    "{area}",
                    "{controller=Home}/{action=Index}");
            }, host, appRoot, controller, action, subdomain, expectedUrl);
            // Act
            var actualLink = htmlHelper.ActionLink(
                                            linkText: "Test",
                                            actionName: action,
                                            controllerName: controller,
                                            routeValues: new { area = subdomain });
            string resultHtml;
            using (var writer = new StringWriter())
            {
                actualLink.WriteTo(writer, new HtmlTestEncoder());
                resultHtml = writer.ToString();
            }

            // Assert
            Assert.Equal(expectedLink, resultHtml);
        }
        
        [Theory]
        [MemberData(nameof(MemberDataFactories.ControllerInSubdomainTestData.Generate), MemberType = typeof(MemberDataFactories.ControllerInSubdomainTestData))]
        public void CanCreateControllerSubdomainActionLinkHtmlHelper(
            string host,
            string appRoot,
            string subdomain,
            string action,
            string expectedUrl)
        {
            // Arrange
            string expectedLink = $"<a href=\"HtmlEncode[[{expectedUrl}]]\">HtmlEncode[[Test]]</a>";
            var htmlHelper = ConfigurationFactories.HtmlHelperFactory.Get(routeBuilder =>
            {
                routeBuilder.MapSubdomainRoute(
                    new[] { "localhost" },
                    "default",
                    "{controller}",
                    "{action=Index}");
            }, host, appRoot, subdomain, action, null, expectedUrl);
            // Act
            var actualLink = htmlHelper.ActionLink(
                                            linkText: "Test",
                                            actionName: action,
                                            controllerName: subdomain);
            string resultHtml;
            using (var writer = new StringWriter())
            {
                actualLink.WriteTo(writer, new HtmlTestEncoder());
                resultHtml = writer.ToString();
            }

            // Assert
            Assert.Equal(expectedLink, resultHtml);
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
            string expectedLink = $"<a href=\"HtmlEncode[[{expectedUrl}]]\">HtmlEncode[[Test]]</a>";
            var htmlHelper = ConfigurationFactories.HtmlHelperFactory.Get(routeBuilder =>
            {
                routeBuilder.MapSubdomainRoute(
                    new[] { "localhost" },
                    "default",
                    "constantsubdomain",
                    "{controller=Home}/{action=Index}");
            }, host, appRoot, controller, action, null, expectedUrl);
            // Act
            var actualLink = htmlHelper.ActionLink(
                                            linkText: "Test",
                                            actionName: action,
                                            controllerName: controller);
            string resultHtml;
            using (var writer = new StringWriter())
            {
                actualLink.WriteTo(writer, new HtmlTestEncoder());
                resultHtml = writer.ToString();
            }

            // Assert
            Assert.Equal(expectedLink, resultHtml);
        }
    }
}
