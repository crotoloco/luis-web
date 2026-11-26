<%@ Page Language="C#" AutoEventWireup="true" CodeFile="admin.aspx.cs" Inherits="admin" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Entre Hojas | Administradores</title>
    <link rel="stylesheet" href="css/entre-hojas-theme.css" />
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:Panel ID="pnlActions" runat="server">
                <asp:Button ID="btnNuevo" runat="server" Text="Nuevo" OnClick="btnNuevo_Click" CssClass="accion btn-blue" />
                <asp:Button ID="btnModificar" runat="server" Text="Modificar" OnClick="btnModificar_Click" CssClass="accion btn-blue" />
                <asp:Button ID="btnEliminar" runat="server" Text="Eliminar" OnClick="btnEliminar_Click" CssClass="accion btn-blue" />
                <asp:Button ID="btnGuardar" runat="server" Text="Guardar" OnClick="btnGuardar_Click" CssClass="accion btn-green" />
                <asp:Button ID="btnCancelar" runat="server" Text="Cancelar" OnClick="btnCancelar_Click" CssClass="accion btn-blue" />
                <asp:Button ID="btnSalir" runat="server" Text="Salir" OnClick="btnSalir_Click" CssClass="accion btn-red" />
            </asp:Panel>

            <!-- Navegación superior -->
            <div id="topNav">
                <asp:Button ID="btnNavPrev" runat="server" Text="&lt; Anterior" CssClass="accion" />
                <asp:Button ID="btnNavNext" runat="server" Text="Siguiente &gt;" CssClass="accion" />
                <asp:Button ID="btnNavFirst" runat="server" Text="Primero" CssClass="accion" />
                <asp:Button ID="btnNavLast" runat="server" Text="Último" CssClass="accion" />
            </div>

            <!-- Campos de Datos Personales y lista -->
            <div id="datosPersonales">
                <fieldset>
                    <legend style="font-weight:bold;">Datos Personales</legend>
                    <asp:Label ID="lblID" runat="server" Text="ID:" AssociatedControlID="txtID" CssClass="lbl" />
                    <asp:TextBox ID="txtID" runat="server" CssClass="input" />
                    <br />
                    <asp:Label ID="lblUsuario" runat="server" Text="Usuario:" AssociatedControlID="txtUsuario" CssClass="lbl" />
                    <asp:TextBox ID="txtUsuario" runat="server" CssClass="input" />
                    <br />
                    <asp:Label ID="lblContrasena" runat="server" Text="Contraseña:" AssociatedControlID="txtContrasena" CssClass="lbl" />
                    <asp:TextBox ID="txtContrasena" runat="server" TextMode="Password" CssClass="input" />
                    <br />
                    <asp:Label ID="lblEstado" runat="server" Text="Estado:" AssociatedControlID="ddlEstado" CssClass="lbl" />
                    <asp:DropDownList ID="ddlEstado" runat="server">
                        <asp:ListItem Text="Seleccione" Value="" />
                        <asp:ListItem Text="Activo" Value="Activo" />
                        <asp:ListItem Text="Inactivo" Value="Inactivo" />
                    </asp:DropDownList>
                    <br />
                    <asp:HiddenField ID="hfEditIndex" runat="server" Value="-1" />
                </fieldset>

                <div>
                    <fieldset>
                        <legend style="font-weight:bold;">Lista de Administradores</legend>
                        <div>
                            <asp:Label ID="lblFiltroUsuario" runat="server" Text="Filtro Usuario:" AssociatedControlID="txtFiltroUsuario" CssClass="lbl" />
                            <asp:TextBox ID="txtFiltroUsuario" runat="server" CssClass="input" AutoPostBack="true" OnTextChanged="txtFiltroUsuario_TextChanged" />
                        </div>
                        <div class="data-area">
                            <asp:GridView ID="gvAdmins" runat="server" AutoGenerateColumns="False" OnSelectedIndexChanged="gvAdmins_SelectedIndexChanged" CssClass="grid" AllowPaging="true" PageSize="10" OnPageIndexChanging="gvAdmins_PageIndexChanging">
                                <Columns>
                                    <asp:CommandField ShowSelectButton="True" SelectText="Seleccionar" />
                                    <asp:BoundField DataField="ID" HeaderText="ID" />
                                    <asp:BoundField DataField="Usuario" HeaderText="Usuario" />
                                    <asp:BoundField DataField="Estado" HeaderText="Estado" />
                                </Columns>
                            </asp:GridView>
                        </div>
                    </fieldset>
                </div>
            </div>

        </div>
    </form>
</body>
</html>
