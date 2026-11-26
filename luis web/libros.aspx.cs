using System;
using System.Data;
using System.Globalization;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using MySqlConnector;

public partial class libros : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["User"] == null)
        {
            Response.Redirect("login.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
            return;
        }

        if (!IsPostBack) BindGrid();
    }

    private void BindGrid()
    {
        const string sql = @"SELECT ID_Libro, Codigo, Titulo, Autor, Genero, Precio, Estado
                             FROM Libros
                             WHERE (@filtro = '' OR Codigo LIKE @busqueda OR Titulo LIKE @busqueda OR Autor LIKE @busqueda OR Genero LIKE @busqueda)
                               AND (@estado = '' OR Estado = @estado)
                             ORDER BY ID_Libro DESC";
        using (var connection = Database.CreateConnection())
        using (var command = new MySqlCommand(sql, connection))
        using (var adapter = new MySqlDataAdapter(command))
        {
            string filtro = txtFiltro.Text.Trim();
            command.Parameters.AddWithValue("@filtro", filtro);
            command.Parameters.AddWithValue("@busqueda", "%" + filtro + "%");
            command.Parameters.AddWithValue("@estado", ddlFiltroEstado.SelectedValue);
            var table = new DataTable();
            adapter.Fill(table);
            gvLibros.DataSource = table;
            gvLibros.DataBind();
        }
    }

    protected void btnNuevo_Click(object sender, EventArgs e)
    {
        LimpiarFormulario();
        Mostrar("Ingresá los datos del nuevo libro.", false);
    }

    protected void btnModificar_Click(object sender, EventArgs e)
    {
        if (hfLibroId.Value == "-1") Mostrar("Seleccioná un libro de la tabla para modificarlo.", true);
        else Mostrar("Modificá los datos y presioná Guardar.", false);
    }

    protected void btnEliminar_Click(object sender, EventArgs e)
    {
        int id;
        if (!int.TryParse(hfLibroId.Value, out id)) { Mostrar("Seleccioná un libro para eliminarlo.", true); return; }
        using (var connection = Database.CreateConnection())
        using (var command = new MySqlCommand("DELETE FROM Libros WHERE ID_Libro = @id", connection))
        {
            command.Parameters.AddWithValue("@id", id);
            connection.Open();
            command.ExecuteNonQuery();
        }
        LimpiarFormulario();
        BindGrid();
        Mostrar("Libro eliminado correctamente.", false);
    }

    protected void btnGuardar_Click(object sender, EventArgs e)
    {
        decimal precio;
        if (string.IsNullOrWhiteSpace(txtCodigo.Text) || string.IsNullOrWhiteSpace(txtTitulo.Text) || string.IsNullOrWhiteSpace(txtAutor.Text) || string.IsNullOrEmpty(ddlGenero.SelectedValue)) { Mostrar("Completá código, título, autor y género.", true); return; }
        if (!TryParsePrecio(txtPrecio.Text, out precio) || precio <= 0) { Mostrar("El precio debe ser un valor numérico positivo.", true); return; }

        int id;
        int.TryParse(hfLibroId.Value, out id);
        const string duplicateSql = "SELECT COUNT(*) FROM Libros WHERE Codigo = @codigo AND ID_Libro <> @id";
        using (var connection = Database.CreateConnection())
        using (var duplicate = new MySqlCommand(duplicateSql, connection))
        {
            duplicate.Parameters.AddWithValue("@codigo", txtCodigo.Text.Trim());
            duplicate.Parameters.AddWithValue("@id", id);
            connection.Open();
            if (Convert.ToInt32(duplicate.ExecuteScalar()) > 0) { Mostrar("El código ya está registrado.", true); return; }
        }

        string sql = id > 0
            ? "UPDATE Libros SET Codigo=@codigo, Titulo=@titulo, Autor=@autor, Genero=@genero, Precio=@precio, Estado=@estado WHERE ID_Libro=@id"
            : "INSERT INTO Libros (Codigo, Titulo, Autor, Genero, Precio, Estado) VALUES (@codigo, @titulo, @autor, @genero, @precio, @estado)";
        using (var connection = Database.CreateConnection())
        using (var command = new MySqlCommand(sql, connection))
        {
            command.Parameters.AddWithValue("@codigo", txtCodigo.Text.Trim());
            command.Parameters.AddWithValue("@titulo", txtTitulo.Text.Trim());
            command.Parameters.AddWithValue("@autor", txtAutor.Text.Trim());
            command.Parameters.AddWithValue("@genero", ddlGenero.SelectedValue);
            command.Parameters.AddWithValue("@precio", precio);
            command.Parameters.AddWithValue("@estado", ddlEstado.SelectedValue);
            command.Parameters.AddWithValue("@id", id);
            connection.Open();
            command.ExecuteNonQuery();
        }
        LimpiarFormulario();
        BindGrid();
        Mostrar("Libro guardado correctamente.", false);
    }

    protected void btnCancelar_Click(object sender, EventArgs e) { LimpiarFormulario(); Mostrar("Operación cancelada.", false); }
    protected void FiltroChanged(object sender, EventArgs e) { gvLibros.PageIndex = 0; BindGrid(); }
    protected void gvLibros_PageIndexChanging(object sender, GridViewPageEventArgs e) { gvLibros.PageIndex = e.NewPageIndex; BindGrid(); }

    protected void gvLibros_SelectedIndexChanged(object sender, EventArgs e)
    {
        int id = Convert.ToInt32(gvLibros.SelectedDataKey.Value);
        using (var connection = Database.CreateConnection())
        using (var command = new MySqlCommand("SELECT * FROM Libros WHERE ID_Libro = @id", connection))
        {
            command.Parameters.AddWithValue("@id", id);
            connection.Open();
            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    hfLibroId.Value = id.ToString(); txtCodigo.Text = reader["Codigo"].ToString(); txtTitulo.Text = reader["Titulo"].ToString(); txtAutor.Text = reader["Autor"].ToString(); ddlGenero.SelectedValue = reader["Genero"].ToString(); txtPrecio.Text = Convert.ToDecimal(reader["Precio"]).ToString(CultureInfo.InvariantCulture); ddlEstado.SelectedValue = reader["Estado"].ToString();
                }
            }
        }
        Mostrar("Libro seleccionado.", false);
    }

    protected void btnSalir_Click(object sender, EventArgs e) { Response.Redirect("menu.html", false); }
    private bool TryParsePrecio(string value, out decimal precio)
    {
        string texto = (value ?? string.Empty).Trim();
        if (texto.Contains(".") && !texto.Contains(",")) return decimal.TryParse(texto, NumberStyles.Number, CultureInfo.InvariantCulture, out precio);
        return decimal.TryParse(texto, NumberStyles.Number, CultureInfo.CurrentCulture, out precio);
    }
    private void LimpiarFormulario() { hfLibroId.Value = "-1"; txtCodigo.Text = string.Empty; txtTitulo.Text = string.Empty; txtAutor.Text = string.Empty; ddlGenero.SelectedIndex = 0; txtPrecio.Text = string.Empty; ddlEstado.SelectedIndex = 0; }
    private void Mostrar(string mensaje, bool error) { lblMensaje.Text = mensaje; lblMensaje.CssClass = error ? "form-message error-message" : "form-message success-message"; }
}
