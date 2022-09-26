using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace MyProfile {

    public class CheckAuthMiddleWare {
        private readonly RequestDelegate _next;

        public CheckAuthMiddleWare(RequestDelegate next) {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context) {
            context.Request.Cookies.TryGetValue("UserId", out string idUserStr);
            if (!string.IsNullOrEmpty(idUserStr)) {
                Startup.userId = Convert.ToInt32(idUserStr);
                Startup.isAuth = true;
            } else {
                Startup.userId = 0;
            }
            
            await _next.Invoke(context);
        }
    }
}
