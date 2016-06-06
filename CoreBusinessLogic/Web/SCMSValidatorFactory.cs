using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using FluentValidation;
using FluentValidation.Attributes;
using FluentValidation.Internal;

namespace SCMS.CoreBusinessLogic.Web
{
    public class SCMSValidatorFactory : AttributedValidatorFactory
    {

        private readonly InstanceCache m_Cache = new InstanceCache();

        public override IValidator GetValidator(Type type)
        {
            if (type != null)
            {
                var attribute = (ValidatorAttribute)Attribute.GetCustomAttribute(type, typeof(ValidatorAttribute));
                if ((attribute != null) && (attribute.ValidatorType != null))
                {
                    var instance = m_Cache.GetOrCreateInstance(attribute.ValidatorType,
                                               x => DependencyResolver.Current.ResolveUnregistered(x));
                    return instance as IValidator;
                }
            }
            return null;

        }
    }
}
