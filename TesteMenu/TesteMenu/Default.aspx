<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="TesteMenu._Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <!-- CSS específico da página -->
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="row">
        <div class="col-md-12">
            <h1><i class="bi bi-house-door"></i> Bem-vindo ao Sistema</h1>
            <p class="lead">Este é um exemplo de página usando a MasterPage com menu dinâmico.</p>
            
            <div class="alert alert-info mt-4">
                O menu foi carregado dinamicamente do código C# e suporta:
                <ul>
                    <li>Itens simples</li>
                    <li>Submenus dropdown</li>
                    <li>Ícones do Bootstrap Icons</li>
                    <li>Destaque do item ativo</li>
                    <li>Controle de acesso</li>
                </ul>
            </div>
        </div>
    </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="Scripts" runat="server">
    <!-- JavaScript específico da página -->
</asp:Content>
