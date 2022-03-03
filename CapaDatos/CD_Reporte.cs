using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;

namespace CapaDatos
{
    public class CD_Reporte
    {
        public DashBoard VerDashBoard()
        {
            var objeto = new DashBoard();

            try
            {
                using (var oconexion = new SqlConnection(Conexion.cn))
                {
                    string query = "sp_ReporteDashboard";

                    var cmd = new SqlCommand()
                    {
                        CommandText = query,
                        Connection = oconexion,
                        CommandType = CommandType.StoredProcedure
                    };

                    oconexion.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            objeto = new DashBoard()
                            {
                                TotalCliente = Convert.ToInt32(dr["TotalCliente"]),
                                TotalProducto = Convert.ToInt32(dr["TotalProducto"]),
                                TotalVenta = Convert.ToInt32(dr["TotalVenta"])
                            };
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            return objeto;
        }

        public List<Reporte> Ventas(string fechainicio, string fechafin, string idtransaccion)
        {
            var lista = new List<Reporte>();

            try
            {
                using (var oconexion = new SqlConnection(Conexion.cn))
                {
                    string query = "sp_ReporteVentas";

                    var cmd = new SqlCommand()
                    {
                        CommandText = query,
                        Connection = oconexion,
                        CommandType = CommandType.StoredProcedure
                    };
                    cmd.Parameters.AddWithValue("fechainicio", DateTime.Parse(fechainicio));
                    cmd.Parameters.AddWithValue("fechafin", DateTime.Parse(fechafin));
                    cmd.Parameters.AddWithValue("idtransaccion", idtransaccion);

                    oconexion.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new Reporte()
                            {
                                FechaVenta = dr["FechaVenta"].ToString(),
                                Cliente = dr["Cliente"].ToString(),
                                Producto = dr["Apellidos"].ToString(),
                                Precio = Convert.ToDecimal(dr["Precio"], new CultureInfo("es-NI")),
                                Cantidad = Convert.ToInt32(dr["Cantidad"]),
                                Total = Convert.ToDecimal(dr["Total"], new CultureInfo("es-NI")),
                                IdTransaccion = dr["IdTransaccion"].ToString()
                            });
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            return lista;
        }
    }
}
