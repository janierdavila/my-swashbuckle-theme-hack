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
                   
                })
                .EnableSwaggerUi("docs/{*assetPath}", c =>
                {
                    c.DocExpansion(DocExpansion.List);
                    
                    c.InjectStylesheet(thisAssembly, "WebApplication3.docs.custom.css");
                    c.CustomAsset("index", thisAssembly, "WebApplication3.docs.index.html");
                    c.CustomAsset("starting", thisAssembly, "WebApplication3.docs.starting.html");
                    
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
