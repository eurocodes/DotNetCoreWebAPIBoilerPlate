using Microsoft.Extensions.DependencyInjection;
using NetCore.AutoRegisterDi;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.DI {
    public static class Resolve {
        public static void resolve(this IServiceCollection services) {
            var result = services.RegisterAssemblyPublicNonGenericClasses()
              .AsPublicImplementedInterfaces();
        }
    }
}
