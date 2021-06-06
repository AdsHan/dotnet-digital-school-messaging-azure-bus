﻿
using MED.Core.Extensions;
using MED.Identidade.Domain.Entities;
using MED.Identidade.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MED.Identidade.API.Configuration
{
    public static class DependencyInjectionConfig
    {
        public static void AddDependencyConfiguration(this IServiceCollection services, IConfiguration configuration)
        {

            // Usando com SqlServer
            services.AddDbContext<IdentidadeDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<UsuarioModel, IdentityRole>(options =>
                {
                    // Configurações de senha
                    options.SignIn.RequireConfirmedAccount = false;
                    options.Password.RequireDigit = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequiredLength = 2;
                    options.Password.RequiredUniqueChars = 0;
                })
                .AddErrorDescriber<IdentityPortugues>()
                .AddEntityFrameworkStores<IdentidadeDbContext>()
                .AddDefaultTokenProviders();

            // Usando com banco de dados em memória
            //services.AddDbContext<AlunoDbContext>(options => options.UseInMemoryDatabase("MinhaEscolaDigitalMonolito"));

            //services.AddScoped<IAlunoRepository, AlunoRepository>();

            //services.AddScoped<IMediatorHandler, MediatorHandler>();

            // services.AddScoped<IRequestHandler<AdicionarAlunoCommand, ValidationResult>, AlunoCommandHandler>();
            // services.AddScoped<IRequestHandler<AlterarEnderecoAlunoCommand, ValidationResult>, AlunoCommandHandler>();
            //services.AddMediatR(typeof(AdicionarAlunoCommand));
        }
    }
}