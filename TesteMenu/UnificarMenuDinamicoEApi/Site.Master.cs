using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace UnificarMenuDinamicoEApi
{
    public partial class SiteMaster : MasterPage
    {
        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Chamada síncrona que envolve a tarefa assíncrona
                LoadDynamicMenu();
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            
        }


        private void LoadDynamicMenu()
        {
            // Obter itens do menu (simulando dados do banco)
            var dadosApi = Task.Run(async () => await ObterDadosDaApi()).GetAwaiter().GetResult();

            var apiResponse = JsonConvert.DeserializeObject<ApiResponse<MenuSistema>>(dadosApi);

            if (apiResponse.Status && apiResponse.Dados != null)
            {
                // Organizar os menus hierarquicamente
                var menusHierarquicos = OrganizarMenusHierarquicamente(apiResponse.Dados);

                // Renderizar o menu na página
                RenderizarMenu(menusHierarquicos);
            }
        }

        private List<MenuSistema> GetMenuItemsFromDataSource()
        {
            // Simulando dados do banco de dados
            // Na prática, você buscaria de um banco de dados, serviço ou arquivo de configuração

            return new List<MenuSistema>();
        //{
        //    new MenuDTO {
        //        Id = 1,
        //        Titulo = "Home",
        //        Url = "~/Default.aspx",
        //        Icone = "bi-house",
        //        OrdemExibicao = 1,
        //        Ativo = true
        //    },
        //    new MenuDTO {
        //        Id = 2,
        //        Titulo = "Produtos",
        //        Url = "~/Produtos",
        //        Icone = "bi-box-seam",
        //        OrdemExibicao = 2,
        //        Ativo = true,
        //        SubItens = new List<MenuDTO> {
        //            new MenuDTO {
        //                Id = 21,
        //                Titulo = "Cadastro",
        //                Url = "~/Produtos/Cadastro.aspx",
        //                Icone = "bi-plus-circle",
        //                OrdemExibicao = 1,
        //                Ativo = true
        //            },
        //            new MenuDTO {
        //                Id = 22,
        //                Titulo = "Consulta",
        //                Url = "~/Produtos/Consulta.aspx",
        //                Icone = "bi-search",
        //                OrdemExibicao = 2,
        //                Ativo = true
        //            }
        //        }
        //    },
        //    new MenuDTO {
        //        Id = 3,
        //        Titulo = "Clientes",
        //        Url = "~/Clientes",
        //        Icone = "bi-people",
        //        OrdemExibicao = 3,
        //        Ativo = true
        //    },
        //    new MenuDTO {
        //        Id = 4,
        //        Titulo = "Relatórios",
        //        Url = "~/Relatorios.aspx",
        //        Icone = "bi-graph-up",
        //        OrdemExibicao = 4,
        //        Ativo = true
        //    }
        //};
        }

        private List<MenuSistema> OrganizarMenusHierarquicamente(List<MenuSistema> menus)
        {
            var menusPai = menus.Where(m => m.MenuSistemaPaiId == null).OrderBy(m => m.OrdemExibicaoMenu).ToList();

            foreach (var menuPai in menusPai)
            {
                menuPai.SubMenus = menus
                    .Where(m => m.MenuSistemaPaiId == menuPai.MenuSistemaId)
                    .OrderBy(m => m.OrdemExibicaoMenu)
                    .ToList();
            }

            return menusPai;
        }

        private void RenderizarMenu(List<MenuSistema> menus)
        {
            var menuContainer = (HtmlGenericControl)FindControl("dynamicMenu");
            if (menuContainer == null) return;

            menuContainer.Controls.Clear();

            foreach (var menu in menus.Where(m => m.AtivoMenu))
            {
                if (menu.SubMenus.Any())
                {
                    // Item com submenu
                    var li = new HtmlGenericControl("li");
                    li.Attributes.Add("class", "nav-item dropdown");

                    var a = new HtmlGenericControl("a");
                    a.Attributes.Add("class", "nav-link dropdown-toggle");
                    a.Attributes.Add("href", "#");
                    a.Attributes.Add("role", "button");
                    a.Attributes.Add("data-bs-toggle", "dropdown");
                    a.Attributes.Add("aria-expanded", "false");

                    if (!string.IsNullOrEmpty(menu.IconeMenu))
                    {
                        var icon = new HtmlGenericControl("i");
                        icon.Attributes.Add("class", menu.IconeMenu + " me-1");
                        a.Controls.Add(icon);
                    }

                    a.InnerText = menu.TituloMenu;
                    li.Controls.Add(a);

                    var ul = new HtmlGenericControl("ul");
                    ul.Attributes.Add("class", "dropdown-menu");

                    foreach (var subMenu in menu.SubMenus.Where(m => m.AtivoMenu))
                    {
                        var subLi = new HtmlGenericControl("li");
                        var subA = new HtmlGenericControl("a");
                        subA.Attributes.Add("class", "dropdown-item");
                        subA.Attributes.Add("href", ResolveUrl(subMenu.UrlMenu));

                        if (!string.IsNullOrEmpty(subMenu.IconeMenu))
                        {
                            var subIcon = new HtmlGenericControl("i");
                            subIcon.Attributes.Add("class", subMenu.IconeMenu + " me-1");
                            subA.Controls.Add(subIcon);
                        }

                        subA.InnerText = subMenu.TituloMenu;
                        subLi.Controls.Add(subA);
                        ul.Controls.Add(subLi);
                    }

                    li.Controls.Add(ul);
                    menuContainer.Controls.Add(li);
                }
                else
                {
                    // Item simples sem submenu
                    var li = new HtmlGenericControl("li");
                    li.Attributes.Add("class", "nav-item");

                    var a = new HtmlGenericControl("a");
                    a.Attributes.Add("class", "nav-link");
                    a.Attributes.Add("href", ResolveUrl(menu.UrlMenu));

                    // Marcar item ativo
                    if (Request.Url.AbsolutePath.EndsWith(menu.UrlMenu, StringComparison.OrdinalIgnoreCase))
                    {
                        a.Attributes["class"] = "nav-link active";
                    }

                    if (!string.IsNullOrEmpty(menu.IconeMenu))
                    {
                        var icon = new HtmlGenericControl("i");
                        icon.Attributes.Add("class", menu.IconeMenu + " me-1");
                        a.Controls.Add(icon);
                    }

                    a.InnerText = menu.TituloMenu;
                    li.Controls.Add(a);
                    menuContainer.Controls.Add(li);
                }
            }
        }

        private async Task<string> ObterDadosDaApi()
        {
            string apiUrl = "https://localhost:7291/api/Menu/ReturnMenu";

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(
                    new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await httpClient.GetAsync(apiUrl);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
        }
    }

    public class ApiResponse<T>
    {
        public List<T> Dados { get; set; }
        public string Mensagem { get; set; }
        public bool Status { get; set; }
    }

    public class MenuSistema
    {
        public int MenuSistemaId { get; set; }
        public int? MenuSistemaPaiId { get; set; }
        public string TituloMenu { get; set; }
        public string UrlMenu { get; set; }
        public string IconeMenu { get; set; }
        public int OrdemExibicaoMenu { get; set; }
        public bool AtivoMenu { get; set; }

        // Propriedade para armazenar subitens (não vem do JSON)
        public List<MenuSistema> SubMenus { get; set; } = new List<MenuSistema>();
    }
}