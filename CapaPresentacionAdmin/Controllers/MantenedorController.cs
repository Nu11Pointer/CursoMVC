using CapaEntidad;
using CapaNegocio;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CapaPresentacionAdmin.Controllers
{
    public class MantenedorController : Controller
    {
        // GET: Mantenedor
        public ActionResult Categoria()
        {
            return View();
        }

        public ActionResult Marca()
        {
            return View();
        }

        public ActionResult Producto()
        {
            return View();
        }

        #region Categoria
        [HttpGet]
        public JsonResult ListarCategorias()
        {
            List<Categoria> oLista = new CN_Categoria().Listar();
            return Json(new { data = oLista }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GuardarCategorias(Categoria objeto)
        {
            object resultado;
            string mensaje = string.Empty;

            if (objeto.IdCategoria == 0)
            {
                resultado = new CN_Categoria().Registrar(objeto, out mensaje);
            }
            else
            {
                resultado = new CN_Categoria().Editar(objeto, out mensaje);
            }

            return Json(new { resultado = resultado, mensaje = mensaje }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult EliminarCategoria(int id)
        {
            bool respuesta = false;
            string mensaje = string.Empty;

            respuesta = new CN_Categoria().Eliminar(id, out mensaje);

            return Json(new { resultado = respuesta, mensaje = mensaje }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Marca
        [HttpGet]
        public JsonResult ListarMarca()
        {
            List<Marca> oLista = new CN_Marca().Listar();
            return Json(new { data = oLista }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GuardarMarca(Marca objeto)
        {
            object resultado;
            string mensaje = string.Empty;

            if (objeto.IdMarca == 0)
            {
                resultado = new CN_Marca().Registrar(objeto, out mensaje);
            }
            else
            {
                resultado = new CN_Marca().Editar(objeto, out mensaje);
            }

            return Json(new { resultado = resultado, mensaje = mensaje }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult EliminarMarca(int id)
        {
            bool respuesta = false;
            string mensaje = string.Empty;

            respuesta = new CN_Marca().Eliminar(id, out mensaje);

            return Json(new { resultado = respuesta, mensaje = mensaje }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Producto
        [HttpGet]
        public JsonResult ListarProducto()
        {
            List<Producto> oLista = new CN_Producto().Listar();
            return Json(new { data = oLista }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GuardarProducto(string objeto, HttpPostedFileBase archivoImagen)
        {
            string mensaje = string.Empty;
            bool operacion_exitosa = true;
            bool guardar_imagen_exito = true;
            Producto oProducto = new Producto();

            // Deserializamos el Objeto Json
            oProducto = JsonConvert.DeserializeObject<Producto>(objeto);

            decimal precio;

            // Intentamos Transformar el Precio (Como texto) a decimal.
            if (decimal.TryParse(oProducto.PrecioTexto,
                NumberStyles.AllowDecimalPoint,
                new CultureInfo("es-NI"), out precio))
            {
                oProducto.Precio = precio;
            }
            else
            {
                return Json(new { operacionExitosa = false, mensaje = "El formato del precio debe ser ##.##" },
                    JsonRequestBehavior.AllowGet);
            }

            // ¿Registrar o Editar?
            if (oProducto.IdProducto == 0)
            {
                // Registramos el Producto
                int idProductoGenerado = new CN_Producto().Registrar(oProducto, out mensaje);

                // Verificamos la operación
                if (idProductoGenerado != 0)
                {
                    oProducto.IdProducto = idProductoGenerado;
                }
                else
                {
                    operacion_exitosa = false;
                }
            }
            else
            {
                // Editamos el producto
                operacion_exitosa = new CN_Producto().Editar(oProducto, out mensaje);
            }

            // Si la operación fue exitosa
            if (operacion_exitosa)
            {
                // ¿Se adjunto una imagen?
                if (archivoImagen != null)
                {
                    // Meta-Datos de la imagen.
                    string ruta_guardar = ConfigurationManager.AppSettings["ServidorFotos"];
                    string extension = Path.GetExtension(archivoImagen.FileName);
                    string nombre_imagen = string.Concat(oProducto.IdProducto.ToString(), extension);

                    try
                    {
                        // Guardar Imagen
                        archivoImagen.SaveAs(Path.Combine(ruta_guardar, nombre_imagen));
                    }
                    catch (Exception ex)
                    {
                        string msg = ex.Message;
                        guardar_imagen_exito = false;
                    }

                    // ¿Se realizó con exito?
                    if (guardar_imagen_exito)
                    {
                        oProducto.RutaImagen = ruta_guardar;
                        oProducto.NombreImagen = nombre_imagen;
                        bool rspta = new CN_Producto().GuardarDatosImagen(oProducto, out mensaje);
                    }
                    else
                    {
                        mensaje = "Se guardó el producto pero hubo problemas con la imagen";
                    }
                }
            }

            return Json(new
            {
                operacion_exitosa = true,
                idGenerado = oProducto.IdProducto,
                mensaje = mensaje
            }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult ImagenProducto(int id)
        {
            bool conversion;
            Producto oproducto = new CN_Producto().Listar().Where(p => p.IdProducto == id).FirstOrDefault();

            string textoBase64 = CN_Recursos.ConvertirBase64(Path.Combine(oproducto.RutaImagen, oproducto.NombreImagen), out conversion);

            return Json(new
            {
                conversion = conversion,
                textoBase64 = textoBase64,
                extension = Path.GetExtension(oproducto.NombreImagen)
            },
            JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult EliminarProducto(int id)
        {
            bool respuesta = false;
            string mensaje = string.Empty;

            respuesta = new CN_Producto().Eliminar(id, out mensaje);

            return Json(new { resultado = respuesta, mensaje = mensaje }, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}