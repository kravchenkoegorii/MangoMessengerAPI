﻿using System;
using System.Threading;
using System.Threading.Tasks;
using MangoAPI.Domain.Entities;
using MangoAPI.DTO.Commands.Auth;
using MangoAPI.DTO.Responses;
using MangoAPI.Infrastructure.Database;
using MangoAPI.Infrastructure.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace MangoAPI.Infrastructure.CommandHandlers
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
    {
        private readonly UserManager<UserEntity> _userManager;
        private readonly SignInManager<UserEntity> _signInManager;
        private readonly IJwtGenerator _jwtGenerator;
        private readonly MangoPostgresDbContext _postgresDbContext;

        public LoginCommandHandler(UserManager<UserEntity> userManager, SignInManager<UserEntity> signInManager,
            IJwtGenerator jwtGenerator, MangoPostgresDbContext postgresDbContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtGenerator = jwtGenerator;
            _postgresDbContext = postgresDbContext;
        }

        public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            UserEntity user;

            try
            {
                user = await _userManager.FindByEmailAsync(request.Email);
            }
            catch (Exception)
            {
                return await Task.FromResult(new LoginResponse
                {
                    Message = "Invalid Email error.",
                    AccessToken = "N/A",
                    RefreshToken = "N/A",
                    Success = false
                });
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);

            if (result.Succeeded)
            {
                var refreshToken = _jwtGenerator.GenerateRefreshToken(request.IpAddress);
                var jwtToken = _jwtGenerator.CreateToken(user);

                refreshToken.UserId = user.Id;

                await _postgresDbContext.RefreshTokens.AddAsync(refreshToken, cancellationToken);
                await _postgresDbContext.SaveChangesAsync(cancellationToken);

                return await Task.FromResult(new LoginResponse
                {
                    Message = "Login successful.",
                    AccessToken = jwtToken,
                    RefreshToken = refreshToken.Token,
                    Success = true
                });
            }

            return await Task.FromResult(new LoginResponse
            {
                Message = "Invalid Password error.",
                AccessToken = "N/A",
                RefreshToken = "N/A",
                Success = false
            });
        }
    }
}