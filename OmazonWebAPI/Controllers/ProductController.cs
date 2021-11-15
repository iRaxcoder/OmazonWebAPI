using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using OmazonWebAPI.Connections;
using OmazonWebAPI.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace OmazonWebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController : Controller
    {

        public IConfiguration Configuration { get; }
        public ProductController(IConfiguration Config)
        {
            Configuration = Config;
        }

        [HttpPost]
        [Route("/Omazon/API/SendProviderRequest")]
        public IActionResult DoAccessRequest([FromBody] AccessRequestModel request)
        {
            SqlServerConnection DbConnection = new SqlServerConnection(Configuration);
            this.ExecRegisterRequest(DbConnection, request);
            string response = GetDbResponse(DbConnection).ToString();
            DbConnection.SqlConnection.Close();

            return Ok(response);
        }

        private void ExecRegisterRequest(SqlServerConnection DbConnection, AccessRequestModel model)
        {
            string param_clave = "@param_clave"
            , param_firma = "@param_firma",
             commandText = "OMAZON.sp_CREAR_SOLICITUD";
            DbConnection.InitSqlComponents(commandText, 1);
            DbConnection.CreateParameter(param_clave, SqlDbType.VarChar, model.Key);
            DbConnection.CreateParameter(param_firma, SqlDbType.VarChar, model.Details);
            DbConnection.ExcecuteReader();
        }
        private object GetDbResponse(SqlServerConnection DbConnection)
        {
            var read = DbConnection.SqlDataReader.Read();
            return DbConnection.SqlDataReader.GetValue(0);
        }

        [HttpPost]
        [Route("/Omazon/API/ManageAccess")]
        public IActionResult DoManageAccess([FromBody] AccessRequestModel request)
        {
            SqlServerConnection DbConnection = new SqlServerConnection(Configuration);
            this.ExecManageRequest(DbConnection, request);
            string response = GetDbResponse(DbConnection).ToString();
            DbConnection.SqlConnection.Close();

            switch (response)
            {
                case "-1":
                    return Ok("La solicitud no pudo ser procesada. Ya ha sido atendida.");
                case "0":
                    return Ok("La solicitud de acceso no corresponde a ninguna clave de acceso. Consulte con el administrador.");
                case "1":
                    this.SendToOmazonProducts(this.GetP1Products(), int.Parse(response));
                    break;
                case "2":
                    this.SendToOmazonProducts(this.GetP2Products(), int.Parse(response));
                    break;
                case "3":
                    this.SendToOmazonProducts(this.GetP3Products(), int.Parse(response));
                    break;
            }
            return Ok("La solicitud de acceso ha sido atendida con éxito");
        }

        private void ExecManageRequest(SqlServerConnection DbConnection, AccessRequestModel model)
        {
            string param_clave = "@param_id",
                commandText = "OMAZON.sp_VERIFICAR_VALIDAR_SOLICITUD";

            DbConnection.InitSqlComponents(commandText, 1);
            DbConnection.CreateParameter(param_clave, SqlDbType.VarChar, model.RequestId);
            DbConnection.ExcecuteReader();
        }

        private void SendToOmazonProducts(List<ProductModel> ProviderProductList, int Id_proveedor)
        {
            SqlServerConnection DbConnection = new SqlServerConnection(Configuration);
            string commandText = "OMAZON.sp_INSERTAR_PRODUCTO_PROVEEDOR_EXTERNO";
            DbConnection.InitSqlComponents(commandText, 1);
            DbConnection.SqlCommand.CommandType = CommandType.StoredProcedure;
            DbConnection.SqlConnection.Open();
            SqlTransaction transaction = DbConnection.SqlConnection.BeginTransaction();
            DbConnection.SqlCommand.Transaction = transaction;

            DbConnection.SqlCommand.Parameters.Add("@param_NOMBRE", SqlDbType.VarChar);
            DbConnection.SqlCommand.Parameters.Add("@param_STOCK", SqlDbType.VarChar);
            DbConnection.SqlCommand.Parameters.Add("@param_PRECIO", SqlDbType.VarChar);
            DbConnection.SqlCommand.Parameters.Add("@param_ID_PROVEEDOR", SqlDbType.VarChar);
            DbConnection.SqlCommand.Parameters.Add("@param_RUTA_IMAGEN", SqlDbType.VarChar);
            DbConnection.SqlCommand.Parameters.Add("@param_CATEGORIA", SqlDbType.VarChar);
            try
            {
                ProviderProductList.ForEach(p =>
                {
                    DbConnection.SqlCommand.Parameters["@param_NOMBRE"].Value = p.Name;
                    DbConnection.SqlCommand.Parameters["@param_STOCK"].Value = p.Stock;
                    DbConnection.SqlCommand.Parameters["@param_PRECIO"].Value = p.Price;
                    DbConnection.SqlCommand.Parameters["@param_ID_PROVEEDOR"].Value = Id_proveedor;
                    DbConnection.SqlCommand.Parameters["@param_RUTA_IMAGEN"].Value = p.ImagePath;
                    DbConnection.SqlCommand.Parameters["@param_CATEGORIA"].Value = p.Category;

                    DbConnection.JustExecuteNonQuery();
                });
                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                DbConnection.SqlConnection.Close();
                throw;
            }
            DbConnection.SqlConnection.Close();
        }

        private List<ProductModel> GetP1Products()
        {
            SqlServerConnection DbConnection = new SqlServerConnection(Configuration);
            string commandText = "PRODUCTO.OBTENER_PRODUCTOS";
            DbConnection.InitSqlComponents(commandText, 2);
            DbConnection.ExcecuteReader();

            List<ProductModel> ProductList = new List<ProductModel>();

            while (DbConnection.SqlDataReader.Read())
            {
                ProductList.Add(new ProductModel
                {
                    Name = DbConnection.SqlDataReader["NOMBRE"].ToString(),
                    Price = DbConnection.SqlDataReader["PRECIO"].ToString(),
                    Stock = int.Parse(DbConnection.SqlDataReader["STOCK"].ToString()),
                    ImagePath = DbConnection.SqlDataReader["RUTA_IMAGEN"].ToString(),
                    Category = DbConnection.SqlDataReader["NOMBRE_CATEGORIA"].ToString(),
                });
            }
            DbConnection.SqlConnection.Close();
            return ProductList;
        }

        //Obtener Productos proveedor 2
        private List<ProductModel> GetP2Products()
        {
            _MySqlConnection DbConnection = new _MySqlConnection(Configuration);
            string commandText = "sp_mostrar_productos";
            DbConnection.InitMySqlComponents(commandText, 3);
            DbConnection.ExcecuteReader();

            List<ProductModel> ProductList = new List<ProductModel>();

            while (DbConnection.MySqlDataReader.Read())
            {
                ProductList.Add(new ProductModel
                {
                    Name = DbConnection.MySqlDataReader["NOMBRE_PRODUCTO"].ToString(),
                    Price = DbConnection.MySqlDataReader["PRECIO"].ToString(),
                    Stock = int.Parse(DbConnection.MySqlDataReader["STOCK"].ToString()),
                    ImagePath = DbConnection.MySqlDataReader["RUTA_IMAGEN"].ToString(),
                    Category = DbConnection.MySqlDataReader["NOMBRE_CATEGORIA"].ToString(),
                });
            }
            DbConnection.MySqlConnection.Close();
            return ProductList;
        }

        //Obtener Productos proveedor 3
        private List<ProductModel> GetP3Products()
        {
            PostgreSQL DbConnection = new PostgreSQL(Configuration);
            string commandText = "public.obtenerproducto";
            DbConnection.InitPostgresSqlComponents(commandText);
            DbConnection.ExcecuteReader();

            List<ProductModel> ProductList = new List<ProductModel>();

            while (DbConnection.NpgsqlReader.Read())
            {
                ProductList.Add(new ProductModel
                {

                    Name = DbConnection.NpgsqlReader.GetString(2),
                    Price = DbConnection.NpgsqlReader.GetInt32(3).ToString(),
                    Stock = DbConnection.NpgsqlReader.GetInt32(5),
                    ImagePath = DbConnection.NpgsqlReader.GetString(4),
                    Category = DbConnection.NpgsqlReader.GetString(1)
                });
            }
            DbConnection.NpgsqlConnection.Close();
            return ProductList;
        }

        [HttpGet]
        [Route("/Omazon/API/Requests")]
        public IActionResult GetRequests()
        {
            SqlServerConnection DbConnection = new SqlServerConnection(Configuration);
            string commandText = "OMAZON.sp_OBTENER_SOLICITUDES";
            DbConnection.InitSqlComponents(commandText, 1);
            DbConnection.ExcecuteReader();

            List<AccessRequestModel> RequestList = new List<AccessRequestModel>();

            while (DbConnection.SqlDataReader.Read())
            {
                RequestList.Add(new AccessRequestModel
                {
                    RequestId = int.Parse(DbConnection.SqlDataReader["ID_SOLICITUD"].ToString()),
                    Key = DbConnection.SqlDataReader["CLAVE"].ToString(),
                    Details = DbConnection.SqlDataReader["DETALLES"].ToString(),
                });
            }
            DbConnection.SqlConnection.Close();
            return Ok(RequestList);
        }

        [HttpDelete]
        [Route("/Omazon/API/Delete-request/{id}")]
        public IActionResult DeleteRequest(int id)
        {
                SqlServerConnection DbConnection = new SqlServerConnection(Configuration);

            string param_clave = "@param_ID_SOLICITUD",
            commandText = "OMAZON.sp_BORRAR_SOLICITUD";

            DbConnection.InitSqlComponents(commandText, 1);
            DbConnection.CreateParameter(param_clave, SqlDbType.Int, id);
            DbConnection.ExcecuteReader();
                DbConnection.SqlConnection.Close();
                return Ok("Borrada con éxito");
        }
    }
}
