using System;
using System.IO;
using System.Reflection;
using System.Web.Http;
using System.Xml.XPath;
using WebActivatorEx;
using WebApplication3;
using Swashbuckle.Application;
using Swashbuckle.Swagger.XmlComments;

[assembly: PreApplicationStartMethod(typeof(SwaggerConfig), "Register")]
namespace WebApplication3
{
    public class SwaggerConfig
    {
        public static void Register()
        {
            var thisAssembly = typeof(SwaggerConfig).Assembly;

            GlobalConfiguration.Configuration
                .EnableSwagger("docs/{apiVersion}/swagger", c =>
                {
                    c.SingleApiVersion("v1", "My API")
                     .Description("This API provides access to My APIs.");

                    // Set this flag to omit descriptions for any actions decorated with the Obsolete attribute
                    c.IgnoreObsoleteActions();

                    //c.Schemes(new[] { "https" });

                    var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                    var commentsFileName = Assembly.GetExecutingAssembly().GetName().Name + ".XML";
                    var commentsFile = Path.Combine(baseDirectory, "bin", commentsFileName);
                    c.IncludeXmlComments(commentsFile);
                    c.IgnoreObsoleteProperties();

                    c.GroupActionsBy(apiDesc =>
                    {
                        var controllerName = apiDesc.ActionDescriptor.ControllerDescriptor.ControllerName;
                        var controllerType = apiDesc.ActionDescriptor.ControllerDescriptor.ControllerType;
                        return $"{controllerName} : {GetResourceDescription(commentsFile, controllerType)}";
                    });
                    // In accordance with the built in JsonSerializer, Swashbuckle will, by default, describe enums as integers.
                    // You can change the serializer behavior by configuring the StringToEnumConverter globally or for a given
                    // enum type. Swashbuckle will honor this change out-of-the-box. However, if you use a different
                    // approach to serialize enums as strings, you can also force Swashbuckle to describe them as strings.
                    // 
                    //c.DescribeAllEnumsAsStrings();

                    // In contrast to WebApi, Swagger 2.0 does not include the query string component when mapping a URL
                    // to an action. As a result, Swashbuckle will raise an exception if it encounters multiple actions
                    // with the same path (sans query string) and HTTP method. You can workaround this by providing a
                    // custom strategy to pick a winner or merge the descriptions for the purposes of the Swagger docs 
                    //
                    //c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
                })
                .EnableSwaggerUi("docs/{*assetPath}", c =>
                {
                    c.DocExpansion(DocExpansion.List);
                    // Use the "InjectStylesheet" option to enrich the UI with one or more additional CSS stylesheets.
                    // The file must be included in your project as an "Embedded Resource", and then the resource's
                    // "Logical Name" is passed to the method as shown below.
                    //
                    c.InjectStylesheet(thisAssembly, "WebApplication3.docs.custom.css");

                    // Use the "InjectJavaScript" option to invoke one or more custom JavaScripts after the swagger-ui
                    // has loaded. The file must be included in your project as an "Embedded Resource", and then the resource's
                    // "Logical Name" is passed to the method as shown above.
                    //
                    //c.InjectJavaScript(thisAssembly, "Swashbuckle.Dummy.SwaggerExtensions.testScript1.js");

                    // The swagger-ui renders boolean data types as a dropdown. By default, it provides "true" and "false"
                    // strings as the possible choices. You can use this option to change these to something else,
                    // for example 0 and 1.
                    //
                    //c.BooleanValues(new[] { "0", "1" });

                    // By default, swagger-ui will validate specs against swagger.io's online validator and display the result
                    // in a badge at the bottom of the page. Use these options to set a different validator URL or to disable the
                    // feature entirely.
                    //c.SetValidatorUrl("http://localhost/validator");
                    //c.DisableValidator();


                    // Use the CustomAsset option to provide your own version of assets used in the swagger-ui.
                    // It's typically used to instruct Swashbuckle to return your version instead of the default
                    // when a request is made for "index.html". As with all custom content, the file must be included
                    // in your project as an "Embedded Resource", and then the resource's "Logical Name" is passed to
                    // the method as shown below.
                    //
                    c.CustomAsset("index", thisAssembly, "WebApplication3.docs.index.html");
                    c.CustomAsset("starting", thisAssembly, "WebApplication3.docs.starting.html");

                    // If your API has multiple versions and you've applied the MultipleApiVersions setting
                    // as described above, you can also enable a select box in the swagger-ui, that displays
                    // a discovery URL for each version. This provides a convenient way for users to browse documentation
                    // for different API versions.
                    //
                    //c.EnableDiscoveryUrlSelector();
                });
        }

        private static string GetResourceDescription(string xml, Type controllerType)
        {
            var navigator = new XPathDocument(xml).CreateNavigator();
            var id = XmlCommentsIdHelper.GetCommentIdForType(controllerType);
            var node = navigator.SelectSingleNode($"/doc/members/member[@name='{id}']");

            var summaryTag = node?.SelectSingleNode("summary");
            return summaryTag == null ? string.Empty : summaryTag.InnerXml;
        }
    }
}
