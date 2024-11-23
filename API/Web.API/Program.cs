using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.Text;
using Web.API;
using Web.Application;
using Web.Application.Dto_s.Group;
using Web.Application.Dto_s.User;
using Web.Application.Services;
using Web.Application.Services.Interfaces;
using Web.Application.Services.Interfaces.IServices;
using Web.Application.Validators.User;
using Web.Core.IRepositories;
using Web.Infrastructure;
using Web.Persistence;
using Web.Persistence.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("JwtOptions"));
var jwtOptions = builder.Configuration.GetSection("JwtOptions").Get<JwtOptions>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey))
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            context.Token = context.Request.Cookies["AppCookie"];
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policyBuilder =>
    {
        policyBuilder
            .WithOrigins("http://localhost:3000", "https://localhost:7205")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var connectionString = builder.Configuration.GetConnectionString("RedisConnectionString");
    var configurationOptions = new ConfigurationOptions
    {
        EndPoints = { connectionString },
        AbortOnConnectFail = false
    };
    return ConnectionMultiplexer.Connect(configurationOptions);
});

builder.Services.AddSingleton<ICasheProvider, CasheProvider>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IJwtProvider, JwtProvider>();
builder.Services.AddScoped<IEncryptionService, EncryptionService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPrivateChatRepository, PrivateChatRepository>();
builder.Services.AddScoped<IPrivateChatService, PrivateChatService>();
builder.Services.AddScoped<IMessageRepository, MessageRepository>();
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<IGroupRepository, GroupRepository>();
builder.Services.AddScoped<IGroupService, GroupService>();
builder.Services.AddScoped<IUserConnectionRepository, UserConnectionRepository>();
builder.Services.AddScoped<IUserConnectionService, UserConnectionService>();
builder.Services.AddScoped<IUnreadMessagesRepository, UnreadMessagesRepository>();
builder.Services.AddScoped<IUnreadMessagesService, UnreadMessagesService>();
builder.Services.AddScoped<IValidator<AuthUserDto>, AuthUserDtoValidator>();
builder.Services.AddScoped<IValidator<RegisterUserDto>, RegisterUserDtoValidator>();
builder.Services.AddScoped<IValidator<UserPasswordDto>, UpdatePasswordDtoValidator>();

builder.Services.AddDbContext<WebContext>();
builder.Services.AddSignalR();
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(typeof(MappingProfile));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapHub<ChatHub>("/chatHub");
app.MapControllers();

app.Run();