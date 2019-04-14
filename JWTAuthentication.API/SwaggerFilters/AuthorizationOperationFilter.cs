using Swashbuckle.Swagger;
using System.Collections.Generic;
using System.Web.Http.Description;

namespace JWTAuthentication.API.SwaggerFilters
{
    public class AuthorizationOperationFilter : IOperationFilter
    {
        public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            //for get actions without parameters, add or init it with an empty collection of parameters
            if(operation.parameters == null)
            {
                operation.parameters = new List<Parameter>();
            }

            operation.parameters.Add(
                new Parameter
                {
                    type="string",
                    name="Authorization",
                    @in="header",
                    description="Authorization header holding the access_token",
                    required=false
                });
        }
    }
}