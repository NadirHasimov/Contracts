using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AdyContracts.DomainModels;
using System.Data;
using System.Data.SqlClient;
using AdyContracts.Utils;
using System.Security.Cryptography;
using System.Net;
using System.Net.Sockets;
using System.Web.Mvc;
using AdyContracts.Models;
using System.Security.Principal;
using System.Web.Security;

namespace AdyContracts.DALC
{
    public static class UserDALC
    {
        public static bool CheckPermission(string username)
        {
            using (SqlConnection con = new SqlConnection(AppConfig.ConnectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(SqlQueries.User.checkPermission, con))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@username", username);
                    int result = (int)(cmd.ExecuteScalar() ?? 0);
                    return result > 0;
                }
            }
        }
        public static bool add(UserModel model)
        {
            bool confirmationStatus = model.roleId == null ? false : true;
            model.roleId = model.roleId == null ? 1 : model.roleId;
            string passwordHash;
            using (MD5 md5Hash = MD5.Create())
            {
                passwordHash = HashUtils.GetMd5Hash(md5Hash, model.password);
            }
            using (SqlConnection con = new SqlConnection(AppConfig.ConnectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(SqlQueries.User.addUser, con))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@firstname", model.firstName);
                    cmd.Parameters.AddWithValue("@lastname", model.lastName);
                    cmd.Parameters.AddWithValue("@gender", model.gender);
                    cmd.Parameters.AddWithValue("@birthdate", model.birthdate.Date);
                    cmd.Parameters.AddWithValue("@email", model.email);
                    cmd.Parameters.AddWithValue("@username", model.username);
                    cmd.Parameters.AddWithValue("@password", passwordHash);
                    cmd.Parameters.AddWithValue("@department_id", model.departId);
                    cmd.Parameters.AddWithValue("@role_id", model.roleId);
                    cmd.Parameters.AddWithValue("@confirmation_status", confirmationStatus);
                    int affectedRows = cmd.ExecuteNonQuery();
                    return affectedRows > 0;
                }
            }
        }

        public static bool CheckUsername(string username)
        {
            using (SqlConnection con = new SqlConnection(AppConfig.ConnectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(SqlQueries.User.checkUsername, con))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@username", username);
                    int count = (int)cmd.ExecuteScalar();
                    return count == 0;
                }
            }
        }

        public static void Delete(int[] ids)
        {
            int result = 0;
            using (SqlConnection con = new SqlConnection(AppConfig.ConnectionString))
            {
                con.Open();
                foreach (int id in ids)
                {
                    using (SqlCommand cmd = new SqlCommand(SqlQueries.User.deleteUser, con))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                        result = result + cmd.ExecuteNonQuery();
                    }
                }
            }
        }
        public static bool Login(string username, string password)
        {
            string passwordHash;
            using (MD5 md5Hash = MD5.Create())
            {
                passwordHash = HashUtils.GetMd5Hash(md5Hash, password);
            }
            using (SqlConnection con = new SqlConnection(AppConfig.ConnectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(SqlQueries.User.checkUserLogin, con))
                {
                    int id = 0;
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@password", passwordHash);
                    SqlDataReader reader = cmd.ExecuteReader();
                    reader.Read();
                    if (reader.HasRows)
                    {
                        id = reader.GetInt32(0);
                    }
                    return id > 0;
                }
            }
        }

        public static void InsertLog(string username, string password, bool login_success_state)
        {
            string externalIp = "0", localIp = "0";

            IPHostEntry iPHostEntry = Dns.GetHostEntry(Dns.GetHostName());
            int length = iPHostEntry.AddressList.Length - 1;
            if (!String.IsNullOrEmpty(iPHostEntry.AddressList[length].ToString()))
            {
                localIp = iPHostEntry.AddressList[length].ToString();
            }
            if (HttpContext.Current != null)
            {
                externalIp = string.IsNullOrEmpty(HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"])
                    ? HttpContext.Current.Request.UserHostAddress
                    : HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            }
            if (string.IsNullOrEmpty(externalIp) || externalIp.Trim() == "::1")
            { // still can't decide or is LAN
                var lan = Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault(r => r.AddressFamily == AddressFamily.InterNetwork);
                externalIp = lan == null ? string.Empty : lan.ToString();
            }
            using (SqlConnection con = new SqlConnection(AppConfig.ConnectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(SqlQueries.User.insertLog, con))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@password", password);
                    cmd.Parameters.AddWithValue("@local_machine_ip", localIp);
                    cmd.Parameters.AddWithValue("@external_ip", externalIp);
                    cmd.Parameters.AddWithValue("@login_success_state", login_success_state);
                    cmd.ExecuteNonQuery();
                }
            }

        }

        public static List<SelectListItem> GetDepartments()
        {
            var departList = new List<SelectListItem>();
            using (var con = new SqlConnection(AppConfig.ConnectionString))
            {
                con.Open();
                using (var cmd = new SqlCommand(SqlQueries.User.getDepartments, con))
                {
                    cmd.CommandType = CommandType.Text;
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        departList.Add(
                            new SelectListItem
                            {
                                Value = reader["ID"].ToString(),
                                Text = reader["depart_name"].ToString()
                            });
                    }
                }
            }
            return departList;
        }

        /*Methods for Menus*/

        public static MvcHtmlString GetMenus(string username)
        {
            string role = System.Web.Security.Roles.GetRolesForUser().Single();
            string lang = HttpContext.Current.Request.Cookies["culture"].Value.ToString();
            lang = String.IsNullOrEmpty(lang) ? "Az" : lang;
            //List<MenuViewModel> menus = new List<MenuViewModel>();
            string url = "";
            using (SqlConnection con = new SqlConnection(AppConfig.ConnectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(SqlQueries.User.getMenus, con))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@role_name", role);
                    cmd.Parameters.AddWithValue("@username", username);

                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        url = url + " " + reader[lang].ToString();
                    }
                }
            }
            return MvcHtmlString.Create(url);
        }


        public static string GetRolesForMenu(string actionName)
        {
            string roles = "";
            int counter = 0;
            using (SqlConnection con = new SqlConnection(AppConfig.ConnectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(SqlQueries.User.getRolesForMenu, con))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@menu_name", actionName);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        roles = counter == 0 ? reader["role_name"].ToString()
                                          : roles + "," + reader["role_name"].ToString();
                        counter++;
                    }
                }
            }
            return roles;
        }

        public static string GetUsersForMenu(string actionName)
        {
            string users = "";
            int counter = 0;
            using (SqlConnection con = new SqlConnection(AppConfig.ConnectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(SqlQueries.User.getUsersForMenu, con))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@menu_name", actionName);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        users = counter == 0 ? reader["username"].ToString() : users + "," + reader["username"].ToString();
                        counter++;
                    }
                }
            }
            return users;
        }

        public static List<SelectListItem> GetUsers()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            using (SqlConnection con = new SqlConnection(AppConfig.ConnectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(SqlQueries.User.getUsers, con))
                {
                    cmd.CommandType = CommandType.Text;
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        selectListItems.Add(
                            new SelectListItem
                            {
                                Text = reader["name"].ToString(),
                                Value = reader["id"].ToString()
                            });
                    }
                }
            }
            return selectListItems;
        }

        public static void addLog(HttpContextBase context, bool status, string description)
        {
            string ip = string.IsNullOrEmpty(HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"])
                    ? HttpContext.Current.Request.UserHostAddress
                    : HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ip) || ip.Trim() == "::1")
            { // still can't decide or is LAN
                var lan = Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault(r => r.AddressFamily == AddressFamily.InterNetwork);
                ip = lan == null ? string.Empty : lan.ToString();
            }
            using (SqlConnection con = new SqlConnection(AppConfig.ConnectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(SqlQueries.User.addLog, con))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@username", context.User.Identity.Name);
                    cmd.Parameters.AddWithValue("@user_ip", ip);
                    cmd.Parameters.AddWithValue("@action_name", context.Request.RequestContext.RouteData.Values["action"].ToString());
                    cmd.Parameters.AddWithValue("@status", status);
                    cmd.Parameters.AddWithValue("@description", description ?? "");
                    try
                    {
                        int result = cmd.ExecuteNonQuery();
                    }
                    catch (Exception exc)
                    {
                        Email.SendEmail("hasimov.nadir@yandex.com", "ADY Contracts EXCEPTION", exc.ToString());
                    }
                }
            }
        }

        public static string GetRole()
        {
            RolePrincipal rolePrincipal = (RolePrincipal)HttpContext.Current.User;
            string rolename = rolePrincipal.GetRoles().FirstOrDefault();
            return rolename;
        }
    }
}