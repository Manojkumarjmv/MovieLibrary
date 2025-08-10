// Program.cs
using MovieLibrary.API.Models;
using MovieLibrary.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;


var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddFilter("Microsoft.AspNetCore.Authentication", LogLevel.Debug);
builder.Logging.AddFilter("Microsoft.IdentityModel", LogLevel.Debug);

builder.Services.AddSingleton<IMovieService, MovieService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ✅ Register HTTP logging service
builder.Services.AddHttpLogging(logging =>
{
    logging.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.All;
    logging.RequestBodyLogLimit = 4096;
    logging.ResponseBodyLogLimit = 4096;
});

/*builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));*/

   builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = "https://login.microsoftonline.com/36a3cff1-97c9-4fc9-96fb-037dbecf6b0f/v2.0";
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidAudience = "api://f9330316-05a4-4375-a748-86ef773e9549",
            ValidateIssuer = true,
            ValidIssuers = new[]
              {
                $"https://sts.windows.net/36a3cff1-97c9-4fc9-96fb-037dbecf6b0f/",
                $"https://login.microsoftonline.com/36a3cff1-97c9-4fc9-96fb-037dbecf6b0f/v2.0"
              },

            ValidateLifetime = true,
            ValidateIssuerSigningKey = true
        };
    });

builder.Services.AddHttpLogging(logging =>
{
    logging.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.All;
});

builder.Services.AddAuthorization();


var app = builder.Build();
Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/error");
    app.UseHsts();
}
app.UseHttpLogging();
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

var movieService = app.Services.GetRequiredService<IMovieService>();
app.MapGet("/ping", () => "pong");
app.MapGet("/api/movies", (HttpContext context) =>
{
  var movies = movieService.GetAll();
    return Results.Ok(movies);
}).RequireAuthorization();

app.MapGet("/api/movies/{id:guid}", (Guid id) =>
    movieService.GetById(id) is Movie m ? Results.Ok(m) : Results.NotFound()).RequireAuthorization();

app.MapGet("/api/movies/search/{query}", (string query) =>
    Results.Ok(movieService.Search(query))).RequireAuthorization();


app.MapGet("/debug-token", (HttpContext context) =>
{
    var token = context.Request.Headers["Authorization"].ToString();
    return Results.Ok(new { token });
});


app.MapPost("/api/movies", (Movie movie) =>
{
    movieService.Add(movie);
    return Results.Created($"/api/movies/{movie.Id}", movie);
}).RequireAuthorization();

app.MapPut("/api/movies/{id:guid}", (Guid id, Movie updatedMovie) =>
{
    updatedMovie.Id = id;
    return movieService.Update(updatedMovie) ? Results.Ok(updatedMovie) : Results.NotFound();
}).RequireAuthorization();

app.MapDelete("/api/movies/{id:guid}", (Guid id) =>
    movieService.Delete(id) ? Results.Ok() : Results.NotFound()).RequireAuthorization();


app.Run();