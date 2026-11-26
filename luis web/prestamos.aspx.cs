using System;
using System.Data;
using System.Globalization;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using MySqlConnector;

public partial class prestamos : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["User"] == null)
        {
            Response.Redirect("login.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
            return;
        }
        if (!IsPostBack)
        {
            CargarOpciones();
            LimpiarFormulario();
            BindGrid();
        }
    }

    private void CargarOpciones()
    {
        using (var connection = Database.CreateConnection())
        {
            connection.Open();
            using (var clients = new MySqlDataAdapter("SELECT Id_cliente, CONCAT(Apellido, ', ', Nombre, ' - DNI ', DNI) AS NombreCompleto FROM Clientes ORDER BY Apellido, Nombre", connection))
            {
                var table = new DataTable(); clients.Fill(table); ddlCliente.DataSource = table; ddlCliente.DataTextField = "NombreCompleto"; ddlCliente.DataValueField = "Id_cliente"; ddlCliente.DataBind(); ddlCliente.Items.Insert(0, new ListItem("Seleccione un cliente", ""));
            }
            using (var books = new MySqlDataAdapter("SELECT ID_Libro, CONCAT(Codigo, ' - ', Titulo) AS LibroCompleto FROM Libros ORDER BY Titulo", connection))
            {
                var table = new DataTable(); books.Fill(table); ddlLibro.DataSource = table; ddlLibro.DataTextField = "LibroCompleto"; ddlLibro.DataValueField = "ID_Libro"; ddlLibro.DataBind(); ddlLibro.Items.Insert(0, new ListItem("Seleccione un libro", ""));
            }
        }
    }

    private void BindGrid()
    {
        const string sql = @"SELECT p.ID_Prestamo, p.Fecha_Inicio, p.Fecha_Fin, p.Estado,
                                    CONCAT(c.Apellido, ', ', c.Nombre) AS Cliente,
                                    CONCAT(l.Codigo, ' - ', l.Titulo) AS Libro
                             FROM Prestamos p
                             INNER JOIN Clientes c ON c.Id_cliente = p.Id_cliente
                             INNER JOIN Libros l ON l.ID_Libro = p.ID_Libro
                             WHERE (@filtro = '' OR c.Apellido LIKE @busqueda OR c.Nombre LIKE @busqueda OR c.DNI LIKE @busqueda OR l.Codigo LIKE @busqueda OR l.Titulo LIKE @busqueda)
                               AND (@estado = '' OR p.Estado = @estado)
                             ORDER BY p.ID_Prestamo DESC";
        using (var connection = Database.CreateConnection())
        using (var command = new MySqlCommand(sql, connection))
        using (var adapter = new MySqlDataAdapter(command))
        {
            string filtro = txtFiltro.Text.Trim(); command.Parameters.AddWithValue("@filtro", filtro); command.Parameters.AddWithValue("@busqueda", "%" + filtro + "%"); command.Parameters.AddWithValue("@estado", ddlFiltroEstado.SelectedValue);
            var table = new DataTable(); adapter.Fill(table); gvPrestamos.DataSource = table; gvPrestamos.DataBind();
        }
    }

    protected void btnNuevo_Click(object sender, EventArgs e) { LimpiarFormulario(); Mostrar("Ingresá los datos del nuevo préstamo.", false); }
    protected void btnModificar_Click(object sender, EventArgs e) { Mostrar(hfPrestamoId.Value == "-1" ? "Seleccioná un préstamo para modificarlo." : "Modificá los datos y presioná Guardar.", hfPrestamoId.Value == "-1"); }

    protected void btnEliminar_Click(object sender, EventArgs e)
    {
        int id; if (!int.TryParse(hfPrestamoId.Value, out id)) { Mostrar("Seleccioná un préstamo para eliminarlo.", true); return; }
        using (var connection = Database.CreateConnection()) using (var command = new MySqlCommand("DELETE FROM Prestamos WHERE ID_Prestamo=@id", connection)) { command.Parameters.AddWithValue("@id", id); connection.Open(); command.ExecuteNonQuery(); }
        LimpiarFormulario(); BindGrid(); Mostrar("Préstamo eliminado correctamente.", false);
    }

    protected void btnGuardar_Click(object sender, EventArgs e)
    {
        DateTime inicio, fin; int id; int.TryParse(hfPrestamoId.Value, out id);
        if (string.IsNullOrEmpty(ddlCliente.SelectedValue) || string.IsNullOrEmpty(ddlLibro.SelectedValue)) { Mostrar("Seleccioná un cliente y un libro.", true); return; }
        if (!DateTime.TryParseExact(txtFechaInicio.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out inicio) || !DateTime.TryParseExact(txtFechaFin.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out fin)) { Mostrar("Ingresá fechas válidas.", true); return; }
        if (fin < inicio) { Mostrar("La fecha de devolución no puede ser anterior a la fecha de inicio.", true); return; }
        string sql = id > 0 ? "UPDATE Prestamos SET ID_Libro=@libro, Id_cliente=@cliente, Fecha_Inicio=@inicio, Fecha_Fin=@fin, Estado=@estado WHERE ID_Prestamo=@id" : "INSERT INTO Prestamos (ID_Libro, Id_cliente, Fecha_Inicio, Fecha_Fin, Estado) VALUES (@libro, @cliente, @inicio, @fin, @estado)";
        using (var connection = Database.CreateConnection()) using (var command = new MySqlCommand(sql, connection))
        {
            command.Parameters.AddWithValue("@libro", ddlLibro.SelectedValue); command.Parameters.AddWithValue("@cliente", ddlCliente.SelectedValue); command.Parameters.AddWithValue("@inicio", inicio); command.Parameters.AddWithValue("@fin", fin); command.Parameters.AddWithValue("@estado", ddlEstado.SelectedValue); command.Parameters.AddWithValue("@id", id); connection.Open(); command.ExecuteNonQuery();
        }
        LimpiarFormulario(); BindGrid(); Mostrar("Préstamo guardado correctamente.", false);
    }

    protected void btnCancelar_Click(object sender, EventArgs e) { LimpiarFormulario(); Mostrar("Operación cancelada.", false); }
    protected void FiltroChanged(object sender, EventArgs e) { gvPrestamos.PageIndex = 0; BindGrid(); }
    protected void gvPrestamos_PageIndexChanging(object sender, GridViewPageEventArgs e) { gvPrestamos.PageIndex = e.NewPageIndex; BindGrid(); }

    protected void gvPrestamos_SelectedIndexChanged(object sender, EventArgs e)
    {
        int id = Convert.ToInt32(gvPrestamos.SelectedDataKey.Value);
        using (var connection = Database.CreateConnection()) using (var command = new MySqlCommand("SELECT * FROM Prestamos WHERE ID_Prestamo=@id", connection))
        {
            command.Parameters.AddWithValue("@id", id); connection.Open(); using (var reader = command.ExecuteReader()) if (reader.Read()) { hfPrestamoId.Value = id.ToString(); ddlCliente.SelectedValue = reader["Id_cliente"].ToString(); ddlLibro.SelectedValue = reader["ID_Libro"].ToString(); txtFechaInicio.Text = Convert.ToDateTime(reader["Fecha_Inicio"]).ToString("yyyy-MM-dd"); txtFechaFin.Text = Convert.ToDateTime(reader["Fecha_Fin"]).ToString("yyyy-MM-dd"); ddlEstado.SelectedValue = reader["Estado"].ToString(); }
        }
        Mostrar("Préstamo seleccionado.", false);
    }

    protected void btnSalir_Click(object sender, EventArgs e) { Response.Redirect("menu.html", false); }
    private void LimpiarFormulario() { hfPrestamoId.Value = "-1"; if (ddlCliente.Items.Count > 0) ddlCliente.SelectedIndex = 0; if (ddlLibro.Items.Count > 0) ddlLibro.SelectedIndex = 0; txtFechaInicio.Text = DateTime.Today.ToString("yyyy-MM-dd"); txtFechaFin.Text = DateTime.Today.AddDays(7).ToString("yyyy-MM-dd"); ddlEstado.SelectedIndex = 0; }
    private void Mostrar(string mensaje, bool error) { lblMensaje.Text = mensaje; lblMensaje.CssClass = error ? "form-message error-message" : "form-message success-message"; }
}
