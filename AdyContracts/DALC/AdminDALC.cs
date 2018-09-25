using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AdyContracts.DomainModels;

namespace AdyContracts.DALC
{
    public static class AdminDALC
    {
        public static List<UserModel> GetAllUsers()
        {
            var userList = new List<UserModel>();
            using (var con = new SqlConnection(AppConfig.ConnectionString))
            {
                con.Open();
                using (var cmd = new SqlCommand(SqlQueries.Admin.selectUsers, con))
                {
                    cmd.CommandType = CommandType.Text;
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        userList.Add(new UserModel
                        {
                            Id = int.Parse(reader["id"].ToString()),
                            firstName = reader["fullname"].ToString(),
                            gender = reader["gender"].ToString(),
                            department = reader["depart_name"].ToString(),
                            email = reader["email"].ToString(),
                            username = reader["username"].ToString(),
                            confirmationStatus = bool.Parse(reader["confirmation_status"].ToString())
                        });
                    }
                }
            }
            return userList;
        }
        public static bool approveUser(int id, int roleId)
        {
            int affectedRows = 0;
            using (var con = new SqlConnection(AppConfig.ConnectionString))
            {
                con.Open();
                using (var cmd = new SqlCommand(SqlQueries.Admin.updateConfirmationStatus, con))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@role_id", roleId);
                    affectedRows = cmd.ExecuteNonQuery();
                    return affectedRows == 1;
                }
            }
        }
        public static List<SelectListItem> getAllRoleList()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            using (SqlConnection con = new SqlConnection(AppConfig.ConnectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(SqlQueries.Role.getAllRoles, con))
                {
                    cmd.CommandType = CommandType.Text;
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        selectListItems.Add(
                            new SelectListItem
                            {
                                Value = reader["sc_code"].ToString(),
                                Text = reader["sc_value"].ToString()
                            });
                    }
                }
            }
            return selectListItems;
        }

        public static List<MenuModel> GetMenus()
        {
            List<MenuModel> menus = new List<MenuModel>();
            using (SqlConnection con = new SqlConnection(AppConfig.ConnectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(SqlQueries.Menu.getMenus, con))
                {
                    cmd.CommandType = CommandType.Text;
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        menus.Add(new MenuModel
                        {
                            MenuId = int.Parse(reader["mnu_id"].ToString()),
                            MenuName = reader["mnu_caption1"].ToString(),
                            MenuParentPath = reader["Path"].ToString()
                        });
                    }
                }
            }
            return menus;
        }

        public static bool CreateRole(int[] ids, string roleName)
        {
            int result = 0;
            using (SqlConnection con = new SqlConnection(AppConfig.ConnectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(SqlQueries.Role.insert, con, con.BeginTransaction()))
                {
                    cmd.Parameters.AddWithValue("@role_name", roleName);
                    cmd.Parameters.AddWithValue_Tvp_Int("@ids", ids.ToList());
                    try
                    {
                        result = cmd.ExecuteNonQuery();
                        cmd.Transaction.Commit();
                    }
                    catch (Exception exc)
                    {
                        cmd.Transaction.Rollback();
                        Utils.Email.SendEmail("hasimov.nadir@yandex.com", "EXCEPTION ADY_CONTRACTS", exc.ToString());
                        result = 0;
                    }
                    return result >= 2;
                }
            }
        }
        public static void AddWithValue_Tvp_Int(this SqlParameterCollection paramCollection, string parameterName, List<int> data)
        {
            if (paramCollection != null)
            {
                var p = paramCollection.Add(parameterName, SqlDbType.Structured);
                p.TypeName = "dbo.tvp_Int";
                DataTable _dt = new DataTable() { Columns = { "Value" } };
                data.ForEach(value => _dt.Rows.Add(value));
                p.Value = _dt;
            }
        }
    }

}