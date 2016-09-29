# ZNetCS.AspNetCore.Compression

[![NuGet](https://img.shields.io/nuget/v/ZNetCS.AspNetCore.Compression.svg)](https://www.nuget.org/packages/ZNetCS.AspNetCore.Compression)

A small package to allow decompress incoming request and compress outgoing response inside ASP.NET Core application.


## Installing 

Install using the [ZNetCS.AspNetCore.Compression NuGet package](https://www.nuget.org/packages/ZNetCS.AspNetCore.Compression)

```
PM> Install-Package ZNetCS.AspNetCore.Compression
```

##Usage 

When you install the package, it should be added to your `package.json`. Alternatively, you can add it directly by adding:


```json
{
  "dependencies" : {
    "ZNetCS.AspNetCore.Compression": "1.0.0"
  }
}
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

You can alternatively setup additional options for commpression and decompression

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

The default option when empty constructor is used are listed above.


