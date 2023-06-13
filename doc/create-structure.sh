#!/bin/bash

echo "Creando solución y estructura básica para book-manager.."

# Solution and gitignore
dotnet new sln --name  book-manager
dotnet new gitignore

# Layers
dotnet new classlib --name BookManager.Domain --output src/BookManager.Domain
dotnet new classlib --name BookManager.Application --output src/BookManager.Application
dotnet new web --name BookManager --output src/BookManager

# Dependencies between layers
dotnet add src/BookManager.Application reference src/BookManager.Domain
dotnet add src/BookManager reference src/BookManager.Application

# Test projects
dotnet new xunit --name BookManager.Domain.UnitTests --output test/BookManager.Domain.UnitTests
dotnet new xunit --name BookManager.Application.UnitTests --output test/BookManager.Application.UnitTests
dotnet new xunit --name BookManager.FunctionalTests --output test/BookManager.FunctionalTests

# Dependencies for test projects
dotnet add test/BookManager.Domain.UnitTests reference src/BookManager.Domain
dotnet add test/BookManager.Application.UnitTests reference src/BookManager.Application
dotnet add test/BookManager.FunctionalTests reference src/BookManager

# Packages for unit tests projects
dotnet add test/BookManager.Domain.UnitTests package FluentAssertions
dotnet add test/BookManager.Application.UnitTests package FluentAssertions
dotnet add test/BookManager.Domain.UnitTests package Moq
dotnet add test/BookManager.Application.UnitTests package Moq

# Packages for functional test project
dotnet add test/BookManager.FunctionalTests package FluentAssertions
dotnet add test/BookManager.FunctionalTests package Microsoft.AspNetCore.Mvc.Testing

# Add projects to solution
dotnet sln add src/BookManager.Domain
dotnet sln add src/BookManager.Application
dotnet sln add src/BookManager
dotnet sln add test/BookManager.Domain.UnitTests
dotnet sln add test/BookManager.Application.UnitTests
dotnet sln add test/BookManager.FunctionalTests
