// Project-wide usings for Web.BlazorServer.
// These namespaces are used across nearly every code-behind; centralizing them here
// keeps individual .cs files focused on their feature-specific imports.
// (Razor components import their common namespaces via Components/_Imports.razor.)

// Framework
global using Microsoft.AspNetCore.Components;
global using Microsoft.JSInterop;
global using Radzen;
global using Radzen.Blazor;
global using Mapster;
global using MediatR;

// Shared libraries
global using Shared.Entities;

// Web project — utilities & services
global using Web.BlazorServer.Defaults;
global using Web.BlazorServer.Helpers;
global using Web.BlazorServer.Services.Repositories;
global using Web.BlazorServer.Services.Implementation;

// Web project — view models
global using Web.BlazorServer.ViewModels.Abstraction;
global using Web.BlazorServer.ViewModels.Commons;
global using Web.BlazorServer.ViewModels.Enums;
global using Web.BlazorServer.ViewModels.System;
global using Web.BlazorServer.ViewModels.Administration.Role;
global using Web.BlazorServer.ViewModels.Administration.User;
