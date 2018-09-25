using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using AdyContracts.DomainModels;
using System.Data;
using S22.Imap;
using System.Net.Mail;
using System.IO;
using System.Text;

namespace AdyContracts.DALC
{
    public static class AttachmentDALC
    {
        public static void AddAttachmentInfoIntoDb(List<AttachmentModel> listOfAttachments)
        {
            using (SqlConnection con = new SqlConnection(AppConfig.ConnectionString))
            {
                con.Open();
                foreach (var model in listOfAttachments)
                {
                    using (SqlCommand cmd = new SqlCommand(SqlQueries.Attachment.insert, con))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@file_name", model.fileName);
                        cmd.Parameters.AddWithValue("@gen_file_name", model.filePath);
                        cmd.Parameters.AddWithValue("@content_type", model.conntentType);
                        cmd.Parameters.AddWithValue("@file_size", model.FileSize);
                        cmd.Parameters.AddWithValue("@doc_number", model.docNumber);
                        cmd.Parameters.AddWithValue("@username", HttpContext.Current.User.Identity.Name);
                        int result = cmd.ExecuteNonQuery();
                    }
                }
            }
        }
        public static List<AttachmentModel> getAttachmentsFroMail(string filePath)
        {
            using (ImapClient client = new ImapClient("imap.gmail.com", 993, "test.tester0006@gmail.com", "test0006", AuthMethod.Login, false))
            {
                string subject = "";
                IEnumerable<uint> uids = client.Search(SearchCondition.Unseen());
                List<AttachmentModel> listOfAttechments = new List<AttachmentModel>();
                foreach (var uid in uids)
                {
                    MailMessage message = client.GetMessage(uid);
                    subject = message.Subject;
                    foreach (var attachment in message.Attachments)
                    {

                        if (!String.Equals(attachment.ContentType.MediaType.ToString(), "application/pdf"))
                        {
                            continue;
                        }
                        string fileFullName = Path.Combine(filePath, attachment.Name);
                        using (FileStream fileStream = File.Create(fileFullName))
                        {
                            attachment.ContentStream.Seek(0, SeekOrigin.Begin);
                            attachment.ContentStream.CopyTo(fileStream);
                        }
                        listOfAttechments.Add(
                            new AttachmentModel
                            {
                                conntentType = attachment.ContentType.MediaType,
                                fileName = attachment.Name.Remove(attachment.Name.IndexOf(".pdf"), 4),
                                filePath = fileFullName,
                                FileSize = attachment.ContentStream.Length,
                                docNumber = subject.ToString()/* ----->><<-------*/
                            });
                        fileFullName = "";
                    }
                }
                return listOfAttechments;
            }
        }

        public static List<AttachmentModel> getAtttachmentsFromDb(int until2018 = 2)
        {
            List<AttachmentModel> attachmentList = new List<AttachmentModel>();
            using (SqlConnection con = new SqlConnection(AppConfig.ConnectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(SqlQueries.Attachment.get, con))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@until2018", until2018);
                    cmd.Parameters.AddWithValue("@username", HttpContext.Current.User.Identity.Name);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        attachmentList.Add(
                            new AttachmentModel
                            {
                                id = reader["doc_number"].ToString(),
                                typeId = int.Parse(reader["type_id"].ToString()),
                                contractType = reader["type"].ToString(),
                                description = reader["description"].ToString(),
                                effectiveDate = DateTime.Parse(reader["registration_date"].ToString()),
                                filePath = reader["gen_file_name"].ToString(),
                                fileName = reader["file_name"].ToString(),
                                opreationsColumn = replace(reader["doc_number"].ToString(), reader["file_name"].ToString(), reader["type_id"].ToString()),
                            });
                    }
                }
            }
            return attachmentList;
        }

        public static string replace(string id, string fileName, string typeId)
        {
            string result, a = "<a href=\"../../../DMS/Home/ViewFile?FileName=file_name&typeId=type_id\" target=\"_blank\" class=\"btn btn-info btn-icon icon-left btn-sm\">" +
                           "<i class=\"entypo-eye\"></i>" +
                           "Sənədə bax" +
                       "</a>" +
                       "<a href=\"../../../DMS/Home/Edit?id=i_d&typeId=type_id\"class=\"btn btn-default btn-icon icon-left btn-sm\">"
                           + "<i class=\"entypo-pencil\"></i>" +
                           "Dəyişdir" +
                       "</a>" +
                       "<a href=\"../../../DMS/Home/DeleteDocument?id=i_d\"class=\"btn btn-danger btn-icon icon-left btn-sm delete\" id=\"btn_delete\">" +
                          "<i class=\"entypo-trash\"></i>" +
                           "Sil" +
                       "</a>";
            StringBuilder sb = new StringBuilder(a);
            result = sb.Replace("file_name", fileName)
                       .Replace("i_d", id)
                       .Replace("type_id", typeId).ToString();
            return result;
        }
        public static int Count()
        {
            int count = 0;
            using (SqlConnection con = new SqlConnection(AppConfig.ConnectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(SqlQueries.Attachment.count, con))
                {
                    cmd.CommandType = CommandType.Text;
                    count = (int)cmd.ExecuteScalar();
                }
            }
            return count;
        }

    }
}