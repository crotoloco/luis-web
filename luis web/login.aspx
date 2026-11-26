<%@ Page Language="C#" AutoEventWireup="true" CodeFile="login.aspx.cs" Inherits="login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Entre Hojas | Ingreso</title>
    <link rel="stylesheet" href="css/entre-hojas-theme.css" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="login-card">
            <fieldset>
                <h2>Bienvenido</h2>
                <p>INGRESA TUS DATOS PARA CONTINUAR</p>

                <asp:Label ID="lblUsuarioLogin" runat="server" Text="Usuario:" CssClass="lbl" AssociatedControlID="txtUsuario" />
                <asp:TextBox ID="txtUsuario" runat="server" CssClass="input" />

                <asp:Label ID="lblPasswordLogin" runat="server" Text="Contraseña:" CssClass="lbl" AssociatedControlID="txtPassword" />
                <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="input" />

                <asp:Button ID="btn_login" runat="server" Text="Ingresar" OnClick="btn_login_Click" CssClass="accion btn-blue" />

                <div class="helper">Si olvidaste la contraseña contacta al administrador.</div>
            </fieldset>
        </div>
    </form>
</body>
</html>
