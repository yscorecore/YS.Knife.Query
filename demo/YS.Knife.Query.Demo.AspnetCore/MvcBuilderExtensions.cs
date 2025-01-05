using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace YS.Knife.Query.Demo.AspnetCore
{
    public static class MvcBuilderExtensions
    {
        public static IMvcBuilder AddFilterJson(this IMvcBuilder mvcBuilder)
        {
            mvcBuilder.Services.AddSingleton<IConfigureOptions<MvcOptions>>(sp =>
            {
                var originalJsonOptions = sp.GetRequiredService<IOptionsMonitor<JsonOptions>>();
                var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
                return new ConfigMvcJsonOptions(loggerFactory, originalJsonOptions);
            });
            return mvcBuilder;
        }
    }
    public class ConfigMvcJsonOptions : IConfigureOptions<MvcOptions>
    {
        private readonly ILoggerFactory loggerFactory;
        private readonly IOptionsMonitor<JsonOptions> jsonOptions;

        public ConfigMvcJsonOptions(ILoggerFactory loggerFactory,IOptionsMonitor<JsonOptions> jsonOptions)
        {
            this.loggerFactory = loggerFactory;
            this.jsonOptions = jsonOptions;
        }
        public void Configure(MvcOptions options)
        {
            var optionValue = jsonOptions.CurrentValue;
            options.OutputFormatters.Insert(0, new FilterColumnJsonFormatter(optionValue.JsonSerializerOptions));
        }
    }
}
