using BlazorServerCms.Areas.Identity;
using BlazorServerCms.Data;
using BlazorServerCms.servicos;
using business;
using business.business;
using business.business.Group;
using business.Group;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PSC.Blazor.Components.Tours;


var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;
string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

// Add services to the container.

builder.Services.AddScoped<IStoryService, StoryService>();
builder.Services.AddScoped<Marcacao>();
builder.Services.AddSingleton<HttpClient>();
builder.Services.AddSingleton<RepositoryPagina>();
builder.Services.AddSingleton<BlazorTimer>();
builder.Services.AddSingleton<ChatGpt>();
var connectionString =
builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services
.AddDefaultIdentity<UserModel>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<UserModel>>();
builder.Services.AddScoped<IEmailSender<UserModel>, EmailSender>();

builder.Services.UseTour();

// builder.Services.AddServerSideBlazor().AddHubOptions(options =>
// {
//     options.MaximumReceiveMessageSize = 64 * 1024;
// });

//builder.Services.AddAuthentication()
//               .AddGoogle(options =>
//               {
//                   options.ClientId = config["Authentication:Google:ClientId"];
//                   options.ClientSecret = config["Authentication:Google:ClientSecret"];
//                   IConfigurationSection googleAuthNSection =
//                       config.GetSection("Authentication:Google");
//                   options.ClientId = googleAuthNSection["ClientId"];
//                   options.ClientSecret = googleAuthNSection["ClientSecret"];
//               });

// builder.Services.AddServerSideBlazor().AddCircuitOptions(options => { options.DetailedErrors = true; });

// builder.Services.AddSignalR().AddStackExchangeRedis(builder.Configuration.GetConnectionString("dominio")!, options =>
// {
//     options.Configuration.ChannelPrefix = "BlazorServerCms";
// });


builder.Services.AddMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(10);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


var app = builder.Build();

app.UseSession();



var webSocketOptions = new WebSocketOptions()
{
    KeepAliveInterval = TimeSpan.FromSeconds(120),
};

app.UseWebSockets(webSocketOptions);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

