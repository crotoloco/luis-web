<%@ Page Language="C#" AutoEventWireup="true" CodeFile="libros.aspx.cs" Inherits="libros" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>Entre Hojas | Libros</title>
    <link rel="stylesheet" href="css/entre-hojas-theme.css" />
</head>
<body>
<form id="form1" runat="server">
<div class="eh-page">
    <div class="page-heading"><span class="eyebrow">Catálogo</span><h1>Gestión de libros</h1><p>Administrá los libros disponibles en Entre Hojas.</p></div>
    <asp:Panel ID="pnlActions" runat="server">
        <asp:Button ID="btnNuevo" runat="server" Text="Nuevo" CssClass="accion" OnClick="btnNuevo_Click" />
        <asp:Button ID="btnModificar" runat="server" Text="Modificar" CssClass="accion" OnClick="btnModificar_Click" />
        <asp:Button ID="btnEliminar" runat="server" Text="Eliminar" CssClass="accion btn-red" OnClick="btnEliminar_Click" />
        <asp:Button ID="btnGuardar" runat="server" Text="Guardar" CssClass="accion btn-green" OnClick="btnGuardar_Click" />
        <asp:Button ID="btnCancelar" runat="server" Text="Cancelar" CssClass="accion" OnClick="btnCancelar_Click" />
        <asp:Button ID="btnSalir" runat="server" Text="Salir" CssClass="accion btn-red" OnClick="btnSalir_Click" />
    </asp:Panel>
    <asp:Label ID="lblMensaje" runat="server" CssClass="form-message" />
    <div class="management-layout">
        <fieldset>
            <legend>Datos del libro</legend>
            <asp:HiddenField ID="hfLibroId" runat="server" Value="-1" />
            <asp:Label ID="lblCodigo" runat="server" Text="Código" CssClass="lbl" AssociatedControlID="txtCodigo" />
            <asp:TextBox ID="txtCodigo" runat="server" CssClass="input" MaxLength="20" />
            <asp:Label ID="lblTitulo" runat="server" Text="Título" CssClass="lbl" AssociatedControlID="txtTitulo" />
            <asp:TextBox ID="txtTitulo" runat="server" CssClass="input" MaxLength="100" />
            <asp:Label ID="lblAutor" runat="server" Text="Autor" CssClass="lbl" AssociatedControlID="txtAutor" />
            <asp:TextBox ID="txtAutor" runat="server" CssClass="input" MaxLength="50" />
            <asp:Label ID="lblGenero" runat="server" Text="Género" CssClass="lbl" AssociatedControlID="ddlGenero" />
            <asp:DropDownList ID="ddlGenero" runat="server" CssClass="input"><asp:ListItem Text="Seleccione" Value="" /><asp:ListItem Text="Drama" Value="Drama" /><asp:ListItem Text="Comedia" Value="Comedia" /><asp:ListItem Text="Terror" Value="Terror" /></asp:DropDownList>
            <asp:Label ID="lblPrecio" runat="server" Text="Precio" CssClass="lbl" AssociatedControlID="txtPrecio" />
            <asp:TextBox ID="txtPrecio" runat="server" CssClass="input" TextMode="Number" />
            <asp:Label ID="lblEstado" runat="server" Text="Estado" CssClass="lbl" AssociatedControlID="ddlEstado" />
            <asp:DropDownList ID="ddlEstado" runat="server" CssClass="input"><asp:ListItem Text="Disponible" Value="Disponible" /><asp:ListItem Text="Prestado" Value="Prestado" /><asp:ListItem Text="Reservado" Value="Reservado" /></asp:DropDownList>
        </fieldset>
        <div>
            <fieldset class="filter-panel"><legend>Filtros de búsqueda</legend><asp:Label ID="lblFiltro" runat="server" Text="Código, título, autor o género" CssClass="lbl" AssociatedControlID="txtFiltro" /><asp:TextBox ID="txtFiltro" runat="server" CssClass="input" AutoPostBack="true" OnTextChanged="FiltroChanged" /><asp:Label ID="lblFiltroEstado" runat="server" Text="Estado" CssClass="lbl" AssociatedControlID="ddlFiltroEstado" /><asp:DropDownList ID="ddlFiltroEstado" runat="server" CssClass="input" AutoPostBack="true" OnSelectedIndexChanged="FiltroChanged"><asp:ListItem Text="Todos" Value="" /><asp:ListItem Text="Disponible" Value="Disponible" /><asp:ListItem Text="Prestado" Value="Prestado" /><asp:ListItem Text="Reservado" Value="Reservado" /></asp:DropDownList></fieldset>
            <div class="data-area"><asp:GridView ID="gvLibros" runat="server" CssClass="grid" AutoGenerateColumns="False" DataKeyNames="ID_Libro" OnSelectedIndexChanged="gvLibros_SelectedIndexChanged" AllowPaging="true" PageSize="10" OnPageIndexChanging="gvLibros_PageIndexChanging"><Columns><asp:CommandField ShowSelectButton="true" SelectText="Seleccionar" /><asp:BoundField DataField="ID_Libro" HeaderText="ID" /><asp:BoundField DataField="Codigo" HeaderText="Código" /><asp:BoundField DataField="Titulo" HeaderText="Título" /><asp:BoundField DataField="Autor" HeaderText="Autor" /><asp:BoundField DataField="Genero" HeaderText="Género" /><asp:BoundField DataField="Precio" HeaderText="Precio" DataFormatString="{0:N2}" /><asp:BoundField DataField="Estado" HeaderText="Estado" /></Columns></asp:GridView></div>
        </div>
    </div>
</div>
</form>
</body>
</html>
