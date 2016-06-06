using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using SCMS.CoreBusinessLogic;

namespace System.Web.Mvc
{
    public static class IDependencyResolveExtensions
    {
        public static T Resolve<T>(this IDependencyResolver container)
        {
            return container.GetService<T>();
        }

        public static object ResolveUnregistered(this IDependencyResolver container, Type type)
        {
            var constructors = type.GetConstructors();
            foreach (var constructor in constructors)
            {
                try
                {
                    var parameters = constructor.GetParameters();
                    var parameterInstances = new List<object>();
                    foreach (var parameter in parameters)
                    {
                        var service = container.GetService(parameter.ParameterType);
                        if (service == null) throw new SCMSException("Unkown dependency");
                        parameterInstances.Add(service);
                    }
                    
                    return Activator.CreateInstance(type, parameterInstances.ToArray());
                }
                catch (SCMSException)
                {

                }
            }
            throw new SCMSException("No contructor was found that had all the dependencies satisfied.");
        }
    }
}