using (var scope = app.Services.CreateScope())
{
    var contexto         = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var repositoryPagina = scope.ServiceProvider.GetRequiredService<RepositoryPagina>();
    var roleManager      = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager      = scope.ServiceProvider.GetRequiredService<UserManager<UserModel>>();
    var email            = builder.Configuration.GetConnectionString("Email");
    var password         = builder.Configuration.GetConnectionString("Senha");
    var userASP          = await userManager.FindByNameAsync(email);

   // var lista = await repositoryPagina.buscarPatternStory();

    if (await contexto!.Set<Story>().AnyAsync())
    {
        List<Story> stories = await contexto.Story!
        .OrderBy(st => st.Capitulo)
        .ToListAsync();
        RepositoryPagina.stories.AddRange(stories);
    }
    else 
    {
       // foreach (var item in lista!)
       //     contexto.Add(item);
       // contexto.SaveChanges();
    }

    if(await contexto!.Set<Content>().AnyAsync())
    {
        var conteudos = await contexto.UserContent
        .Include(f => f.Filtro)
        .Include(f => f.UserModel)
        .Where(c => c.Data > DateTime.Now.AddDays(-repositoryPagina.dias))
        .OrderBy(co => co.Id).ToListAsync();
        RepositoryPagina.Conteudo!.AddRange(conteudos);
    }
    
    
    if(!await contexto!.Set<UserContent>().AnyAsync())
    {
        UserContent[] pages = new UserContent[145];
            pages[0] = new UserContent();
            pages[0].Data = DateTime.Now;
            pages[0].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid02jZ4HpoAWv77nwj3k9jfsRzVvW8GbXQ2BK91w8iF1gZEUbSYKtLowcimZBjjWTxQAl&show_text=true&width=500\" width=\"500\" height=\"384\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";            
            pages[0].StoryId = 5;
            
            pages[1] = new UserContent();
            pages[1].Data = DateTime.Now;
            pages[1].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid0YZpNdTBjJx4bq2HhyV1LxnU7fzMqW7HP3Y1Rbzoudd7XDzXNLZoSzinumCmWmXTWl&show_text=true&width=500\" width=\"500\" height=\"250\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";            
            pages[1].StoryId = 13;            
            
            pages[2] = new UserContent();
            pages[2].Data = DateTime.Now;
            pages[2].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid026igxqSgMzHej1uhcnjtPkUTWX7NQAsSWETgWSfnsQFucZ8xHJzAvUpszoWwYoKAZl&show_text=true&width=500\" width=\"500\" height=\"384\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";            
            pages[2].StoryId = 14;

            pages[3] = new UserContent();
            pages[3].Data = DateTime.Now;
            pages[3].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid0HZ5uXagdRPSTvRmMg247yu9KbztCsN6sVx1V2Hs2wb8hFeSAsopKVk5aFY7Rj8Xfl&show_text=true&width=500\" width=\"500\" height=\"384\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";            
            pages[3].StoryId = 13;
            
            pages[4] = new UserContent();
            pages[4].Data = DateTime.Now;
            pages[4].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid0HZ5uXagdRPSTvRmMg247yu9KbztCsN6sVx1V2Hs2wb8hFeSAsopKVk5aFY7Rj8Xfl&show_text=true&width=500\" width=\"500\" height=\"384\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";            
            pages[4].StoryId = 13;
            
            pages[5] = new UserContent();
            pages[5].Data = DateTime.Now;
            pages[5].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid02kcemj6qQWnagpgFYtRveFLKzsBGfYwtLux2WnYkZccNhFgSiMPAPDvTT5wCXTfYVl&show_text=true&width=500\" width=\"500\" height=\"384\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";            
            pages[5].StoryId = 4;

            pages[6] = new UserContent();
            pages[6].Data = DateTime.Now;
            pages[6].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid0Jc2P8fpsT1MfpJC9LdcV2YhczFZsSN6zZkSKkeqEooLbxEy5FZcXUUc4st4yx31Xl&show_text=true&width=500\" width=\"500\" height=\"464\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";            
            pages[6].StoryId = 2;
            
            pages[7] = new UserContent();
            pages[7].Data = DateTime.Now;
            pages[7].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid0CPXEVfSpuK3eK9MjtSd2Nycfzo87eySm9SRnWbMhxaqezapa9bveUfpcCL9wj7TZl&show_text=true&width=500\" width=\"500\" height=\"481\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";            
            pages[7].StoryId = 17;
            
            pages[8] = new UserContent();
            pages[8].Data = DateTime.Now;
            pages[8].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid036YRVPAqPHgr7goNNr1yfM8jtLzDANpuQio1R1214tYbyowwn9RB8X1KJe5yJqHE3l&show_text=true&width=500\" width=\"500\" height=\"384\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";            
            pages[8].StoryId = 4;
            
            pages[9] = new UserContent();
            pages[9].Data = DateTime.Now;
            pages[9].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid02w3TVhUZHpqd8gHq1pMkiqXfPNm7TJVftfAUPvBv7jxzPA6rFVf7EzNpca8jrNyjHl&show_text=true&width=500\" width=\"500\" height=\"363\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";            
            pages[9].StoryId = 13;
            
            pages[10] = new UserContent();
            pages[10].Data = DateTime.Now;
            pages[10].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid02u8PepbdbWgVK8MFCHytJqX52WbZiw5x53vAsNxKaUJdAJmz4xUmD3hx3e5uQGCmol&show_text=true&width=500\" width=\"500\" height=\"599\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";            
            pages[10].StoryId = 34;
            
            pages[11] = new UserContent();
            pages[11].Data = DateTime.Now;
            pages[11].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid0T4ba8HjbXQuog6nG4UujWPyHwkVCHfo7abZjqqV5wm89EdP6eznv1sFQRnv2YmYQl&show_text=true&width=500\" width=\"500\" height=\"250\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";            
            pages[11].StoryId = 5;
            
            pages[12] = new UserContent();
            pages[12].Data = DateTime.Now;
            pages[12].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid02eZKigMVXnfP8FJeVXA76gmgn4Ndcu5863T5dPdJtaYcyqhothAyxwhjfFXV5CRsil&show_text=true&width=500\" width=\"500\" height=\"384\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";            
            pages[12].StoryId = 4;
            
            pages[13] = new UserContent();
            pages[13].Data = DateTime.Now;
            pages[13].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid0FhJ4hYm57u3gJeAbrXPRTSbmyCb2MtafVGcxcedSn6C2mcHvAb4StGjasr2U6Supl&show_text=true&width=500\" width=\"500\" height=\"384\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";            
            pages[13].StoryId = 25;
            
            pages[14] = new UserContent();
            pages[14].Data = DateTime.Now;
            pages[14].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid0QaL61L6DJqfHEUT7Vhese6V8FwVizcYwPtufjC33bWzTM2u114Rj5Jxf8LGJBXn1l&show_text=true&width=500\" width=\"500\" height=\"497\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";            
            pages[14].StoryId = 14;
            
            pages[15] = new UserContent();
            pages[15].Data = DateTime.Now;
            pages[15].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid021PQYjVGcqxdbhZVQhY451Cn2eXLhxmGbdSbC49nbSjfToZR2Jjohuqd8YpfswkVcl&show_text=true&width=500\" width=\"500\" height=\"250\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";            
            pages[15].StoryId = 14;
            
            pages[16] = new UserContent();
            pages[16].Data = DateTime.Now;
            pages[16].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid02DA5XD8UiFE6pABzjhAqeCMoKXLjh97Ph5cgWeRfmymRdgd2H23hPzAAvzBWNRf8xl&show_text=true&width=500\" width=\"500\" height=\"384\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";            
            pages[16].StoryId = 4;
            
            pages[17] = new UserContent();
            pages[17].Data = DateTime.Now;
            pages[17].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid0fLY7CFDHjEHR7xKqHfXASzJcC2cna4nQaraT6nNRkSszJms4BH2crqpD2ZUHwR3xl&show_text=true&width=500\" width=\"500\" height=\"384\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";            
            pages[17].StoryId = 5;
            
            pages[18] = new UserContent();
            pages[18].Data = DateTime.Now;
            pages[18].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid02muF2BXKUbBn5g4pXon6YbF7kXJkN5BDC1cq6pdCvLE23MAhLcdr3R1rffWDCe5Aml&show_text=true&width=500\" width=\"500\" height=\"384\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";            
            pages[18].StoryId = 1;
            
            pages[19] = new UserContent();
            pages[19].Data = DateTime.Now;
            pages[19].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid02J5SmuwEuuusvpqzVbJncwpYDzg1R79GGD7Dvvqt3NKZFY4nHezuXkfWNxf6w3DBRl&show_text=true&width=500\" width=\"500\" height=\"384\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";            
            pages[19].StoryId = 1;
            
            pages[20] = new UserContent();
            pages[20].Data = DateTime.Now;
            pages[20].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid02NCBJiy6FL14fooYEGFu3avfKdCqh3MwKKyjV7kYWDrLC3cPt5qMGn8DaEvwMhDaGl&show_text=true&width=500\" width=\"500\" height=\"384\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";            
            pages[20].StoryId = 23;
            
            pages[21] = new UserContent();
            pages[21].Data = DateTime.Now;
            pages[21].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid0mRPtUJQD8NBAbWax73pZ4v4Fnz3xPG2aHgzUg8g877ffCTZNCUqYaAucD674kPuCl&show_text=true&width=500\" width=\"500\" height=\"384\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";            
            pages[21].StoryId = 2;
            
            pages[22] = new UserContent();
            pages[22].Data = DateTime.Now;
            pages[22].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid0hSJj2DjzrnuJ8CHLvvUUuLdqKxMsWvdfTX85ADtDc5F4Bp6jVt6jXmJHsHP2YZATl&show_text=true&width=500\" width=\"500\" height=\"384\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";            
            pages[22].StoryId = 1;
            
            pages[23] = new UserContent();
            pages[23].Data = DateTime.Now;
            pages[23].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid0Yoo9dPnt9CNRiGQrTJy9FPdeqSD1vQTeQmCi7Q1qz1dWFb778CKVxAzAQSGg3kJhl&show_text=true&width=500\" width=\"500\" height=\"452\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";            
            pages[23].StoryId = 17;            
            
            pages[24] = new UserContent();
            pages[24].Data = DateTime.Now;
            pages[24].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid037HZVbDxMzibh2cMeH8n9wkX6G2SsyQK4yRrQ7TuqnUrfexCJLsbymJP47EBTCKnZl&show_text=true&width=500\" width=\"500\" height=\"384\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";            
            pages[24].StoryId = 7;
            
            pages[25] = new UserContent();
            pages[25].Data = DateTime.Now;
            pages[25].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid0gQtvmyAbpt2TEf33uo8Sm6DknEkXfxNJf2j2MDHvuN4woLpWEEcwEuQDGCbSwY2sl&show_text=true&width=500\" width=\"500\" height=\"481\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";            
            pages[25].StoryId = 16;
            
            pages[26] = new UserContent();
            pages[26].Data = DateTime.Now;
            pages[26].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid0R6ugdK4D6HkdRCr4wqyFYMPSVrJ7GBo9m9sRNob4SSV3ZKEKP1xYShCAc4TkzwSBl&show_text=true&width=500\" width=\"500\" height=\"384\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";            
            pages[26].StoryId = 26;
            
            pages[27] = new UserContent();
            pages[27].Data = DateTime.Now;
            pages[27].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid02VB28nFtno9dS9FVoCGZPnWC6LzN5Dv4B8CQSDsfuQXToRgGR4vUpqic5ELKaRmhol&show_text=true&width=500\" width=\"500\" height=\"481\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";            
            pages[27].StoryId = 34;
            
            pages[28] = new UserContent();
            pages[28].Data = DateTime.Now;
            pages[28].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid06JLUWEdgZ1p782H79xL1q3V56pZZyWm99pHmKDVJWBVr6c9FSCSDLH2imFrchJcFl&show_text=true&width=500\" width=\"500\" height=\"481\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";            
            pages[28].StoryId = 5;

            pages[29] = new UserContent();
            pages[29].Data = DateTime.Now;
            pages[29].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid02waamEUJAFYnmFeeDSgZkcHi4CsYjeN4bZDiztH4zjErjftPmRqGgpDy36UPJXmzWl&show_text=true&width=500\" width=\"500\" height=\"497\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[29].StoryId = 14;

            pages[30] = new UserContent();
            pages[30].Data = DateTime.Now;
            pages[30].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid02wSW2uUSqYr3G5nxE9S172P8XSGLZA2KEDrvNo95UfuinjFsG2pMHh4GfJaNWJBVyl&show_text=true&width=500\" width=\"500\" height=\"384\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[30].StoryId = 25;

            pages[31] = new UserContent();
            pages[31].Data = DateTime.Now;
            pages[31].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid03111ynief6eSy3h7DnnRZ8x1r3it4U4eyZZ94PTMGjWfUJFSh6k9e15FUK4MZ6FEGl&show_text=true&width=500\" width=\"500\" height=\"384\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[31].StoryId = 5;

            pages[32] = new UserContent();
            pages[32].Data = DateTime.Now;
            pages[32].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid02ZTh5RjRch9m2qa947WYmLAcFTW1hN33zX6Ha8RxZc2zGKJgM8NQpsZLuSwSS4shWl&show_text=true&width=500\" width=\"500\" height=\"250\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[32].StoryId = 25;

            pages[33] = new UserContent();
            pages[33].Data = DateTime.Now;
            pages[33].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid02L3MLBKRrCfycrmSwd8jiBDbYg3jBVWU2QNURPzihYdWjN1YSFjVtfcyde3LnnAfhl&show_text=true&width=500\" width=\"500\" height=\"599\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[33].StoryId = 34;

            pages[34] = new UserContent();
            pages[34].Data = DateTime.Now;
            pages[34].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid02kycxfFPEbp3vZ42QrspdLPkvfmHbgbHaj7fN8tWXYw4gmmiT4eXYejDpp8tNSrqel&show_text=true&width=500\" width=\"500\" height=\"481\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[34].StoryId = 2;

            pages[35] = new UserContent();
            pages[35].Data = DateTime.Now;
            pages[35].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid02Tivnay7E7ND4se6jfm8CZLXDbxjZUJYCQNtkahUTuQgbdtQypfUSfZre2N4cxbdjl&show_text=true&width=500\" width=\"500\" height=\"481\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[35].StoryId = 2;

            pages[36] = new UserContent();
            pages[36].Data = DateTime.Now;
            pages[36].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid0vrRoWEEDVYjTzYSK7QiHjTTnwsFYZ9nSt78sF2bvfqL31A6CnrWb9QPjUGwii2jql&show_text=true&width=500\" width=\"500\" height=\"481\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[36].StoryId = 2;

            pages[37] = new UserContent();
            pages[37].Data = DateTime.Now;
            pages[37].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid02hA9ELEnHsGUBqNQpaPEbWmJVgpqFhKngwLpozwJu2ycE22E3FrKBTDaXWgSHUjXPl&show_text=true&width=500\" width=\"500\" height=\"481\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[37].StoryId = 5;

            pages[38] = new UserContent();
            pages[38].Data = DateTime.Now;
            pages[38].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid05D6yrteB2dShxds8NYYsPLQjdtevbppj82gRXZo9xiuKyA54h1RuH2EUnJxyP4kNl&show_text=true&width=500\" width=\"500\" height=\"384\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[38].StoryId = 25;

            pages[39] = new UserContent();
            pages[39].Data = DateTime.Now;
            pages[39].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid02DKMvkMdZA4G8gKDZXqJcyHGPeEJQ4pUdXQe2zvNLanNx1WdyWranfemqucJM3NeCl&show_text=true&width=500\" width=\"500\" height=\"384\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[39].StoryId = 7;

            pages[40] = new UserContent();
            pages[40].Data = DateTime.Now;
            pages[40].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid0WU3nEiJ81syVoWhDYkVpQXBsSa5Q3KgHj23eTkF2NpGCojinyWaYtUuKk4FtsTStl&show_text=true&width=500\" width=\"500\" height=\"384\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[40].StoryId = 1;

            pages[41] = new UserContent();
            pages[41].Data = DateTime.Now;
            pages[41].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid02H6ezrYn7AEMvrDtLUwXXNWMciZakiyDGW6vgtAy6e8LVWvk4KkfoCinM8VZRZMVkl&show_text=true&width=500\" width=\"500\" height=\"384\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[41].StoryId = 26;

            pages[42] = new UserContent();
            pages[42].Data = DateTime.Now;
            pages[42].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid0u6eNAaTN6NQ5ZS75uUZr27nemyZfzSxRMcveLcvoafHbemzj6Vo7MwcVwkNWTD7Dl&show_text=true&width=500\" width=\"500\" height=\"497\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[42].StoryId = 30;

            pages[43] = new UserContent();
            pages[43].Data = DateTime.Now;
            pages[43].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid02qq4A9XLT1bJf2XYkH3eLQuxmoiY2fr6UZ7ZBNBaY9t7Envd1W5SkkoWKSehAd2e9l&show_text=true&width=500\" width=\"500\" height=\"250\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[43].StoryId = 1;

            pages[44] = new UserContent();
            pages[44].Data = DateTime.Now;
            pages[44].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid02oQq4dJFpyvbyHHk9awr5Vo6U7HPFKQhVA1sAjV8JsswgVRrEZfr28a9q5erMwnn1l&show_text=true&width=500\" width=\"500\" height=\"384\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[44].StoryId = 1;

            pages[45] = new UserContent();
            pages[45].Data = DateTime.Now;
            pages[45].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid0nLMy8bSuPZ3iHRLcLv6gZdTLm1wA75aosWjJDLQkMKVLLrPiYUz66zoJVAiP2z6Rl&show_text=true&width=500\" width=\"500\" height=\"384\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[45].StoryId = 1;

            pages[46] = new UserContent();
            pages[46].Data = DateTime.Now;
            pages[46].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid02bNYFYCqm9WyUzCNtDVEHVUGtc9SoGeckDf7SdKKYY2jBG9tkJQnpCF6fTFN3Ghjrl&show_text=true&width=500\" width=\"500\" height=\"384\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[46].StoryId = 10;

            pages[47] = new UserContent();
            pages[47].Data = DateTime.Now;
            pages[47].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid02AtGAAWWjhjaFcrAUEeSp9zDnUSoreUEGFSfo5iYmH4Q6c5L8bwxDqjChVqPjuvF6l&show_text=true&width=500\" width=\"500\" height=\"384\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[47].StoryId = 14;

            pages[48] = new UserContent();
            pages[48].Data = DateTime.Now;
            pages[48].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid0f6826jE92KZFCPoP97mAnFRn8ND7ppEPfQ8k17eaHwtsCjMoCgJTg3XaMpRTPedsl&show_text=true&width=500\" width=\"500\" height=\"384\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[48].StoryId = 1;

            pages[49] = new UserContent();
            pages[49].Data = DateTime.Now;
            pages[49].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid0BXWBAqn3i2AJaEXuiQDxxjsBE838yTQDo8bmVGhUBSy7JM3MK7tCDeG1QWhaCbC5l&show_text=true&width=500\" width=\"500\" height=\"384\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[49].StoryId = 14;

            pages[50] = new UserContent();
            pages[50].Data = DateTime.Now;
            pages[50].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid023uy2TU8AiVrvZpRRLJH3kX8dTjy6hC5b3q8qBNMSsPsyyWZXKj4kDdxaz6KLVT6cl&show_text=true&width=500\" width=\"500\" height=\"384\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[50].StoryId = 26;

            pages[51] = new UserContent();
            pages[51].Data = DateTime.Now;
            pages[51].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid026g4weaZmi3vZAw6S7etMx4XjcTbc36NJtm9CWhn8c1atXg2qFaxYCeV2vDjgWAful&show_text=true&width=500\" width=\"500\" height=\"384\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[51].StoryId = 1;

            pages[52] = new UserContent();
            pages[52].Data = DateTime.Now;
            pages[52].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid022NrL66G5AmJao3R8HUHNmQR7yo4aNZhiqYyK31VPAfK15Mxx5rAL45j14swKyqyql&show_text=true&width=500\" width=\"500\" height=\"329\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[52].StoryId = 1;

            pages[53] = new UserContent();
            pages[53].Data = DateTime.Now;
            pages[53].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid0nna9ZzwiMbCjRro6SmaweKc8oU1JMbw8yd65hpHypBmusaJJccGcxUPuwyhDr6SGl&show_text=true&width=500\" width=\"500\" height=\"250\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[53].StoryId = 4;

            pages[54] = new UserContent();
            pages[54].Data = DateTime.Now;
            pages[54].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid0pwBUnUjfTzLjzCacpEJaX2h5cLoUjKbLq4JRp1fLcxjrePB3EYcfjALQzTSe8k6Sl&show_text=true&width=500\" width=\"500\" height=\"384\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[54].StoryId = 26;

            pages[55] = new UserContent();
            pages[55].Data = DateTime.Now;
            pages[55].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid0vkkT2vd34KxxaNJppePo1pufcTGzff97M4Siqhq7vEm3AWWzCMdLuYXJZDam2tvdl&show_text=true&width=500\" width=\"500\" height=\"442\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[55].StoryId = 1;

            pages[56] = new UserContent();
            pages[56].Data = DateTime.Now;
            pages[56].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid0rzf5aCR76tqkD98bt1ryzPTQGRyebM53kMvwDxjUWKMbP4QjgrVRUuEVNP7uUd9Nl&show_text=true&width=500\" width=\"500\" height=\"329\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[56].StoryId = 1;

            pages[57] = new UserContent();
            pages[57].Data = DateTime.Now;
            pages[57].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid0MeQ6MyitmVnyWtVLkXMUSGGePaaR86gbpGThyP6oK7hpGpkoHqABNKPvnRY6YnKDl&show_text=true&width=500\" width=\"500\" height=\"384\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[57].StoryId = 1;

            pages[58] = new UserContent();
            pages[58].Data = DateTime.Now;
            pages[58].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid02p1ffjx1w8h9THFBJqJ99sw8Jx3kbxxVwdXQP9bqmpSGJX9CdjhPP2j2iCr4gDjtPl&show_text=true&width=500\" width=\"500\" height=\"384\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[58].StoryId = 26;

            pages[59] = new UserContent();
            pages[59].Data = DateTime.Now;
            pages[59].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid0zTTTdNRxEPZj6Hz9B7kP1jSoZXjzcivo2NbBpkJDt4AgfKWQ57WhcjiynukYtnrrl&show_text=true&width=500\" width=\"500\" height=\"384\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[59].StoryId = 1;

            pages[60] = new UserContent();
            pages[60].Data = DateTime.Now;
            pages[60].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid026WMCaYPq5fXHGCpCevkKFWqCS5SNRtn2BSvkSNTnxyeGZEmCCPPBaJPfSvbF8U1ol&show_text=true&width=500\" width=\"500\" height=\"404\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[60].StoryId = 10;

            pages[61] = new UserContent();
            pages[61].Data = DateTime.Now;
            pages[61].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid0vVtyY8LaxZNRhwtfNwCEqTS95H52tKZUFXenZEasgvdUXAAD1tQrwMt6dLgsoixpl&show_text=true&width=500\" width=\"500\" height=\"599\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[61].StoryId = 34;

            pages[62] = new UserContent();
            pages[62].Data = DateTime.Now;
            pages[62].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid02nYV19sKj1CGKVsFqmLhtvPuceigNbkJq2QDCi7YWDkmBkKXcXd2yVqswLFwpb8ral&show_text=true&width=500\" width=\"500\" height=\"384\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[62].StoryId = 26;

            pages[63] = new UserContent();
            pages[63].Data = DateTime.Now;
            pages[63].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid02d9R5aMSZWnEwEVkKm798z8VnQzgR4tyDzwGRX6EeD1y75tzfnVfw4kc4nMbLhsQnl&show_text=true&width=500\" width=\"500\" height=\"329\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[63].StoryId = 1;

            pages[64] = new UserContent();
            pages[64].Data = DateTime.Now;
            pages[64].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid02qdv56rMSKKp6CsT8DSykoQq5RQ7H7PnM2Go6jvPiFgp2aXX6vthkKydZxZ6FXiiLl&show_text=true&width=500\" width=\"500\" height=\"384\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[64].StoryId = 1;

            pages[65] = new UserContent();
            pages[65].Data = DateTime.Now;
            pages[65].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid0Eus4tURvkvrFWX4ku69EEgKeNroaBS9mkZM7111N2vUDtrNduWUn3LEbg7jqZCKjl&show_text=true&width=500\" width=\"500\" height=\"516\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[65].StoryId = 14;

            pages[66] = new UserContent();
            pages[66].Data = DateTime.Now;
            pages[66].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid02y3DkuYDVjeGJmvrYiQRy4QvpqcB39Tp26grHBnYFwhfDqLr3drFgSQRzTYf3RoDjl&show_text=true&width=500\" width=\"500\" height=\"497\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[66].StoryId = 14;

            pages[67] = new UserContent();
            pages[67].Data = DateTime.Now;
            pages[67].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid027wTVTY7DjYQdYMkzcyatTf4qE7V2XV378XU3poHPTUGYmM9mPBvARK4BMZ7tB5ykl&show_text=true&width=500\" width=\"500\" height=\"384\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[67].StoryId = 1;

            pages[68] = new UserContent();
            pages[68].Data = DateTime.Now;
            pages[68].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid02nKVEbkWYXfU9V8jrwJjdNmcEsKmjBGjmx3M9HMJV6ZpVCGBnrRRUMxNi7ZDFR73Rl&show_text=true&width=500\" width=\"500\" height=\"384\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[68].StoryId = 25;

            pages[69] = new UserContent();
            pages[69].Data = DateTime.Now;
            pages[69].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid02HEjjBYdRrvN32QgozDs6b1s55PeRX27hYC96n66WNtxrasAUNUrvHoGLY2eUcL4Dl&show_text=true&width=500\" width=\"500\" height=\"384\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[69].StoryId = 7;

            pages[70] = new UserContent();
            pages[70].Data = DateTime.Now;
            pages[70].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid0bazQeXnER4EPdBgn2VEdRGwPNmS1po8c2Cixevgt7gcq9nGZyfx1V5ufo1snBh2ql&show_text=true&width=500\" width=\"500\" height=\"384\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[70].StoryId = 25;

            pages[71] = new UserContent();
            pages[71].Data = DateTime.Now;
            pages[71].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid02X751YCFZJdLa9dPxM2fsRawmPU2EDL8bivG6pmW52BHKFbNnGyGUxMdEZBZNyKJNl&show_text=true&width=500\" width=\"500\" height=\"384\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[71].StoryId = 1;

            pages[72] = new UserContent();
            pages[72].Data = DateTime.Now;
            pages[72].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid02rrMd1aA57h9qoyRvWKHeR8RVW6WP7pYQ1xo2k16p3MLncfRtZDd6Qo8kb8LBm4Ztl&show_text=true&width=500\" width=\"500\" height=\"384\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[72].StoryId = 1;

            pages[73] = new UserContent();
            pages[73].Data = DateTime.Now;
            pages[73].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid0bbeSNywypter881KuYQiXpyjYo8t1YSuDL1FYKDFaimS9iM3FNbsyJXHvWKUbZ44l&show_text=true&width=500\" width=\"500\" height=\"599\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[73].StoryId = 1;

            pages[74] = new UserContent();
            pages[74].Data = DateTime.Now;
            pages[74].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid02ciqB41cWFCHLVpu85813n9dY6mLtcm3fuBHFMcDcGFe13CrvWz9Xi3ADWzUrBcbl&show_text=true&width=500\" width=\"500\" height=\"358\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[74].StoryId = 1;

            pages[75] = new UserContent();
            pages[75].Data = DateTime.Now;
            pages[75].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid031jcK5kzmAGnvTLLTTSPt2YHgpHuG3sn1JRZwEWzafeLXjWhHvsyptGsN83ZmwBKAl&show_text=true&width=500\" width=\"500\" height=\"481\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[75].StoryId = 22;

            pages[76] = new UserContent();
            pages[76].Data = DateTime.Now;
            pages[76].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid038CCa6ujjUJgNEKLg634Hby4jedBu1wV2Y8ipxb43wnJX2kF8rFqVKbnuJ3KGpobnl&show_text=true&width=500\" width=\"500\" height=\"481\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[76].StoryId = 7;

            pages[77] = new UserContent();
            pages[77].Data = DateTime.Now;
            pages[77].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid02HHKbk2dR3fkzrnzWo67okuBfvTdRRub7XWFNzatfW6AJoYpwGpb15pNujigTZPNl&show_text=true&width=500\" width=\"500\" height=\"481\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[77].StoryId = 25;

            pages[78] = new UserContent();
            pages[78].Data = DateTime.Now;
            pages[78].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid026vGPQvgUPzMJ7AGK29KHMPuokVChfyAAZACDe8Ls8PB848uRKpfowF5RwpcridpZl&show_text=true&width=500\" width=\"500\" height=\"516\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[78].StoryId = 35;

            pages[79] = new UserContent();
            pages[79].Data = DateTime.Now;
            pages[79].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid02Y7pn3zfCvje1J7jrrLuZBtboJkhD2gh5r4q5cokPb4LKMUXsqxcTs4vv319LXPDil&show_text=true&width=500\" width=\"500\" height=\"497\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[79].StoryId = 14;

            pages[80] = new UserContent();
            pages[80].Data = DateTime.Now;
            pages[80].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid0sxuBia73DNgx4isXS5dD53hzj4UUT6AEAdRkmZw4Guv5CDiW78Yrm1TH4mKRJdjgl&show_text=true&width=500\" width=\"500\" height=\"497\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[80].StoryId = 14;

            pages[81] = new UserContent();
            pages[81].Data = DateTime.Now;
            pages[81].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid02vXrxPnfU5R7qj7qWQyvU5C4UmHj2KHmmefj8DRT1HyMMPy5ek1MGM2KE2T7wHiNnl&show_text=true&width=500\" width=\"500\" height=\"384\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[81].StoryId = 26;

            pages[82] = new UserContent();
            pages[82].Data = DateTime.Now;
            pages[82].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid0LdKKha3vMieVnZdiz7eFPjwn4fensvaDUhwTJaDxKq7fBy6mv8i2Z8Sn814jyvFsl&show_text=true&width=500\" width=\"500\" height=\"442\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[82].StoryId = 26;

            pages[83] = new UserContent();
            pages[83].Data = DateTime.Now;
            pages[83].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid02UG447xr6Y4Paz1wmexGvkH7yk5i8n5mXM5sCSKdMphhye1UfrDt98T66ZnMnASLel&show_text=true&width=500\" width=\"500\" height=\"384\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[83].StoryId = 1;            
            
            pages[84] = new UserContent();
            pages[84].Data = DateTime.Now;
            pages[84].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid02Csb5qm9quFKXxomiUiSDZq7A5WiaRsj1fJhj7xJV33bVsLZWkYMrKUCQDQ4rzQTLl&show_text=true&width=500\" width=\"500\" height=\"358\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[84].StoryId = 34;
            
            pages[85] = new UserContent();
            pages[85].Data = DateTime.Now;
            pages[85].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid02vbaKQgKoeG6BYRWb4tzJdGRuQ4NA1kkRzbcar4WRigP1q78gb24e7UY8BXTbHcAWl&show_text=true&width=500\" width=\"500\" height=\"384\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[85].StoryId = 1;
            
            pages[86] = new UserContent();
            pages[86].Data = DateTime.Now;
            pages[86].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid02zK3fgYR9K8gXtqUGKfuMN9RHKQtNv34VngoJPEVjXBC76obTDqu1RfEBo73AHBQ8l&show_text=true&width=500\" width=\"500\" height=\"384\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[86].StoryId = 1;

            pages[87] = new UserContent();
            pages[87].Data = DateTime.Now;
            pages[87].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid02GQ2gTA1wow6AB6yauDMp4ufg2ViqjG6P4v98iKUk4B5T2b3hiSsF8HhEnkxxxMePl&show_text=true&width=500\" width=\"500\" height=\"250\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[87].StoryId = 1;

            pages[88] = new UserContent();
            pages[88].Data = DateTime.Now;
            pages[88].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid0w23EdJHV2bGNg2ZZYBPntE6ycHVc8rZYmr6xAVDPNhZCAeHw5s8ZWL59hUtErsKjl&show_text=true&width=500\" width=\"500\" height=\"329\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[88].StoryId = 1;

            pages[89] = new UserContent();
            pages[89].Data = DateTime.Now;
            pages[89].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid0ggx5Uzka8uiJq9qtGJyptYUxKjMFQSoN9gcUQB4nhUmMjrnjmKc1CTzLoqUsqzxVl&show_text=true&width=500\" width=\"500\" height=\"358\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[89].StoryId = 1;

            pages[90] = new UserContent();
            pages[90].Data = DateTime.Now;
            pages[90].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid0dsnoaDBH9jv2zvkrHfr39Akwbxaim2iWf2FTReY8utu6iEtHN1iam8yhvM92zjgkl&show_text=true&width=500\" width=\"500\" height=\"329\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[90].StoryId = 25;

            pages[91] = new UserContent();
            pages[91].Data = DateTime.Now;
            pages[91].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid02MgUQbqoYp2qtt5i45oJxGBuLZyFBcNTmpzgpwnCQrqgDiBVpEDv9xyZmiXkxwZKgl&show_text=true&width=500\" width=\"500\" height=\"384\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[91].StoryId = 1;

            pages[92] = new UserContent();
            pages[92].Data = DateTime.Now;
            pages[92].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid0M7GshTGE2CVSD6b1Ab8i1ysJo53mVyAdNyjGMEd9MD4Pfkv2QQ1nngYf66D7oeTCl&show_text=true&width=500\" width=\"500\" height=\"384\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[92].StoryId = 27;

            pages[93] = new UserContent();
            pages[93].Data = DateTime.Now;
            pages[93].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid026WZsLwuNwWZ1NcT24ANtGLn1Zi4cvb2gitC1iFsh4VqEcrbFMUBzWNw6KDPkLH3Gl&show_text=true&width=500\" width=\"500\" height=\"384\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[93].StoryId = 23;

            pages[94] = new UserContent();
            pages[94].Data = DateTime.Now;
            pages[94].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid02bSoHU7GyMUK9rPwDhKa4JPY4NSWVWN3c1Y2QLEPrBgQMWZiH9dhCCGMpNTkVSF93l&show_text=true&width=500\" width=\"500\" height=\"384\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[94].StoryId = 7;

            pages[95] = new UserContent();
            pages[95].Data = DateTime.Now;
            pages[95].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid02dicfwxt3jh4b15YkxrJGySf5DJkikWXiMvbqgHnVaqLPJiC8YV3tMtmdmnj3xHuKl&show_text=true&width=500\" width=\"500\" height=\"384\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[95].StoryId = 23;

            pages[96] = new UserContent();
            pages[96].Data = DateTime.Now;
            pages[96].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid07QzLZT5MzH7bhNkUeTjCStmhGtWQPZH2Mt8oBsF7XgMHhJyFWuyDWV4nnqEtNpBPl&show_text=true&width=500\" width=\"500\" height=\"384\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[96].StoryId = 1;

            pages[97] = new UserContent();
            pages[97].Data = DateTime.Now;
            pages[97].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid0181VE8kZNrfkxZbzHRRFg3XtFgo3vwj7w3VL3YX1Qrgu3FnFUijamuajGgC6YWXVl&show_text=true&width=500\" width=\"500\" height=\"384\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[97].StoryId = 26;

            pages[98] = new UserContent();
            pages[98].Data = DateTime.Now;
            pages[98].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid02y5d7KzpYACyQadbhuQ2ZNq2q7oCw9btNSRn7D7CPekqo8jaAD9dE2GQ9u625jzMNl&show_text=true&width=500\" width=\"500\" height=\"250\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[98].StoryId = 32;

            pages[99] = new UserContent();
            pages[99].Data = DateTime.Now;
            pages[99].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid02JGFrozxbpj3S8NVExrJe2j1MbGRLZFV8wTCY4C2NxkMfd2joG96NXuCVPdjMnykcl&show_text=true&width=500\" width=\"500\" height=\"481\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[99].StoryId = 33;

            pages[100] = new UserContent();
            pages[100].Data = DateTime.Now;
            pages[100].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid02ZEhJsccSDV8dFzdt3uwZSa31hzz4iKhDtUYWUrUviPK8XNzPgnN6Lj2D6kUfhwbhl&show_text=true&width=500\" width=\"500\" height=\"384\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[100].StoryId = 1;

            pages[101] = new UserContent();
            pages[101].Data = DateTime.Now;
            pages[101].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid0C9YUvxUmufECDGL8iwh3QLsbLQM4hvwxH4a2rYPskdqS6Sxz4QMZhGw93Rg3FR2Ul&show_text=true&width=500\" width=\"500\" height=\"384\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[101].StoryId = 26;

            pages[102] = new UserContent();
            pages[102].Data = DateTime.Now;
            pages[102].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid02hRm25Di4dB1xqQNnqR2pJKDJedLVX1ttY5rAHdHPbyzAdwXsoF3iTgwzcG8KiVDCl&show_text=true&width=500\" width=\"500\" height=\"329\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[102].StoryId = 1;

            pages[103] = new UserContent();
            pages[103].Data = DateTime.Now;
            pages[103].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid04tKXTGU3Bp4brZN4Sd6N2zU76AdqicRikZxYMhM4MK6k5YUnJ8SmqKvk43U7V2vgl&show_text=true&width=500\" width=\"500\" height=\"378\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[103].StoryId = 34;

            pages[104] = new UserContent();
            pages[104].Data = DateTime.Now;
            pages[104].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid0e1eqhZsx9wmDoyoCF44r1XHG3TSxnNTnAU6NWkh2kErj7RHPZCMAEZSVm715TcKsl&show_text=true&width=500\" width=\"500\" height=\"384\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[104].StoryId = 25;

            pages[105] = new UserContent();
            pages[105].Data = DateTime.Now;
            pages[105].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid0rHNksb8JBHVCg3X6LABCkYu5YTF4q3ThVJGGeUJRQyBzm1LjtYFg4441MLNn5ft2l&show_text=true&width=500\" width=\"500\" height=\"452\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[105].StoryId = 1;

            pages[106] = new UserContent();
            pages[106].Data = DateTime.Now;
            pages[106].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid02NyPdmduqqQswBWitTipioiJd6V44oKnktUL4Jr4Qkuf9CaRTBNkpwgNDJSSQVxf4l&show_text=true&width=500\" width=\"500\" height=\"384\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[106].StoryId = 1;

            pages[107] = new UserContent();
            pages[107].Data = DateTime.Now;
            pages[107].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid06oPPmP2qLi5FekTHvBaJv6ccY3PnjzcMpnAVQNK1gN9UEg6NVDzm5WDXe8b8my1Bl&show_text=true&width=500\" width=\"500\" height=\"384\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[107].StoryId = 7;

            pages[108] = new UserContent();
            pages[108].Data = DateTime.Now;
            pages[108].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid0G9ixmq3ddCb3sTDLvKho1sKPPZFai9ytftoXc93sAk5wQmGMoUSxrundLNmkpKcRl&show_text=true&width=500\" width=\"500\" height=\"384\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[108].StoryId = 25;

            pages[109] = new UserContent();
            pages[109].Data = DateTime.Now;
            pages[109].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid02UsSL9dP5KxSH71caoUtKiFy7xeNcCktY4uxYcQgYgqX4SRxjwx3zTxVBfBqh9jzal&show_text=true&width=500\" width=\"500\" height=\"481\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[109].StoryId = 11;

            pages[110] = new UserContent();
            pages[110].Data = DateTime.Now;
            pages[110].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid07HTH9TzuvxDziMwA5T3v9qst7SkqCxKSvVJ1a86SCWWarDThxBSv2LAgD1FLP9USl&show_text=true&width=500\" width=\"500\" height=\"384\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[110].StoryId = 4;

            pages[111] = new UserContent();
            pages[111].Data = DateTime.Now;
            pages[111].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid02LqL2LeAy3jezAYZztToqVCZEhEo3JmAsA8XqQ39CfSeDNXocnaBXiThCLxPvjycgl&show_text=true&width=500\" width=\"500\" height=\"481\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[111].StoryId = 5;

            pages[112] = new UserContent();
            pages[112].Data = DateTime.Now;
            pages[112].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid022UK3EDzoavs7ZhevN9NEot3hc7tCHNgWYR7xnzGwQfEzhJq8seJ4dPazyCich7YHl&show_text=true&width=500\" width=\"500\" height=\"497\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[112].StoryId = 30;

            pages[113] = new UserContent();
            pages[113].Data = DateTime.Now;
            pages[113].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid0335uvZ59AoTTtrv2s58mPpMBaAzrLAeCB2ff7naWfgFt7cZxFBHpupuXLVa7oXr3jl&show_text=true&width=500\" width=\"500\" height=\"481\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[113].StoryId = 9;

            pages[114] = new UserContent();
            pages[114].Data = DateTime.Now;
            pages[114].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid02TvBET5V7aKAM2S8Uh8yJRGtoZHUjYrXDcvrHki6HP4J9LzZLrcLTJE6xr9YnN4Val&show_text=true&width=500\" width=\"500\" height=\"384\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[114].StoryId = 25;

            pages[115] = new UserContent();
            pages[115].Data = DateTime.Now;
            pages[115].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid0qyTx6MMFviJ1mE1Dokiqh8Ae8BWMrSo71aLda9zm4JnWggKM19K6iCdCih5XdJUnl&show_text=true&width=500\" width=\"500\" height=\"384\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[115].StoryId = 14;

            pages[116] = new UserContent();
            pages[116].Data = DateTime.Now;
            pages[116].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid0b9SEWLp51rfGJuFs3ChaVQEyc9wZdDy1fnRh7cA6jdYZgrdR3yGQm4yhbxxiaPkUl&show_text=true&width=500\" width=\"500\" height=\"384\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[116].StoryId = 7;

            pages[117] = new UserContent();
            pages[117].Data = DateTime.Now;
            pages[117].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid02qk97vQNbEVoFsVk2YY4F5tH5eGDPqkXjz4Lk1UEHLqspUZGPjeBLYXauynsvoAHDl&show_text=true&width=500\" width=\"500\" height=\"452\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[117].StoryId = 33;

            pages[118] = new UserContent();
            pages[118].Data = DateTime.Now;
            pages[118].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid02mzp696T5n9eXaJRZ78gAkrT7pTPnEtrNhLRvsesudZ8eJ2rPFtrGSxbASxsAESFwl&show_text=true&width=500\" width=\"500\" height=\"384\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[118].StoryId = 14;

            pages[119] = new UserContent();
            pages[119].Data = DateTime.Now;
            pages[119].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid02EVj9pTja9vnVrZBLKTW6kuKAFFcerWJmQtNWmsTfvy3rFvKvwpzL5P35NDDZKp7jl&show_text=true&width=500\" width=\"500\" height=\"384\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[119].StoryId = 14;

            pages[120] = new UserContent();
            pages[120].Data = DateTime.Now;
            pages[120].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid02P1ggF8QDAoZuMcZUSsktNxCv5tPG71DEQ9HiEocNXLLWDCN2wosJvhyThgkud9USl&show_text=true&width=500\" width=\"500\" height=\"452\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[120].StoryId = 9;

            pages[121] = new UserContent();
            pages[121].Data = DateTime.Now;
            pages[121].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid02z2ECjLBrM5qazo2jkuQ6kL8CbGwBgaqsoTJgvH81kC3SQKVZS2RAvMADuEFWDka4l&show_text=true&width=500\" width=\"500\" height=\"384\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[121].StoryId = 5;

            pages[122] = new UserContent();
            pages[122].Data = DateTime.Now;
            pages[122].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid0NH6RrYbNNr5B3Crx8bK8JKpPsCMeU2pjDFyJF1TrJNZeRH59BwbKhQHc4YLpazpQl&show_text=true&width=500\" width=\"500\" height=\"384\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[122].StoryId = 1;

            pages[123] = new UserContent();
            pages[123].Data = DateTime.Now;
            pages[123].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid02MMubXPVrzNLDHpnJz3MwFpNP2rcuWgWWAFhiYFvp7fJMAMvT5xae2apz8NtMWywdl&show_text=true&width=500\" width=\"500\" height=\"384\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[123].StoryId = 14;

            pages[124] = new UserContent();
            pages[124].Data = DateTime.Now;
            pages[124].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid02Um6wwsc9UtUaLrzs3Pk1h3SfbM1FzqrkmGE1WCxu7oVHf111B3E9H1vqahLp8Lc4l&show_text=true&width=500\" width=\"500\" height=\"384\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[124].StoryId = 5;

            pages[125] = new UserContent();
            pages[125].Data = DateTime.Now;
            pages[125].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid024ipR7eYaq2hWFVGkawyZNcLwrJzdY2HiVCK7jD3nzK1xbWWiTLdnvJfLAe4GpMNMl&show_text=true&width=500\" width=\"500\" height=\"384\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[125].StoryId = 5;

            pages[126] = new UserContent();
            pages[126].Data = DateTime.Now;
            pages[126].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid02Q4dBHTBZ3HjrTtNa8kykhrsLDBakMeDmehNSMKAYPqx2mHyDh3JjmdJTc9sJrDmfl&show_text=true&width=500\" width=\"500\" height=\"358\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[126].StoryId = 1;

            pages[127] = new UserContent();
            pages[127].Data = DateTime.Now;
            pages[127].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid02adjGuzdD4wnFwxXf71oXfQqiXHxQJQzPBHU7KAfMucyTzm1FQnsw4TMX5a41pYUAl&show_text=true&width=500\" width=\"500\" height=\"384\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[127].StoryId = 34;

            pages[128] = new UserContent();
            pages[128].Data = DateTime.Now;
            pages[128].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid02NcKBkzXYLWwX6qtWqJ4azgPeHw4oMbhoMKX7GpcmfG1u4NXHbTrg69Re3Pxb2wCwl&show_text=true&width=500\" width=\"500\" height=\"599\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[128].StoryId = 1;

            pages[129] = new UserContent();
            pages[129].Data = DateTime.Now;
            pages[129].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid0CYofU8EmereJb4FzEHz3kq5CVFG5HWoj4i8BFhKc17YYCqecnjqMpvTcn8jr6YWQl&show_text=true&width=500\" width=\"500\" height=\"575\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[129].StoryId = 1;

            pages[130] = new UserContent();
            pages[130].Data = DateTime.Now;
            pages[130].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid0hWTUSPhdpFAQ6ym6U5Sk9vUQ3PdsyS4ybdMTtwMR7En9KdsWDQC28XxK2fBerDkKl&show_text=true&width=500\" width=\"500\" height=\"384\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[130].StoryId = 4;

            pages[131] = new UserContent();
            pages[131].Data = DateTime.Now;
            pages[131].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid02LTbXAJypJPz39XfzTLs6PYF4oXqASaNxFHc5sr556HMcujbYsq6db6WGsDtyLwuPl&show_text=true&width=500\" width=\"500\" height=\"384\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[131].StoryId = 4;

            pages[132] = new UserContent();
            pages[132].Data = DateTime.Now;
            pages[132].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid02Re2sFrFpFCtVNejp4LXCsk7MacHGUU796ztoebBeZSs2XfvjR3Bss2YKkAXWMhzql&show_text=true&width=500\" width=\"500\" height=\"384\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[132].StoryId = 1;

            pages[133] = new UserContent();
            pages[133].Data = DateTime.Now;
            pages[133].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid02hTqNM53nGHXA6WiyPJG37HBPnZPq45BcpWK7qLdx6QRQ77EHKVA9ey4Z1AeUKs1Zl&show_text=true&width=500\" width=\"500\" height=\"384\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[133].StoryId = 1;

            pages[134] = new UserContent();
            pages[134].Data = DateTime.Now;
            pages[134].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid02yanLY7LN37ubqCn8f4ZKkrqzU2csnzkFjfsnUBPLrsjCewCWDxcbegTfGuicvnmWl&show_text=true&width=500\" width=\"500\" height=\"384\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[134].StoryId = 14;

            pages[135] = new UserContent();
            pages[135].Data = DateTime.Now;
            pages[135].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid0xJX9hX1nBGYWbRPKT9mc298D2hfXScYrv2X5mSoqXi3BJ2r8NEvUx1rSgHkJ3bShl&show_text=true&width=500\" width=\"500\" height=\"384\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[135].StoryId = 17;

            pages[136] = new UserContent();
            pages[136].Data = DateTime.Now;
            pages[136].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid02pdFu9H3hPtYCE2oncFDnpb9ky9ukvs8JLkvgWdUpiUi1zcM3WydYwuYzSZn1LMGl&show_text=true&width=500\" width=\"500\" height=\"452\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[136].StoryId = 20;

            pages[137] = new UserContent();
            pages[137].Data = DateTime.Now;
            pages[137].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid0bSFhTZ3Mz6YH3MW8KqvZrF92mPngqfPoXGCdoWNFPSZ7wizigStZrS7XKhTHLyo5l&show_text=true&width=500\" width=\"500\" height=\"384\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[137].StoryId = 7;

            pages[138] = new UserContent();
            pages[138].Data = DateTime.Now;
            pages[138].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid0w5XxK25NaJFu459UWZdFCPtEH2PY1Mc2vDagCw7nPWk258p4AsZ2AUqhpPdUA2Lzl&show_text=true&width=500\" width=\"500\" height=\"458\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[138].StoryId = 1;

            pages[139] = new UserContent();
            pages[139].Data = DateTime.Now;
            pages[139].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid0vLwbak6AfxBidWom3AjHz6RQ5wu7XepHkMMbqGk9WN7G4ghziWEjLf1TN6fPZCtAl&show_text=true&width=500\" width=\"500\" height=\"458\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[139].StoryId = 7;

            pages[140] = new UserContent();
            pages[140].Data = DateTime.Now;
            pages[140].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid0oBcgmzqG5neBpNo6RcTcGzLK3BnWcP17d7QJfUyzggtye8uVhW5mJ3oSBjJH6GjRl&show_text=true&width=500\" width=\"500\" height=\"458\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[140].StoryId = 25;

            pages[141] = new UserContent();
            pages[141].Data = DateTime.Now;
            pages[141].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid0coeektjWueq1mf2uATiVACpZtM6JEQz6Jy9WE4B1Pj7AAkdn7dAG8tQEbvg5rVE3l&show_text=true&width=500\" width=\"500\" height=\"458\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[141].StoryId = 25;

            pages[142] = new UserContent();
            pages[142].Data = DateTime.Now;
            pages[142].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid0h2VvSWVL9ueC5ATA2KXmbnMQdHgBuKXkLs8JdybpPZhwugizYxifXZwfoy6qB3gCl&show_text=true&width=500\" width=\"500\" height=\"458\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[142].StoryId = 25;

            pages[143] = new UserContent();
            pages[143].Data = DateTime.Now;
            pages[143].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid0xRQCpHZafWdVdvcUpB8QUN65dBX8kCkjCwG9sMB5b5tsGEeeaxHrPBfB1uAUGGvVl&show_text=true&width=500\" width=\"500\" height=\"458\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[143].StoryId = 1;

            pages[144] = new UserContent();
            pages[144].Data = DateTime.Now;
            pages[144].Html = "<iframe src=\"https://www.facebook.com/plugins/post.php?href=https%3A%2F%2Fwww.facebook.com%2Fleandroprogramador%2Fposts%2Fpfbid0q71ZEifP292bbDj9ubBRYNcCcmuxENPNRW5b66XbLvvg2UZyG3VN6qj3f3LovLBFl&show_text=true&width=500\" width=\"500\" height=\"458\" style=\"border:none;overflow:hidden\" scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\" allow=\"autoplay; clipboard-write; encrypted-media; picture-in-picture; web-share\"></iframe>";
            pages[144].StoryId = 7;

           // pages[i - 1] = new UserContent();
           // pages[i - 1].Data = DateTime.Now;
           // pages[i - 1].Html = "";
           // pages[i - 1].StoryId = 17;
        for (var i = 0; i < pages.Length; i++)
        {
            contexto.Add(pages[i]);
            contexto.SaveChanges();

        }
    }


    string[] rolesNames = { "Admin", "Manager", "Assinante" };
    IdentityResult result;

    foreach (var namesRole in rolesNames)
    {
        var roleExist = await roleManager.RoleExistsAsync(namesRole);
        if (!roleExist)
        {
            result = await roleManager.CreateAsync(new IdentityRole(namesRole));
        }
    }

    if (userASP == null)
    {
        var user = new UserModel() 
        { 
            UserName = "leandro01832",
            Email = email, EmailConfirmed = true,
            HashUserName = BCrypt.Net.BCrypt.HashPassword("leandro01832")
        };
        await userManager.CreateAsync(user, password);
        await userManager.AddToRoleAsync(user, "Admin");
    }   

    if (!await contexto!.Set<Story>().AnyAsync())
    {

        Story padrao = null;
        padrao = new PatternStory("Padrao", padrao)
        {
            Capitulo = 0
        };
        contexto.Add(padrao);
        contexto.SaveChanges();
        var str = new PatternStory("seres vivos", contexto.Story.Include(s => s.Pagina).OrderBy(s => s.Id).First())
        {
            Capitulo = 1
        };
        contexto.Add(str);
        contexto.SaveChanges();


        Pagina[] pages = new Pagina[99];
        Pagina[] pages2 = new Pagina[2000];
        Pagina[] pages3 = new Pagina[12000];
       
        for (var i = 1; i <= 99; i++)
        {


            pages[i - 1] = new Pagina(i);
            pages[i - 1].Titulo = "pagina";
            pages[i - 1].StoryId = 2;
            pages[i - 1].Titulo += $" {i}";
            



            if (i == 1)
            {
                pages[i - 1].Html = null;

            }

            if (i == 2)
            {
                pages[i - 1].Html = "<p style=\"text-align: center;\"><iframe title=\"ENCONTRANDO UM GIGANTE NA NATUREZA, A BALEIA CINZENTA! | RICHARD RASMUSSEN\" src=\"https://www.youtube.com/embed/r_eLOG096sE\" width=\"320\" height=\"560\" frameborder=\"0\" allowfullscreen=\"allowfullscreen\"></iframe></p>";

            }

            if (i == 3)
            {
                pages[i - 1].Html = "<p>A&nbsp;<strong>baleia-cinzenta</strong>&nbsp;(<em>Eschrichtius robustus</em>) &eacute; um&nbsp;<a class=\"mw-redirect\" title=\"Mam&iacute;fero\" href=\"https://pt.wikipedia.org/wiki/Mam%C3%ADfero\">mam&iacute;fero</a>&nbsp;<a class=\"mw-redirect\" title=\"Cet&aacute;ceo\" href=\"https://pt.wikipedia.org/wiki/Cet%C3%A1ceo\">cet&aacute;ceo</a>&nbsp;da fam&iacute;lia dos&nbsp;<a class=\"mw-redirect\" title=\"Eschrichtiidae\" href=\"https://pt.wikipedia.org/wiki/Eschrichtiidae\">escrict&iacute;deos</a>. &Eacute; a &uacute;nica esp&eacute;cie viva em seu g&ecirc;nero e fam&iacute;lia, mas uma esp&eacute;cie extinta foi descoberta e colocada no g&ecirc;nero em 2017, a&nbsp;<a class=\"new\" title=\"Baleia Akishima (p&aacute;gina n&atilde;o existe)\" href=\"https://pt.wikipedia.org/w/index.php?title=Baleia_Akishima&amp;action=edit&amp;redlink=1\">Baleia Akishima</a>.<sup id=\"cite_ref-N&atilde;o_nomeado-20230316141054_2-0\" class=\"reference\"><a href=\"https://pt.wikipedia.org/wiki/Baleia-cinzenta#cite_note-N%C3%A3o_nomeado-20230316141054-2\">[2]</a></sup></p>\r\n<h2><span id=\"Descri.C3.A7.C3.A3o\"></span><span id=\"Descri&ccedil;&atilde;o\" class=\"mw-headline\">Descri&ccedil;&atilde;o</span></h2>\r\n<figure class=\"mw-halign-left\"><a class=\"mw-file-description\" href=\"https://pt.wikipedia.org/wiki/Ficheiro:Eschrichtius_robustus_01-cropped.jpg\"><img class=\"mw-file-element\" src=\"https://upload.wikimedia.org/wikipedia/commons/thumb/c/c6/Eschrichtius_robustus_01-cropped.jpg/210px-Eschrichtius_robustus_01-cropped.jpg\" srcset=\"//upload.wikimedia.org/wikipedia/commons/thumb/c/c6/Eschrichtius_robustus_01-cropped.jpg/315px-Eschrichtius_robustus_01-cropped.jpg 1.5x, //upload.wikimedia.org/wikipedia/commons/thumb/c/c6/Eschrichtius_robustus_01-cropped.jpg/420px-Eschrichtius_robustus_01-cropped.jpg 2x\" width=\"210\" height=\"133\" data-file-width=\"800\" data-file-height=\"508\" /></a>\r\n<figcaption>Uma baleia-cinzenta&nbsp;<a title=\"Comportamento de superf&iacute;cie dos cet&aacute;ceos\" href=\"https://pt.wikipedia.org/wiki/Comportamento_de_superf%C3%ADcie_dos_cet%C3%A1ceos\">saltando</a>.</figcaption>\r\n</figure>\r\n<p>Os indiv&iacute;duos j&aacute; foram chamados de &ldquo;peixes do diabo&rdquo; porque s&atilde;o muito resistentes e brigam quando ca&ccedil;ados. Esse nome entretanto est&aacute; biologicamente incorreto, pois as baleias n&atilde;o s&atilde;o peixes.<sup id=\"cite_ref-3\" class=\"reference\"><a href=\"https://pt.wikipedia.org/wiki/Baleia-cinzenta#cite_note-3\">[3]</a></sup>&nbsp;Seu tamanho pode atingir cerca de 15 metros de comprimento e pesar cerca de 35 toneladas. Sua alimenta&ccedil;&atilde;o &eacute; a base de anf&iacute;podos (pequenos crust&aacute;ceos que vivem na &aacute;gua ou pr&oacute;ximo, incluindo pulgas de areia e piolhos de baleia), krill, pl&acirc;ncton e moluscos. Ao contr&aacute;rio de outros cet&aacute;ceos, a baleia-cinzenta tende a alimentar-se junto ao fundo do mar, onde agita a &aacute;gua para levantar material do fundo de onde consegue filtrar os seus alimentos. A distribui&ccedil;&atilde;o atual e contida ao Oceano&nbsp;<a title=\"Oceano Pac&iacute;fico\" href=\"https://pt.wikipedia.org/wiki/Oceano_Pac%C3%ADfico\">pac&iacute;fico</a>. A baleia-cinzenta tamb&eacute;m ocorre em &aacute;guas litorais desde o mar de Okhotsk at&eacute; a Coreia do Sul e Jap&atilde;o e desde os mares de Chukchi e de Beaufort no golfo do M&eacute;xico.</p>\r\n<p>O recorde de cerca de maior dist&acirc;ncia percorrida por um vertebrado marinho j&aacute; registrada pertence a uma baleia-cinzenta que que nadou 26,8 mil quil&ocirc;metros. O cet&aacute;ceo em quest&atilde;o &eacute; um macho de 12 metros de comprimento avistado pr&oacute;ximo &agrave;&nbsp;<a title=\"Nam&iacute;bia\" href=\"https://pt.wikipedia.org/wiki/Nam%C3%ADbia\">Nam&iacute;bia</a>&nbsp;em 2013. Ele tamb&eacute;m &eacute; a primeira e &uacute;nica baleia-cinzenta a ser registrada no&nbsp;<a title=\"Hemisf&eacute;rio sul\" href=\"https://pt.wikipedia.org/wiki/Hemisf%C3%A9rio_sul\">Hemisf&eacute;rio Sul</a>.<sup id=\"cite_ref-4\" class=\"reference\"><a href=\"https://pt.wikipedia.org/wiki/Baleia-cinzenta#cite_note-4\">[4]</a></sup></p>";

            }

            if (i == 4)
            {
                pages[i - 1].Html = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://static.nationalgeographicbrasil.com/files/styles/image_3200/public/w7rx5g.webp?w=1450&amp;h=816\" alt=\"\" width=\"320\" height=\"180\" /></p>";

            }

            if (i == 5)
            {
                pages[i - 1].Html = "<p style=\"text-align: center;\"><iframe title=\"AS CINCO BALEIAS MAIS INCR&Iacute;VEIS QUE EU J&Aacute; ENCONTREI! | RICHARD RASMUSSEN\" src=\"https://www.youtube.com/embed/ui70_rhmGtM\" width=\"320\" height=\"560\" frameborder=\"0\" allowfullscreen=\"allowfullscreen\"></iframe></p>";

            }

            if (i == 6)
            {
                pages[i - 1].Html = "<p style=\"text-align: center;\"><iframe title=\"UMA ON&Ccedil;A-PINTADA SELVAGEM INVADIU O INSTITUTO! | RICHARD RASMUSSEN\" src=\"https://www.youtube.com/embed/c6zPTqvDDN8\" width=\"320\" height=\"560\" frameborder=\"0\" allowfullscreen=\"allowfullscreen\"></iframe></p>";

            }

            if (i == 7)
            {
                pages[i - 1].Html = "<p style=\"text-align: center;\"><iframe title=\"OS R&Eacute;PTEIS DA MATA ATL&Acirc;NTICA! | RICHARD RASMUSSEN\" src=\"https://www.youtube.com/embed/1zueHDxHJWA\" width=\"320\" height=\"560\" frameborder=\"0\" allowfullscreen=\"allowfullscreen\"></iframe></p>";

            }

            if (i == 8)
            {
                pages[i - 1].Html = "<p style=\"text-align: center;\"><iframe title=\"UMA SUCURI PRETA GIGANTE! | RICHARD RASMUSSEN\" src=\"https://www.youtube.com/embed/-2PjgVtX_bs\" width=\"320\" height=\"560\" frameborder=\"0\" allowfullscreen=\"allowfullscreen\"></iframe></p>";

            }

            if (i == 9)
            {
                pages[i - 1].Html = "<p style=\"text-align: center;\"><iframe title=\"UM ANIMAL COM MAIS DE 300 MILH&Otilde;ES DE ANOS DE EVOLU&Ccedil;&Atilde;O! | BRASIL BIOMAS\" src=\"https://www.youtube.com/embed/zKcPVi43gqo\" width=\"320\" height=\"560\" frameborder=\"0\" allowfullscreen=\"allowfullscreen\"></iframe></p>";

            }

            if (i == 10)
            {
                pages[i - 1].Html = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://cdn.ambientes.ambientebrasil.com.br/wp-content/uploads/2020/11/jacare-2645898_1280.jpg\" alt=\"\" width=\"320\" height=\"213\" /></p>";

            }

            if (i == 11)
            {
                pages[i - 1].Html = "<p style=\"text-align: center;\"><iframe title=\"UMA CACATUA MUITO LOUCA,  UM TUCANO RESMUNG&Atilde;O E A ARARA AZUL MAIS FOFA DO MUNDO | RICHARD RASMUSSEN\" src=\"https://www.youtube.com/embed/KBDUODeW5vI\" width=\"320\" height=\"560\" frameborder=\"0\" allowfullscreen=\"allowfullscreen\"></iframe></p>";

            }

            if (i == 12)
            {
                pages[i - 1].Html = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://s2-g1.glbimg.com/FR6jrT4gKQuTXit78yN94mCkhes=/0x0:1900x1461/1008x0/smart/filters:strip_icc()/i.s3.glbimg.com/v1/AUTH_59edd422c0c84a879bd37670ae4f538a/internal_photos/bs/2019/1/g/BjKNYuSBeVn9zupTRamA/araraazul.jpg\" alt=\"\" width=\"320\" height=\"246\" /></p>";

            }

            if (i == 13)
            {
                pages[i - 1].Html = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"http://www.fiocruz.br/biosseguranca/Bis/infantil/araraazul.jpg\" alt=\"\" width=\"320\" height=\"258\" /></p>";

            }

            if (i == 14)
            {
                pages[i - 1].Html = "<p style=\"text-align: center;\"><iframe title=\"OS 5 INSETOS MAIS PERIGOSOS DO MUNDO! | RICHARD RASMUSSEN\" src=\"https://www.youtube.com/embed/ZpKZpv7XGsY\" width=\"320\" height=\"560\" frameborder=\"0\" allowfullscreen=\"allowfullscreen\"></iframe></p>";

            }

            if (i == 15)
            {
                pages[i - 1].Html = "<p style=\"text-align: center;\"><iframe title=\"&quot;Explorando a Eleg&acirc;ncia Natural: Conhe&ccedil;a a Fascinante Palmeira Azul de Madagascar 🌴💙\" src=\"https://www.youtube.com/embed/9-EOAxUcwiA\" width=\"320\" height=\"560\" frameborder=\"0\" allowfullscreen=\"allowfullscreen\"></iframe></p>";

            }

            if (i == 16)
            {
                pages[i - 1].Html = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://blog.cobasi.com.br/wp-content/webpc-passthru.php?src=https://blog.cobasi.com.br/wp-content/uploads/2020/07/Tipos-de-orqui%CC%81deas-Phalaenopsis.png&amp;nocache=1\" alt=\"\" width=\"320\" height=\"213\" /></p>";

            }

            if (i == 17)
            {
                pages[i - 1].Html = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://images.tcdn.com.br/img/img_prod/660625/orquidea_cattleya_blc_durigan_big_spots_adulta_1113_1_8900f911dbede8793c3bf2dc131fb647.jpg\" alt=\"\" width=\"320\" height=\"320\" /></p>";

            }

            if (i == 18)
            {
                pages[i - 1].Html = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://static.giulianaflores.com.br/images/product/27330gg.jpg?ims=405x405\" alt=\"\" width=\"320\" height=\"320\" /></p>";

            }

            if (i == 19)
            {
                pages[i - 1].Html = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://s2.glbimg.com/7dJjXrhNzxwRW4jboPSWiEuZZ5Q=/620x620/smart/e.glbimg.com/og/ed/f/original/2022/07/22/orquidea-cymbidium-como-cuidar-e-cultivar-5.jpg\" alt=\"\" width=\"320\" height=\"320\" /></p>";

            }

            if (i == 20)
            {
                pages[i - 1].Html = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://weflores.com/wp-content/uploads/Orqu%C3%ADdea-phalaenopsis-cascata-pink-em-vaso-riscatto.jpg\" alt=\"\" width=\"320\" height=\"320\" /></p>";

            }

            if (i == 21)
            {
                pages[i - 1].Html = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://http2.mlstatic.com/D_NQ_NP_774145-MLB41462303073_042020-O.webp\" alt=\"\" width=\"320\" height=\"320\" /></p>";

            }

            if (i == 22)
            {
                pages[i - 1].Html = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://http2.mlstatic.com/D_NQ_NP_2X_951320-MLB48602367585_122021-F.webp\" alt=\"\" width=\"320\" height=\"320\" /></p>";

            }

            if (i == 23)
            {
                pages[i - 1].Html = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://static.mundoeducacao.uol.com.br/mundoeducacao/conteudo_legenda/c4f684947cd4a982e58416940e5ea7c1.jpg\" alt=\"\" width=\"320\" height=\"214\" /></p>";

            }

            if (i == 24)
            {
                pages[i - 1].Html = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://www.tgservices.com.br/wp-content/uploads/2021/10/26-10-2021.jpg\" alt=\"\" width=\"320\" height=\"200\" /></p>";

            }

            if (i == 25)
            {
                pages[i - 1].Html = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://emsinapse.files.wordpress.com/2021/09/louva-deus-653x330-1.jpg\" alt=\"\" width=\"320\" height=\"162\" /></p>";

            }

            if (i == 26)
            {
                pages[i - 1].Html = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://scontent.fiza4-1.fna.fbcdn.net/v/t1.18169-9/13331074_1813075485589893_2720588748137086315_n.jpg?_nc_cat=105&amp;ccb=1-7&amp;_nc_sid=c2f564&amp;_nc_ohc=6qQPmnz-0eoAX_AD6-E&amp;_nc_ht=scontent.fiza4-1.fna&amp;oh=00_AfAr5C_St05ZZed7c2yob_t7fyErJvBeAtK_ENelgSj10A&amp;oe=6554A8E9\" alt=\"\" width=\"320\" height=\"193\" /></p>";

            }

            if (i == 27)
            {
                pages[i - 1].Html = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://integralismo.org.br/wp-content/uploads/2019/05/9222c546c490561f6c8f63e6d35ffad8.jpg\" alt=\"\" width=\"320\" height=\"240\" /></p>";

            }

            if (i == 28)
            {
                pages[i - 1].Html = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://static.biologianet.com/2020/05/onca-pintada.jpg\" alt=\"\" width=\"320\" height=\"213\" /></p>";

            }

            if (i == 29)
            {
                pages[i - 1].Html = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://portalamazonia.com/images/p/24827/Foto_1_WWF_AFP.jpg\" alt=\"\" width=\"320\" height=\"213\" /></p>";

            }

            if (i == 30)
            {
                pages[i - 1].Html = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://www.aqualovers.pt/images/42/amphiprion-ocellaris_large.jpg\" alt=\"\" width=\"320\" height=\"320\" /></p>";

            }

            if (i == 31)
            {
                pages[i - 1].Html = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://static.mundoeducacao.uol.com.br/mundoeducacao/2022/05/peixe-leao.jpg\" alt=\"\" width=\"320\" height=\"213\" /></p>";

            }

            if (i == 32)
            {
                pages[i - 1].Html = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://euamomeusanimais.com.br/wp-content/uploads/2013/10/Cavalo-Marinho-800x500.jpg\" alt=\"\" width=\"320\" height=\"200\" /></p>";

            }

            if (i == 33)
            {
                pages[i - 1].Html = "<p style=\"text-align: center;\"><iframe title=\"HIPPOCAMPUS, UM PEIXE MITOL&Oacute;GICO! | RICHARD RASMUSSEN\" src=\"https://www.youtube.com/embed/_qRM-gmgMXY\" width=\"320\" height=\"560\" frameborder=\"0\" allowfullscreen=\"allowfullscreen\"></iframe></p>";

            }

            if (i == 34)
            {
                pages[i - 1].Html = "<p style=\"text-align: center;\"><iframe title=\"VOC&Ecirc; J&Aacute; NADOU COM CARPAS? | RICHARD RASMUSSEN\" src=\"https://www.youtube.com/embed/tIafILj-GEs\" width=\"320\" height=\"560\" frameborder=\"0\" allowfullscreen=\"allowfullscreen\"></iframe></p>";

            }

            if (i == 35)
            {
                pages[i - 1].Html = "<p style=\"text-align: center;\"><iframe title=\"A ON&Ccedil;A-PRETA | QUE BICHO &Eacute; ESSE? | RICHARD RASMUSSEN\" src=\"https://www.youtube.com/embed/_TOKrAiFw-Q\" width=\"320\" height=\"560\" frameborder=\"0\" allowfullscreen=\"allowfullscreen\"></iframe></p>";

            }

            if (i == 36)
            {
                pages[i - 1].Html = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://s2-g1.glbimg.com/_X7Js0_sNHtd1FWxqDlMTzjvXL8=/0x0:5026x3351/1000x0/smart/filters:strip_icc()/i.s3.glbimg.com/v1/AUTH_59edd422c0c84a879bd37670ae4f538a/internal_photos/bs/2018/B/q/nPhGJKROiGDvdNNsvxIw/mg-4968.jpg\" alt=\"\" width=\"320\" height=\"213\" /></p>\r\n<div>\r\n<p>Conhecidas por on&ccedil;a-preta ou 'pantera negra', as on&ccedil;as-pintadas com pelagem escura possuem essa caracter&iacute;stica marcante gra&ccedil;as a uma muta&ccedil;&atilde;o gen&eacute;tica, que as tornam felinos mel&acirc;nicos. \"<span class=\"highlight highlighted\">O melanismo &eacute; uma muta&ccedil;&atilde;o que eleva a produ&ccedil;&atilde;o de melanina -&nbsp;</span><span class=\"highlight highlighted\"><em>prote&iacute;na presente no corpo respons&aacute;vel pela pigmenta&ccedil;&atilde;o preta</em></span><span class=\"highlight highlighted\">.</span>&nbsp;Com isso, os indiv&iacute;duos apresentam cor predominantemente escura na superf&iacute;cie do corpo (seja pele, pelagem ou plumagem) em rela&ccedil;&atilde;o ao padr&atilde;o de cor t&iacute;pico da esp&eacute;cie\", explica o bi&oacute;logo e coordenador cient&iacute;fico do On&ccedil;afari, Eduardo Fragoso.</p>\r\n</div>";

            }

            if (i == 37)
            {
                pages[i - 1].Html = "<p style=\"text-align: center;\"><iframe title=\"ALIMENTANDO UMA ON&Ccedil;A-PINTADA BEB&Ecirc;! | RICHARD RASMUSSEN\" src=\"https://www.youtube.com/embed/adSepe5UEVY\" width=\"320\" height=\"560\" frameborder=\"0\" allowfullscreen=\"allowfullscreen\"></iframe></p>";

            }

            if (i == 38)
            {
                pages[i - 1].Html = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://i0.wp.com/oeco.org.br/wp-content/uploads/2021/10/SAPO.jpg?w=1920&amp;ssl=1\" alt=\"\" width=\"320\" height=\"213\" /></p>";

            }

            if (i == 39)
            {
                pages[i - 1].Html = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://s4.static.brasilescola.uol.com.br/be/2020/10/sapos.jpg\" alt=\"\" width=\"320\" height=\"213\" /></p>";

            }

            if (i == 40)
            {
                pages[i - 1].Html = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://www.anfibiosdasaraucarias.com.br/assets/img/anfibios/boana-cf-curupi-jonas-toscan.jpg\" alt=\"\" width=\"320\" height=\"240\" /></p>";

            }

            if (i == 41)
            {
                pages[i - 1].Html = "<p style=\"text-align: center;\"><iframe title=\"A ARANHA COMEDORA DA P&Aacute;SSAROS! | RICHARD RASMUSSEN\" src=\"https://www.youtube.com/embed/BHdyUaVz3Os\" width=\"320\" height=\"560\" frameborder=\"0\" allowfullscreen=\"allowfullscreen\"></iframe></p>";

            }

            if (i == 42)
            {
                pages[i - 1].Html = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://www.infoescola.com/wp-content/uploads/2014/01/DSC09768.jpg\" alt=\"\" width=\"320\" height=\"256\" /></p>";

            }

            if (i == 43)
            {
                pages[i - 1].Html = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://mid.curitiba.pr.gov.br/2021/capa/00308006.jpg\" alt=\"\" width=\"320\" height=\"180\" /></p>";

            }

            if (i == 44)
            {
                pages[i - 1].Html = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://kelldrin.com.br/wp-content/uploads/2020/11/Aranha-Viuva-negra.jpg\" alt=\"\" width=\"320\" height=\"209\" /></p>";

            }

            if (i == 45)
            {
                pages[i - 1].Html = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://p2.trrsf.com/image/fget/cf/774/0/images.terra.com/2023/08/09/meet-the-persian-gold-qe4bp4ahj48o.jpg\" alt=\"\" width=\"320\" height=\"213\" /></p>";

            }

            if (i == 46)
            {
                pages[i - 1].Html = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://www.coisasdaroca.com/wp-content/uploads/2023/05/Aranha-da-areia.jpg\" alt=\"\" width=\"320\" height=\"173\" /></p>";

            }

            if (i == 47)
            {
                pages[i - 1].Html = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://p2.trrsf.com/image/fget/cf/774/0/images.terra.com/2018/09/24/lirios-brancos-foto-independent-agriculture.jpg\" alt=\"\" width=\"320\" height=\"240\" /></p>";

            }

            if (i == 48)
            {
                pages[i - 1].Html = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://cdnm.westwing.com.br/glossary/uploads/br/2022/11/18183507/L%C3%ADrio-Amarelo-Pixabay.jpg\" alt=\"\" width=\"320\" height=\"205\" /></p>";

            }

            if (i == 49)
            {
                pages[i - 1].Html = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://cptstatic.s3.amazonaws.com/imagens/enviadas/materias/materia8034/producao-de-lirios-propagacao-plantio-adubacao-e-cultivo-cpt.jpg\" alt=\"\" width=\"320\" height=\"240\" /></p>";

            }

            if (i == 50)
            {
                pages[i - 1].Html = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://static.giulianaflores.com.br/images/product/24621gg.jpg?ims=405x405\" alt=\"\" width=\"320\" height=\"320\" /></p>";

            }

            if (i == 51)
            {
                pages[i - 1].Html = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://www.cameliadecor.com.br/upload/produto/imagem/l-rio-rosa-nude-3.jpg\" alt=\"\" width=\"320\" height=\"320\" /></p>";

            }

            if (i == 52)
            {
                pages[i - 1].Html = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://62530.cdn.lojaquevende.com.br/static/62530/sku/plantas-ornamentais-lirio-laranja--p-1610131042637.png\" alt=\"\" width=\"320\" height=\"248\" /></p>";

            }

            if (i == 53)
            {
                pages[i - 1].Html = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://images.tcdn.com.br/img/img_prod/799330/lirio_asiatico_brindisi_1239_1_20200525104549.jpg\" alt=\"\" width=\"320\" height=\"320\" /></p>";

            }

            if (i == 54)
            {
                pages[i - 1].Html = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://cdnm.westwing.com.br/glossary/uploads/br/2023/08/11191434/lirio-azul.png\" alt=\"\" width=\"320\" height=\"205\" /></p>";

            }

            if (i == 55)
            {
                pages[i - 1].Html = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://florasantaclara.com.br/wp-content/uploads/2022/12/4-motivos-para-presentear-com-um-buque-de-lirio.jpg\" alt=\"\" width=\"320\" height=\"214\" /></p>";

            }

            if (i == 56)
            {
                pages[i - 1].Html = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://sp-ao.shortpixel.ai/client/to_auto,q_glossy,ret_img,w_600,h_600/https://lucianagonzalez.com.br/wp-content/uploads/2019/11/LIRIO-LARANJA-2019.jpg\" alt=\"\" width=\"320\" height=\"320\" /></p>";

            }

            if (i == 57)
            {
                pages[i - 1].Html = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://cdn.leroymerlin.com.br/products/lirio_pote_12_89203324_0001_600x600.jpg\" alt=\"\" width=\"320\" height=\"320\" /></p>";

            }

            if (i == 58)
            {
                pages[i - 1].Html = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"http://www.portal.zoo.bio.br/local/cache-vignettes/L560xH421/red_crab-001-1c7b2.jpg?1638638127\" alt=\"\" width=\"320\" height=\"241\" /></p>";

            }

            if (i == 59)
            {
                pages[i - 1].Html = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://media.gazetadopovo.com.br/vozes/2015/12/caranguejo-ar-b3f23706.jpg\" alt=\"\" width=\"320\" height=\"213\" /></p>";

            }

            if (i == 60)
            {
                pages[i - 1].Html = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://s2-umsoplaneta.glbimg.com/Ly-PEzWE8Kp3uMHDMymuuu2gUEE=/0x0:1280x853/888x0/smart/filters:strip_icc()/i.s3.glbimg.com/v1/AUTH_7d5b9b5029304d27b7ef8a7f28b4d70f/internal_photos/bs/2021/o/V/OV4Z0rSmSmgC0VUv8KsA/whatsapp-image-2021-08-31-at-09.56.21.jpeg\" alt=\"\" width=\"320\" height=\"213\" /></p>";

            }

            if (i == 61)
            {
                pages[i - 1].Html = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://istoedinheiro.com.br/wp-content/uploads/sites/17/2019/11/3_din1147_cobica3.jpg?x46096\" alt=\"\" width=\"320\" height=\"180\" /></p>";

            }

            if (i == 62)
            {
                pages[i - 1].Html = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://diariodonordeste.verdesmares.com.br/image/contentid/policy:1.3158270:1636583887/Pesca-de-lagosta-em-Icapui.jpeg?f=default&amp;$p$f=8500fd8\" alt=\"\" width=\"320\" height=\"240\" /></p>";

            }

            if (i == 63)
            {
                pages[i - 1].Html = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://acdn.mitiendanube.com/stores/001/498/336/products/a5b41115-d59a-47da-827c-b60e0fd46ff41-c91c369b9d6ca8ec0716617909870220-640-0.webp\" alt=\"\" width=\"320\" height=\"320\" /></p>";

            }

            if (i == 64)
            {
                pages[i - 1].Html = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://conexaoplaneta.com.br/wp-content/uploads/2021/11/lagosta-algodao-doce-conexao-planeta.jpg\" alt=\"\" width=\"320\" height=\"180\" /></p>";

            }

            if (i == 65)
            {
                pages[i - 1].Html = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://www.coisasdaroca.com/wp-content/uploads/2020/12/tatu-2-2048x1552.jpg\" alt=\"\" width=\"320\" height=\"243\" /></p>";

            }

            if (i == 66)
            {
                pages[i - 1].Html = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://scontent.fiza4-1.fna.fbcdn.net/v/t1.6435-9/191660497_4068354933210851_7224002615706470838_n.jpg?_nc_cat=108&amp;ccb=1-7&amp;_nc_sid=7f8c78&amp;_nc_ohc=9zDHo-kI-FwAX_ckcd5&amp;_nc_ht=scontent.fiza4-1.fna&amp;oh=00_AfD6HxStz5_WMVhSlf-TMTfIMq8QdxZZSZVwP_y5UFgPPQ&amp;oe=65567A83\" alt=\"\" width=\"320\" height=\"143\" /></p>";

            }

            if (i == 67)
            {
                pages[i - 1].Html = "<p style=\"text-align: center;\"><iframe title=\"VOC&Ecirc; PEGARIA EM UMA ARANHA? #shorts\" src=\"https://www.youtube.com/embed/rlrXdcG-69U\" width=\"390\" height=\"692\" frameborder=\"0\" allowfullscreen=\"allowfullscreen\"></iframe></p>";

            }

            if (i == 68)
            {
                pages[i - 1].Html = "<p style=\"text-align: center;\"><iframe title=\"A DORY EXISTE DE VERDADE? #shorts\" src=\"https://www.youtube.com/embed/db-qqVLT3L8\" width=\"390\" height=\"692\" frameborder=\"0\" allowfullscreen=\"allowfullscreen\"></iframe></p>";

            }

            if (i == 69)
            {
                pages[i - 1].Html = "<p style=\"text-align: center;\"><iframe title=\"QUAL A DIFEREN&Ccedil;A ENTRE O SHIH-TZU E O LHASA APSO?  #shorts\" src=\"https://www.youtube.com/embed/SHNFZwva838\" width=\"390\" height=\"692\" frameborder=\"0\" allowfullscreen=\"allowfullscreen\"></iframe></p>";

            }

            if (i == 70)
            {
                pages[i - 1].Html = "<p style=\"text-align: center;\"><iframe title=\"ESSE &Eacute; O ANIMAL MAIS FORTE DO MUNDO? #shorts\" src=\"https://www.youtube.com/embed/scmu9XHAkds\" width=\"390\" height=\"692\" frameborder=\"0\" allowfullscreen=\"allowfullscreen\"></iframe></p>";

            }

            if (i == 71)
            {
                pages[i - 1].Html = "<p style=\"text-align: center;\"><iframe title=\"DOBERMAN OU CANE CORSO, QUAL O MELHOR? #shorts\" src=\"https://www.youtube.com/embed/h-4aKF2ZoPQ\" width=\"390\" height=\"692\" frameborder=\"0\" allowfullscreen=\"allowfullscreen\"></iframe></p>";

            }

            if (i == 72)
            {
                pages[i - 1].Html = "<p style=\"text-align: center;\"><iframe title=\"PEIXE VOADOR! #shorts\" src=\"https://www.youtube.com/embed/x2V6x46ww2k\" width=\"390\" height=\"692\" frameborder=\"0\" allowfullscreen=\"allowfullscreen\"></iframe></p>";

            }

            if (i == 73)
            {
                pages[i - 1].Html = "<p style=\"text-align: center;\"><iframe title=\"O PORCO DO CERRADO! #shorts\" src=\"https://www.youtube.com/embed/k5aSVQDMwcw\" width=\"390\" height=\"692\" frameborder=\"0\" allowfullscreen=\"allowfullscreen\"></iframe></p>";

            }

            if (i == 74)
            {
                pages[i - 1].Html = "<p style=\"text-align: center;\"><iframe title=\"VOC&Ecirc; CONHECE A ARAPONGA? #shorts\" src=\"https://www.youtube.com/embed/6Cs3u8JsilM\" width=\"390\" height=\"692\" frameborder=\"0\" allowfullscreen=\"allowfullscreen\"></iframe></p>";

            }

            if (i == 75)
            {
                pages[i - 1].Html = "<p style=\"text-align: center;\"><iframe title=\"ESSE &Eacute; O CAVALO DOS CIGANOS! #shorts\" src=\"https://www.youtube.com/embed/8da8cTfGwfw\" width=\"390\" height=\"692\" frameborder=\"0\" allowfullscreen=\"allowfullscreen\"></iframe></p>";

            }

            if (i == 76)
            {
                pages[i - 1].Html = "<p style=\"text-align: center;\"><iframe title=\"GALAPAGOS, A ILHA DAS TARTARUGAS GIGANTES!\" src=\"https://www.youtube.com/embed/U-hYSQUpEoY\" width=\"320\" height=\"560\" frameborder=\"0\" allowfullscreen=\"allowfullscreen\"></iframe></p>";

            }

            if (i == 77)
            {
                pages[i - 1].Html = "<p style=\"text-align: center;\"><iframe title=\"ESSE &Eacute; O PEIXE VAMPIRO! #shorts\" src=\"https://www.youtube.com/embed/cKH889QYCec\" width=\"389\" height=\"692\" frameborder=\"0\" allowfullscreen=\"allowfullscreen\"></iframe></p>";

            }

            if (i == 78)
            {
                pages[i - 1].Html = "<p style=\"text-align: center;\"><iframe title=\"ESSE &Eacute; O PINGUIM DE MAGALH&Atilde;ES! #shorts\" src=\"https://www.youtube.com/embed/vefNdtakmUQ\" width=\"389\" height=\"692\" frameborder=\"0\" allowfullscreen=\"allowfullscreen\"></iframe></p>";

            }

            if (i == 79)
            {
                pages[i - 1].Html = "<p style=\"text-align: center;\"><iframe title=\"O MAIOR PEIXE DE &Aacute;GUA DOCE DO PLANETA! #shorts\" src=\"https://www.youtube.com/embed/AgwP8pqqU5Q\" width=\"389\" height=\"692\" frameborder=\"0\" allowfullscreen=\"allowfullscreen\"></iframe></p>";

            }

            if (i == 80)
            {
                pages[i - 1].Html = "<p style=\"text-align: center;\"><iframe title=\"VOC&Ecirc; CONHECE A JANDAIA MINEIRA? #shorts\" src=\"https://www.youtube.com/embed/F-DCatGERFk\" width=\"389\" height=\"692\" frameborder=\"0\" allowfullscreen=\"allowfullscreen\"></iframe></p>";

            }

            if (i == 81)
            {
                pages[i - 1].Html = "<p style=\"text-align: center;\"><iframe title=\"Esse &eacute; o maior escorpi&atilde;o que j&aacute; vi no Brasil! #shorts\" src=\"https://www.youtube.com/embed/8I0kCYmq86Y\" width=\"389\" height=\"692\" frameborder=\"0\" allowfullscreen=\"allowfullscreen\"></iframe></p>";

            }

            if (i == 82)
            {
                pages[i - 1].Html = "<p style=\"text-align: center;\"><iframe title=\"VOC&Ecirc; CONHECE O PACMAN? #shorts\" src=\"https://www.youtube.com/embed/QP_tDLkkNho\" width=\"389\" height=\"692\" frameborder=\"0\" allowfullscreen=\"allowfullscreen\"></iframe></p>";

            }

            if (i == 83)
            {
                pages[i - 1].Html = "<p style=\"text-align: center;\"><iframe title=\"A TRUTA ARCO-&Iacute;RIS! #shorts\" src=\"https://www.youtube.com/embed/-zE0QRTOnMA\" width=\"389\" height=\"692\" frameborder=\"0\" allowfullscreen=\"allowfullscreen\"></iframe></p>";

            }

            if (i == 84)
            {
                pages[i - 1].Html = "<p style=\"text-align: center;\"><iframe title=\"ISSO PODE ACABAR COM A SUA TARTARUGA DE ESTIMA&Ccedil;&Atilde;O! #shorts\" src=\"https://www.youtube.com/embed/O-NWYzySs_o\" width=\"389\" height=\"692\" frameborder=\"0\" allowfullscreen=\"allowfullscreen\"></iframe></p>";

            }

            if (i == 85)
            {
                pages[i - 1].Html = "<p style=\"text-align: center;\"><iframe title=\"Voc&ecirc; conhece o Tatu Galinha? #shorts\" src=\"https://www.youtube.com/embed/K6KiUU3hKkU\" width=\"389\" height=\"692\" frameborder=\"0\" allowfullscreen=\"allowfullscreen\"></iframe></p>";

            }

            if (i == 86)
            {
                pages[i - 1].Html = "<p style=\"text-align: center;\"><iframe title=\"A COBRA DE VIDRO! #shorts\" src=\"https://www.youtube.com/embed/DdLowoeRo6k\" width=\"389\" height=\"692\" frameborder=\"0\" allowfullscreen=\"allowfullscreen\"></iframe></p>";

            }

            if (i == 87)
            {
                pages[i - 1].Html = "<p style=\"text-align: center;\"><iframe title=\"ESSE ANIMAL ESTAVA EXTINTO! #shorts\" src=\"https://www.youtube.com/embed/kgLPSJjoHJ4\" width=\"389\" height=\"692\" frameborder=\"0\" allowfullscreen=\"allowfullscreen\"></iframe></p>";

            }

            if (i == 88)
            {
                pages[i - 1].Html = "<p style=\"text-align: center;\"><iframe title=\"A CRIA&Ccedil;&Atilde;O DE CAMAR&Atilde;O GIGANTE! #shorts\" src=\"https://www.youtube.com/embed/qFQUA2ZSviU\" width=\"389\" height=\"692\" frameborder=\"0\" allowfullscreen=\"allowfullscreen\"></iframe></p>";

            }

            if (i == 89)
            {
                pages[i - 1].Html = "<p style=\"text-align: center;\"><iframe title=\"A RA&Ccedil;A DE CAVALOS MAIS RARA DO BRASIL! #shorts\" src=\"https://www.youtube.com/embed/vhVrhi7Sek8\" width=\"389\" height=\"692\" frameborder=\"0\" allowfullscreen=\"allowfullscreen\"></iframe></p>";

            }

            if (i == 90)
            {
                pages[i - 1].Html = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://blog.7mboots.com.br/wp-content/webp-express/webp-images/uploads/2020/06/the-black-horse-of-the-frisian-breed-walks-in-the-P77UURU_Easy-Resize.com_.jpg.webp\" alt=\"\" width=\"320\" height=\"218\" /></p>";

            }

            if (i == 91)
            {
                pages[i - 1].Html = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://www.lancerural.com.br/wp-content/uploads/2017/03/Puro-sangue-%C3%A1rabe.jpg\" alt=\"\" width=\"320\" height=\"240\" /></p>";

            }

            if (i == 92)
            {
                pages[i - 1].Html = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://cavalus.com.br/wp-content/uploads/2020/11/O-cavalo-andaluz-e-uma-das-racas-mais-antigas-do-mundo-pinterest.jpg\" alt=\"\" width=\"320\" height=\"180\" /></p>";

            }

            if (i == 93)
            {
                pages[i - 1].Html = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://static.todamateria.com.br/upload/ca/va/cavaloraca-cke.jpg?auto_optimize=low\" alt=\"\" width=\"320\" height=\"252\" /></p>";

            }

            if (i == 94)
            {
                pages[i - 1].Html = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://blogs.canalrural.com.br/coisasdocampo/wp-content/uploads/sites/11/2016/08/13559049_1165091073532073_444411928186702723_o.jpg\" alt=\"\" width=\"320\" height=\"235\" /></p>";

            }

            if (i == 95)
            {
                pages[i - 1].Html = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://s2.glbimg.com/H46NBKxhNFL6r8fuzit9trFoHAw=/smart/e.glbimg.com/og/ed/f/original/2020/06/30/whatsapp_image_2020-06-30_at_12.04.08.jpeg\" alt=\"\" width=\"320\" height=\"320\" /></p>";

            }

            if (i == 96)
            {
                pages[i - 1].Html = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://animalbusiness.com.br/wp-content/uploads/2020/11/cavalo.jpg\" alt=\"\" width=\"320\" height=\"234\" /></p>";

            }

            if (i == 97)
            {
                pages[i - 1].Html = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://www.petz.com.br/blog/wp-content/uploads/2022/10/como-os-peixes-se-locomovem-final.jpg\" alt=\"\" width=\"320\" height=\"215\" /></p>";

            }

            if (i == 98)
            {
                pages[i - 1].Html = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://p2.trrsf.com/image/fget/cf/774/0/images.terra.com/2014/12/04/peixes1.jpg\" alt=\"\" width=\"320\" height=\"213\" /></p>";

            }

            if (i == 99)
            {
                pages[i - 1].Html = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://oregional.net/wp-content/uploads/2022/11/Peixe-Palhaco.jpg\" alt=\"\" width=\"320\" height=\"320\" /></p>";

            }

            if (i != 1)
            {
                contexto.Add(pages[i - 1]);
                contexto.SaveChanges();
            }
        }
       
        var str2 = new PatternStory("comentários", contexto.Story.Include(s => s.Pagina).OrderBy(s => s.Id).First())
        {
            Capitulo = 2
        };
        contexto.Add(str2);
        contexto.SaveChanges();

        var str3 = new PatternStory("roupas", contexto.Story.Include(s => s.Pagina).OrderBy(s => s.Id).First())
        {
            Capitulo = 3
        };
        contexto.Add(str3);
        contexto.SaveChanges();

        for (var i = 1; i <= 2000; i++)
        {
            pages2[i - 1] = new Pagina(i);
            pages2[i - 1].Html = $"<br /> <br /> <br /> <p> <h1> conteudo {pages2[i - 1].Versiculo}  </h1> </p>";
            pages2[i - 1].Titulo = "pagina";
            pages2[i - 1].StoryId = str2.Id;
            pages2[i - 1].Titulo += $" {i}";

            contexto.Add(pages2[i - 1]);
            contexto.SaveChanges();

        }

        for (var i = 1; i <= 12000; i++)
        {
            var indice = i.ToString();
            var texto = "";

            if (indice[indice.Length - 1] == '0' ||  // para mudanca de estado
                indice[indice.Length - 1] == '9' ||  // para mudanca de estado
                indice[indice.Length - 1] == '8')   // para mudanca de estado
            {
                pages3[i - 1] = new ChangeContent(i);
                texto = "Mudanca";
            }
            else
            if (indice[indice.Length - 1] == '7')    // para Conteudo de admin
            {
                pages3[i - 1] = new AdminContent(i);
                texto = "Conteudo de admin";
            }
            else
            {                                       // para produtos - pages[i - 1].ProdutoId != null
                pages3[i - 1] = new ProductContent(i);
                texto = "Produto";
            }
            pages3[i - 1].Html = $"<br /> <br /> <br /> <p> <h1> {texto} {pages3[i - 1].Versiculo}  </h1> </p>";
            pages3[i - 1].Titulo = "pagina";
            pages3[i - 1].StoryId = str3.Id;
            pages3[i - 1].Titulo += $" {i}";

            contexto.Add(pages3[i - 1]);
            contexto.SaveChanges();

        }



    }



}

app.Run();

