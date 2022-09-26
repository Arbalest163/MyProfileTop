using Microsoft.AspNetCore.Http;

using MyProfile.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyProfile {

    public class CheckAuthMiddleWare {
        private readonly RequestDelegate _next;

        public CheckAuthMiddleWare(RequestDelegate next) {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context) {
            string idUserStr;
            context.Request.Cookies.TryGetValue("UserId", out idUserStr);
            if (idUserStr != null) {
                Startup.userId = Convert.ToInt32(idUserStr);
                Startup.isAuth = true;
            } else {
                Startup.userId = 0;
            }
            
            await _next.Invoke(context);
        }
    }
}
