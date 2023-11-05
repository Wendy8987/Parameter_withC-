using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using static Parameter_ML.Models.Models;

namespace Parameter_ML.Controllers
{
    public class MemberController : Controller
    {
        // GET: Model
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Register()
        {
            return View();
        }
        public ActionResult DoRegister(DoRegisterIn inModel)
        {
            DoRegisterOut outModel = new DoRegisterOut();
            var test = inModel;
            if (string.IsNullOrEmpty(inModel.UserID) || string.IsNullOrEmpty(inModel.UserPwd) || string.IsNullOrEmpty(inModel.UserName) || string.IsNullOrEmpty(inModel.UserEmail))
            {
                outModel.ErrMsg = "請輸入資料";
            }
            else
            {
                SqlConnection conn = null;
                try
                {
                    // 資料庫連線
                    string connStr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["Model_Paramter"].ConnectionString;
                    conn = new SqlConnection();
                    conn.ConnectionString = connStr;
                    conn.Open();

                    // 檢查帳號是否存在
                    string sql = "select * from Member where UserID = @UserID";
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandText = sql;
                    cmd.Connection = conn;

                    // 使用參數化填值
                    cmd.Parameters.AddWithValue("@UserID", inModel.UserID);

                    // 執行資料庫查詢動作
                    DbDataAdapter adpt = new SqlDataAdapter();
                    adpt.SelectCommand = cmd;
                    DataSet ds = new DataSet();
                    adpt.Fill(ds);

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        outModel.ErrMsg = "此登入帳號已存在";
                    }
                    else
                    {
                        // 將密碼使用 SHA256 雜湊運算(不可逆)
                        string salt = inModel.UserID.Substring(0, 1).ToLower(); //使用帳號前一碼當作密碼鹽
                        SHA256 sha256 = SHA256.Create();
                        byte[] bytes = Encoding.UTF8.GetBytes(salt + inModel.UserPwd); //將密碼鹽及原密碼組合
                        byte[] hash = sha256.ComputeHash(bytes);
                        StringBuilder result = new StringBuilder();
                        for (int i = 0; i < hash.Length; i++)
                        {
                            result.Append(hash[i].ToString("X2"));
                        }
                        string NewPwd = result.ToString(); // 雜湊運算後密碼

                        // 註冊資料新增至資料庫
                        sql = @"INSERT INTO Member (UserID,UserPwd,UserName,UserEmail) VALUES (@UserID, @UserPwd, @UserName, @UserEmail)";
                        cmd = new SqlCommand();
                        cmd.Connection = conn;
                        cmd.CommandText = sql;

                        // 使用參數化填值
                        cmd.Parameters.AddWithValue("@UserID", inModel.UserID);
                        cmd.Parameters.AddWithValue("@UserPwd", NewPwd); // 雜湊運算後密碼
                        cmd.Parameters.AddWithValue("@UserName", inModel.UserName);
                        cmd.Parameters.AddWithValue("@UserEmail", inModel.UserEmail);

                        // 執行資料庫更新動作
                        cmd.ExecuteNonQuery();

                        outModel.ResultMsg = "註冊完成";
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    if (conn != null)
                    {
                        //關閉資料庫連線
                        conn.Close();
                        conn.Dispose();
                    }
                }
            }

            // 輸出json
            return Json(outModel);
        }
    }
}