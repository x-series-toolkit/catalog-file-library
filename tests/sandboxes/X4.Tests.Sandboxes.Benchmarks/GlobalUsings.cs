﻿global using BenchmarkDotNet.Attributes;
global using BenchmarkDotNet.Running;

global using System;
global using System.Buffers;
global using System.Collections.Generic;
global using System.Collections.Immutable;
global using System.IO;
global using System.IO.Abstractions;
global using System.Linq;
global using System.Threading;
global using System.Threading.Tasks;

global using X4.CatalogFileLib;
global using X4.CatalogFileLib.Domain;
global using X4.CatalogFileLib.Exceptions;
global using X4.CatalogFileLib.Extensions;
global using X4.CatalogFileLib.Services;
global using X4.CatalogFileLib.Services.Components;
global using X4.CatalogFileLib.Streams;