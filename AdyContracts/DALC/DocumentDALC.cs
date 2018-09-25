using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using AdyContracts.DomainModels;
using Spire.Doc;
using AdyContracts.Models;
using System.IO;

namespace AdyContracts.DALC
{
    public static class DocumentDALC
    {
        public static List<SelectListItem> GetTypesDueToQuery(string sqlQuery)
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            using (SqlConnection con = new SqlConnection(AppConfig.ConnectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(sqlQuery, con))
                {
                    cmd.CommandType = CommandType.Text;
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        selectListItems.Add(new SelectListItem
                        {
                            Value = reader["sc_code"].ToString(),
                            Text = reader["sc_value"].ToString()
                        });
                    }
                }
            }
            return selectListItems;
        }

        public static List<SelectListItem> GetSelectLists(int typeId)
        {
            List<SelectListItem> listItems = new List<SelectListItem>();
            using (SqlConnection con = new SqlConnection(AppConfig.ConnectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(SqlQueries.Document.getSelectLists, con))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@type_id", typeId);
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        listItems.Add(
                            new SelectListItem
                            {
                                Value = reader["doc_number"].ToString(),
                                Text = reader["text"].ToString()
                            });
                    }
                }
            }
            return listItems;
        }

        public static bool InsertIntoDb(DocumentModel document, string extension, string word_text = null)
        {
            int result = 0;
            using (var con = new SqlConnection(AppConfig.ConnectionString))
            {
                con.Open();
                using (var cmd = new SqlCommand(SqlQueries.Document.insert, con, con.BeginTransaction()))
                {
                    cmd.CommandType = CommandType.Text;
                    if (String.IsNullOrEmpty(word_text))
                    {
                        cmd.Parameters.AddWithValue("@word_text", DBNull.Value);
                    }
                    else
                        cmd.Parameters.AddWithValue("@word_text", word_text);
                    cmd.Parameters.AddWithValue("@file_name", document.docNumber + extension);
                    cmd.Parameters.AddWithValue("@gen_file_name", document.attachmentModel.filePath);
                    cmd.Parameters.AddWithValue("@content_type", document.attachmentModel.conntentType);
                    cmd.Parameters.AddWithValue("@file_size", document.attachmentModel.FileSize);
                    cmd.Parameters.AddWithValue("@doc_number", document.docNumber);
                    cmd.Parameters.AddWithValue("@gov_reg_number", String.IsNullOrEmpty(document.govRegNumber) ? "" : document.govRegNumber);
                    cmd.Parameters.AddWithValue("@receiving_authority_id", document.receivingAuthorityId);
                    cmd.Parameters.AddWithValue("@description", document.description);
                    cmd.Parameters.AddWithValue("@until_2018", String.Equals(extension, ".pdf") ? 0 : 1);
                    cmd.Parameters.AddWithValue("@registration_date", document.registrationDate);
                    cmd.Parameters.AddWithValue("@type_id", document.typeId);
                    cmd.Parameters.AddWithValue("@termination_doc", document.terminatedDocNumber ?? "0");
                    cmd.Parameters.AddWithValue("@username", HttpContext.Current.User.Identity.Name);
                    try
                    {
                        result = cmd.ExecuteNonQuery();
                        cmd.Transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        Utils.Email.SendEmail("hasimov.nadir@yandex.com", "EXCEPTION ADY_CONTRACTS", ex.ToString());
                    }
                    return result > 1;
                }
            }
        }

        public static bool InsertOrderDocument(DocumentViewModel documentModel, string extension, string word_text = null)
        {
            bool result = false;
            int affectedRows = 2;
            OrderViewModel orderViewModel = documentModel.orderViewModel;
            documentModel.attachmentViewModel = documentModel.attachmentViewModel;
            documentModel.ReceivingAuthorityId = documentModel.type == 2 ? "3" : documentModel.ReceivingAuthorityId;
            using (SqlConnection con = new SqlConnection(AppConfig.ConnectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(SqlQueries.Document.insertOrder, con, con.BeginTransaction()))
                {
                    cmd.CommandType = CommandType.Text;
                    if (String.IsNullOrEmpty(word_text))
                    {
                        cmd.Parameters.AddWithValue("@word_text", DBNull.Value);
                    }
                    else
                        cmd.Parameters.AddWithValue("@word_text", word_text);
                    cmd.Parameters.AddWithValue("@file_name", documentModel.id + extension);
                    cmd.Parameters.AddWithValue("@gen_file_name", documentModel.attachmentViewModel.filePath);
                    cmd.Parameters.AddWithValue("@content_type", documentModel.attachmentViewModel.conntentType);
                    cmd.Parameters.AddWithValue("@file_size", documentModel.attachmentViewModel.FileSize);
                    cmd.Parameters.AddWithValue("@doc_number", documentModel.id);
                    cmd.Parameters.AddWithValue("@gov_reg_number", String.IsNullOrEmpty(documentModel.govRegNumber) ? "" : documentModel.govRegNumber);
                    cmd.Parameters.AddWithValue("@receiving_authority_id", documentModel.ReceivingAuthorityId);
                    cmd.Parameters.AddWithValue("@description", documentModel.description);
                    cmd.Parameters.AddWithValue("@until_2018", String.Equals(extension, ".pdf") ? 0 : 1);
                    cmd.Parameters.AddWithValue("@registration_date", documentModel.registrationDate);
                    cmd.Parameters.AddWithValue("@effective_date", documentModel.effectiveDate);
                    cmd.Parameters.AddWithValue("@type_id", documentModel.type);
                    cmd.Parameters.AddWithValue("@terminated_order_num", documentModel.terminatedDocNumber ?? "0");
                    if (!String.IsNullOrEmpty(documentModel.terminatedDocNumber))
                    {
                        affectedRows++;
                        bool terminateEntireOrder = documentModel.rbtnTerminationType == "1";
                        cmd.Parameters.AddWithValue("@entire_order_status", terminateEntireOrder);
                        cmd.Parameters.AddWithValue("@part_order_status", !terminateEntireOrder);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@entire_order_status", false);
                        cmd.Parameters.AddWithValue("@part_order_status", false);
                    }
                    try
                    {
                        result = affectedRows <= cmd.ExecuteNonQuery();
                        if (result)
                        {
                            cmd.Transaction.Commit();
                        }
                    }
                    catch (Exception exc)
                    {
                        Utils.Email.SendEmail("hasimov.nadir@yandex.com", "ADY_CONTRACTS EXCEPTION", exc.ToString());
                    }
                    return result;
                }
            }
        }

        public static void TerminateOrders(string[] ids)
        {
            using (SqlConnection con = new SqlConnection(AppConfig.ConnectionString))
            {
                con.Open();
                foreach (string id in ids)
                {
                    using (SqlCommand cmd = new SqlCommand(SqlQueries.Document.terminateOrder, con))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        public static void InsertChangedOrders(string orderNum, string[] changedOrders)
        {
            using (SqlConnection con = new SqlConnection(AppConfig.ConnectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(SqlQueries.Document.insertChangedOrders, con))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@order_doc_num", orderNum);
                    cmd.Parameters.AddWithValue_Tvp_Nvarchar("@changed_orders", changedOrders.ToList());
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void AddWithValue_Tvp_Nvarchar(this SqlParameterCollection paramCollection, string parameterName, List<string> data)
        {
            if (paramCollection != null)
            {
                var p = paramCollection.Add(parameterName, SqlDbType.Structured);
                p.TypeName = "dbo.tvp_nvarchar";
                DataTable _dt = new DataTable() { Columns = { "Value" } };
                data.ForEach(value => _dt.Rows.Add(value));
                p.Value = _dt;
            }
        }

        public static AttachmentModel CreateDocumentBasedOnTemplate(TemplateParameters templateParameters, int documentType, string contractnumber, DateTime registerDate)
        {
            Document document = new Document();
            AttachmentModel attachmentModel = new AttachmentModel();
            TemplateParameters parameters = templateParameters;
            string _path = "";
            string _fileName = "";

            string day = registerDate.Day.ToString("00");
            string month = registerDate.Month.ToString("00");
            string year = registerDate.Year.ToString();
            if (documentType == 3)
            {
                _path = HttpContext.Current.Server.MapPath("~/UploadedFiles");
                _fileName = contractnumber.ToString() + ".pdf";
                document.LoadFromFile(HttpContext.Current.Server.MapPath("~/TemplateFiles/muqavilə.doc"));
                document.Replace("contractname", parameters.documentName, false, true);
                document.Replace("day", day.ToString(), false, true);
                document.Replace("month", month.ToString(), false, true);
                document.Replace("year", year.ToString(), false, true);
                document.Replace("companydirector", parameters.companyDirector, false, true);
                document.Replace("contractpredmet", parameters.companyPredmet, false, true);
                document.Replace("companyaddress", parameters.companyAddress, false, true);
                document.Replace("companyname", parameters.companyName, false, true);
                document.SaveToFile(HttpContext.Current.Server.MapPath(Path.Combine("~/UploadedFiles", _fileName)), FileFormat.PDF);

                attachmentModel.fileName = contractnumber.ToString();
                attachmentModel.filePath = Path.Combine(_path, _fileName);
                var fileInfo = new FileInfo(HttpContext.Current.Server.MapPath(Path.Combine("~/UploadedFiles", _fileName)));
                attachmentModel.FileSize = fileInfo.Length;
                attachmentModel.conntentType = "application/pdf";
            }
            return attachmentModel;
        }

        public static bool Delete(string id)
        {
            using (SqlConnection con = new SqlConnection(AppConfig.ConnectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(SqlQueries.Document.delete, con))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@doc_number", id);
                    var result = cmd.ExecuteNonQuery();
                    return result == 1;
                }
            }
        }

        public static DocumentModel GetDocumentById(string id, int typeId)
        {
            DocumentModel documentModel = new DocumentModel();
            documentModel.templateParameters = new TemplateParameters();
            using (var con = new SqlConnection(AppConfig.ConnectionString))
            {
                con.Open();
                using (var cmd = new SqlCommand(SqlQueries.Document.getDocumentById, con))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@doc_number", id);
                    cmd.Parameters.AddWithValue("@type_id", typeId);

                    var reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        documentModel.docNumber = id;
                        documentModel.registrationDate = DateTime.Parse(reader["registration_date"].ToString());
                        documentModel.description = reader["description"].ToString();
                        documentModel.typeId = int.Parse(reader["type_id"].ToString());
                        documentModel.govRegNumber = reader["gov_reg_number"].ToString();
                        documentModel.receivingAuthorityId = int.Parse(reader["receiving_authority_id"].ToString());
                        if (typeId == 1 || typeId == 4 || typeId == 2)
                        {
                            documentModel.terminatedDocNumber = reader["terminated_order_num"].ToString();
                            if (!String.Equals("0", documentModel.terminatedDocNumber))
                            {
                                documentModel.rbtnTerminationType = bool.Parse(reader["entire_order_status"].ToString()) ? "1" : "2";
                            }
                            documentModel.effectiveDate = DateTime.Parse(reader["effective_date"].ToString());
                        }
                        else
                            documentModel.terminatedDocNumber = reader["termination_doc"].ToString();
                        if (reader.NextResult())
                        {
                            reader.Read();
                            documentModel.changedOrders = reader["changed_orders"].ToString().Split(',');
                        }
                    }
                }
            }
            return documentModel;
        }

        public static bool Edit(DocumentModel documentModel, string orginalId)
        {
            documentModel.receivingAuthorityId = documentModel.typeId == 2 ? 3 : documentModel.receivingAuthorityId;
            if (documentModel.changedOrders == null)
            {
                string[] aryString = { "000" };
                documentModel.changedOrders = aryString;
            }
            using (SqlConnection con = new SqlConnection(AppConfig.ConnectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(SqlQueries.Document.update, con, con.BeginTransaction()))
                {
                    cmd.Parameters.AddWithValue("@doc_number", documentModel.docNumber);
                    cmd.Parameters.AddWithValue("@gov_reg_number", String.IsNullOrEmpty(documentModel.govRegNumber) ? "" : documentModel.govRegNumber);
                    cmd.Parameters.AddWithValue("@receiving_authority_id", documentModel.receivingAuthorityId);
                    cmd.Parameters.AddWithValue("@description", documentModel.description);
                    cmd.Parameters.AddWithValue("@registration_date", documentModel.registrationDate);
                    cmd.Parameters.AddWithValue("@type_id", documentModel.typeId);
                    cmd.Parameters.AddWithValue("@termination_doc", documentModel.terminatedDocNumber ?? "0");
                    cmd.Parameters.AddWithValue("@orginal_id", orginalId);
                    if (documentModel.typeId == 1 || documentModel.typeId == 4)
                    {
                        cmd.Parameters.AddWithValue("@effective_date", documentModel.effectiveDate);
                        cmd.Parameters.AddWithValue_Tvp_Nvarchar("@changed_orders", documentModel.changedOrders.ToList());
                        if (documentModel.terminatedDocNumber != null)
                        {
                            bool terminateEntireOrder = documentModel.rbtnTerminationType == "1";
                            cmd.Parameters.AddWithValue("@entire_order_status", terminateEntireOrder);
                            cmd.Parameters.AddWithValue("@part_order_status", !terminateEntireOrder);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@entire_order_status", false);
                            cmd.Parameters.AddWithValue("@part_order_status", false);
                        }
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@effective_date", DateTime.Now);
                        cmd.Parameters.AddWithValue("@entire_order_status", false);
                        cmd.Parameters.AddWithValue("@part_order_status", false);
                        cmd.Parameters.AddWithValue_Tvp_Nvarchar("@changed_orders", documentModel.changedOrders.ToList());
                    }
                    try
                    {
                        int result = cmd.ExecuteNonQuery();
                        cmd.Transaction.Commit();
                        return result > 0;
                    }
                    catch (Exception exc)
                    {
                        Utils.Email.SendEmail("hasimov.nadir@yandex.com", "EXCEPTION ADY_CONTRACTS", exc.ToString());
                        return false;
                    }
                }
            }
        }

        public static int CreateNewReceiver(string receiver)
        {
            int receiverId;
            using (SqlConnection con = new SqlConnection(AppConfig.ConnectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(SqlQueries.Document.insertNewReceiver, con, con.BeginTransaction()))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@receiver", receiver);
                    try
                    {
                        receiverId = int.Parse(cmd.ExecuteScalar().ToString());
                        cmd.Transaction.Commit();
                        return receiverId;
                    }
                    catch (Exception exc)
                    {
                        Utils.Email.SendEmail("hasimov.nadir@yandex.com", "EXCEPTION ADY_CONTRACTS", exc.ToString());
                        return -1;
                    }
                }
            }
        }

        public static bool CheckDocNumber(string docNumber)
        {
            using (SqlConnection con = new SqlConnection(AppConfig.ConnectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(SqlQueries.Document.checkDocNumber, con))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@doc_number", docNumber);
                    return 0 == int.Parse(cmd.ExecuteScalar().ToString());
                }
            }
        }

        public static List<SelectListItem> GetOrderDocumentList(int typeId)
        {
            List<SelectListItem> listItems = new List<SelectListItem>();
            listItems.Add(new SelectListItem { });
            using (SqlConnection con = new SqlConnection(AppConfig.ConnectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(SqlQueries.Document.getOrderDocs, con))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@type_id", typeId);
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        listItems.Add(
                            new SelectListItem
                            {
                                Value = reader["doc_number"].ToString(),
                                Text = reader["text"].ToString()
                            });
                    }
                }
            }
            return listItems;
        }

        public static bool InsertParagraph(ParagraphModel paragraph)
        {
            using (SqlConnection con = new SqlConnection(AppConfig.ConnectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(SqlQueries.Document.insertParagraph, con))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@order_number", paragraph.orderNumber);
                    cmd.Parameters.AddWithValue("@paragraph_number", paragraph.paragraphNumber);
                    cmd.Parameters.AddWithValue("@paragraph_text", paragraph.paragraphText);
                    cmd.Parameters.AddWithValue("@parent", paragraph.parent);
                    return cmd.ExecuteNonQuery() == 1;
                }
            }
        }

        public static List<SelectListItem> GetParagraphs(string docNumber)
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            selectListItems.Add(new SelectListItem { });
            using (SqlConnection con = new SqlConnection(AppConfig.ConnectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(SqlQueries.Document.getParagraphs, con))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@order_number", docNumber);
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        selectListItems.Add(new SelectListItem
                        {
                            Value = reader["id"].ToString(),
                            Text = reader["text"].ToString()
                        });
                    }
                }
            }
            return selectListItems;
        }

        public static DocumentModel GetDocumentsForDescription(string docNumber, string typeId)
        {
            DocumentModel documentModel = new DocumentModel();
            using (SqlConnection con = new SqlConnection(AppConfig.ConnectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(SqlQueries.Document.getDocumentForDescription, con))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@doc_number", docNumber);
                    cmd.Parameters.AddWithValue("@type_id", int.Parse(typeId));

                    var reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        documentModel.docNumber = docNumber;
                        documentModel.registrationDate = DateTime.Parse(reader["registration_date"].ToString());
                        documentModel.fileName = reader["file_name2"].ToString();
                        documentModel.description = reader["description"].ToString();
                        documentModel.govRegNumber = reader["gov_reg_number"].ToString();
                        documentModel.receiver = reader["receiver"].ToString();
                        documentModel.type = reader["doc_type"].ToString();
                        documentModel.status = bool.Parse(reader["status"].ToString());
                        documentModel.typeId = int.Parse(typeId);
                        if (int.Parse(typeId) == 1 || int.Parse(typeId) == 4 || int.Parse(typeId) == 2)
                        {
                            documentModel.terminatedDocNumber = reader["terminated_order_num"].ToString();
                            if (!String.Equals("0", documentModel.terminatedDocNumber))
                            {
                                documentModel.rbtnTerminationType = bool.Parse(reader["entire_order_status"].ToString()) ? "1" : "2";
                            }
                            documentModel.effectiveDate = DateTime.Parse(reader["effective_date"].ToString());
                        }
                        else
                            documentModel.terminatedDocNumber = reader["termination_doc"].ToString();
                        if (reader.NextResult())
                        {
                            reader.Read();
                            documentModel.changedOrders = reader["changed_orders"].ToString().Split(',');
                        }
                    }
                }
            }
            return documentModel;
        }

        public static List<AttachmentModel> Filter(DocumentFilterModel filterModel)
        {
            int[] arrayDefault = new int[] { 0 };
            filterModel.docTypes = filterModel.docTypes == null ? arrayDefault : filterModel.docTypes;
            filterModel.receivers = filterModel.receivers == null ? arrayDefault : filterModel.receivers;
            List<AttachmentModel> attachments = new List<AttachmentModel>();
            using (SqlConnection con = new SqlConnection(AppConfig.ConnectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(SqlQueries.Document.filter, con))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@doc_number", filterModel.docNumber ?? "");
                    cmd.Parameters.AddWithValue("@description", filterModel.description ?? "");
                    cmd.Parameters.AddWithValue("@reg_gov_number", filterModel.regGovNumber ?? "");
                    if (filterModel.registrationDate1 == null)
                    {
                        cmd.Parameters.AddWithValue("@reg_date1", DBNull.Value);
                        cmd.Parameters.AddWithValue("@reg_date2", DBNull.Value);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@reg_date1", filterModel.registrationDate1);
                        cmd.Parameters.AddWithValue("@reg_date2", filterModel.registrationDate2);
                    }
                    if (filterModel.effectiveDate1 == null || filterModel.effectiveDate2 == null)
                    {
                        cmd.Parameters.AddWithValue("@effect_date1", DBNull.Value);
                        cmd.Parameters.AddWithValue("@effect_date2", DBNull.Value);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@effect_date1", filterModel.effectiveDate1);
                        cmd.Parameters.AddWithValue("@effect_date2", filterModel.effectiveDate2);
                    }
                    cmd.Parameters.AddWithValue("@search_type", filterModel.searchType);
                    cmd.Parameters.AddWithValue("@exact_same", filterModel.exactSame);
                    cmd.Parameters.AddWithValue("@search_order", filterModel.searchOrder);
                    cmd.Parameters.AddWithValue("@descending_order", filterModel.descendingOrder);
                    cmd.Parameters.AddWithValue("@status", filterModel.status ?? 6);
                    cmd.Parameters.AddWithValue_Tvp_Int("@doc_types", filterModel.docTypes.ToList());
                    cmd.Parameters.AddWithValue_Tvp_Int("@receivers", filterModel.receivers.ToList());
                    cmd.Parameters.AddWithValue("@username", HttpContext.Current.User.Identity.Name);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        attachments.Add(
                        new AttachmentModel
                        {
                            id = reader["doc_number"].ToString(),
                            typeId = int.Parse(reader["type_id"].ToString()),
                            contractType = reader["type"].ToString(),
                            description = reader["description"].ToString(),
                            date = DateTime.Parse(reader["registration_date"].ToString()).ToShortDateString(),
                            filePath = reader["gen_file_name"].ToString(),
                            fileName = reader["file_name"].ToString(),
                            opreationsColumn = AttachmentDALC.replace(reader["doc_number"].ToString(),
                                                                      reader["file_name"].ToString(),
                                                                      reader["type_id"].ToString())
                        });
                    }
                }
            }
            return attachments;
        }
    }
}