global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Hosting;

global using Serilog;
global using Serilog.Core;

global using System;
global using System.Diagnostics;
global using System.IO;
global using System.IO.Abstractions;
global using System.Threading;
global using System.Threading.Tasks;


global using X4.CatalogFileLib.Contracts;
global using X4.CatalogFileLib.Services;
global using X4.CatalogFileLib.Services.Components;
global using X4.Tests.Sandboxes.DI;

global using static System.Environment;
global using static X4.Tests.Sandboxes.DI.EnvironmentHelper;
global using static X4.Tests.Sandboxes.DI.StaticLogger;
