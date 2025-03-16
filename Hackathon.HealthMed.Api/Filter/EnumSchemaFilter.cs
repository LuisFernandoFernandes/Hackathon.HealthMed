using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Hackathon.HealthMed.Api.Filter;

public class EnumSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type.IsEnum)
        {
            var names = Enum.GetNames(context.Type);
            var values = Enum.GetValues(context.Type).Cast<int>().ToArray();

            string descricaoValores = string.Join("<br>", values
                .Select((val, idx) => $"{{ \"{val}\": \"{names[idx]}\" }}"));

            schema.Description = descricaoValores;
        }
    }
}
