global using Microsoft.AspNetCore.Identity;
global using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
global using Microsoft.EntityFrameworkCore.Metadata.Builders;
global using Microsoft.EntityFrameworkCore;
global using FluentValidation;
global using Microsoft.AspNetCore.Mvc;

global using Microsoft.IdentityModel.Tokens;
global using System.IdentityModel.Tokens.Jwt;
global using Mapster;
global using System.Security.Cryptography;
global using System.Security.Claims;
global using System.Text;
global using FitMe.Models;
global using FitMe.Persistence;
global using FitMe.Contracts.Authentication;
global using FitMe.Abstractions;
global using FitMe.Authentication;
global using FitMe.Errors;
global using FitMe.Helpers;
global using FitMe.Services;

global using Microsoft.AspNetCore.Identity.UI.Services;
global using Microsoft.AspNetCore.WebUtilities;

global using RegisterRequest = FitMe.Contracts.Authentication.RegisterRequest;
global using ResendConfirmationEmailRequest = FitMe.Contracts.Authentication.ResendConfirmationEmailRequest;