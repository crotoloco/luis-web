using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;

public partial class admin : System.Web.UI.Page
{
    private const string SessionKey = "AdminUsers";

    protected void Page_Load(object sender, EventArgs e)
    {
        // Requerir login
        if (Session["User"] == null)
        {
            Response.Redirect("login.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
            return;
        }
        if (!IsPostBack)
        {
            EnsureSampleData();
            BindGrid();
            ClearForm();
        }
    }

    private List<AdminUser> GetData()
    {
        var list = Session[SessionKey] as List<AdminUser>;
        if (list == null)
        {
            list = new List<AdminUser>();
            Session[SessionKey] = list;
        }
        return list;
    }

    private void SaveData(List<AdminUser> list)
    {
        Session[SessionKey] = list;
    }

    private void EnsureSampleData()
    {
        var list = GetData();
        if (list.Count == 0)
        {
            list.Add(new AdminUser { ID = 1, Usuario = "admin", Contrasena = "admin", Estado = "Activo" });
            list.Add(new AdminUser { ID = 2, Usuario = "user1", Contrasena = "1234", Estado = "Inactivo" });
            SaveData(list);
        }
    }

    private void BindGrid()
    {
        var list = GetData();
        string filtro = txtFiltroUsuario.Text.Trim();
        if (!string.IsNullOrEmpty(filtro))
        {
            gvAdmins.DataSource = list.FindAll(delegate(AdminUser u) { return u.Usuario.IndexOf(filtro, StringComparison.OrdinalIgnoreCase) >= 0; });
        }
        else
        {
            gvAdmins.DataSource = list;
        }
        gvAdmins.DataBind();
    }

    protected void btnSalir_Click(object sender, EventArgs e)
    {
        Response.Redirect("menu.html", false);
    }

    protected void btnNuevo_Click(object sender, EventArgs e)
    {
        ClearForm();
        hfEditIndex.Value = "-1";
    }

    protected void btnModificar_Click(object sender, EventArgs e)
    {
        if (gvAdmins.SelectedIndex >= 0)
        {
            int idx = gvAdmins.SelectedIndex + (gvAdmins.PageIndex * gvAdmins.PageSize);
            var list = GetData();
            if (idx >= 0 && idx < list.Count)
            {
                var u = list[idx];
                txtID.Text = u.ID.ToString();
                txtUsuario.Text = u.Usuario;
                txtContrasena.Text = u.Contrasena;
                ddlEstado.SelectedValue = u.Estado;
                hfEditIndex.Value = idx.ToString();
            }
        }
    }

    protected void btnEliminar_Click(object sender, EventArgs e)
    {
        if (gvAdmins.SelectedIndex >= 0)
        {
            int idx = gvAdmins.SelectedIndex + (gvAdmins.PageIndex * gvAdmins.PageSize);
            var list = GetData();
            if (idx >= 0 && idx < list.Count)
            {
                list.RemoveAt(idx);
                SaveData(list);
                BindGrid();
                ClearForm();
            }
        }
    }

    protected void btnGuardar_Click(object sender, EventArgs e)
    {
        int editIndex = -1;
        try { editIndex = int.Parse(hfEditIndex.Value); } catch { editIndex = -1; }
        var list = GetData();
        // Validaciones básicas
        if (string.IsNullOrWhiteSpace(txtUsuario.Text))
        {
            ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Ingrese usuario.');", true);
            return;
        }

        if (editIndex >= 0 && editIndex < list.Count)
        {
            // Actualizar
            var u = list[editIndex];
            u.Usuario = txtUsuario.Text.Trim();
            u.Contrasena = txtContrasena.Text;
            u.Estado = ddlEstado.SelectedValue;
        }
        else
        {
            // Nuevo registro
            int nextId = 1;
            if (list.Count > 0) nextId = list[list.Count - 1].ID + 1;
            var u = new AdminUser
            {
                ID = nextId,
                Usuario = txtUsuario.Text.Trim(),
                Contrasena = txtContrasena.Text,
                Estado = ddlEstado.SelectedValue
            };
            list.Add(u);
        }

        SaveData(list);
        BindGrid();
        ClearForm();
        hfEditIndex.Value = "-1";
    }

    protected void btnCancelar_Click(object sender, EventArgs e)
    {
        ClearForm();
        hfEditIndex.Value = "-1";
    }

    private void ClearForm()
    {
        txtID.Text = string.Empty;
        txtUsuario.Text = string.Empty;
        txtContrasena.Text = string.Empty;
        ddlEstado.SelectedIndex = 0;
    }

    protected void gvAdmins_SelectedIndexChanged(object sender, EventArgs e)
    {
        // Marcar selección y cargar datos
        if (gvAdmins.SelectedIndex >= 0)
        {
            int idx = gvAdmins.SelectedIndex + (gvAdmins.PageIndex * gvAdmins.PageSize);
            var list = GetData();
            if (idx >= 0 && idx < list.Count)
            {
                var u = list[idx];
                txtID.Text = u.ID.ToString();
                txtUsuario.Text = u.Usuario;
                txtContrasena.Text = u.Contrasena;
                ddlEstado.SelectedValue = u.Estado;
                hfEditIndex.Value = idx.ToString();
            }
        }
    }

    protected void txtFiltroUsuario_TextChanged(object sender, EventArgs e)
    {
        // Filtrar y refrescar grid
        gvAdmins.PageIndex = 0;
        BindGrid();
    }

    protected void gvAdmins_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        gvAdmins.PageIndex = e.NewPageIndex;
        BindGrid();
    }
}
