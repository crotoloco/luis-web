using System;
using System.Web.UI;
using MySqlConnector;

public partial class login : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Request.QueryString["salir"] == "1")
        {
            Session.Clear();
            Session.Abandon();
        }

        // Si ya está autenticado, enviar al menú principal
        if (Session["User"] != null)
        {
            Response.Redirect("menu.html");
        }
    }

    protected void btn_login_Click(object sender, EventArgs e)
    {
        // Obtener valores directamente del formulario para evitar dependencias con controles en tiempo de compilación
        string usuarioInput = Request.Form["txtUsuario"] ?? string.Empty;
        string passInput = Request.Form["txtPassword"] ?? string.Empty;
        if (string.IsNullOrWhiteSpace(usuarioInput) || string.IsNullOrWhiteSpace(passInput))
        {
            ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Por favor complete usuario y contraseña');", true);
            return;
        }

        string usuario = usuarioInput.Trim();
        string pass = passInput;

        const string consulta = @"SELECT usuario
                                  FROM Administradores
                                  WHERE usuario = @usuario
                                    AND contrasena = @contrasena
                                  LIMIT 1";

        try
        {
            using (var conexion = Database.CreateConnection())
            using (var comando = new MySqlCommand(consulta, conexion))
            {
                comando.Parameters.AddWithValue("@usuario", usuario);
                comando.Parameters.AddWithValue("@contrasena", pass);
                conexion.Open();

                object resultado = comando.ExecuteScalar();
                if (resultado != null && resultado != DBNull.Value)
                {
                    string usuarioAutenticado = resultado.ToString();
                    Session["User"] = usuarioAutenticado;
                    Response.Redirect("menu.html");
                }
                else
                {
                    MostrarMensaje("Usuario o contraseña incorrectos");
                }
            }
        }
        catch (MySqlException ex)
        {
            MostrarMensaje("No se pudo conectar con MySQL: " + ex.Message);
        }
    }

    private void MostrarMensaje(string mensaje)
    {
        string script = "alert('" + mensaje.Replace("'", "\\'") + "');";
        ClientScript.RegisterStartupScript(this.GetType(), "loginError", script, true);
    }
}
