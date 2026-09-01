using HotelManagement.Application.Domain.Entities;
using HotelManagement.Application.DTOs.AuthDTOs;
using MediatR;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;


namespace HotelManagement.Application.Features.Auth.Commands.Login
{
    public record LoginCommand(
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
     string Email ,

    [Required(ErrorMessage = "Password is required.")]
     string password ) : IRequest<LoginResponseDto>;

    public class LoginUserCommandHandler : IRequestHandler<LoginCommand, LoginResponseDto>
    {
        private readonly UserManager<User> _userManager;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public LoginUserCommandHandler(UserManager<User> userManager, IJwtTokenGenerator jwtTokenGenerator)
        {
            _userManager = userManager;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<LoginResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            // 1. Find user by email
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid credentials.");
            }

            // 2. ASSESSMENT REQUIREMENT: Inactive users cannot log in
            if (!user.IsActive)
            {
                throw new UnauthorizedAccessException("User account is inactive. Please contact support.");
            }

            // 3. Check password hash
            var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.password);
            if (!isPasswordValid)
            {
                throw new UnauthorizedAccessException("Invalid credentials.");
            }

            // 4. Generate JWT Token
            var token = _jwtTokenGenerator.GenerateToken(user);

            return new LoginResponseDto { Email=user.Email,Token= token };
        }
    }
}