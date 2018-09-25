using AdyContracts.DomainModels;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;

namespace AdyContracts.DALC
{
    public static class CourtDALC
    {
        public static List<CourtDomainModel> GetTrials()
        {
            List<CourtDomainModel> trials = new List<CourtDomainModel>();
            using (SqlConnection con = new SqlConnection(AppConfig.ConnectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(SqlQueries.Court.getTrials, con))
                {
                    cmd.CommandType = CommandType.Text;
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        trials.Add(new CourtDomainModel
                        {
                            id = int.Parse(reader["id"].ToString()),
                            fullname = reader["fullname"].ToString(),
                            trialDate = DateTime.Parse(reader["hearing_date"].ToString()),
                            location = reader["address_location"].ToString()
                        });
                    }
                }
            }
            return trials;
        }
        public static bool CreateTrial(CourtDomainModel model)
        {
            using (SqlConnection con = new SqlConnection(AppConfig.ConnectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(SqlQueries.Court.insertTrial, con))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@user_id", model.userId);
                    cmd.Parameters.AddWithValue("@address_location", model.location);
                    cmd.Parameters.AddWithValue("@hearing_date", model.trialDate);
                    int affectedRows = cmd.ExecuteNonQuery();
                    return affectedRows == 1;
                }
            }
        }

        public static List<CourtNotificationModel> GetCourtNotifications(string username)
        {
            List<CourtNotificationModel> courtNotifications = new List<CourtNotificationModel>();
            using (SqlConnection con = new SqlConnection(AppConfig.ConnectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(SqlQueries.Court.getNotifations,con))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@username", username);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        courtNotifications.Add(
                            new CourtNotificationModel
                            {
                                Court_id = reader["court_id"].ToString(),
                                Day = reader["day"].ToString(),
                                Fullname = reader["fullname"].ToString(),
                                Message = reader["message"].ToString()
                            });
                    }
                }
            }
            return courtNotifications;
        }
    }
}