using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace TesteMenu
{
    public partial class SiteMaster : MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadDynamicMenu();
            }
        }

        
        private void LoadDynamicMenu()
        {
            // Obter itens do menu (simulando dados do banco)
            List<MenuDTO> menuItems = GetMenuItemsFromDataSource();

            // Ordenar por OrdemExibicao
            menuItems = menuItems.OrderBy(x => x.OrdemExibicao).ToList();

            foreach (var item in menuItems)
            {
                AddMenuItemToNavigation(item);
            }
        }

        private List<MenuDTO> GetMenuItemsFromDataSource()
        {
            // Simulando dados do banco de dados
            // Na prática, você buscaria de um banco de dados, serviço ou arquivo de configuração

            return new List<MenuDTO>
        {
            new MenuDTO {
                Id = 1,
                Titulo = "Home",
                Url = "~/Default.aspx",
                Icone = "bi-house",
                OrdemExibicao = 1,
                Ativo = true
            },
            new MenuDTO {
                Id = 2,
                Titulo = "Produtos",
                Url = "~/Produtos",
                Icone = "bi-box-seam",
                OrdemExibicao = 2,
                Ativo = true,
                SubItens = new List<MenuDTO> {
                    new MenuDTO {
                        Id = 21,
                        Titulo = "Cadastro",
                        Url = "~/Produtos/Cadastro.aspx",
                        Icone = "bi-plus-circle",
                        OrdemExibicao = 1,
                        Ativo = true
                    },
                    new MenuDTO {
                        Id = 22,
                        Titulo = "Consulta",
                        Url = "~/Produtos/Consulta.aspx",
                        Icone = "bi-search",
                        OrdemExibicao = 2,
                        Ativo = true
                    }
                }
            },
            new MenuDTO {
                Id = 3,
                Titulo = "Clientes",
                Url = "~/Clientes",
                Icone = "bi-people",
                OrdemExibicao = 3,
                Ativo = true
            },
            new MenuDTO {
                Id = 4,
                Titulo = "Relatórios",
                Url = "~/Relatorios.aspx",
                Icone = "bi-graph-up",
                OrdemExibicao = 4,
                Ativo = true
            }
        };
        }

        private void AddMenuItemToNavigation(MenuDTO item)
        {
            var dynamicMenu = (HtmlGenericControl)FindControl("dynamicMenu");
            if (!item.Ativo) return;

            if (item.SubItens != null && item.SubItens.Any())
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

                if (!string.IsNullOrEmpty(item.Icone))
                {
                    var icon = new HtmlGenericControl("i");
                    icon.Attributes.Add("class", item.Icone + " me-1");
                    a.Controls.Add(icon);
                }

                a.InnerText = item.Titulo;
                li.Controls.Add(a);

                var ul = new HtmlGenericControl("ul");
                ul.Attributes.Add("class", "dropdown-menu");

                foreach (var subItem in item.SubItens.Where(x => x.Ativo).OrderBy(x => x.OrdemExibicao))
                {
                    var subLi = new HtmlGenericControl("li");
                    var subA = new HtmlGenericControl("a");
                    subA.Attributes.Add("class", "dropdown-item");
                    subA.Attributes.Add("href", ResolveUrl(subItem.Url));

                    if (!string.IsNullOrEmpty(subItem.Icone))
                    {
                        var subIcon = new HtmlGenericControl("i");
                        subIcon.Attributes.Add("class", subItem.Icone + " me-1");
                        subA.Controls.Add(subIcon);
                    }

                    subA.InnerText = subItem.Titulo;
                    subLi.Controls.Add(subA);
                    ul.Controls.Add(subLi);
                }

                li.Controls.Add(ul);
                dynamicMenu.Controls.Add(li);
            }
            else
            {
                // Item simples sem submenu
                var li = new HtmlGenericControl("li");
                li.Attributes.Add("class", "nav-item");

                var a = new HtmlGenericControl("a");
                a.Attributes.Add("class", "nav-link");
                a.Attributes.Add("href", ResolveUrl(item.Url));

                // Marcar item ativo
                if (Request.Url.AbsolutePath.ToLower().Contains(item.Url.Replace("~/", "").ToLower()))
                {
                    a.Attributes["class"] = "nav-link active";
                }

                if (!string.IsNullOrEmpty(item.Icone))
                {
                    var icon = new HtmlGenericControl("i");
                    icon.Attributes.Add("class", item.Icone + " me-1");
                    a.Controls.Add(icon);
                }

                a.InnerText = item.Titulo;
                li.Controls.Add(a);
                dynamicMenu.Controls.Add(li);
            }
        }
    }

    public class MenuDTO
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Url { get; set; }
        public string Icone { get; set; }
        public int OrdemExibicao { get; set; }
        public bool Ativo { get; set; }
        public List<MenuDTO> SubItens { get; set; }
    }
}