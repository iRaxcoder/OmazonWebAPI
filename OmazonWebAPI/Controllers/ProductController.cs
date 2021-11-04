using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using OmazonWebAPI.Connections;
using OmazonWebAPI.Models;
using OmazonWebAPI.Utility;
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
        [Route("SendProviderRequest")]
        public IActionResult DoAccessRequest([FromBody] ServiceResponse request)
        {
            SqlServerConnection DbConnection = new SqlServerConnection(Configuration);
            this.ExecRegisterRequest(DbConnection, request);
            string response = GetDbResponse(DbConnection);
            DbConnection.SqlConnection.Close();

            return Ok(response);
        }

        private void ExecRegisterRequest(SqlServerConnection DbConnection, ServiceResponse model)
        {
            string param_clave = "@param_clave"
            , param_firma = "@param_firma",
             commandText = "OMAZON.sp_CREAR_SOLICITUD";
            DbConnection.InitSqlComponents(commandText, 1);
            DbConnection.CreateParameter(param_clave, SqlDbType.VarChar, model.Msg1);
            DbConnection.CreateParameter(param_firma, SqlDbType.VarChar, model.Msg2);
            DbConnection.ExcecuteReader();
        }
        private String GetDbResponse(SqlServerConnection DbConnection)
        {
            var read = DbConnection.SqlDataReader.Read();
            return DbConnection.SqlDataReader.GetString(0);
        }

        [HttpPost]
        [Route("ManageAccess")]
        public IActionResult DoManageAccess([FromBody] ServiceResponse request)
        {
            SqlServerConnection DbConnection = new SqlServerConnection(Configuration);
            this.ExecManageRequest(DbConnection, request);
            string response = GetDbResponse(DbConnection);
            DbConnection.SqlConnection.Close();

            if (response.Equals("0"))
            {
                return Ok("La solicitud de acceso no corresponde a ninguna clave");
            }
            else
            {
                switch (response)
                {
                    case "1":
                        this.SendToOmazonProducts(this.GetP1Products(), Int32.Parse(response));
                        break;
                    case "2":
                        this.SendToOmazonProducts(this.GetP2Products(), Int32.Parse(response));
                        break;
                    case "3":
                        this.SendToOmazonProducts(this.GetP3Products(), Int32.Parse(response));
                        break;
                }


                return Ok("La solicitud de acceso ha sido atendida con éxito");
            }
        }

        private void ExecManageRequest(SqlServerConnection DbConnection, ServiceResponse model)
        {
            string param_clave = "@param_clave",
                commandText = "OMAZON.sp_VERIFICAR_VALIDAR_SOLICITUD";

            DbConnection.InitSqlComponents(commandText, 1);
            DbConnection.CreateParameter(param_clave, SqlDbType.VarChar, model.Msg1);
            DbConnection.ExcecuteReader();
        }

        private void SendToOmazonProducts(List<ProductModel> ProviderProductList, int Id_proveedor)
        {
            SqlServerConnection DbConnection = new SqlServerConnection(Configuration);
            DbConnection.SqlConnection.Open();
            String commandText = "OMAZON.sp_INSERTAR_PRODUCTO_PROVEEDOR_EXTERNO";
            DbConnection.InitSqlComponents(commandText, 1);
            SqlTransaction transaction = DbConnection.SqlConnection.BeginTransaction();
            DbConnection.SqlCommand.Transaction = transaction;
            DbConnection.SqlCommand.CommandType = CommandType.StoredProcedure;
            try
            {
                ProviderProductList.ForEach(p =>
                {
                    DbConnection.CreateParameter("@param_NOMBRE", SqlDbType.VarChar, p.Name);
                    DbConnection.CreateParameter("@param_STOCK", SqlDbType.VarChar, p.Stock);
                    DbConnection.CreateParameter("@param_PRECIO", SqlDbType.VarChar, p.Price);
                    DbConnection.CreateParameter("@param_ID_PROVEEDOR", SqlDbType.VarChar, Id_proveedor);
                    DbConnection.CreateParameter("@param_RUTA_IMAGEN", SqlDbType.VarChar, p.ImagePath);
                    DbConnection.CreateParameter("@param_CATEGORIA", SqlDbType.VarChar, p.Category);
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
                    Price= DbConnection.SqlDataReader["PRECIO"].ToString(),
                    Stock= int.Parse(DbConnection.SqlDataReader["STOCK"].ToString()),
                    ImagePath= DbConnection.SqlDataReader["RUTA_IMAGEN"].ToString(),
                    Category= DbConnection.SqlDataReader["NOMBRE_CATEGORIA"].ToString(),
                }) ;
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
                    Name = DbConnection.NpgsqlReader["nombre"].ToString(),
                    Price = DbConnection.NpgsqlReader["nombre"].ToString(),
                    Stock = int.Parse(DbConnection.NpgsqlReader["stock"].ToString()),
                    ImagePath = DbConnection.NpgsqlReader["ruta_imagen"].ToString(),
                    Category = DbConnection.NpgsqlReader["nombre_categoria"].ToString(),
                });
            }
            DbConnection.NpgsqlConnection.Close();
            return ProductList;
        }
    }
}
