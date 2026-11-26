using System;
using System.Data;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using MySqlConnector;

public partial class clientes : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["User"] == null) { Response.Redirect("login.aspx", false); Context.ApplicationInstance.CompleteRequest(); return; }
        if (!IsPostBack) BindGrid();
    }

    private void BindGrid()
    {
        const string sql = @"SELECT Id_cliente, Apellido, Nombre, DNI, Correo, Genero, Ciudad, Celular, Estado
                             FROM Clientes
                             WHERE (@filtro = '' OR Apellido LIKE @busqueda OR Nombre LIKE @busqueda OR DNI LIKE @busqueda OR Correo LIKE @busqueda)
                             ORDER BY Id_cliente DESC";
        using (var connection = Database.CreateConnection())
        using (var command = new MySqlCommand(sql, connection))
        using (var adapter = new MySqlDataAdapter(command))
        {
            string filtro = txtFiltro.Text.Trim(); command.Parameters.AddWithValue("@filtro", filtro); command.Parameters.AddWithValue("@busqueda", "%" + filtro + "%");
            var table = new DataTable(); adapter.Fill(table); gvClientes.DataSource = table; gvClientes.DataBind();
        }
    }

    protected void btnNuevo_Click(object sender, EventArgs e) { LimpiarFormulario(); Mostrar("Ingresá los datos del nuevo cliente.", false); }
    protected void btnModificar_Click(object sender, EventArgs e) { Mostrar(hfClienteId.Value == "-1" ? "Seleccioná un cliente para modificarlo." : "Modificá los datos y presioná Guardar.", hfClienteId.Value == "-1"); }

    protected void btnEliminar_Click(object sender, EventArgs e)
    {
        int id; if (!int.TryParse(hfClienteId.Value, out id)) { Mostrar("Seleccioná un cliente para eliminarlo.", true); return; }
        using (var connection = Database.CreateConnection()) using (var command = new MySqlCommand("DELETE FROM Clientes WHERE Id_cliente=@id", connection)) { command.Parameters.AddWithValue("@id", id); connection.Open(); command.ExecuteNonQuery(); }
        LimpiarFormulario(); BindGrid(); Mostrar("Cliente eliminado correctamente.", false);
    }

    protected void btnGuardar_Click(object sender, EventArgs e)
    {
        string dni = txtDni.Text.Trim(); string celular = txtCelular.Text.Trim();
        if (string.IsNullOrWhiteSpace(txtApellido.Text) || string.IsNullOrWhiteSpace(txtNombre.Text) || string.IsNullOrWhiteSpace(txtCiudad.Text) || string.IsNullOrEmpty(ddlGenero.SelectedValue)) { Mostrar("Completá los datos personales y seleccioná un género.", true); return; }
        if (dni.Length != 8) { Mostrar("El DNI debe contener exactamente 8 caracteres.", true); return; }
        if (celular.Length != 8 || celular.StartsWith("0") || celular.StartsWith("15")) { Mostrar("El celular debe tener 8 caracteres y no comenzar con 0 ni 15.", true); return; }
        if (!txtCorreo.Text.Contains("@")) { Mostrar("El correo debe contener el símbolo @.", true); return; }
        int id; int.TryParse(hfClienteId.Value, out id);
        using (var connection = Database.CreateConnection()) using (var duplicate = new MySqlCommand("SELECT COUNT(*) FROM Clientes WHERE DNI=@dni AND Id_cliente<>@id", connection))
        {
            duplicate.Parameters.AddWithValue("@dni", dni); duplicate.Parameters.AddWithValue("@id", id); connection.Open(); if (Convert.ToInt32(duplicate.ExecuteScalar()) > 0) { Mostrar("El DNI ya está registrado.", true); return; }
        }
        string sql = id > 0 ? "UPDATE Clientes SET Apellido=@apellido, Nombre=@nombre, DNI=@dni, Correo=@correo, Genero=@genero, Ciudad=@ciudad, Celular=@celular, Estado=@estado WHERE Id_cliente=@id" : "INSERT INTO Clientes (Apellido, Nombre, DNI, Correo, Genero, Ciudad, Celular, Estado) VALUES (@apellido, @nombre, @dni, @correo, @genero, @ciudad, @celular, @estado)";
        using (var connection = Database.CreateConnection()) using (var command = new MySqlCommand(sql, connection))
        {
            command.Parameters.AddWithValue("@apellido", txtApellido.Text.Trim()); command.Parameters.AddWithValue("@nombre", txtNombre.Text.Trim()); command.Parameters.AddWithValue("@dni", dni); command.Parameters.AddWithValue("@correo", txtCorreo.Text.Trim()); command.Parameters.AddWithValue("@genero", ddlGenero.SelectedValue); command.Parameters.AddWithValue("@ciudad", txtCiudad.Text.Trim()); command.Parameters.AddWithValue("@celular", celular); command.Parameters.AddWithValue("@estado", ddlEstado.SelectedValue); command.Parameters.AddWithValue("@id", id); connection.Open(); command.ExecuteNonQuery();
        }
        LimpiarFormulario(); BindGrid(); Mostrar("Cliente guardado correctamente.", false);
    }

    protected void btnCancelar_Click(object sender, EventArgs e) { LimpiarFormulario(); Mostrar("Operación cancelada.", false); }
    protected void FiltroChanged(object sender, EventArgs e) { gvClientes.PageIndex = 0; BindGrid(); }
    protected void gvClientes_PageIndexChanging(object sender, GridViewPageEventArgs e) { gvClientes.PageIndex = e.NewPageIndex; BindGrid(); }

    protected void gvClientes_SelectedIndexChanged(object sender, EventArgs e)
    {
        int id = Convert.ToInt32(gvClientes.SelectedDataKey.Value);
        using (var connection = Database.CreateConnection()) using (var command = new MySqlCommand("SELECT * FROM Clientes WHERE Id_cliente=@id", connection))
        {
            command.Parameters.AddWithValue("@id", id); connection.Open(); using (var reader = command.ExecuteReader()) if (reader.Read()) { hfClienteId.Value = id.ToString(); txtApellido.Text = reader["Apellido"].ToString(); txtNombre.Text = reader["Nombre"].ToString(); txtDni.Text = reader["DNI"].ToString(); txtCorreo.Text = reader["Correo"].ToString(); ddlGenero.SelectedValue = reader["Genero"].ToString(); txtCiudad.Text = reader["Ciudad"].ToString(); txtCelular.Text = reader["Celular"].ToString(); ddlEstado.SelectedValue = reader["Estado"].ToString(); }
        }
        Mostrar("Cliente seleccionado.", false);
    }

    protected void btnSalir_Click(object sender, EventArgs e) { Response.Redirect("menu.html", false); }
    private void LimpiarFormulario() { hfClienteId.Value = "-1"; txtApellido.Text = string.Empty; txtNombre.Text = string.Empty; txtDni.Text = string.Empty; txtCorreo.Text = string.Empty; ddlGenero.SelectedIndex = 0; txtCiudad.Text = string.Empty; txtCelular.Text = string.Empty; ddlEstado.SelectedIndex = 0; }
    private void Mostrar(string mensaje, bool error) { lblMensaje.Text = mensaje; lblMensaje.CssClass = error ? "form-message error-message" : "form-message success-message"; }
}
