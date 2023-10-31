using BlazorServerCms.Areas.Identity;
using BlazorServerCms.Data;
using business.Group;
using business;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Models;
using Newtonsoft.Json;
using BlazorServerCms.servicos;
using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using business.business;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<HttpClient>();
builder.Services.AddSingleton<RepositoryPagina>();
builder.Services.AddSingleton<BlazorTimer>();
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<IdentityUser>>();


var app = builder.Build();

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
    var contexto = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var repositoryPagina = scope.ServiceProvider.GetRequiredService<RepositoryPagina>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    var email = builder.Configuration.GetConnectionString("Email");
    var password = builder.Configuration.GetConnectionString("Senha");
    var userASP = await userManager.FindByNameAsync(email);



    // await contexto.Database.MigrateAsync();

    if (repositoryPagina.paginasCurtidas!.Count == 0)
    {
        var pages = await contexto.PageLiked.ToListAsync();
        repositoryPagina.paginasCurtidas!.AddRange(pages);
    }


        if (userASP == null)
    {
        string[] rolesNames = { "Admin" };
    IdentityResult result;
    foreach (var namesRole in rolesNames)
    {
        var roleExist = await roleManager.RoleExistsAsync(namesRole);
        if (!roleExist)
        {
            result = await roleManager.CreateAsync(new IdentityRole(namesRole));
        }
    }    
        var user = new IdentityUser { UserName = "leandro01832", Email = email, EmailConfirmed = true };
        await userManager.CreateAsync(user, password);
        await userManager.AddToRoleAsync(user, "Admin");
    }

    if (!await contexto!.Set<Story>().AnyAsync())
    {
        var padrao = new Story
        {
            PaginaPadraoLink = 0,
            Comentario = false,
            Nome = "Padrao"
        };
        var str = new Story
        {
            PaginaPadraoLink = 1,
            Comentario = false,
            Nome = "seres vivos"
        };
        contexto.Add(padrao);
        contexto.Add(str);
        contexto.SaveChanges();

        var pagPadrao = new Pagina
        {
            Classificacao = new Classificacao(),
            Comentario = 0,
            ContentUser = "<a href=''#'' id=''LinkPadrao''> <h1> Story seres vivos</h1> </a>",
            Content = null,
            Data = DateTime.Now,
            ImagemContent = null,
            StoryId = 1,
            Titulo = "capitulos"
        };
        contexto.Add(pagPadrao);
        contexto.SaveChanges();
        var p = await repositoryPagina.includes().FirstAsync(pa => pa.Id == pagPadrao.Id);
        repositoryPagina.paginas.Add(p);

        Pagina[] pages = new Pagina[99];
        foreach(var item in contexto.Story!.Include(str => str.Pagina).First(str => str.Id == 1).Pagina!.ToList())
        {
            var p1 = await repositoryPagina.includes().FirstAsync(pa => pa.Id == item.Id);
            repositoryPagina.paginas.Add(p1);
        }
        for (var i = 1; i<= 99; i++)
        {
                var pag = new Pagina();

                pages[i - 1] = new Pagina();
                pages[i - 1].Titulo = "pagina";
                pages[i - 1].Titulo += $" {i}";
                pages[i - 1].Data = DateTime.Now;
                pages[i - 1].Classificacao = new Classificacao();
                pages[i - 1].Produto = null;
                pages[i - 1].Content = null;
                pages[i - 1].ImagemContent = null;
                pages[i - 1].Comentario = 0;
                pages[i - 1].StoryId = 2;

            if(i == 1)
            {
                pages[i - 1].ContentUser = "<p> <h1> Seja bem vindo a Story seres vivos</h1> </p>";
                
            }
            
            if(i == 2)
            {
                pages[i - 1].ContentUser = "<p style=\"text-align: center;\"><iframe title=\"ENCONTRANDO UM GIGANTE NA NATUREZA, A BALEIA CINZENTA! | RICHARD RASMUSSEN\" src=\"https://www.youtube.com/embed/r_eLOG096sE\" width=\"320\" height=\"560\" frameborder=\"0\" allowfullscreen=\"allowfullscreen\"></iframe></p>";
                
            }
            
            if(i == 3)
            {
                pages[i - 1].ContentUser = "<p>A&nbsp;<strong>baleia-cinzenta</strong>&nbsp;(<em>Eschrichtius robustus</em>) &eacute; um&nbsp;<a class=\"mw-redirect\" title=\"Mam&iacute;fero\" href=\"https://pt.wikipedia.org/wiki/Mam%C3%ADfero\">mam&iacute;fero</a>&nbsp;<a class=\"mw-redirect\" title=\"Cet&aacute;ceo\" href=\"https://pt.wikipedia.org/wiki/Cet%C3%A1ceo\">cet&aacute;ceo</a>&nbsp;da fam&iacute;lia dos&nbsp;<a class=\"mw-redirect\" title=\"Eschrichtiidae\" href=\"https://pt.wikipedia.org/wiki/Eschrichtiidae\">escrict&iacute;deos</a>. &Eacute; a &uacute;nica esp&eacute;cie viva em seu g&ecirc;nero e fam&iacute;lia, mas uma esp&eacute;cie extinta foi descoberta e colocada no g&ecirc;nero em 2017, a&nbsp;<a class=\"new\" title=\"Baleia Akishima (p&aacute;gina n&atilde;o existe)\" href=\"https://pt.wikipedia.org/w/index.php?title=Baleia_Akishima&amp;action=edit&amp;redlink=1\">Baleia Akishima</a>.<sup id=\"cite_ref-N&atilde;o_nomeado-20230316141054_2-0\" class=\"reference\"><a href=\"https://pt.wikipedia.org/wiki/Baleia-cinzenta#cite_note-N%C3%A3o_nomeado-20230316141054-2\">[2]</a></sup></p>\r\n<h2><span id=\"Descri.C3.A7.C3.A3o\"></span><span id=\"Descri&ccedil;&atilde;o\" class=\"mw-headline\">Descri&ccedil;&atilde;o</span></h2>\r\n<figure class=\"mw-halign-left\"><a class=\"mw-file-description\" href=\"https://pt.wikipedia.org/wiki/Ficheiro:Eschrichtius_robustus_01-cropped.jpg\"><img class=\"mw-file-element\" src=\"https://upload.wikimedia.org/wikipedia/commons/thumb/c/c6/Eschrichtius_robustus_01-cropped.jpg/210px-Eschrichtius_robustus_01-cropped.jpg\" srcset=\"//upload.wikimedia.org/wikipedia/commons/thumb/c/c6/Eschrichtius_robustus_01-cropped.jpg/315px-Eschrichtius_robustus_01-cropped.jpg 1.5x, //upload.wikimedia.org/wikipedia/commons/thumb/c/c6/Eschrichtius_robustus_01-cropped.jpg/420px-Eschrichtius_robustus_01-cropped.jpg 2x\" width=\"210\" height=\"133\" data-file-width=\"800\" data-file-height=\"508\" /></a>\r\n<figcaption>Uma baleia-cinzenta&nbsp;<a title=\"Comportamento de superf&iacute;cie dos cet&aacute;ceos\" href=\"https://pt.wikipedia.org/wiki/Comportamento_de_superf%C3%ADcie_dos_cet%C3%A1ceos\">saltando</a>.</figcaption>\r\n</figure>\r\n<p>Os indiv&iacute;duos j&aacute; foram chamados de &ldquo;peixes do diabo&rdquo; porque s&atilde;o muito resistentes e brigam quando ca&ccedil;ados. Esse nome entretanto est&aacute; biologicamente incorreto, pois as baleias n&atilde;o s&atilde;o peixes.<sup id=\"cite_ref-3\" class=\"reference\"><a href=\"https://pt.wikipedia.org/wiki/Baleia-cinzenta#cite_note-3\">[3]</a></sup>&nbsp;Seu tamanho pode atingir cerca de 15 metros de comprimento e pesar cerca de 35 toneladas. Sua alimenta&ccedil;&atilde;o &eacute; a base de anf&iacute;podos (pequenos crust&aacute;ceos que vivem na &aacute;gua ou pr&oacute;ximo, incluindo pulgas de areia e piolhos de baleia), krill, pl&acirc;ncton e moluscos. Ao contr&aacute;rio de outros cet&aacute;ceos, a baleia-cinzenta tende a alimentar-se junto ao fundo do mar, onde agita a &aacute;gua para levantar material do fundo de onde consegue filtrar os seus alimentos. A distribui&ccedil;&atilde;o atual e contida ao Oceano&nbsp;<a title=\"Oceano Pac&iacute;fico\" href=\"https://pt.wikipedia.org/wiki/Oceano_Pac%C3%ADfico\">pac&iacute;fico</a>. A baleia-cinzenta tamb&eacute;m ocorre em &aacute;guas litorais desde o mar de Okhotsk at&eacute; a Coreia do Sul e Jap&atilde;o e desde os mares de Chukchi e de Beaufort no golfo do M&eacute;xico.</p>\r\n<p>O recorde de cerca de maior dist&acirc;ncia percorrida por um vertebrado marinho j&aacute; registrada pertence a uma baleia-cinzenta que que nadou 26,8 mil quil&ocirc;metros. O cet&aacute;ceo em quest&atilde;o &eacute; um macho de 12 metros de comprimento avistado pr&oacute;ximo &agrave;&nbsp;<a title=\"Nam&iacute;bia\" href=\"https://pt.wikipedia.org/wiki/Nam%C3%ADbia\">Nam&iacute;bia</a>&nbsp;em 2013. Ele tamb&eacute;m &eacute; a primeira e &uacute;nica baleia-cinzenta a ser registrada no&nbsp;<a title=\"Hemisf&eacute;rio sul\" href=\"https://pt.wikipedia.org/wiki/Hemisf%C3%A9rio_sul\">Hemisf&eacute;rio Sul</a>.<sup id=\"cite_ref-4\" class=\"reference\"><a href=\"https://pt.wikipedia.org/wiki/Baleia-cinzenta#cite_note-4\">[4]</a></sup></p>";
                
            }
            
            if(i == 4)
            {
                pages[i - 1].ContentUser = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://static.nationalgeographicbrasil.com/files/styles/image_3200/public/w7rx5g.webp?w=1450&amp;h=816\" alt=\"\" width=\"320\" height=\"180\" /></p>";
                
            }
            
            if(i == 5)
            {
                pages[i - 1].ContentUser = "<p style=\"text-align: center;\"><iframe title=\"AS CINCO BALEIAS MAIS INCR&Iacute;VEIS QUE EU J&Aacute; ENCONTREI! | RICHARD RASMUSSEN\" src=\"https://www.youtube.com/embed/ui70_rhmGtM\" width=\"320\" height=\"560\" frameborder=\"0\" allowfullscreen=\"allowfullscreen\"></iframe></p>";
                
            }
            
            if(i == 6)
            {
                pages[i - 1].ContentUser = "<p style=\"text-align: center;\"><iframe title=\"UMA ON&Ccedil;A-PINTADA SELVAGEM INVADIU O INSTITUTO! | RICHARD RASMUSSEN\" src=\"https://www.youtube.com/embed/c6zPTqvDDN8\" width=\"320\" height=\"560\" frameborder=\"0\" allowfullscreen=\"allowfullscreen\"></iframe></p>";
                
            }
            
            if(i == 7)
            {
                pages[i - 1].ContentUser = "<p style=\"text-align: center;\"><iframe title=\"OS R&Eacute;PTEIS DA MATA ATL&Acirc;NTICA! | RICHARD RASMUSSEN\" src=\"https://www.youtube.com/embed/1zueHDxHJWA\" width=\"320\" height=\"560\" frameborder=\"0\" allowfullscreen=\"allowfullscreen\"></iframe></p>";
                
            }
            
            if(i == 8)
            {
                pages[i - 1].ContentUser = "<p style=\"text-align: center;\"><iframe title=\"UMA SUCURI PRETA GIGANTE! | RICHARD RASMUSSEN\" src=\"https://www.youtube.com/embed/-2PjgVtX_bs\" width=\"320\" height=\"560\" frameborder=\"0\" allowfullscreen=\"allowfullscreen\"></iframe></p>";
                
            }
            
            if(i == 9)
            {
                pages[i - 1].ContentUser = "<p style=\"text-align: center;\"><iframe title=\"UM ANIMAL COM MAIS DE 300 MILH&Otilde;ES DE ANOS DE EVOLU&Ccedil;&Atilde;O! | BRASIL BIOMAS\" src=\"https://www.youtube.com/embed/zKcPVi43gqo\" width=\"320\" height=\"560\" frameborder=\"0\" allowfullscreen=\"allowfullscreen\"></iframe></p>";
                
            }
            
            if(i == 10)
            {
                pages[i - 1].ContentUser = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://cdn.ambientes.ambientebrasil.com.br/wp-content/uploads/2020/11/jacare-2645898_1280.jpg\" alt=\"\" width=\"320\" height=\"213\" /></p>";
                
            }
            
            if(i == 11)
            {
                pages[i - 1].ContentUser = "<p style=\"text-align: center;\"><iframe title=\"UMA CACATUA MUITO LOUCA,  UM TUCANO RESMUNG&Atilde;O E A ARARA AZUL MAIS FOFA DO MUNDO | RICHARD RASMUSSEN\" src=\"https://www.youtube.com/embed/KBDUODeW5vI\" width=\"320\" height=\"560\" frameborder=\"0\" allowfullscreen=\"allowfullscreen\"></iframe></p>";
                
            }
            
            if(i == 12)
            {
                pages[i - 1].ContentUser = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://s2-g1.glbimg.com/FR6jrT4gKQuTXit78yN94mCkhes=/0x0:1900x1461/1008x0/smart/filters:strip_icc()/i.s3.glbimg.com/v1/AUTH_59edd422c0c84a879bd37670ae4f538a/internal_photos/bs/2019/1/g/BjKNYuSBeVn9zupTRamA/araraazul.jpg\" alt=\"\" width=\"320\" height=\"246\" /></p>";
                
            }
            
            if(i == 13)
            {
                pages[i - 1].ContentUser = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"http://www.fiocruz.br/biosseguranca/Bis/infantil/araraazul.jpg\" alt=\"\" width=\"320\" height=\"258\" /></p>";
                
            }
            
            if(i == 14)
            {
                pages[i - 1].ContentUser = "<p style=\"text-align: center;\"><iframe title=\"OS 5 INSETOS MAIS PERIGOSOS DO MUNDO! | RICHARD RASMUSSEN\" src=\"https://www.youtube.com/embed/ZpKZpv7XGsY\" width=\"320\" height=\"560\" frameborder=\"0\" allowfullscreen=\"allowfullscreen\"></iframe></p>";
                
            }
            
            if(i == 15)
            {
                pages[i - 1].ContentUser = "<p style=\"text-align: center;\"><iframe title=\"&quot;Explorando a Eleg&acirc;ncia Natural: Conhe&ccedil;a a Fascinante Palmeira Azul de Madagascar 🌴💙\" src=\"https://www.youtube.com/embed/9-EOAxUcwiA\" width=\"320\" height=\"560\" frameborder=\"0\" allowfullscreen=\"allowfullscreen\"></iframe></p>";
                
            }
            
            if(i == 16)
            {
                pages[i - 1].ContentUser = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://blog.cobasi.com.br/wp-content/webpc-passthru.php?src=https://blog.cobasi.com.br/wp-content/uploads/2020/07/Tipos-de-orqui%CC%81deas-Phalaenopsis.png&amp;nocache=1\" alt=\"\" width=\"320\" height=\"213\" /></p>";
                
            }
            
            if(i == 17)
            {
                pages[i - 1].ContentUser = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://images.tcdn.com.br/img/img_prod/660625/orquidea_cattleya_blc_durigan_big_spots_adulta_1113_1_8900f911dbede8793c3bf2dc131fb647.jpg\" alt=\"\" width=\"320\" height=\"320\" /></p>";
                
            }
            
            if(i == 18)
            {
                pages[i - 1].ContentUser = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://static.giulianaflores.com.br/images/product/27330gg.jpg?ims=405x405\" alt=\"\" width=\"320\" height=\"320\" /></p>";
                
            }
            
            if(i == 19)
            {
                pages[i - 1].ContentUser = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://s2.glbimg.com/7dJjXrhNzxwRW4jboPSWiEuZZ5Q=/620x620/smart/e.glbimg.com/og/ed/f/original/2022/07/22/orquidea-cymbidium-como-cuidar-e-cultivar-5.jpg\" alt=\"\" width=\"320\" height=\"320\" /></p>";
                
            }
            
            if(i == 20)
            {
                pages[i - 1].ContentUser = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://weflores.com/wp-content/uploads/Orqu%C3%ADdea-phalaenopsis-cascata-pink-em-vaso-riscatto.jpg\" alt=\"\" width=\"320\" height=\"320\" /></p>";
                
            }
            
            if(i == 21)
            {
                pages[i - 1].ContentUser = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://http2.mlstatic.com/D_NQ_NP_774145-MLB41462303073_042020-O.webp\" alt=\"\" width=\"320\" height=\"320\" /></p>";
                
            }
            
            if(i == 22)
            {
                pages[i - 1].ContentUser = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://http2.mlstatic.com/D_NQ_NP_2X_951320-MLB48602367585_122021-F.webp\" alt=\"\" width=\"320\" height=\"320\" /></p>";
                
            }
            
            if(i == 23)
            {
                pages[i - 1].ContentUser = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://static.mundoeducacao.uol.com.br/mundoeducacao/conteudo_legenda/c4f684947cd4a982e58416940e5ea7c1.jpg\" alt=\"\" width=\"320\" height=\"214\" /></p>";
                
            }
            
            if(i == 24)
            {
                pages[i - 1].ContentUser = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://www.tgservices.com.br/wp-content/uploads/2021/10/26-10-2021.jpg\" alt=\"\" width=\"320\" height=\"200\" /></p>";
                
            }
            
            if(i == 25)
            {
                pages[i - 1].ContentUser = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://emsinapse.files.wordpress.com/2021/09/louva-deus-653x330-1.jpg\" alt=\"\" width=\"320\" height=\"162\" /></p>";
                
            }
            
            if(i == 26)
            {
                pages[i - 1].ContentUser = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://scontent.fiza4-1.fna.fbcdn.net/v/t1.18169-9/13331074_1813075485589893_2720588748137086315_n.jpg?_nc_cat=105&amp;ccb=1-7&amp;_nc_sid=c2f564&amp;_nc_ohc=6qQPmnz-0eoAX_AD6-E&amp;_nc_ht=scontent.fiza4-1.fna&amp;oh=00_AfAr5C_St05ZZed7c2yob_t7fyErJvBeAtK_ENelgSj10A&amp;oe=6554A8E9\" alt=\"\" width=\"320\" height=\"193\" /></p>";
                
            }
            
            if(i == 27)
            {
                pages[i - 1].ContentUser = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://integralismo.org.br/wp-content/uploads/2019/05/9222c546c490561f6c8f63e6d35ffad8.jpg\" alt=\"\" width=\"320\" height=\"240\" /></p>";
                
            }
            
            if(i == 28)
            {
                pages[i - 1].ContentUser = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://static.biologianet.com/2020/05/onca-pintada.jpg\" alt=\"\" width=\"320\" height=\"213\" /></p>";
                
            }
            
            if(i == 29)
            {
                pages[i - 1].ContentUser = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://portalamazonia.com/images/p/24827/Foto_1_WWF_AFP.jpg\" alt=\"\" width=\"320\" height=\"213\" /></p>";
                
            }
            
            if(i == 30)
            {
                pages[i - 1].ContentUser = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://www.aqualovers.pt/images/42/amphiprion-ocellaris_large.jpg\" alt=\"\" width=\"320\" height=\"320\" /></p>";
                
            }
            
            if(i == 31)
            {
                pages[i - 1].ContentUser = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://static.mundoeducacao.uol.com.br/mundoeducacao/2022/05/peixe-leao.jpg\" alt=\"\" width=\"320\" height=\"213\" /></p>";
                
            }

            if (i == 32)
            {
                pages[i - 1].ContentUser = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://euamomeusanimais.com.br/wp-content/uploads/2013/10/Cavalo-Marinho-800x500.jpg\" alt=\"\" width=\"320\" height=\"200\" /></p>";
                
            }

                if (i == 33)
            {
                pages[i - 1].ContentUser = "<p style=\"text-align: center;\"><iframe title=\"HIPPOCAMPUS, UM PEIXE MITOL&Oacute;GICO! | RICHARD RASMUSSEN\" src=\"https://www.youtube.com/embed/_qRM-gmgMXY\" width=\"320\" height=\"560\" frameborder=\"0\" allowfullscreen=\"allowfullscreen\"></iframe></p>";
                
                }
            
            if(i == 34)
            {
                pages[i - 1].ContentUser = "<p style=\"text-align: center;\"><iframe title=\"VOC&Ecirc; J&Aacute; NADOU COM CARPAS? | RICHARD RASMUSSEN\" src=\"https://www.youtube.com/embed/tIafILj-GEs\" width=\"320\" height=\"560\" frameborder=\"0\" allowfullscreen=\"allowfullscreen\"></iframe></p>";
                
             }
            
            if(i == 35)
            {
                pages[i - 1].ContentUser = "<p style=\"text-align: center;\"><iframe title=\"A ON&Ccedil;A-PRETA | QUE BICHO &Eacute; ESSE? | RICHARD RASMUSSEN\" src=\"https://www.youtube.com/embed/_TOKrAiFw-Q\" width=\"320\" height=\"560\" frameborder=\"0\" allowfullscreen=\"allowfullscreen\"></iframe></p>";
                    
                }
            
            if(i == 36)
            {
                pages[i - 1].ContentUser = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://s2-g1.glbimg.com/_X7Js0_sNHtd1FWxqDlMTzjvXL8=/0x0:5026x3351/1000x0/smart/filters:strip_icc()/i.s3.glbimg.com/v1/AUTH_59edd422c0c84a879bd37670ae4f538a/internal_photos/bs/2018/B/q/nPhGJKROiGDvdNNsvxIw/mg-4968.jpg\" alt=\"\" width=\"320\" height=\"213\" /></p>\r\n<div>\r\n<p>Conhecidas por on&ccedil;a-preta ou 'pantera negra', as on&ccedil;as-pintadas com pelagem escura possuem essa caracter&iacute;stica marcante gra&ccedil;as a uma muta&ccedil;&atilde;o gen&eacute;tica, que as tornam felinos mel&acirc;nicos. \"<span class=\"highlight highlighted\">O melanismo &eacute; uma muta&ccedil;&atilde;o que eleva a produ&ccedil;&atilde;o de melanina -&nbsp;</span><span class=\"highlight highlighted\"><em>prote&iacute;na presente no corpo respons&aacute;vel pela pigmenta&ccedil;&atilde;o preta</em></span><span class=\"highlight highlighted\">.</span>&nbsp;Com isso, os indiv&iacute;duos apresentam cor predominantemente escura na superf&iacute;cie do corpo (seja pele, pelagem ou plumagem) em rela&ccedil;&atilde;o ao padr&atilde;o de cor t&iacute;pico da esp&eacute;cie\", explica o bi&oacute;logo e coordenador cient&iacute;fico do On&ccedil;afari, Eduardo Fragoso.</p>\r\n</div>";
                
            }
            
            if(i == 37)
            {
                pages[i - 1].ContentUser = "<p style=\"text-align: center;\"><iframe title=\"ALIMENTANDO UMA ON&Ccedil;A-PINTADA BEB&Ecirc;! | RICHARD RASMUSSEN\" src=\"https://www.youtube.com/embed/adSepe5UEVY\" width=\"320\" height=\"560\" frameborder=\"0\" allowfullscreen=\"allowfullscreen\"></iframe></p>";
                
            }
            
            if(i == 38)
            {
                pages[i - 1].ContentUser = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://i0.wp.com/oeco.org.br/wp-content/uploads/2021/10/SAPO.jpg?w=1920&amp;ssl=1\" alt=\"\" width=\"320\" height=\"213\" /></p>";
                
            }
            
            if(i == 39)
            {
                pages[i - 1].ContentUser = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://s4.static.brasilescola.uol.com.br/be/2020/10/sapos.jpg\" alt=\"\" width=\"320\" height=\"213\" /></p>";
                
            }
            
            if(i == 40)
            {
                pages[i - 1].ContentUser = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://www.anfibiosdasaraucarias.com.br/assets/img/anfibios/boana-cf-curupi-jonas-toscan.jpg\" alt=\"\" width=\"320\" height=\"240\" /></p>";
                
            }
            
            if(i == 41)
            {
                pages[i - 1].ContentUser = "<p style=\"text-align: center;\"><iframe title=\"A ARANHA COMEDORA DA P&Aacute;SSAROS! | RICHARD RASMUSSEN\" src=\"https://www.youtube.com/embed/BHdyUaVz3Os\" width=\"320\" height=\"560\" frameborder=\"0\" allowfullscreen=\"allowfullscreen\"></iframe></p>";
                
            }
            
            if(i == 42)
            {
                pages[i - 1].ContentUser = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://www.infoescola.com/wp-content/uploads/2014/01/DSC09768.jpg\" alt=\"\" width=\"320\" height=\"256\" /></p>";
                
            }
            
            if(i == 43)
            {
                pages[i - 1].ContentUser = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://mid.curitiba.pr.gov.br/2021/capa/00308006.jpg\" alt=\"\" width=\"320\" height=\"180\" /></p>";
                
            }
            
            if(i == 44)
            {
                pages[i - 1].ContentUser = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://kelldrin.com.br/wp-content/uploads/2020/11/Aranha-Viuva-negra.jpg\" alt=\"\" width=\"320\" height=\"209\" /></p>";
                
            }
            
            if(i == 45)
            {
                pages[i - 1].ContentUser = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://p2.trrsf.com/image/fget/cf/774/0/images.terra.com/2023/08/09/meet-the-persian-gold-qe4bp4ahj48o.jpg\" alt=\"\" width=\"320\" height=\"213\" /></p>";
                
            }
            
            if(i == 46)
            {
                pages[i - 1].ContentUser = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://www.coisasdaroca.com/wp-content/uploads/2023/05/Aranha-da-areia.jpg\" alt=\"\" width=\"320\" height=\"173\" /></p>";
                
            }
            
            if(i == 47)
            {
                pages[i - 1].ContentUser = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://p2.trrsf.com/image/fget/cf/774/0/images.terra.com/2018/09/24/lirios-brancos-foto-independent-agriculture.jpg\" alt=\"\" width=\"320\" height=\"240\" /></p>";
                
            }
            
            if(i == 48)
            {
                pages[i - 1].ContentUser = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://cdnm.westwing.com.br/glossary/uploads/br/2022/11/18183507/L%C3%ADrio-Amarelo-Pixabay.jpg\" alt=\"\" width=\"320\" height=\"205\" /></p>";
                
            }
            
            if(i == 49)
            {
                pages[i - 1].ContentUser = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://cptstatic.s3.amazonaws.com/imagens/enviadas/materias/materia8034/producao-de-lirios-propagacao-plantio-adubacao-e-cultivo-cpt.jpg\" alt=\"\" width=\"320\" height=\"240\" /></p>";
                
            }
            
            if(i == 50)
            {
                pages[i - 1].ContentUser = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://static.giulianaflores.com.br/images/product/24621gg.jpg?ims=405x405\" alt=\"\" width=\"320\" height=\"320\" /></p>";
                
            }
            
            if(i == 51)
            {
                pages[i - 1].ContentUser = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://www.cameliadecor.com.br/upload/produto/imagem/l-rio-rosa-nude-3.jpg\" alt=\"\" width=\"320\" height=\"320\" /></p>";
                
            }
            
            if(i == 52)
            {
                pages[i - 1].ContentUser = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://62530.cdn.lojaquevende.com.br/static/62530/sku/plantas-ornamentais-lirio-laranja--p-1610131042637.png\" alt=\"\" width=\"320\" height=\"248\" /></p>";
                
            }
            
            if(i == 53)
            {
                pages[i - 1].ContentUser = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://images.tcdn.com.br/img/img_prod/799330/lirio_asiatico_brindisi_1239_1_20200525104549.jpg\" alt=\"\" width=\"320\" height=\"320\" /></p>";
                
            }
            
            if(i == 54)
            {
                pages[i - 1].ContentUser = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://cdnm.westwing.com.br/glossary/uploads/br/2023/08/11191434/lirio-azul.png\" alt=\"\" width=\"320\" height=\"205\" /></p>";
                
            }
            
            if(i == 55)
            {
                pages[i - 1].ContentUser = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://florasantaclara.com.br/wp-content/uploads/2022/12/4-motivos-para-presentear-com-um-buque-de-lirio.jpg\" alt=\"\" width=\"320\" height=\"214\" /></p>";
                
            }
            
            if(i == 56)
            {
                pages[i - 1].ContentUser = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://sp-ao.shortpixel.ai/client/to_auto,q_glossy,ret_img,w_600,h_600/https://lucianagonzalez.com.br/wp-content/uploads/2019/11/LIRIO-LARANJA-2019.jpg\" alt=\"\" width=\"320\" height=\"320\" /></p>";
                
            }
            
            if(i == 57)
            {
                pages[i - 1].ContentUser = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://cdn.leroymerlin.com.br/products/lirio_pote_12_89203324_0001_600x600.jpg\" alt=\"\" width=\"320\" height=\"320\" /></p>";
                
            }
            
            if(i == 58)
            {
                pages[i - 1].ContentUser = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"http://www.portal.zoo.bio.br/local/cache-vignettes/L560xH421/red_crab-001-1c7b2.jpg?1638638127\" alt=\"\" width=\"320\" height=\"241\" /></p>";
                
            }
            
            if(i == 59)
            {
                pages[i - 1].ContentUser = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://media.gazetadopovo.com.br/vozes/2015/12/caranguejo-ar-b3f23706.jpg\" alt=\"\" width=\"320\" height=\"213\" /></p>";
                
            }
            
            if(i == 60)
            {
                pages[i - 1].ContentUser = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://s2-umsoplaneta.glbimg.com/Ly-PEzWE8Kp3uMHDMymuuu2gUEE=/0x0:1280x853/888x0/smart/filters:strip_icc()/i.s3.glbimg.com/v1/AUTH_7d5b9b5029304d27b7ef8a7f28b4d70f/internal_photos/bs/2021/o/V/OV4Z0rSmSmgC0VUv8KsA/whatsapp-image-2021-08-31-at-09.56.21.jpeg\" alt=\"\" width=\"320\" height=\"213\" /></p>";
                
            }
            
            if(i == 61)
            {
                pages[i - 1].ContentUser = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://istoedinheiro.com.br/wp-content/uploads/sites/17/2019/11/3_din1147_cobica3.jpg?x46096\" alt=\"\" width=\"320\" height=\"180\" /></p>";
                
            }
            
            if(i == 62)
            {
                pages[i - 1].ContentUser = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://diariodonordeste.verdesmares.com.br/image/contentid/policy:1.3158270:1636583887/Pesca-de-lagosta-em-Icapui.jpeg?f=default&amp;$p$f=8500fd8\" alt=\"\" width=\"320\" height=\"240\" /></p>";
                
            }
            
            if(i == 63)
            {
                pages[i - 1].ContentUser = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://acdn.mitiendanube.com/stores/001/498/336/products/a5b41115-d59a-47da-827c-b60e0fd46ff41-c91c369b9d6ca8ec0716617909870220-640-0.webp\" alt=\"\" width=\"320\" height=\"320\" /></p>";
                
            }
            
            if(i == 64)
            {
                pages[i - 1].ContentUser = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://conexaoplaneta.com.br/wp-content/uploads/2021/11/lagosta-algodao-doce-conexao-planeta.jpg\" alt=\"\" width=\"320\" height=\"180\" /></p>";
                
            }

            if (i == 65)
            {
                pages[i - 1].ContentUser = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://www.coisasdaroca.com/wp-content/uploads/2020/12/tatu-2-2048x1552.jpg\" alt=\"\" width=\"320\" height=\"243\" /></p>";
                
            }

            if (i == 66)
            {
                pages[i - 1].ContentUser = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://scontent.fiza4-1.fna.fbcdn.net/v/t1.6435-9/191660497_4068354933210851_7224002615706470838_n.jpg?_nc_cat=108&amp;ccb=1-7&amp;_nc_sid=7f8c78&amp;_nc_ohc=9zDHo-kI-FwAX_ckcd5&amp;_nc_ht=scontent.fiza4-1.fna&amp;oh=00_AfD6HxStz5_WMVhSlf-TMTfIMq8QdxZZSZVwP_y5UFgPPQ&amp;oe=65567A83\" alt=\"\" width=\"320\" height=\"143\" /></p>";
                
            }

            if (i == 67)
            {
                pages[i - 1].ContentUser = "<p style=\"text-align: center;\"><iframe title=\"VOC&Ecirc; PEGARIA EM UMA ARANHA? #shorts\" src=\"https://www.youtube.com/embed/rlrXdcG-69U\" width=\"390\" height=\"692\" frameborder=\"0\" allowfullscreen=\"allowfullscreen\"></iframe></p>";
                
            }

            if (i == 68)
            {
                pages[i - 1].ContentUser = "<p style=\"text-align: center;\"><iframe title=\"A DORY EXISTE DE VERDADE? #shorts\" src=\"https://www.youtube.com/embed/db-qqVLT3L8\" width=\"390\" height=\"692\" frameborder=\"0\" allowfullscreen=\"allowfullscreen\"></iframe></p>";
                
            }

            if (i == 69)
            {
                pages[i - 1].ContentUser = "<p style=\"text-align: center;\"><iframe title=\"QUAL A DIFEREN&Ccedil;A ENTRE O SHIH-TZU E O LHASA APSO?  #shorts\" src=\"https://www.youtube.com/embed/SHNFZwva838\" width=\"390\" height=\"692\" frameborder=\"0\" allowfullscreen=\"allowfullscreen\"></iframe></p>";
                
            }

            if (i == 70)
            {
                pages[i - 1].ContentUser = "<p style=\"text-align: center;\"><iframe title=\"ESSE &Eacute; O ANIMAL MAIS FORTE DO MUNDO? #shorts\" src=\"https://www.youtube.com/embed/scmu9XHAkds\" width=\"390\" height=\"692\" frameborder=\"0\" allowfullscreen=\"allowfullscreen\"></iframe></p>";
                
            }

            if (i == 71)
            {
                pages[i - 1].ContentUser = "<p style=\"text-align: center;\"><iframe title=\"DOBERMAN OU CANE CORSO, QUAL O MELHOR? #shorts\" src=\"https://www.youtube.com/embed/h-4aKF2ZoPQ\" width=\"390\" height=\"692\" frameborder=\"0\" allowfullscreen=\"allowfullscreen\"></iframe></p>";
                
            }

            if (i == 72)
            {
                pages[i - 1].ContentUser = "<p style=\"text-align: center;\"><iframe title=\"PEIXE VOADOR! #shorts\" src=\"https://www.youtube.com/embed/x2V6x46ww2k\" width=\"390\" height=\"692\" frameborder=\"0\" allowfullscreen=\"allowfullscreen\"></iframe></p>";
                
            }

            if (i == 73)
            {
                pages[i - 1].ContentUser = "<p style=\"text-align: center;\"><iframe title=\"O PORCO DO CERRADO! #shorts\" src=\"https://www.youtube.com/embed/k5aSVQDMwcw\" width=\"390\" height=\"692\" frameborder=\"0\" allowfullscreen=\"allowfullscreen\"></iframe></p>";
                
            }

            if (i == 74)
            {
                pages[i - 1].ContentUser = "<p style=\"text-align: center;\"><iframe title=\"VOC&Ecirc; CONHECE A ARAPONGA? #shorts\" src=\"https://www.youtube.com/embed/6Cs3u8JsilM\" width=\"390\" height=\"692\" frameborder=\"0\" allowfullscreen=\"allowfullscreen\"></iframe></p>";
                
            }

            if (i == 75)
            {
                pages[i - 1].ContentUser = "<p style=\"text-align: center;\"><iframe title=\"ESSE &Eacute; O CAVALO DOS CIGANOS! #shorts\" src=\"https://www.youtube.com/embed/8da8cTfGwfw\" width=\"390\" height=\"692\" frameborder=\"0\" allowfullscreen=\"allowfullscreen\"></iframe></p>";
                
            }

            if (i == 76)
            {
                pages[i - 1].ContentUser = "<p style=\"text-align: center;\"><iframe title=\"GALAPAGOS, A ILHA DAS TARTARUGAS GIGANTES!\" src=\"https://www.youtube.com/embed/U-hYSQUpEoY\" width=\"320\" height=\"560\" frameborder=\"0\" allowfullscreen=\"allowfullscreen\"></iframe></p>";
                
            }

            if (i == 77)
            {
                pages[i - 1].ContentUser = "<p style=\"text-align: center;\"><iframe title=\"ESSE &Eacute; O PEIXE VAMPIRO! #shorts\" src=\"https://www.youtube.com/embed/cKH889QYCec\" width=\"389\" height=\"692\" frameborder=\"0\" allowfullscreen=\"allowfullscreen\"></iframe></p>";
                
            }

            if (i == 78)
            {
                pages[i - 1].ContentUser = "<p style=\"text-align: center;\"><iframe title=\"ESSE &Eacute; O PINGUIM DE MAGALH&Atilde;ES! #shorts\" src=\"https://www.youtube.com/embed/vefNdtakmUQ\" width=\"389\" height=\"692\" frameborder=\"0\" allowfullscreen=\"allowfullscreen\"></iframe></p>";
                
            }

            if (i == 79)
            {
                pages[i - 1].ContentUser = "<p style=\"text-align: center;\"><iframe title=\"O MAIOR PEIXE DE &Aacute;GUA DOCE DO PLANETA! #shorts\" src=\"https://www.youtube.com/embed/AgwP8pqqU5Q\" width=\"389\" height=\"692\" frameborder=\"0\" allowfullscreen=\"allowfullscreen\"></iframe></p>";
                
            }

            if (i == 80)
            {
                pages[i - 1].ContentUser = "<p style=\"text-align: center;\"><iframe title=\"VOC&Ecirc; CONHECE A JANDAIA MINEIRA? #shorts\" src=\"https://www.youtube.com/embed/F-DCatGERFk\" width=\"389\" height=\"692\" frameborder=\"0\" allowfullscreen=\"allowfullscreen\"></iframe></p>";
                
            }

            if (i == 81)
            {
                pages[i - 1].ContentUser = "<p style=\"text-align: center;\"><iframe title=\"Esse &eacute; o maior escorpi&atilde;o que j&aacute; vi no Brasil! #shorts\" src=\"https://www.youtube.com/embed/8I0kCYmq86Y\" width=\"389\" height=\"692\" frameborder=\"0\" allowfullscreen=\"allowfullscreen\"></iframe></p>";
                
            }

            if (i == 82)
            {
                pages[i - 1].ContentUser = "<p style=\"text-align: center;\"><iframe title=\"VOC&Ecirc; CONHECE O PACMAN? #shorts\" src=\"https://www.youtube.com/embed/QP_tDLkkNho\" width=\"389\" height=\"692\" frameborder=\"0\" allowfullscreen=\"allowfullscreen\"></iframe></p>";
                
            }

            if (i == 83)
            {
                pages[i - 1].ContentUser = "<p style=\"text-align: center;\"><iframe title=\"A TRUTA ARCO-&Iacute;RIS! #shorts\" src=\"https://www.youtube.com/embed/-zE0QRTOnMA\" width=\"389\" height=\"692\" frameborder=\"0\" allowfullscreen=\"allowfullscreen\"></iframe></p>";
                
            }

            if (i == 84)
            {
                pages[i - 1].ContentUser = "<p style=\"text-align: center;\"><iframe title=\"ISSO PODE ACABAR COM A SUA TARTARUGA DE ESTIMA&Ccedil;&Atilde;O! #shorts\" src=\"https://www.youtube.com/embed/O-NWYzySs_o\" width=\"389\" height=\"692\" frameborder=\"0\" allowfullscreen=\"allowfullscreen\"></iframe></p>";
                
            }

            if (i == 85)
            {
                pages[i - 1].ContentUser = "<p style=\"text-align: center;\"><iframe title=\"Voc&ecirc; conhece o Tatu Galinha? #shorts\" src=\"https://www.youtube.com/embed/K6KiUU3hKkU\" width=\"389\" height=\"692\" frameborder=\"0\" allowfullscreen=\"allowfullscreen\"></iframe></p>";
                
            }

            if (i == 86)
            {
                pages[i - 1].ContentUser = "<p style=\"text-align: center;\"><iframe title=\"A COBRA DE VIDRO! #shorts\" src=\"https://www.youtube.com/embed/DdLowoeRo6k\" width=\"389\" height=\"692\" frameborder=\"0\" allowfullscreen=\"allowfullscreen\"></iframe></p>";
                
            }

            if (i == 87)
            {
                pages[i - 1].ContentUser = "<p style=\"text-align: center;\"><iframe title=\"ESSE ANIMAL ESTAVA EXTINTO! #shorts\" src=\"https://www.youtube.com/embed/kgLPSJjoHJ4\" width=\"389\" height=\"692\" frameborder=\"0\" allowfullscreen=\"allowfullscreen\"></iframe></p>";
                
            }

            if (i == 88)
            {
                pages[i - 1].ContentUser = "<p style=\"text-align: center;\"><iframe title=\"A CRIA&Ccedil;&Atilde;O DE CAMAR&Atilde;O GIGANTE! #shorts\" src=\"https://www.youtube.com/embed/qFQUA2ZSviU\" width=\"389\" height=\"692\" frameborder=\"0\" allowfullscreen=\"allowfullscreen\"></iframe></p>";
                
            }

            if (i == 89)
            {
                pages[i - 1].ContentUser = "<p style=\"text-align: center;\"><iframe title=\"A RA&Ccedil;A DE CAVALOS MAIS RARA DO BRASIL! #shorts\" src=\"https://www.youtube.com/embed/vhVrhi7Sek8\" width=\"389\" height=\"692\" frameborder=\"0\" allowfullscreen=\"allowfullscreen\"></iframe></p>";
                
            }

            if (i == 90)
            {
                pages[i - 1].ContentUser = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://blog.7mboots.com.br/wp-content/webp-express/webp-images/uploads/2020/06/the-black-horse-of-the-frisian-breed-walks-in-the-P77UURU_Easy-Resize.com_.jpg.webp\" alt=\"\" width=\"320\" height=\"218\" /></p>";
                
            }

            if (i == 91)
            {
                pages[i - 1].ContentUser = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://www.lancerural.com.br/wp-content/uploads/2017/03/Puro-sangue-%C3%A1rabe.jpg\" alt=\"\" width=\"320\" height=\"240\" /></p>";
                
            }

            if (i == 92)
            {
                pages[i - 1].ContentUser = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://cavalus.com.br/wp-content/uploads/2020/11/O-cavalo-andaluz-e-uma-das-racas-mais-antigas-do-mundo-pinterest.jpg\" alt=\"\" width=\"320\" height=\"180\" /></p>";
                
            }

            if (i == 93)
            {
                pages[i - 1].ContentUser = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://static.todamateria.com.br/upload/ca/va/cavaloraca-cke.jpg?auto_optimize=low\" alt=\"\" width=\"320\" height=\"252\" /></p>";
                
            }

            if (i == 94)
            {
                pages[i - 1].ContentUser = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://blogs.canalrural.com.br/coisasdocampo/wp-content/uploads/sites/11/2016/08/13559049_1165091073532073_444411928186702723_o.jpg\" alt=\"\" width=\"320\" height=\"235\" /></p>";
                
            }

            if (i == 95)
            {
                pages[i - 1].ContentUser = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://s2.glbimg.com/H46NBKxhNFL6r8fuzit9trFoHAw=/smart/e.glbimg.com/og/ed/f/original/2020/06/30/whatsapp_image_2020-06-30_at_12.04.08.jpeg\" alt=\"\" width=\"320\" height=\"320\" /></p>";
                
            }

            if (i == 96)
            {
                pages[i - 1].ContentUser = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://animalbusiness.com.br/wp-content/uploads/2020/11/cavalo.jpg\" alt=\"\" width=\"320\" height=\"234\" /></p>";
                
            }

            if (i == 97)
            {
                pages[i - 1].ContentUser = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://www.petz.com.br/blog/wp-content/uploads/2022/10/como-os-peixes-se-locomovem-final.jpg\" alt=\"\" width=\"320\" height=\"215\" /></p>";
                
            }

            if (i == 98)
            {
                pages[i - 1].ContentUser = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://p2.trrsf.com/image/fget/cf/774/0/images.terra.com/2014/12/04/peixes1.jpg\" alt=\"\" width=\"320\" height=\"213\" /></p>";
                
            }

            if (i == 99)
            {
                pages[i - 1].ContentUser = "<p><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://oregional.net/wp-content/uploads/2022/11/Peixe-Palhaco.jpg\" alt=\"\" width=\"320\" height=\"320\" /></p>";
                
            }

            

            contexto.Add(pages[i - 1]); 
            contexto.SaveChanges();
            var p2 = await repositoryPagina.includes().FirstAsync(pa => pa.Id == pages[i - 1].Id);
            repositoryPagina.paginas.Add(p2);
        }


    }

    if (!await contexto!.Set<Story>().AnyAsync())
    {

        var storyPadrao = new Story
        {
            PaginaPadraoLink = 0,
            Comentario = false,
            Nome = "Padrao"
        };

        contexto.Add(storyPadrao);
        contexto.SaveChanges();


        string[] stories =
            {
                    "brinquedos", "moveis", "roupas", "calçados", "relogios", "eletrônicos", "eletrodomesticos",
                    "celular", "instrumentos", "cosmeticos", "notbooks"
                    };
        string[] storiesEn =
            {
                    "toy", "furniture", "clothes", "shoe", "clock", "electronics", "appliance",
                    "iphone", "instrument", "cosmetic", "notbooks"
                    };

        Random randNum = new Random();
        for (var t = 0; t < stories.Length; t++)
        {

            var name = stories[t];
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri =
            new Uri($"https://serpapi.com/search.json?q={storiesEn[t]}" +
            "&tbm=shop&location=Dallas&hl=pt&gl=us&key=d0ebdc40d5e725ce2764208d4153772f10f531519c3952b626ce2f3a73191dd3"),
                Headers =
                {
                    { "accept", "application/json" },
                },
            };

            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                // Console.WriteLine(body);
                Welcome livro = JsonConvert.DeserializeObject<Welcome>(body)!;

                var indice = 0;
                var lst = await contexto.Story!.Where(st => st.Nome != "Padrao").ToListAsync();

                var str = new Story();

                str.PaginaPadraoLink = lst.Count + 1;
                str.Nome = name;


                contexto.Add(str);
                await contexto.SaveChangesAsync();

                var indice0 = new Pagina()
                {
                    Data = DateTime.Now,
                    Titulo = "Story - " + str.Nome,
                    StoryId = str.Id,
                    Content = "<p> <h1> Seja bem vindo a Story " + str.Nome + "</h1> </p>",
                    Classificacao = new Classificacao()
                };

                contexto.Add(indice0);
                try
                {
                    contexto.SaveChanges();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                var Story = await contexto.Story!.FirstAsync(st => st.Nome == "Padrao");

                var pag = new Pagina()
                {
                    Data = DateTime.Now,
                    Titulo = "Story - " + str.Nome,
                    StoryId = Story.Id,
                    Content = "<a href='#' id='LinkPadrao'> <h1> Story " + str.Nome + "</h1> </a>",
                    Classificacao = new Classificacao()
                };

                contexto.Add(pag);
                try
                {
                    contexto.SaveChanges();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }



                for (var i = 0; i < livro.ShoppingResults!.Length; i++)
                {
                    Pagina pagina = new Pagina();
                    pagina.Produto = new Produto
                    {
                        Descricao = livro.ShoppingResults[i].Title,
                        Nome = name,
                        Imagem = new List<ImagemProduto>
                        { new ImagemProduto { ArquivoImagem = livro.ShoppingResults[i].Thumbnail!.ToString() } },
                        Preco = decimal.Parse(randNum.Next(50, 3000).ToString()),
                        QuantEstoque = 10,
                         
                    };
                    pagina.Classificacao = new Classificacao();
                    pagina.Story = str;
                    pagina.Titulo = name;
                    contexto.Pagina!.Add(pagina);
                    try
                    {
                        contexto.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                    indice++;

                    if (indice == 2)
                    {
                        for (var j = 0; j <= 2; j++)
                        {
                            Pagina pagi = new Pagina();
                            pagi.Story = str;
                            pagi.Titulo = name;
                            pagi.Classificacao = new Classificacao();
                            contexto.Pagina.Add(pagi);
                            try
                            {
                                contexto.SaveChanges();
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                            }
                        }
                        indice = 0;
                    }


                }
            }
        }

    }

}




    app.Run();

