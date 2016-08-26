using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using WebClimbing.Models;
using System.Xml.Serialization;

namespace WebClimbing
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            /*config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );*/
            config.Routes.MapHttpRoute(
                name: "DefaultActionApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            /*config.Routes.MapHttpRoute(
            name: "PostApi",
            routeTemplate: "api/{controller}/{action}"
            );*/


            config.Formatters.Remove(config.Formatters.JsonFormatter);
            var xmlFormatter = config.Formatters.XmlFormatter;
            xmlFormatter.UseXmlSerializer = true;
            /*var xSer = new XmlSerializer(typeof(Comp_AgeGroupApiModel));
            xmlFormatter.SetSerializer<Comp_AgeGroupApiModel>(new XmlSerializer(typeof(Comp_AgeGroupApiModel)));
            xmlFormatter.SetSerializer<API_AgeGroupCollection>(new XmlSerializer(typeof(API_AgeGroupCollection)));*/
        }
    }
}
