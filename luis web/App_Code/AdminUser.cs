using System;

[Serializable]
public class AdminUser
{
    public int ID { get; set; }
    public string Usuario { get; set; }
    public string Contrasena { get; set; }
    public string Estado { get; set; }
}
