using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using MFB.API.Shared.Middleware.SMB;

namespace System.Web
{
    public static class HttpContext
    {
        private static IHttpContextAccessor m_httpContextAccessor;

        public static void Configure(IHttpContextAccessor httpContextAccessor)
        {
            m_httpContextAccessor = httpContextAccessor;
        }

        public static Microsoft.AspNetCore.Http.HttpContext Current
        {
            get
            {
                if (m_httpContextAccessor == null)
                    return null;
                else
                    return m_httpContextAccessor.HttpContext;
            }
        }

        public static void UseEventBus(this IApplicationBuilder app)
        {
            if (Current == null)
            {
                IHttpContextAccessor m_httpContextAccessor = app.ApplicationServices.GetRequiredService<IHttpContextAccessor>();
                Configure(m_httpContextAccessor);
                EventChannelBus.SetContext(m_httpContextAccessor);
            }

            app.Use(async (context, next) =>
            {
                try
                {
                    await next.Invoke();
                }
                finally
                {
                    EventChannelBus.DeleteChannels(context.TraceIdentifier);
                }
            });
        }
    }
}