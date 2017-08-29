# ZNetCS.AspNetCore.Compression

[![NuGet](https://img.shields.io/nuget/v/ZNetCS.AspNetCore.Compression.svg)](https://www.nuget.org/packages/ZNetCS.AspNetCore.Compression)

A small package to allow decompress incoming request and compress outgoing response inside ASP.NET Core application.
This package by default supports GZIP and Deflate compression and decompression.


## Installing 

Install using the [ZNetCS.AspNetCore.Compression NuGet package](https://www.nuget.org/packages/ZNetCS.AspNetCore.Compression)

```
PM> Install-Package ZNetCS.AspNetCore.Compression
```

# ASP CORE 2.0
The previous version 1.0.4 is not compatible with Core 2.0. There are breaking changes in that relase. So this is only compilation and code fixes to make project compatible again.

## Usage

When you install the package, it should be added to your `.csproj`. Alternatively, you can add it directly by adding:

```xml
<ItemGroup>
    <PackageReference Include="ZNetCS.AspNetCore.Compression" Version="2.0.0" />
</ItemGroup>
```

In order to use the Compression middleware, you must configure the services in the `ConfigureServices` and `Configure` call of `Startup`:

```csharp
using ZNetCS.AspNetCore.Compression.DependencyInjection;
```

```
...
```

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddCompression();
}

public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
{
    app.UseCompression();

    // other middleware e.g. MVC etc
}
```

You can alternatively setup additional options for compression and decompression

```csharp
public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
{
    app.UseCompression(new CompressionOptions 
    {
            AllowedMediaTypes = new List<MediaTypeHeaderValue>
            {
                MediaTypeHeaderValue.Parse("text/*"),
                MediaTypeHeaderValue.Parse("message/*"),
                MediaTypeHeaderValue.Parse("application/x-javascript"),
                MediaTypeHeaderValue.Parse("application/javascript"),
                MediaTypeHeaderValue.Parse("application/json"),
                MediaTypeHeaderValue.Parse("application/xml"),
                MediaTypeHeaderValue.Parse("application/atom+xml"),
                MediaTypeHeaderValue.Parse("application/xaml+xml")
            },
            IgnoredPaths = new List<string>
            {
                "/css/",
                "/images/",
                "/js/",
                "/lib/"
            },
            MinimumCompressionThreshold = 860,
            Compressors = new List<ICompressor> { new GZipCompressor(), new DeflateCompressor() },
            Decompressors = new List<IDecompressor> { new GZipDecompressor(), new DeflateDecompressor() }
    });

    // other middleware e.g. MVC etc  
}
```

The default options when empty constructor is used are listed above.

Compressors also allow to specify compression level.

```csharp
public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
{
    app.UseCompression(new CompressionOptions 
    {
            Compressors = new List<ICompressor> { new GZipCompressor(CompressionLevel.Fastest), new DeflateCompressor(CompressionLevel.Fastest) }
    });

    // other middleware e.g. MVC etc  
}
```


