using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using OmazonWebAPI.Connections;
using OmazonWebAPI.Models;
using OmazonWebAPI.Utility;
using System;
using System.Collections.Generic;
using System.Data;
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

        private void ExecRegisterRequest (SqlServerConnection DbConnection, ServiceResponse model)
        {
            string param_clave = "@param_clave"
            ,param_firma = "@param_firma",
             commandText = "OMAZON.sp_CREAR_SOLICITUD";
            DbConnection.InitSqlComponents(commandText, 1);
            DbConnection.CreateParameter(param_clave, SqlDbType.VarChar, model.Msg1);
            DbConnection.CreateParameter(param_firma, SqlDbType.VarChar, model.Msg2);
            DbConnection.ExcecuteReader();
        }
        private String GetDbResponse(SqlServerConnection DbConnection)
        {
            var read= DbConnection.SqlDataReader.Read();
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
                        //1. Pedir los productos
                        //2. mandarlos a la base de datos principal.
                        break;
                    case "2":

                        break;
                    case "3":

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

        private void SendToOmazonProducts(List<ProductModel> ProviderProductList)
        {
            SqlServerConnection DbConnection = new SqlServerConnection(Configuration);
            String commandText = "";
            ProviderProductList.ForEach(p =>
            {
                



            });

        }

        

    }
}
