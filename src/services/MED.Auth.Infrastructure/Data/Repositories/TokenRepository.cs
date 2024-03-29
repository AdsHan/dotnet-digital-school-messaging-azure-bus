﻿using MED.Auth.Domain.Entities;
using MED.Auth.Domain.Repositories;
using MED.Core.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MED.Auth.Infrastructure.Data.Repositories
{
    public class TokenRepository : ITokenRepository
    {
        private readonly AuthDbContext _dbContext;
        private readonly IConfiguration _configuration;

        public TokenRepository(AuthDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }

        public async Task<TokenModel> GetByUserNameAsync(string userName)
        {
            return await _dbContext.Tokens
                .Where(a => a.Status == EntityStatusEnum.Ativa)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.UserName == userName);
        }

        public async Task<List<TokenModel>> GetAllAsync()
        {
            return await _dbContext.Tokens
                .Where(a => a.Status == EntityStatusEnum.Ativa)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<TokenModel> GetByIdAsync(Guid id)
        {
            return await _dbContext.Tokens
                .Where(a => a.Status == EntityStatusEnum.Ativa)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public void Update(TokenModel token)
        {
            // Reforço que as entidades foram alteradas
            _dbContext.Entry(token).State = EntityState.Modified;
            _dbContext.Update(token);
        }

        public void Add(TokenModel token)
        {
            _dbContext.Add(token);
        }

        public async Task SaveAsync()
        {
            await _dbContext.SaveChangesAsync();
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }

        public string GenerateToken(string UserName)
        {
            // Define as claims do usuário (não é obrigatório, mas melhora a segurança (cria mais chaves no Payload))
            var claims = new[]
            {
                 new Claim(JwtRegisteredClaimNames.UniqueName, UserName),
                 new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
             };

            // Gera uma chave
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:key"]));

            // Gera a assinatura digital do token
            var credenciais = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Tempo de expiracão do token
            var expiracao = _configuration["TokenConfiguration:ExpireHours"];
            var expiration = DateTime.UtcNow.AddHours(double.Parse(expiracao));

            // Gera o token
            JwtSecurityToken token = new JwtSecurityToken(
              issuer: _configuration["TokenConfiguration:Issuer"],
              audience: _configuration["TokenConfiguration:Audience"],
              claims: claims,
              expires: expiration,
              signingCredentials: credenciais);

            // Retorna o token e demais informações
            return new JwtSecurityTokenHandler().WriteToken(token);

        }

        public async Task<string> RefreshToken(string token)
        {
            var result = await _dbContext.Tokens.AsNoTracking().FirstOrDefaultAsync(u => u.Token == token);

            return result != null && result.DateExpiration.ToLocalTime() > DateTime.Now ? GenerateToken(result.UserName) : null;
        }


    }
}
