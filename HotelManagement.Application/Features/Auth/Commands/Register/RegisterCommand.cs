using HotelManagement.Application.Domain.Entities;
using HotelManagement.Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text;

namespace HotelManagement.Application.Features.Auth.Commands.Register
{
    public record RegisterCommand([Required(ErrorMessage = "Full name is required.")]
    [StringLength(100, ErrorMessage = "Full name cannot exceed 100 characters.")]
    string FullName,

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    string Email,

    [Required(ErrorMessage = "Password is required.")]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
    string Password) : IRequest<AuthResponse>
    { }
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponse>
    {
        private readonly UserManager<User> _userManager;

        public RegisterCommandHandler(UserManager<User> userManager)
        {
            _userManager = userManager;
        }
        public async Task<AuthResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            
   

    
        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
        {
            throw new InvalidOperationException("Email is already registered.");
        }

        var user = new User
        {
            FullName = request.FullName,
            Email = request.Email,
            UserName = request.Email, 
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"User registration failed: {errors}");
        }
            var authResponse = new AuthResponse
            {
                UserId = user.Id,
                Email = user.Email


            };
            return await  Task.FromResult(authResponse);
        }

       
    }
}
