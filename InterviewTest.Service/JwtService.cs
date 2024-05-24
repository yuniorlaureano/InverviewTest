﻿using InterviewTest.Common;
using InterviewTest.Service.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace InterviewTest.Service
{
    public class JwtService : IJwtService
    {
        private readonly JwtOptions _jwtOptions;
        public JwtService(JwtOptions jwtOptions)
        {
            _jwtOptions = jwtOptions;
        }

        public string GenerateTokenAsync(Claim[] claims)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtOptions.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Issuer = _jwtOptions.Issuer,
                Audience = _jwtOptions.Audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            if (_jwtOptions.ExpiresIn is not null)
            {
                tokenDescriptor.Expires = DateTime.UtcNow.AddHours(_jwtOptions.ExpiresIn.Value);
            }

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public async Task<bool> ValidateTokenAsync(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtOptions.Secret);

            var result = await tokenHandler.ValidateTokenAsync(token, new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _jwtOptions.Issuer,
                ValidAudience = _jwtOptions.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(key)
            });

            return result.IsValid;
        }
    }
}
