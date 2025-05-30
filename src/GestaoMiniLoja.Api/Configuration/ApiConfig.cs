﻿namespace GestaoMiniLoja.Api.Configuration
{
    public static class ApiConfig
    {
        public static WebApplicationBuilder AddApiConfig(this WebApplicationBuilder builder)
        {
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddControllers()
                            .ConfigureApiBehaviorOptions(options =>
                            {
                                options.SuppressModelStateInvalidFilter = true;
                            });
            return builder;
        }
    }
}
