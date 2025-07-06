using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using MudBlazor6.Components;
using MudBlazor6.Models;
using MudBlazor6.Services;

namespace MudBlazor6
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();
            builder.Services.AddMudServices();
            builder.Services.AddDbContextFactory<CJDataContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
            
            // следующие строки регистрируют необходимые сервисы для аутентификации и авторизации пользователя
            builder.Services.AddScoped<ControlJobUserService>();
            builder.Services.AddScoped<ControlJobAuthenticationStateProvider>();
            builder.Services.AddScoped<AuthenticationStateProvider>(sp=>sp.GetRequiredService<ControlJobAuthenticationStateProvider>());
            //builder.Services.AddAuthentication("ControlJob");
            
            
            var app = builder.Build();

            

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();
            app.UseAntiforgery();

            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            app.Run();
        }
    }
}
