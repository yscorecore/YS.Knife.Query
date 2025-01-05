using System.Runtime.ExceptionServices;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;

namespace YS.Knife.Query.Demo.AspnetCore
{
    internal static class MediaTypeHeaderValues
    {
        public static readonly MediaTypeHeaderValue ApplicationJson
            = MediaTypeHeaderValue.Parse("application/json").CopyAsReadOnly();

        public static readonly MediaTypeHeaderValue TextJson
            = MediaTypeHeaderValue.Parse("text/json").CopyAsReadOnly();

        public static readonly MediaTypeHeaderValue ApplicationAnyJsonSyntax
            = MediaTypeHeaderValue.Parse("application/*+json").CopyAsReadOnly();

        public static readonly MediaTypeHeaderValue ApplicationXml
            = MediaTypeHeaderValue.Parse("application/xml").CopyAsReadOnly();

        public static readonly MediaTypeHeaderValue TextXml
            = MediaTypeHeaderValue.Parse("text/xml").CopyAsReadOnly();

        public static readonly MediaTypeHeaderValue ApplicationAnyXmlSyntax
            = MediaTypeHeaderValue.Parse("application/*+xml").CopyAsReadOnly();
    }

    /// <summary>
    /// A <see cref="TextOutputFormatter"/> for JSON content that uses <see cref="JsonSerializer"/>.
    /// </summary>
    public class SystemTextJsonOutputFormatter2 : TextOutputFormatter
    {
        /// <summary>
        /// Initializes a new <see cref="SystemTextJsonOutputFormatter"/> instance.
        /// </summary>
        /// <param name="jsonSerializerOptions">The <see cref="JsonSerializerOptions"/>.</param>
        public SystemTextJsonOutputFormatter2(JsonSerializerOptions jsonSerializerOptions)
        {
            SerializerOptions = jsonSerializerOptions;

            SupportedEncodings.Add(Encoding.UTF8);
            SupportedEncodings.Add(Encoding.Unicode);
            SupportedMediaTypes.Add(MediaTypeHeaderValues.ApplicationJson);
            SupportedMediaTypes.Add(MediaTypeHeaderValues.TextJson);
            SupportedMediaTypes.Add(MediaTypeHeaderValues.ApplicationAnyJsonSyntax);
        }

        internal static SystemTextJsonOutputFormatter CreateFormatter(JsonOptions jsonOptions)
        {
            var jsonSerializerOptions = jsonOptions.JsonSerializerOptions;

            //jsonSerializerOptions.
            if (jsonSerializerOptions.Encoder is null)
            {
                // If the user hasn't explicitly configured the encoder, use the less strict encoder that does not encode all non-ASCII characters.
                jsonSerializerOptions = new JsonSerializerOptions(jsonSerializerOptions)
                {
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                };
            }

            return new SystemTextJsonOutputFormatter(jsonSerializerOptions);
        }

        /// <summary>
        /// Gets the <see cref="JsonSerializerOptions"/> used to configure the <see cref="JsonSerializer"/>.
        /// </summary>
        /// <remarks>
        /// A single instance of <see cref="SystemTextJsonOutputFormatter"/> is used for all JSON formatting. Any
        /// changes to the options will affect all output formatting.
        /// </remarks>
        public JsonSerializerOptions SerializerOptions { get; }

        /// <inheritdoc />
        public sealed override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (selectedEncoding == null)
            {
                throw new ArgumentNullException(nameof(selectedEncoding));
            }

            var httpContext = context.HttpContext;

            // context.ObjectType reflects the declared model type when specified.
            // For polymorphic scenarios where the user declares a return type, but returns a derived type,
            // we want to serialize all the properties on the derived type. This keeps parity with
            // the behavior you get when the user does not declare the return type and with Json.Net at least at the top level.
            var objectType = context.Object?.GetType() ?? context.ObjectType ?? typeof(object);

            var responseStream = httpContext.Response.Body;
            if (selectedEncoding.CodePage == Encoding.UTF8.CodePage)
            {
                await JsonSerializer.SerializeAsync(responseStream, context.Object, objectType, GetJsonSerializerOptions(context));
                await responseStream.FlushAsync();
            }
            else
            {
                // JsonSerializer only emits UTF8 encoded output, but we need to write the response in the encoding specified by
                // selectedEncoding
                var transcodingStream = Encoding.CreateTranscodingStream(httpContext.Response.Body, selectedEncoding, Encoding.UTF8, leaveOpen: true);

                ExceptionDispatchInfo exceptionDispatchInfo = null;
                try
                {
                    await JsonSerializer.SerializeAsync(transcodingStream, context.Object, objectType, GetJsonSerializerOptions(context));
                    await transcodingStream.FlushAsync();
                }
                catch (Exception ex)
                {
                    // TranscodingStream may write to the inner stream as part of it's disposal.
                    // We do not want this exception "ex" to be eclipsed by any exception encountered during the write. We will stash it and
                    // explicitly rethrow it during the finally block.
                    exceptionDispatchInfo = ExceptionDispatchInfo.Capture(ex);
                }
                finally
                {
                    try
                    {
                        await transcodingStream.DisposeAsync();
                    }
                    catch when (exceptionDispatchInfo != null)
                    {
                    }

                    exceptionDispatchInfo?.Throw();
                }
            }
        }

        protected virtual JsonSerializerOptions GetJsonSerializerOptions(OutputFormatterWriteContext context)
        {
            var converters = GetDynamicConverters(context.HttpContext);
            if (converters == null || converters.Length == 0)
            {
                return this.SerializerOptions;
            }
            else
            {
                var newSerializerOptions = new JsonSerializerOptions(this.SerializerOptions);
                Array.ForEach(converters, p => newSerializerOptions.Converters.Add(p));
                
                return newSerializerOptions;
            }
        }
        private JsonConverter[] GetDynamicConverters(HttpContext context)
        {
            //TODO
            return null;
        }

        private SelectInfo GetSelectInfo(HttpContext context)
        {
            var text = context.Items["__query_select"] as string;
            if (string.IsNullOrEmpty(text))
            {
                return null;
            }
            else
            {
                return SelectInfo.Parse(text);
            }
        }
    }

    public class FilterColumnJsonFormatter : SystemTextJsonOutputFormatter
    {
        public FilterColumnJsonFormatter(JsonSerializerOptions jsonSerializerOptions) : base(jsonSerializerOptions)
        {
        }
        public override bool CanWriteResult(OutputFormatterCanWriteContext context)
        {
            return base.CanWriteResult(context);
        }
        protected override bool CanWriteType(Type type)
        {
            return base.CanWriteType(type);
        }
        public override Task WriteAsync(OutputFormatterWriteContext context)
        {
            var selectInfo = GetSelectInfo(context.HttpContext);
            return base.WriteAsync(context);
        }
        private SelectInfo GetSelectInfo(HttpContext context)
        {
            var text = context.Items["__query_select"] as string;
            if (string.IsNullOrEmpty(text))
            {
                return null;
            }
            else
            {
                return SelectInfo.Parse(text);
            }
        }
    }
}
