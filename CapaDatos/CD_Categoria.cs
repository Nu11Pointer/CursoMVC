using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDatos
{
    public class CD_Categoria
    {
        public List<Categoria> Listar()
        {
            var lista = new List<Categoria>();

            try
            {
                using (var oconexion = new SqlConnection(Conexion.cn))
                {
                    string query = "select IdCategoria, Descripcion, Activo from CATEGORIA";

                    var cmd = new SqlCommand()
                    {
                        CommandText = query,
                        Connection = oconexion,
                        CommandType = CommandType.Text
                    };

                    oconexion.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new Categoria()
                            {
                                IdCategoria = Convert.ToInt32(dr["IdCategoria"]),
                                Descripcion = dr["Descripcion"].ToString(),
                                Activo = Convert.ToBoolean(dr["Activo"])
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

        public int Registrar(Categoria obj, out string Mensaje)
        {
            int idautogenerado = 0;
            Mensaje = string.Empty;

            try
            {
                using (var oconexion = new SqlConnection(Conexion.cn))
                {
                    var cmd = new SqlCommand()
                    {
                        CommandText = "sp_RegistrarCategoria",
                        Connection = oconexion,
                        CommandType = CommandType.StoredProcedure
                    };
                    cmd.Parameters.AddWithValue("Descripcion", obj.Descripcion);
                    cmd.Parameters.AddWithValue("Activo", obj.Activo);
                    cmd.Parameters.Add("Resultado", SqlDbType.Int).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("Mensaje", SqlDbType.VarChar, 500).Direction = ParameterDirection.Output;

                    oconexion.Open();

                    cmd.ExecuteNonQuery();

                    idautogenerado = Convert.ToInt32(cmd.Parameters["Resultado"].Value);
                    Mensaje = cmd.Parameters["Mensaje"].Value.ToString();
                }
            }
            catch (Exception e)
            {
                idautogenerado = 0;
                Mensaje = e.Message;
            }
            return idautogenerado;
        }

        public bool Editar(Categoria obj, out string Mensaje)
        {
            bool resultado = false;
            Mensaje = string.Empty;

            try
            {
                using (var oconexion = new SqlConnection(Conexion.cn))
                {
                    var cmd = new SqlCommand()
                    {
                        CommandText = "sp_EditarCategoria",
                        Connection = oconexion,
                        CommandType = CommandType.StoredProcedure
                    };
                    cmd.Parameters.AddWithValue("IdCategoria", obj.IdCategoria);
                    cmd.Parameters.AddWithValue("Descripcion", obj.Descripcion);
                    cmd.Parameters.AddWithValue("Activo", obj.Activo);
                    cmd.Parameters.Add("Mensaje", SqlDbType.VarChar, 500).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("Resultado", SqlDbType.Int).Direction = ParameterDirection.Output;

                    oconexion.Open();

                    cmd.ExecuteNonQuery();

                    resultado = Convert.ToBoolean(cmd.Parameters["Resultado"].Value);
                    Mensaje = cmd.Parameters["Mensaje"].Value.ToString();
                }
            }
            catch (Exception e)
            {
                resultado = false;
                Mensaje = e.Message;
            }

            return resultado;
        }

        public bool Eliminar(int id, out string Mensaje)
        {
            bool resultado = false;
            Mensaje = string.Empty;

            try
            {
                using (var oconexion = new SqlConnection(Conexion.cn))
                {
                    var cmd = new SqlCommand()
                    {
                        CommandText = "sp_EliminarCategoria",
                        Connection = oconexion,
                        CommandType = CommandType.StoredProcedure
                    };
                    cmd.Parameters.AddWithValue("IdCategoria", id);
                    cmd.Parameters.Add("Mensaje", SqlDbType.VarChar, 500).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("Resultado", SqlDbType.Int).Direction = ParameterDirection.Output;

                    oconexion.Open();

                    cmd.ExecuteNonQuery();

                    resultado = Convert.ToBoolean(cmd.Parameters["Resultado"].Value);
                    Mensaje = cmd.Parameters["Mensaje"].Value.ToString();
                }
            }
            catch (Exception e)
            {
                resultado = false;
                Mensaje = e.Message;
            }

            return resultado;
        }
    }
}
