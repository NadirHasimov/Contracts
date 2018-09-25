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
    public static class ContractDALC
    {
        public static bool addToDb(ContractModel model)
        {
            using (SqlConnection con = new SqlConnection(AppConfig.ConnectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(SqlQueries.Contract.insert, con))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@contract_id", model.id);
                    cmd.Parameters.AddWithValue("@type_id", model.type);
                    cmd.Parameters.AddWithValue("@effective_date", model.effectiveDate);
                    cmd.Parameters.AddWithValue("@registration_date", model.registrationDate);
                    cmd.Parameters.AddWithValue("@admission_date", model.admissionDate);
                    cmd.Parameters.AddWithValue("@description", model.description);
                    cmd.Parameters.AddWithValue("@attachment_id", model.id);
                    int result = cmd.ExecuteNonQuery();
                    return result > 0;
                }
            }
        }

        public static List<SelectListItem> GetDocTypes()
        {
            var departList = new List<SelectListItem>();
            using (var con = new SqlConnection(AppConfig.ConnectionString))
            {
                con.Open();
                using (var cmd = new SqlCommand(SqlQueries.Contract.getDocTypes, con))
                {
                    cmd.CommandType = CommandType.Text;
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        departList.Add(
                            new SelectListItem
                            {
                                Value = reader["ID"].ToString(),
                                Text = reader["type"].ToString()
                            });
                    }
                }
            }
            return departList;
        }
    }
}