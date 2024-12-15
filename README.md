# ZNetCS.AspNetCore.Compression

[![NuGet](https://img.shields.io/nuget/v/ZNetCS.AspNetCore.Compression.svg)](https://www.nuget.org/packages/ZNetCS.AspNetCore.Compression)
[![Build](https://github.com/msmolka/ZNetCS.AspNetCore.Compression/workflows/build/badge.svg)](https://github.com/msmolka/ZNetCS.AspNetCore.Compression/actions)

A small package to allow decompress incoming request and compress outgoing response inside ASP.NET Core application.
This package by default supports Brotli, GZIP and Deflate compression and decompression.

This package compresses all content in memory before sending it to client to provide new `Content-Length`.

## Installing 

Install using the [ZNetCS.AspNetCore.Compression NuGet package](https://www.nuget.org/packages/ZNetCS.AspNetCore.Compression)

```
PM> Install-Package ZNetCS.AspNetCore.Compression
```

## Usage

When you install the package, it should be added to your `.csproj`. Alternatively, you can add it directly by adding:

```xml
<ItemGroup>
    <PackageReference Include="ZNetCS.AspNetCore.Compression" Version="9.0.0" />
</ItemGroup>
```

### .NET 6
In order to use the Compression middleware, you must configure the services  in the `Program.cs` file.

```c#
// Add services to the container.
builder.Services.AddCompression();
```
or
```c#
// Add services to the container.
builder.Services.AddCompression(
        options =>
        {
            options.AllowedMediaTypes = new List<MediaTypeHeaderValue>
            {
                MediaTypeHeaderValue.Parse("text/*"),
                MediaTypeHeaderValue.Parse("message/*"),
                MediaTypeHeaderValue.Parse("application/x-javascript"),
                MediaTypeHeaderValue.Parse("application/javascript"),
                MediaTypeHeaderValue.Parse("application/json"),
                MediaTypeHeaderValue.Parse("application/xml"),
                MediaTypeHeaderValue.Parse("application/atom+xml"),
                MediaTypeHeaderValue.Parse("application/xaml+xml")
            };
            options.IgnoredPaths = new List<string>
            {
                "/css/",
                "/images/",
                "/js/",
                "/lib/"
            };
            options.MinimumCompressionThreshold = 860;
            options.Compressors = new List<ICompressor> { new BrotliCompressor(), new GZipCompressor(), new DeflateCompressor() };
            options.Decompressors = new List<IDecompressor> { new BrotliDecompressor(), new GZipDecompressor(), new DeflateDecompressor() };
        });
```

then
```c#
// Configure IP filtering
app.UseCompression();
```


### .NET 5 and Below 

In order to use the Compression middleware, you must configure the services in the `ConfigureServices` and `Configure` call of `Startup`:

```c#
public void ConfigureServices(IServiceCollection services)
{
    services.AddCompression();
}

public void Configure(IApplicationBuilder app)
{
    app.UseCompression();

    // other middleware e.g. MVC etc
}
```

You can alternatively setup additional options for compression and decompression

```c#
public void Configure(IApplicationBuilder app)
{
    app.UseCompression();

    // other middleware e.g. MVC etc  
}

public void ConfigureServices(IServiceCollection services)
{
    services.AddCompression(
        options =>
        {
            options.AllowedMediaTypes = new List<MediaTypeHeaderValue>
            {
                MediaTypeHeaderValue.Parse("text/*"),
                MediaTypeHeaderValue.Parse("message/*"),
                MediaTypeHeaderValue.Parse("application/x-javascript"),
                MediaTypeHeaderValue.Parse("application/javascript"),
                MediaTypeHeaderValue.Parse("application/json"),
                MediaTypeHeaderValue.Parse("application/xml"),
                MediaTypeHeaderValue.Parse("application/atom+xml"),
                MediaTypeHeaderValue.Parse("application/xaml+xml")
            };
            options.IgnoredPaths = new List<string>
            {
                "/css/",
                "/images/",
                "/js/",
                "/lib/"
            };
            options.MinimumCompressionThreshold = 860;
            options.Compressors = new List<ICompressor> { new BrotliCompressor(), new GZipCompressor(), new DeflateCompressor() };
            options.Decompressors = new List<IDecompressor> { new BrotliDecompressor(), new GZipDecompressor(), new DeflateDecompressor() };
        });
}
```

The default options when empty constructor is used are listed above.

Compressors also allow to specify compression level.

```c#
public void Configure(IApplicationBuilder app)
{
    app.UseCompression();

    // other middleware e.g. MVC etc  
}

public void ConfigureServices(IServiceCollection services)
{
    services.AddCompression(
        options =>
        {
            options.Compressors = new List<ICompressor> { new BrotliDecompressor(CompressionLevel.Fastest), new GZipCompressor(CompressionLevel.Fastest), new DeflateCompressor(CompressionLevel.Fastest) };
        });
}
```


